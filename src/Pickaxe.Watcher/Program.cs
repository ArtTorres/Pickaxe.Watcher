using QApp;
using Pickaxe.Watcher.IO.Twitter.Security;
using MagnetArgs;
using QApp.Documentation;

namespace Pickaxe.Watcher
{
    class ApplicationOptions : MagnetOption
    {
        [Arg("--api-key", Alias = "-apk")]
        [Help("Sets the aplication key for Twitter authentication.", Example = "--api-key <code>", Group = "Authentication", Order = 20)]
        public string ApiKey { get; set; }

        [Arg("--api-secret", Alias = "-aps")]
        [Help("Sets the aplication key for Twitter authentication.", Example = "--api-secret <code>", Group = "Authentication", Order = 21)]
        public string ApiSecret { get; set; }

        [Arg("--token-key", Alias = "-tkk")]
        [Help("Sets the aplication key for Twitter authentication.", Example = "--token-key <code>", Group = "Authentication", Order = 22)]
        public string TokenKey { get; set; }

        [Arg("--token-secret", Alias = "-tks")]
        [Help("Sets the aplication key for Twitter authentication.", Example = "--token-secret <code>", Group = "Authentication", Order = 23)]
        public string TokenSecret { get; set; }
    }

    class Application : QApplication
    {
        [OptionSet]
        private ApplicationOptions AppOptions { get; set; }

        [OptionSet]
        private StreamListenerOptions ProcessOptions { get; set; }

        public override void ExecutionProcess()
        {
            if (
                !string.IsNullOrEmpty(this.AppOptions.ApiKey) &&
                !string.IsNullOrEmpty(this.AppOptions.ApiSecret) &&
                !string.IsNullOrEmpty(this.AppOptions.TokenKey) &&
                !string.IsNullOrEmpty(this.AppOptions.TokenSecret)
            )
            {
                this.ProcessOptions.Credential = new TwitterCredential()
                {
                    ApiKey = this.AppOptions.ApiKey,
                    ApiSecret = this.AppOptions.ApiSecret,
                    TokenKey = this.AppOptions.TokenKey,
                    TokenSecret = this.AppOptions.TokenSecret
                };
            }
            else
            {
                this.ProcessOptions.Credential = new TwitterCredential()
                {
                    ApiKey = Properties.Settings.Default.APIKEY,
                    ApiSecret = Properties.Settings.Default.APISECRET,
                    TokenKey = Properties.Settings.Default.TOKENKEY,
                    TokenSecret = Properties.Settings.Default.TOKENSECRET
                };
            }

            using (var process = new StreamListener(this.ProcessOptions))
            {
                this.MonitorTask(process);
                process.Start();
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var application = new Application();
            application.Execute(args);
        }
    }
}
