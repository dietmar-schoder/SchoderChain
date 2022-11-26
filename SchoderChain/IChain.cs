using System;
using System.Threading.Tasks;

namespace SchoderChain
{
    public interface IChain
    {
        Task ProcessAsync(Parameters parameters, params Type[] processorChain);
    }
}
