using QApp.Options;

namespace Pickaxe.Watcher.Options
{
    class FilenameModeParser : IOptionParser
    {
        public object Parse(string value)
        {
            switch (value.ToLowerInvariant())
            {
                case "date":
                    return FilenameMode.Date;
                case "secuence":
                    return FilenameMode.Secuence;
                case "manual":
                default:
                    return FilenameMode.Manual;
            }
        }
    }
}
