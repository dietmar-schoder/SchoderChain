using SchoderChain;

namespace SchoderChainUnitTests
{
    public class TestProcessorException2 : Processor
	{
        public TestProcessorException2(ISlackManager slackManager) : base(slackManager) { }

        protected override bool ProcessOk()
        {
            int zero = 0;
			int error = 1 / zero;
			return true;
		}

        protected override async Task UndoAsync()
        {
            _chainResult.StackTrace.Add($"Undo{GetType().Name}");
            await Task.CompletedTask;
        }
    }
}
