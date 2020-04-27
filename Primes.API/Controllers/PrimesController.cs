using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Primes.Core.Services;
using System.Numerics;

namespace Primes.API.Controllers
{
    public class PrimesController : BaseController
    {
        public PrimesController(IPrimeService primeService)
        {
            this.primeService = primeService;
        }

        [HttpPost("primes/is-prime")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult IsPrime([FromBody]string number)
        {
            if (!BigInteger.TryParse(number, out BigInteger parsedNum))
            {
                return BadRequest("number", new string[] { "Please provide a number."});
            }

            var result = primeService.IsPrime(parsedNum);
            if (!result.Succeeded)
            {
                return BadRequest(new string[] { result.Error });
            }

            return Ok(result.Value);
        }

        [HttpPost("primes/find-next-prime")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult FindNextPrime([FromBody]string number)
        {
            if (!BigInteger.TryParse(number, out BigInteger parsedNum))
            {
                return BadRequest("number", new string[] { "Please provide a number." });
            }

            var result = primeService.FindNextPrime(parsedNum);
            if (!result.Succeeded)
            {
                return BadRequest(new string[] { result.Error });
            }

            return Ok(result.Value);
        }

        private readonly IPrimeService primeService;
    }
}
