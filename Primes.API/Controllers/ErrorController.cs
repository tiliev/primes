using Microsoft.AspNetCore.Mvc;

namespace Primes.API.Controllers
{
    public class ErrorController : BaseController
    {
        [Route("/error")]
        public IActionResult Error() => Problem();
    }
}