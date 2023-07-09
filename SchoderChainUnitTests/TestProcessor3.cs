using SchoderChain;

namespace SchoderChainUnitTests
{
    public class TestProcessor3 : Processor
	{
        public TestProcessor3(ISlackManager slackManager) : base(slackManager) { }

        protected override async Task UndoAsync()
        {
            _chainResult.StackTrace.Add($"Undo{GetType().Name}");
            await Task.CompletedTask;
        }
    }
}
