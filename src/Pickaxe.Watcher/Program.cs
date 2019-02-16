using EasyApp;

namespace Pickaxe.Watcher
{
    class Program
    {
        static void Main(string[] args)
        {
            AppRunner.Execute<BasicApp>(args, new StreamListener());
        }
    }
}
