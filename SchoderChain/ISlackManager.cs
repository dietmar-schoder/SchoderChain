namespace SchoderChain
{
    public interface ISlackManager
    {
        Task SlackErrorAsync(string messageBody);
    }
}
