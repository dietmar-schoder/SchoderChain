using SchoderChain;

namespace SchoderChainUnitTests
{
	public class TestProcessorException : Processor
	{
        public TestProcessorException(ChainData chainData, ISlackManager slackManager) : base(chainData, slackManager) { }

#pragma warning disable 1998
        protected override async Task<bool> ProcessOkAsync()
#pragma warning restore 1998
        {
            int zero = 0;
			int error = 1 / zero;
			return true;
		}

#pragma warning disable 1998
        protected override async Task UndoAsync()
#pragma warning restore 1998
        {
            _chainData.StackTrace.Add($"Undo{GetType().Name}");
        }
    }
}
