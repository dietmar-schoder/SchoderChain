using SchoderChain;
using System;
using System.Threading.Tasks;

namespace SchoderChainUnitTests
{
    public class TestProcessor1 : Processor
	{
        private readonly BLLData _bllData;

        public TestProcessor1(BLLData bllData, ISlackManager slackManager) : base(slackManager) => _bllData = bllData;

#pragma warning disable 1998
        protected override async Task<bool> ProcessOkAsync() => string.IsNullOrEmpty(_bllData.Email); // Continues the chain only if the email is null/empty

#pragma warning disable 1998
        protected override async Task UndoAsync() => _chainResult.StackTrace.Add($"Undo{GetType().Name}");
    }
}
