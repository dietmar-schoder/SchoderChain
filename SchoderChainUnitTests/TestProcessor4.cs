using SchoderChain;

namespace SchoderChainUnitTests
{
    public class TestProcessor4 : Processor
	{
        public TestProcessor4(ISlackManager slackManager) : base(slackManager) { }

        protected override async Task UndoAsync()
        {
            _chainResult.StackTrace.Add($"Undo{GetType().Name}");
            await Task.CompletedTask;
        }
    }
}
