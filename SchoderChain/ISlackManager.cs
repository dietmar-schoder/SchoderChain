namespace SchoderChain
{
    public interface ISlackManager
    {
        Task SlackErrorChainResultAsync(ChainResult chainResult);
    }
}
