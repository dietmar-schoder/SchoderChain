using SchoderChain;

namespace SchoderChainUnitTests
{
    public class TestProcessor1 : Processor
	{
        private readonly BLLData _bllData;

        public TestProcessor1(BLLData bllData, ISlackManager slackManager) : base(slackManager) => _bllData = bllData;

        protected override async Task<bool> ProcessOkAsync()
            => await Task.FromResult(string.IsNullOrEmpty(_bllData.Email)); // Continues the chain only if the email is null/empty

        protected override async Task UndoAsync()
        {
            _chainResult.StackTrace.Add($"Undo{GetType().Name}");
            await Task.CompletedTask;
        }
    }
}
