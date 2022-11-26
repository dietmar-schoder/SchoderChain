using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SchoderChain
{
    public class Chain : IChain
	{
		private readonly IEnumerable<IProcessor> _allProcessors;

        public Chain(IEnumerable<IProcessor> allProcessors) => _allProcessors = allProcessors;

        public async Task ProcessAsync(Parameters parameters, params Type[] processorChainTypes)
        {
			var firstProcessor = FirstLinkedProcessor(processorChainTypes);
			if (firstProcessor == null) { return; }
			await firstProcessor.ProcessChainAsync(parameters);
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