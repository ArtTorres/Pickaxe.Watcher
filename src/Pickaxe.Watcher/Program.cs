using QApp;
using QApp.Options;
using  Pickaxe.Watcher.IO.Twitter.Security;

namespace Pickaxe.Watcher
{
    class ApplicationOptions : QOption
    {
        [Option("--api-key", Alias = "-apk")]
        [Help("Sets the aplication key for Twitter authentication.", Example = "--api-key <code>", Group = "Authentication", Order = 20)]
        public string ApiKey { get; set; }

        [Option("--api-secret", Alias = "-aps")]
        [Help("Sets the aplication key for Twitter authentication.", Example = "--api-secret <code>", Group = "Authentication", Order = 21)]
        public string ApiSecret { get; set; }

        [Option("--token-key", Alias = "-tkk")]
        [Help("Sets the aplication key for Twitter authentication.", Example = "--token-key <code>", Group = "Authentication", Order = 22)]
        public string TokenKey { get; set; }

        [Option("--token-secret", Alias = "-tks")]
        [Help("Sets the aplication key for Twitter authentication.", Example = "--token-secret <code>", Group = "Authentication", Order = 23)]
        public string TokenSecret { get; set; }
    }

    class Application : QApplication
    {
        [OptionSet(1)]
        private ApplicationOptions AppOptions { get; set; }

        [OptionSet(2)]
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
                this.RegisterProcess(process);
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
