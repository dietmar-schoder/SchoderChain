using System;
using System.Threading.Tasks;

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
				var ok = await ProcessOkAsync();
				if (ok && Successor is not null)
				{
					await Successor.ProcessChainAsync(_chainResult);
				}
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
			if (Predecessor is not null)
			{
				await Predecessor.UndoChainAsync(_chainResult);
			}
		}

#pragma warning disable 1998
		protected async virtual Task<bool> ProcessOkAsync() => true;
#pragma warning restore 1998

#pragma warning disable 1998
		protected async virtual Task UndoAsync() { }
#pragma warning restore 1998
    }
}