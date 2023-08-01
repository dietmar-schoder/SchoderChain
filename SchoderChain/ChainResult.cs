namespace SchoderChain
{
    public class ChainResult
    {
        public string CalledBy { get; set; }

        public List<string> StackTrace { get; set; }

        public Exception Exception { get; set; }

        public List<string> TrackTimePoints { get; set; }

        public void StartInterval() => _intervalStartTime = DateTime.UtcNow.Ticks;

        public void TrackInterval(string name)
        {
            var now = DateTime.UtcNow.Ticks;
            TrackTimePoints.Add($"{name} {IntervalMilliSeconds(now):#,##0} {TotalMilliSeconds(now):#,##0}");
        }

        private long _totalStartTime;
        private long _intervalStartTime;

        public ChainResult() { }

        private ChainResult(string calledBy, long startTime)
            => (CalledBy, _totalStartTime, StackTrace, TrackTimePoints) = (calledBy, startTime, new(), new());

        public static ChainResult Create(string calledBy, long startTime) => new ChainResult(calledBy, startTime);

        private float TotalMilliSeconds(long ticks) => (float)(ticks - _totalStartTime) / TimeSpan.TicksPerMillisecond;

        private float IntervalMilliSeconds(long ticks) => (float)(ticks - _intervalStartTime) / TimeSpan.TicksPerMillisecond;
    }
}
