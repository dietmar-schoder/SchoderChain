using SchoderChain;
using System.Threading.Tasks;

namespace SchoderChainUnitTests
{
    public class TestProcessor4 : Processor
	{
        public TestProcessor4(ISlackManager slackManager) : base(slackManager) { }

#pragma warning disable 1998
        protected override async Task<bool> ProcessOkAsync<BLLParameters>(BLLParameters parameters)
#pragma warning restore 1998
        {
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
