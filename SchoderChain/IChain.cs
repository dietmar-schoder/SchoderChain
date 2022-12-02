using System;
using System.Threading.Tasks;

namespace SchoderChain
{
    public interface IChain
    {
        Task ProcessAsync(string calledBy, params Type[] processorChain);
    }
}
