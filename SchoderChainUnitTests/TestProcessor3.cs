using SchoderChain;
using System.Threading.Tasks;

namespace SchoderChainUnitTests
{
    public class TestProcessor3 : Processor
	{
        public TestProcessor3(ChainData chainData, ISlackManager slackManager) : base(chainData, slackManager) { }

#pragma warning disable 1998
        protected override async Task<bool> ProcessOkAsync()
#pragma warning restore 1998
        {
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
