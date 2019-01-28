using System;
using  Pickaxe.Watcher.IO.Twitter.Model;

namespace Pickaxe.Watcher.IO.Twitter.Events
{
    public class TweetReceivedEventArgs : EventArgs
    {
        public Tweet Tweet { get; private set; }

        public static TweetReceivedEventArgs Create(Tweet tweet)
        {
            return new TweetReceivedEventArgs() { Tweet = tweet };
        }
    }
}
