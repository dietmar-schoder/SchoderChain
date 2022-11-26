using Microsoft.AspNetCore.Mvc;
using System.Reflection;
using System.Security.Claims;

namespace SchoderChain
{
    public class ChainData
    {
        public string ChainStart { get; set; }

        public List<string> StackTrace { get; set; }

        public Exception Exception { get; set; }

        public ActionResult ActionResult { get; set; }

        public void Initialize(string chainStart)
        {
            ChainStart = chainStart;
            StackTrace = new List<string>();
            Exception = null;
            ActionResult = null;
        }
    }
}