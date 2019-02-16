using EasyApp.Documentation;
using MagnetArgs;
using Pickaxe.Watcher.IO.Twitter.Model;
using Pickaxe.Watcher.IO.Twitter.Stream;
using Pickaxe.Watcher.Options;

namespace Pickaxe.Watcher
{
    enum FilenameMode
    {
        Manual,
        Date,
        Secuence
    }

    class StreamListenerOptions : MagnetSet
    {
        [Arg("--track", Alias = "-t")]
        [Help("Enable the search for twitter search terms.", Example = "--track \"@user\"", Group = "Execution", Order = 1)]
        public string Track { get; set; }

        [Arg("--locations", Alias = "-l"), Parser(typeof(LocationParser))]
        [Help("Enables the search for an specific set of locations in json format.", Example = "--locations [{latitude1:20.747814,longitude1:-103.418550,latitude2:20.560952,longitude2:-103.217340}]", Group = "Execution", Order = 2)]
        public Location[] SearchLocations { get; set; }

        [Arg("--match-condition", Alias = "-mc"), Parser(typeof(MatchConditionParser))]
        [Help("Indicates the type of match allowed.", Example = "--match-condition \"all\"", Group = "Execution", Order = 3)]
        public MatchCondition MatchCondition { get; set; }

        [Arg("--file", Alias = "-f"), IsRequired]
        [Help("Indicates the output file.", Example = "--filename \"C:\\filename.data\"", Group = "Execution", Order = 4)]
        public string Filename { get; set; }

        [Arg("--filename-mode", Alias = "-fm"), Parser(typeof(FilenameModeParser))]
        [Help("Sets the mode of file creation.", Example = "--filename-mode \"date\"", Group = "Execution", Order = 5)]
        public FilenameMode Mode { get; set; }

        [Arg("--tweet-limit", Alias = "-tl")]
        [Help("Sets the quantity of messages saved in each file.", Example = "--tweet-limit \"500\"", Group = "Execution", Order = 6)]
        public int TweetLimit { get; set; }

        [Arg("--restart-time", Alias = "-rst")]
        [Help("Sets the time in minutes of wait if the listen process stopped.", Example = "--restart-time 5.0", Group = "Execution", Order = 7)]
        public double RestartTime { get; set; }

        [Arg("--proxy", Alias = "-pxy")]
        [Help("Specifies a proxy Settings.", Example = "--proxy \"user:pass@proxy.com:445\"", Group = "Execution", Order = 8)]
        public string ProxyConnectionString { get; set; }

        [Arg("--show-capture", Alias = "-sc"), IfPresent]
        [Help("Displays the captured messages in the output.", Example = "--show-capture \"true\"", Group = "Execution", Order = 9)]
        public bool ShowCapture { get; set; }
    }
}
