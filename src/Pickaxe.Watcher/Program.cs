using EasyApp;

namespace Pickaxe.Watcher
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var task = new StreamListener())
            {
                AppRunner.Execute<BasicApp>(args, task);
            }
        }
    }
}
