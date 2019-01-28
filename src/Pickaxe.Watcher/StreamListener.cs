using Pickaxe.Watcher.IO.Twitter.Events;
using Pickaxe.Watcher.IO.Twitter.Stream;
using QApp;
using QApp.Events;
using System;
using System.IO;
using System.Threading;

namespace Pickaxe.Watcher
{
    class StreamListener : QProcess, IDisposable
    {
        private const char UNIT_SEPARATOR = '\u241F';
        private StreamListenerOptions _options;
        private StreamWriter _writer;
        private int _secuence = 0;

        public int TweetReceived { get; private set; }

        public StreamListener(StreamListenerOptions options)
        {
            _options = options;

            // Restart time default value.
            if (_options.RestartTime == 0)
                _options.RestartTime = 5;
        }

        public override void Start()
        {
            var thread = new Thread(new ThreadStart(StreamTweets));
            thread.Start();
        }

        private void StreamTweets()
        {
            var stream = new TwitterStream(
                _options.Credential.ApiKey,
                _options.Credential.ApiSecret,
                _options.Credential.TokenKey,
                _options.Credential.TokenSecret
            );

            // Set Proxy
            if (!string.IsNullOrEmpty(_options.ProxyConnectionString))
            {
                stream.SetProxy(_options.ProxyConnectionString);
            }

            stream.StreamStarted += stream_StreamStarted;
            stream.StreamStopped += stream_StreamStopped;
            stream.TweetReceived += stream_TweetReceived;
            stream.JsonObjectReceived += stream_JsonObjectReceived;

            stream.Start(_options.Track, _options.MatchCondition, _options.SearchLocations);
        }

        private string GetConfiguredFilename()
        {
            if (_options.Mode == FilenameMode.Secuence)
            {
                int ix = _options.Filename.LastIndexOf('.') + 1;
                return _options.Filename.Substring(0, ix) + _secuence++ + ".data";
            }
            else if (_options.Mode == FilenameMode.Date)
            {
                var dir = Path.GetDirectoryName(_options.Filename);
                return Path.Combine(dir, string.Format("{0:yyyyMMdd_HHmmss}.data", DateTime.Now));
            }
            else
            {
                return _options.Filename;
            }
        }

        private void SetWriter(string filename)
        {
            var dir = Path.GetDirectoryName(filename);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            _writer = new StreamWriter(filename, true);
        }

        void stream_StreamStarted(object sender, EventArgs e)
        {
            this.SetWriter(GetConfiguredFilename());
            this.OnProcessProgress(new MessageEventArgs("The Twitter stream has been started.", MessageType.Info));
        }
        void stream_StreamStopped(object sender, EventArgs e)
        {
            var args = (StreamExceptionEventArgs)e;

            _writer.Close();
            this.OnProcessProgress(new MessageEventArgs("The Twitter stream has been stopped.", MessageType.Info));

            this.OnProcessFailed(new MessageEventArgs(args.Exception.Message, MessageType.Error));

            this.OnProcessProgress(new MessageEventArgs(MessageType.Info, MessagePriority.Medium, "Restarting... New start time: {0}", DateTime.Now.AddMinutes(_options.RestartTime).ToShortTimeString()));
            System.Threading.Thread.Sleep(TimeSpan.FromMinutes(_options.RestartTime));

            this.StreamTweets();
        }

        void stream_TweetReceived(object sender, IO.Twitter.Events.TweetReceivedEventArgs e)
        {
            if (_options.ShowCapture)
                this.OnProcessProgress(new MessageEventArgs(e.Tweet.ToString(), MessageType.Data));
        }

        void stream_JsonObjectReceived(object sender, IO.Twitter.Events.JsonObjectEventArgs e)
        {
            lock (_writer)
            {
                _writer.Write(e.Json);
                _writer.Write(UNIT_SEPARATOR);
            }
            this.TweetReceived++;

            if (_options.TweetLimit > 0 && this.TweetReceived % _options.TweetLimit == 0)
            {
                // close writer and create a new one.
                _writer.Flush();
                _writer.Close();
                this.SetWriter(GetConfiguredFilename());
            }

            if (!_options.ShowCapture)
                this.OnProcessProgress(new MessageEventArgs(string.Format("Captured Tweets: {0}", this.TweetReceived), MessageType.Progress));
        }

        public void Dispose()
        {
            if (_writer != null)
                _writer.Close();
        }
    }
}
