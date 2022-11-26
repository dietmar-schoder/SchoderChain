using System.Threading.Tasks;

namespace SchoderChain
{
    public interface ISlackManager
    {
        Task SlackShopMessageAsync(string messageBody);

        Task SlackTestMessageAsync(string messageBody);

        Task SlackErrorAsync(string messageBody);
    }
}
