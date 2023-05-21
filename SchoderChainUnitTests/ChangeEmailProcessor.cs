using SchoderChain;
using System.Threading.Tasks;

namespace SchoderChainUnitTests
{
    public class ChangeEmailProcessor : Processor
	{
        private readonly BLLData _bllData;

        public ChangeEmailProcessor(BLLData bllData, ISlackManager slackManager) : base(slackManager) => _bllData = bllData;

        protected override void Process() => _bllData.Email = "changed";

        protected override Task UndoAsync()
        {
            _chainResult.StackTrace.Add($"Undo{GetType().Name}");
            return Task.CompletedTask;
        }
    }
}
