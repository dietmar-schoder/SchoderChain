//// Install NuGet Package "SlackBotMessages by Paul Seal"

//using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
//using SchoderChain;
//using SlackBotMessages;
//using SlackBotMessages.Models;
//using System.Threading.Tasks;

//namespace MyProject.Helpers
//{
//    public class SlackManager : ISlackManager
//    {

//        public SlackManager() { }

//        [Obsolete]
//        public async Task SlackErrorAsync(string messageBody) { }

//        public async Task SlackErrorChainResultAsync(ChainResult chainResult)
//            => await new SbmClient(SlackSecrets.SLACK_WEBHOOKURL_ERROR).Send(
//                new Message
//                {
//                    Username = SlackSecrets.SLACK_USER,
//                    Text = chainResult.ToJson(),
//                    IconEmoji = Emoji.Bomb
//                });
//    }
//}
