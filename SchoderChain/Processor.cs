using System;
using System.Threading.Tasks;

namespace SchoderChain
{
    public class Processor : IProcessor
	{
		public IProcessor Predecessor { get; set; }

		public IProcessor Successor { get; set; }

        protected readonly ChainData _chainData;
        protected readonly ISlackManager _slackManager;

		public Processor(ChainData chainData, ISlackManager slackManager)
		{
            _slackManager = slackManager;
            _chainData = chainData;
        }

        public async Task ProcessChainAsync()
		{
			try
			{
                _chainData.StackTrace.Add(GetType().Name);
				var ok = await ProcessOkAsync();
				if (ok && Successor is not null)
				{
					await Successor.ProcessChainAsync();
				}
			}
			catch (Exception ex)
			{
                _chainData.Exception = ex;
				await UndoChainAsync();
                await _slackManager.SlackErrorAsync($"{_chainData.CalledBy}" +
					$"{Environment.NewLine}{new string('-', 20)}{Environment.NewLine}{string.Join(Environment.NewLine, _chainData.StackTrace)}" +
                    $"{Environment.NewLine}{Environment.NewLine}{_chainData.Exception.Message}{Environment.NewLine}{_chainData.Exception.InnerException?.Message}");
            }
        }

		public async Task UndoChainAsync()
		{
			await UndoAsync();
			if (Predecessor is not null)
			{
				await Predecessor.UndoChainAsync();
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