using SchoderChain;
using System.Threading.Tasks;

namespace SchoderChainUnitTests
{
    public class ChangeEmailProcessor : Processor
	{
        private readonly BLLData _bllData;

        public ChangeEmailProcessor(BLLData bllData, ChainData chainData, ISlackManager slackManager)
            : base(chainData, slackManager) => _bllData = bllData;

#pragma warning disable 1998
        protected override async Task<bool> ProcessOkAsync()
#pragma warning restore 1998
        {
            _bllData.Email = "changed";
            return true;
		}

#pragma warning disable 1998
        protected override async Task UndoAsync() => _chainData.StackTrace.Add($"Undo{GetType().Name}");
    }
}
