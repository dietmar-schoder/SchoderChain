using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SchoderChain
{
    public class Chain : IChain
	{
        private readonly ChainData _chainData;
		private readonly IEnumerable<IProcessor> _allProcessors;

        public Chain(ChainData chainData, IEnumerable<IProcessor> allProcessors)
		{
			_chainData = chainData;
			_allProcessors = allProcessors;
		}

        public async Task ProcessAsync(string calledBy, params Type[] processorChainTypes)
        {
			_chainData.Initialize(calledBy);
			var firstProcessor = FirstLinkedProcessor(processorChainTypes);
			if (firstProcessor == null) { return; }
			await firstProcessor.ProcessChainAsync();
        }

        private IProcessor FirstLinkedProcessor(Type[] processorChainTypes)
		{
			IProcessor firstProcessor = null, previousProcessor = null;

			foreach (var processorType in processorChainTypes)
            {
				var processor = _allProcessors.Single(p => p.GetType() == processorType);
				processor.Successor = null;
				processor.Predecessor = previousProcessor;
				if (processor.Predecessor == null)
				{
					firstProcessor = processor;
				}
				else
                {
					processor.Predecessor.Successor = processor;
				}
				previousProcessor = processor;
			}

			return firstProcessor;
		}
	}
}