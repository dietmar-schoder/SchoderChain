using SchoderChain;

namespace SchoderChainUnitTests
{
	public class TestProcessorException : Processor
	{
        public TestProcessorException(ISlackManager slackManager) : base(slackManager) { }

#pragma warning disable 1998
        protected override async Task<bool> ProcessOkAsync<BLLParameters>(BLLParameters parameters)
#pragma warning restore 1998
        {
            int zero = 0;
			int error = 1 / zero;
			return true;
		}

#pragma warning disable 1998
        protected override async Task UndoAsync<BLLParameters>(BLLParameters parameters)
#pragma warning restore 1998
        {
            parameters.StackTrace.Add($"Undo{GetType().Name}");
		}
	}
}
