using System.Threading.Tasks;

namespace SchoderChain
{
    public interface IProcessor
    {
        IProcessor Predecessor { get; set; }

        IProcessor Successor { get; set; }

        Task ProcessChainAsync(ChainResult chainResult);

        Task UndoChainAsync(ChainResult chainResult);
    }
}
