using System;

namespace Pickaxe.Watcher.IO.Twitter.Events
{
    public class StreamExceptionEventArgs : EventArgs
    {
        public Exception Exception { get; set; }
        public string DisconnectionMessage { get; set; }

        public static StreamExceptionEventArgs Create(Exception exception, string disconnectionMessage)
        {
            return new StreamExceptionEventArgs() { Exception = exception, DisconnectionMessage = disconnectionMessage };
        }
    }
}
