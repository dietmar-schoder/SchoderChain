using SchoderChain;
using System.Threading.Tasks;

namespace SchoderChainUnitTests
{
    public class TestProcessor3 : Processor
	{
        public TestProcessor3(ISlackManager slackManager) : base(slackManager) { }

#pragma warning disable 1998
        protected override async Task UndoAsync() => _chainResult.StackTrace.Add($"Undo{GetType().Name}");
    }
}
