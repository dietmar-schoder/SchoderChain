namespace SchoderChain
{
    public class Processor : IProcessor
	{
		public IProcessor Predecessor { get; set; }

		public IProcessor Successor { get; set; }

        protected readonly ISlackManager _slackManager;
        protected ChainResult _chainResult;

        public Processor(ISlackManager slackManager) => _slackManager = slackManager;

        public async Task ProcessChainAsync(ChainResult chainResult)
		{
            _chainResult = chainResult;
            _chainResult.StartInterval();
            var processorName = GetType().Name;
			try
			{
                _chainResult.StackTrace.Add(processorName);
                if (!await ProcessOkAsync())
                {
                    _chainResult.TrackInterval(processorName);
                    return;
                }
                if (!ProcessOk())
                {
                    _chainResult.TrackInterval(processorName);
                    return;
                }
                await ProcessAsync();
                Process();
                _chainResult.TrackInterval(processorName);

                await (Successor?.ProcessChainAsync(_chainResult) ?? Task.CompletedTask);
			}
			catch (Exception ex)
			{
                _chainResult.TrackInterval(processorName);
                _chainResult.Exception = ex;
				await UndoChainAsync(_chainResult);
                await _slackManager.SlackErrorChainResultAsync(_chainResult);
            }
        }

		public async Task UndoChainAsync(ChainResult chainResult)
		{
			await UndoAsync();
			await (Predecessor?.UndoChainAsync(_chainResult) ?? Task.CompletedTask);
		}

        protected async virtual Task<bool> ProcessOkAsync() => await Task.FromResult(true);

        protected virtual bool ProcessOk() => true;

        protected async virtual Task ProcessAsync() => await Task.CompletedTask;

        protected virtual void Process() { }

        protected async virtual Task UndoAsync() => await Task.CompletedTask;
    }
}
