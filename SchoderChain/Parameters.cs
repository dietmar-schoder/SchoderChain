using Microsoft.AspNetCore.Mvc;
using System.Reflection;
using System.Security.Claims;

namespace SchoderChain
{
    public class Parameters : IParameters
    {
        public string ChainStart { get; set; }

        public List<string> StackTrace { get; set; } = new List<string>();

        public Exception Exception { get; set; }

        public ActionResult ActionResult { get; set; }
    }
}