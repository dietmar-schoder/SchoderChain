using SchoderChain;

namespace SchoderChainUnitTests
{
    public class TestProcessorException3 : Processor
	{
        public TestProcessorException3(ISlackManager slackManager) : base(slackManager) { }

        protected async override Task ProcessAsync()
        {
            int zero = 0;
			int error = 1 / zero;
			await Task.CompletedTask;
		}

        protected override async Task UndoAsync()
        {
            _chainResult.StackTrace.Add($"Undo{GetType().Name}");
            await Task.CompletedTask;
        }
    }
}
