using System;

namespace Pickaxe.Watcher.IO.Twitter.Events
{
    public class JsonObjectEventArgs : EventArgs
    {
        public string Json { get; set; }
    }
}
