using EasyApp;
using EasyApp.Events;
using Pickaxe.Watcher.IO.Twitter.Events;
using Pickaxe.Watcher.IO.Twitter.Stream;
using System;
using System.IO;
using System.Threading;

namespace Pickaxe.Watcher
{
    class StreamListener : EasyTask, IDisposable
    {
        public SecurityOptions SecurityOptions { get; set; }
        public StreamListenerOptions Options { get; set; }

        private const char UNIT_SEPARATOR = '\u241F';
        private StreamWriter _writer;
        private int _secuence = 0;

        public int TweetReceived { get; private set; }

        public override void BeforeStart()
        {
            // Restart time default value.
            if (this.Options.RestartTime == 0)
                this.Options.RestartTime = 5;
        }

        public override void AfterCompleted()
        {
            this.Dispose();
        }

        public override void Start()
        {
            var thread = new Thread(new ThreadStart(StreamTweets));
            thread.Start();
        }

        private void StreamTweets()
        {
            var stream = new TwitterStream(
                this.SecurityOptions.Credential.ApiKey,
                this.SecurityOptions.Credential.ApiSecret,
                this.SecurityOptions.Credential.TokenKey,
                this.SecurityOptions.Credential.TokenSecret
            );

            // Set Proxy
            if (!string.IsNullOrEmpty(this.Options.ProxyConnectionString))
            {
                stream.SetProxy(this.Options.ProxyConnectionString);
            }

            stream.StreamStarted += stream_StreamStarted;
            stream.StreamStopped += stream_StreamStopped;
            stream.TweetReceived += stream_TweetReceived;
            stream.JsonObjectReceived += stream_JsonObjectReceived;

            stream.Start(this.Options.Track, this.Options.MatchCondition, this.Options.SearchLocations);
        }

        private string GetConfiguredFilename()
        {
            if (this.Options.Mode == FilenameMode.Secuence)
            {
                int ix = this.Options.Filename.LastIndexOf('.') + 1;
                return this.Options.Filename.Substring(0, ix) + _secuence++ + ".data";
            }
            else if (this.Options.Mode == FilenameMode.Date)
            {
                var dir = Path.GetDirectoryName(this.Options.Filename);
                return Path.Combine(dir, string.Format("{0:yyyyMMdd_HHmmss}.data", DateTime.Now));
            }
            else
            {
                return this.Options.Filename;
            }
        }

        private void SetWriter(string filename)
        {
            var dir = Path.GetDirectoryName(filename);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            _writer = new StreamWriter(filename, true);
        }

        private void stream_StreamStarted(object sender, EventArgs e)
        {
            this.SetWriter(GetConfiguredFilename());
            this.OnProgress(new MessageEventArgs("The Twitter stream has been started.", MessageType.Info));
        }
        private void stream_StreamStopped(object sender, EventArgs e)
        {
            var args = (StreamExceptionEventArgs)e;

            _writer.Close();
            this.OnProgress(new MessageEventArgs("The Twitter stream has been stopped.", MessageType.Info));

            this.OnFailed(new MessageEventArgs(args.Exception.Message, MessageType.Error));

            this.OnProgress(new MessageEventArgs(MessageType.Info, Priority.Medium, "Restarting... New start time: {0}", DateTime.Now.AddMinutes(this.Options.RestartTime).ToShortTimeString()));
            System.Threading.Thread.Sleep(TimeSpan.FromMinutes(this.Options.RestartTime));

            this.StreamTweets();
        }

        private void stream_TweetReceived(object sender, IO.Twitter.Events.TweetReceivedEventArgs e)
        {
            if (this.Options.ShowCapture)
                this.OnProgress(new MessageEventArgs(e.Tweet.ToString(), MessageType.Data));
        }

        private void stream_JsonObjectReceived(object sender, IO.Twitter.Events.JsonObjectEventArgs e)
        {
            lock (_writer)
            {
                _writer.Write(e.Json);
                _writer.Write(UNIT_SEPARATOR);
            }
            this.TweetReceived++;

            if (this.Options.TweetLimit > 0 && this.TweetReceived % this.Options.TweetLimit == 0)
            {
                // close writer and create a new one.
                _writer.Flush();
                _writer.Close();
                this.SetWriter(GetConfiguredFilename());
            }

            if (!this.Options.ShowCapture)
                this.OnProgress(new MessageEventArgs(string.Format("Captured Tweets: {0}", this.TweetReceived), MessageType.Progress));
        }

        public void Dispose()
        {
            if (_writer != null)
                _writer.Close();
        }
    }
}
