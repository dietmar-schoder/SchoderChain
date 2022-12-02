using SchoderChain;
using System.Threading.Tasks;

namespace SchoderChainUnitTests
{
    public class TestProcessor4 : Processor
	{
        public TestProcessor4(ChainData chainData, ISlackManager slackManager) : base(chainData, slackManager) { }

#pragma warning disable 1998
        protected override async Task UndoAsync() => _chainData.StackTrace.Add($"Undo{GetType().Name}");
    }
}
