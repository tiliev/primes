using Microsoft.AspNetCore.Mvc;

namespace Primes.API.Controllers
{
    [ApiController]
    public class BaseController : ControllerBase
    {
        [NonAction]
        public BadRequestObjectResult BadRequest(string errorKey, string[] errors)
        {
            ValidationProblemDetails valProblems = new ValidationProblemDetails();
            valProblems.Errors.Add(errorKey, errors);
            return BadRequest(valProblems);
        }

        [NonAction]
        public BadRequestObjectResult BadRequest(string[] errors)
        {
            return BadRequest("", errors);
        }
    }
}
