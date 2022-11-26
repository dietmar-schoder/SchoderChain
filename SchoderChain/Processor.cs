using System;
using System.Threading.Tasks;

namespace SchoderChain
{
    public class Processor : IProcessor
	{
		public IProcessor Predecessor { get; set; }

		public IProcessor Successor { get; set; }

        protected readonly ISlackManager _slackManager;

        public Processor(ISlackManager slackManager) => _slackManager = slackManager;

        public async Task ProcessChainAsync(Parameters parameters)
		{
			try
			{
				parameters.StackTrace.Add(GetType().Name);
				var ok = await ProcessOkAsync(parameters);
				if (ok && Successor is not null)
				{
					await Successor.ProcessChainAsync(parameters);
				}
			}
			catch (Exception ex)
			{
				parameters.Exception = ex;
				await UndoChainAsync(parameters);
                await _slackManager.SlackErrorAsync($"{parameters.ChainStart}" +
					$"{Environment.NewLine}{new string('-', 20)}{Environment.NewLine}{string.Join(Environment.NewLine, parameters.StackTrace)}" +
                    $"{Environment.NewLine}{Environment.NewLine}{parameters.Exception.Message}{Environment.NewLine}{parameters.Exception.InnerException?.Message}");
            }
        }

		public async Task UndoChainAsync(Parameters parameters)
		{
			await UndoAsync(parameters);
			if (Predecessor is not null)
			{
				await Predecessor.UndoChainAsync(parameters);
			}
		}

#pragma warning disable 1998
		protected async virtual Task<bool> ProcessOkAsync<T>(T parameters) where T : IParameters => true;
#pragma warning restore 1998

#pragma warning disable 1998
		protected async virtual Task UndoAsync<T>(T parameters) where T : IParameters { }
#pragma warning restore 1998
    }
}