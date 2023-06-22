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
			try
			{
                _chainResult.StackTrace.Add(GetType().Name);
				if (!await ProcessOkAsync()) { return; }
				await (Successor?.ProcessChainAsync(_chainResult) ?? Task.FromResult<object>(null));
			}
			catch (Exception ex)
			{
                _chainResult.Exception = ex;
				await UndoChainAsync(_chainResult);
                await _slackManager.SlackErrorChainResultAsync(_chainResult);
            }
        }

		public async Task UndoChainAsync(ChainResult chainResult)
		{
			await UndoAsync();
			await (Predecessor?.UndoChainAsync(_chainResult) ?? Task.FromResult<object>(null));
		}

        protected async virtual Task<bool> ProcessOkAsync()
        {
            await ProcessAsync();
            Process();
            return ProcessOk();
        }

        protected async virtual Task ProcessAsync() => await Task.CompletedTask;

        protected virtual void Process() { }

        protected virtual bool ProcessOk() => true;

        protected virtual Task UndoAsync() => Task.CompletedTask;
    }
}