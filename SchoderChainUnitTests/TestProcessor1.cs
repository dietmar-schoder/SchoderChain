using SchoderChain;
using System;
using System.Threading.Tasks;

namespace SchoderChainUnitTests
{
    public class TestProcessor1 : Processor
	{
        private readonly BLLData _bllData;
        
		public TestProcessor1(BLLData bllData, ChainData chainData, ISlackManager slackManager)
			: base (chainData, slackManager)
		{
			_bllData = bllData;
		}

#pragma warning disable 1998
		protected override async Task<bool> ProcessOkAsync()
#pragma warning restore 1998
		{
            _bllData.Email = "test2";
			return _chainData.ActionResult is null;
		}

#pragma warning disable 1998
		protected override async Task UndoAsync()
#pragma warning restore 1998
        {
            _chainData.StackTrace.Add($"Undo{GetType().Name}");
        }
    }
}
