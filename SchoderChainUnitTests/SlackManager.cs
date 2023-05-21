// Install NuGet Package "SlackBotMessages by Paul Seal"

//using SchoderChain;
//using SlackBotMessages;
//using SlackBotMessages.Models;
//using System.Threading.Tasks;

//namespace MyProject.Helpers
//{
//    public class SlackManager : ISlackManager
//    {

//        public SlackManager() { }

//        public async Task SlackErrorAsync(string messageBody) => await SlackMessageAsync(SlackSecrets.SLACK_WEBHOOKURL_ERROR, messageBody, Emoji.Bomb);

//        private async Task SlackMessageAsync(string slackWebHookUrl, string messageBody, string emoji)
//            => await new SbmClient(slackWebHookUrl).Send(new Message
//                {
//                    Username = "Shop",
//                    Text = messageBody,
//                    IconEmoji = emoji
//            });
//    }
//}
