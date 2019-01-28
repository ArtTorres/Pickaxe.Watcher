using System;
using Tweetinvi.Models;
using Tweetinvi.Streaming;
using Tweetinvi.Streaming.Parameters;
using  Pickaxe.Watcher.IO.Twitter.Events;
using  Pickaxe.Watcher.IO.Twitter.Security;

namespace Pickaxe.Watcher.IO.Twitter.Stream
{
    public class TwitterStream : TwitterManager
    {
        public event EventHandler<TweetReceivedEventArgs> TweetReceived;
        public event EventHandler<LimitReachedEventArgs> LimitReached;
        public event EventHandler<JsonObjectEventArgs> JsonObjectReceived;
        public event EventHandler<EventArgs> StreamStarted;
        public event EventHandler<EventArgs> StreamResumed;
        public event EventHandler<EventArgs> StreamStopped;
        public event EventHandler<EventArgs> StreamPaused;

        #region Event Definition
        private void OnTweetReceived(TweetReceivedEventArgs e)
        {
            if (TweetReceived != null)
                TweetReceived(this, e);
        }
        private void OnJsonObjectReceived(JsonObjectEventArgs e)
        {
            if (JsonObjectReceived != null)
                JsonObjectReceived(this, e);
        }
        private void OnLimitReached(LimitReachedEventArgs e)
        {
            if (LimitReached != null)
                LimitReached(this, e);
        }
        private void OnStreamStarted(EventArgs e)
        {
            if (StreamStarted != null)
                StreamStarted(this, e);
        }
        private void OnStreamResumed(EventArgs e)
        {
            if (StreamResumed != null)
                StreamResumed(this, e);
        }
        private void OnStreamStopped(EventArgs e)
        {
            if (StreamStopped != null)
                StreamStopped(this, e);
        }
        private void OnStreamPaused(EventArgs e)
        {
            if (StreamPaused != null)
                StreamPaused(this, e);
        }
        #endregion

        private IFilteredStream _stream;

        public TwitterStream(TwitterCredential credential)
            : this(credential.ApiKey, credential.ApiSecret, credential.TokenKey, credential.TokenSecret)
        {
        }

        public TwitterStream(string consumerKey, string consumerSecret, string accessTokenKey, string accessTokenSecret)
            : base(consumerKey, consumerSecret, accessTokenKey, accessTokenSecret)
        {
            _stream = Tweetinvi.Stream.CreateFilteredStream();
            _stream.FilterLevel = StreamFilterLevel.Low;
            _stream.StallWarnings = true;

            _stream.MatchingTweetReceived += _stream_MatchingTweetReceived;
            _stream.StreamStarted += _stream_StreamStarted;
            _stream.StreamPaused += _stream_StreamPaused;
            _stream.StreamResumed += _stream_StreamResumed;
            _stream.StreamStopped += _stream_StreamStopped;
            _stream.LimitReached += _stream_LimitReached;
            _stream.JsonObjectReceived += _stream_JsonObjectReceived;
        }

        public void Start(MatchCondition condition, params Model.Location[] locations)
        {
            this.Start(null, condition, locations);
        }

        public void Start(string track, MatchCondition condition, params Model.Location[] locations)
        {
            // Filters
            if (!string.IsNullOrEmpty(track))
                _stream.AddTrack(track);

            if (null != locations && locations.Length > 0)
            {
                foreach (var location in locations)
                {
                    _stream.AddLocation(
                        new Coordinates(location.Latitude1, location.Longitude1),
                        new Coordinates(location.Latitude2, location.Longitude2)
                    );
                }
            }

            // Start
            if (condition == MatchCondition.AnyCondition)
                _stream.StartStreamMatchingAnyCondition();
            else
                _stream.StartStreamMatchingAllConditions();
        }

        public void Pause()
        {
            _stream.PauseStream();
        }

        public void Stop()
        {
            _stream.StopStream();
        }

        #region Events
        private void _stream_MatchingTweetReceived(object sender, Tweetinvi.Events.MatchedTweetReceivedEventArgs e)
        {
            this.OnTweetReceived(TweetReceivedEventArgs.Create(ParseTweet(e.Tweet)));
        }
        void _stream_JsonObjectReceived(object sender, Tweetinvi.Events.JsonObjectEventArgs e)
        {
            this.OnJsonObjectReceived(new JsonObjectEventArgs() { Json = e.Json });
        }
        void _stream_LimitReached(object sender, Tweetinvi.Events.LimitReachedEventArgs e)
        {
            this.OnLimitReached(LimitReachedEventArgs.Create(e.NumberOfTweetsNotReceived));
        }
        void _stream_StreamStarted(object sender, EventArgs e)
        {
            this.OnStreamStarted(e);
        }
        void _stream_StreamResumed(object sender, EventArgs e)
        {
            this.OnStreamResumed(e);
        }
        void _stream_StreamStopped(object sender, Tweetinvi.Events.StreamExceptionEventArgs e)
        {
            var message = "{code:0,reason:NA,name:NA}";
            if (e.DisconnectMessage != null)
            {
                message = string.Format("{code:{0},reason:{1},name:{2}}", e.DisconnectMessage.Code, e.DisconnectMessage.Reason, e.DisconnectMessage.StreamName);
            }

            this.OnStreamStopped(
                StreamExceptionEventArgs.Create(
                    e.Exception,
                    message
                )
            );
        }
        void _stream_StreamPaused(object sender, EventArgs e)
        {
            this.OnStreamPaused(e);
        }
        #endregion
    }
}
