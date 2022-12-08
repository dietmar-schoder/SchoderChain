namespace SchoderChain
{
	public class Chain : IChain
	{
		private readonly IEnumerable<IProcessor> _allProcessors;

        public ChainResult ChainResult { get; set; }

        public Chain(IEnumerable<IProcessor> allProcessors) => _allProcessors = allProcessors;

        public async Task<ChainResult> ProcessAsync(string calledBy, params Type[] processorChainTypes)
        {
            ChainResult = ChainResult.Create(calledBy);
            await (FirstLinkedProcessor(processorChainTypes)?.ProcessChainAsync(ChainResult) ?? Task.FromResult<ChainResult>(null));
            return ChainResult;
        }

        private IProcessor FirstLinkedProcessor(Type[] processorChainTypes)
		{
			IProcessor firstProcessor = null, previousProcessor = null;

			foreach (var processorType in processorChainTypes)
            {
				var processor = _allProcessors.Single(p => p.GetType() == processorType);
				processor.Successor = null;
				processor.Predecessor = previousProcessor;
				if (processor.Predecessor != null)
                {
					processor.Predecessor.Successor = processor;
				}
				firstProcessor = firstProcessor ?? processor;
				previousProcessor = processor;
			}

			return firstProcessor;
		}
	}
}
