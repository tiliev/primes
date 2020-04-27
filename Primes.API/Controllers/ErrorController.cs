using Microsoft.AspNetCore.Mvc;

namespace Primes.API.Controllers
{
    public class ErrorController : BaseController
    {
        [HttpPost("/error")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult Error() => Problem();
    }
}