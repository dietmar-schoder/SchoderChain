using SlackBotMessages;
using SlackBotMessages.Models;
using System.Threading.Tasks;

namespace SchoderChain
{
    public class SlackManager : ISlackManager
    {
        private const string SlackWebHookUrlShop = "https://hooks.slack.com/services/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";
        private const string SlackWebHookUrlTest = "https://hooks.slack.com/services/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";
        private const string SlackWebHookUrlError = "https://hooks.slack.com/services/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";

        public SlackManager() { }

        public async Task SlackShopMessageAsync(string messageBody) => await SlackMessageAsync(SlackWebHookUrlShop, messageBody, Emoji.Dollar);

        public async Task SlackTestMessageAsync(string messageBody) => await SlackMessageAsync(SlackWebHookUrlTest, messageBody, Emoji.Question);

        public async Task SlackErrorAsync(string messageBody) => await SlackMessageAsync(SlackWebHookUrlError, messageBody, Emoji.Bomb);

        private async Task SlackMessageAsync(string slackWebHookUrl, string messageBody, string emoji)
            => await new SbmClient(slackWebHookUrl).Send(new Message
                {
                    Username = "Shop",
                    Text = messageBody,
                    IconEmoji = emoji
            });
    }
}
