using System.Threading.Tasks;

namespace SchoderChain
{
    public interface IProcessor
    {
        IProcessor Predecessor { get; set; }

        IProcessor Successor { get; set; }

        Task ProcessChainAsync(Parameters parameters);

        Task UndoChainAsync(Parameters parameters);
    }
}
