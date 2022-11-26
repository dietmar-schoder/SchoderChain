using Microsoft.AspNetCore.Mvc;

namespace SchoderChain
{
    public interface IParameters
    {
        string ChainStart { get; set; }

        List<string> StackTrace { get; set; }

        Exception Exception { get; set; }

        ActionResult ActionResult { get; set; }
    }
}