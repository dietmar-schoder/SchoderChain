using System;
using System.Threading.Tasks;

namespace SchoderChain
{
    public interface IChain
    {
        Task ProcessAsync(params Type[] processorChain);
    }
}
