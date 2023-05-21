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
                await _slackManager.SlackErrorAsync($"{_chainResult.CalledBy}" +
					$"{Environment.NewLine}{new string('-', 20)}{Environment.NewLine}{string.Join(Environment.NewLine, _chainResult.StackTrace)}" +
                    $"{Environment.NewLine}{Environment.NewLine}{_chainResult.Exception.Message}{Environment.NewLine}{_chainResult.Exception.InnerException?.Message}");
            }
        }

		public async Task UndoChainAsync(ChainResult chainResult)
		{
			await UndoAsync();
			await (Predecessor?.UndoChainAsync(_chainResult) ?? Task.FromResult<object>(null));
		}

        protected async virtual Task<bool> ProcessOkAsync()
        {
            Process();
            await ProcessAsync();
            return true;
        }

        protected async virtual Task ProcessAsync() => await Task.CompletedTask;

        protected virtual void Process() { }

        protected virtual Task UndoAsync() => Task.CompletedTask;
    }
}