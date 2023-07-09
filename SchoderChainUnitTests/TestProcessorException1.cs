using SchoderChain;

namespace SchoderChainUnitTests
{
    public class TestProcessorException1 : Processor
	{
        public TestProcessorException1(ISlackManager slackManager) : base(slackManager) { }

        protected override async Task<bool> ProcessOkAsync()
        {
            int zero = 0;
			int error = 1 / zero;
			return await Task.FromResult(true);
		}

        protected override async Task UndoAsync()
        {
            _chainResult.StackTrace.Add($"Undo{GetType().Name}");
            await Task.CompletedTask;
        }
    }
}
