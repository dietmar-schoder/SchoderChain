namespace SchoderChain
{
    public class ChainResult
    {
        public string CalledBy { get; set; }

        public List<string> StackTrace { get; set; } = new List<string>();

        public Exception Exception { get; set; }

        public static ChainResult Create(string calledBy) => new ChainResult { CalledBy = calledBy };
    }
}