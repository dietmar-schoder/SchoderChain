using SchoderChain;
using System;
using System.Threading.Tasks;

namespace SchoderChainUnitTests
{
    public class TestProcessor1 : Processor
	{
        private readonly BLLData _bllData;

        public TestProcessor1(BLLData bllData, ChainData chainData, ISlackManager slackManager)
            : base(chainData, slackManager) => _bllData = bllData;

#pragma warning disable 1998
        protected override async Task<bool> ProcessOkAsync() => string.IsNullOrEmpty(_bllData.Email);

#pragma warning disable 1998
        protected override async Task UndoAsync() => _chainData.StackTrace.Add($"Undo{GetType().Name}");
    }
}
