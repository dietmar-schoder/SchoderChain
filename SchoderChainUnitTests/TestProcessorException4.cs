using SchoderChain;

namespace SchoderChainUnitTests
{
    public class TestProcessorException4 : Processor
	{
        public TestProcessorException4(ISlackManager slackManager) : base(slackManager) { }

        protected override void Process()
        {
            int zero = 0;
			int error = 1 / zero;
		}

        protected override async Task UndoAsync()
        {
            _chainResult.StackTrace.Add($"Undo{GetType().Name}");
            await Task.CompletedTask;
        }
    }
}
