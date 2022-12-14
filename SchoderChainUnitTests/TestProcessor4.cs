using SchoderChain;
using System.Threading.Tasks;

namespace SchoderChainUnitTests
{
    public class TestProcessor4 : Processor
	{
        public TestProcessor4(ISlackManager slackManager) : base(slackManager) { }

#pragma warning disable 1998
        protected override async Task UndoAsync() => _chainResult.StackTrace.Add($"Undo{GetType().Name}");
    }
}
