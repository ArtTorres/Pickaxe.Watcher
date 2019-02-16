using EasyApp.Documentation;
using MagnetArgs;
using Pickaxe.Watcher.IO.Twitter.Security;

namespace Pickaxe.Watcher
{
    class SecurityOptions : MagnetSet
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

        public TwitterCredential Credential
        {
            get
            {
                if (
                    !string.IsNullOrEmpty(this.ApiKey) &&
                    !string.IsNullOrEmpty(this.ApiSecret) &&
                    !string.IsNullOrEmpty(this.TokenKey) &&
                    !string.IsNullOrEmpty(this.TokenSecret)
                )
                {
                    return new TwitterCredential()
                    {
                        ApiKey = this.ApiKey,
                        ApiSecret = this.ApiSecret,
                        TokenKey = this.TokenKey,
                        TokenSecret = this.TokenSecret
                    };
                }
                else
                {
                    return new TwitterCredential()
                    {
                        ApiKey = Properties.Settings.Default.APIKEY,
                        ApiSecret = Properties.Settings.Default.APISECRET,
                        TokenKey = Properties.Settings.Default.TOKENKEY,
                        TokenSecret = Properties.Settings.Default.TOKENSECRET
                    };
                }
            }
        }
    }
}
