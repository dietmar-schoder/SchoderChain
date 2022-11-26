using SchoderChain;
using System;
using System.Threading.Tasks;

namespace SchoderChainUnitTests
{
    public class TestProcessor1 : Processor
	{
		public TestProcessor1(ISlackManager slackManager) : base (slackManager) { }

#pragma warning disable 1998
		protected override async Task<bool> ProcessOkAsync<T>(T parameters)
#pragma warning restore 1998
		{
			(parameters as BLLParameters).Email = "test2";
			return parameters.ActionResult is null;
		}

#pragma warning disable 1998
		protected override async Task UndoAsync<BLLParameters>(BLLParameters parameters)
#pragma warning restore 1998
        {
            parameters.StackTrace.Add($"Undo{GetType().Name}");
        }
    }
}
