using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Primes.Core.DTOs;
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

        /// <summary>
        /// Primality test.
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// POST /primes/is-prime
        /// "29"
        /// </remarks>
        /// <param name="number">The number for the primality test. Its size should be &lt;= 128 bytes.</param>
        /// <returns>Whether the number is prime or not.</returns>
        /// <response code="400">If you do not provide a number or the number you provided is &gt; 128 bytes.</response>
        /// <response code="200">
        /// Returns a primality test object showing whether the number is prime. 
        /// Also, it shows the accuracy of the test. The value could be from 0.0 (0%) to 1.0 (100%).
        /// </response>
        [HttpPost("primes/is-prime")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PrimalityTestResult))]
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

        /// <summary>
        /// Finds next prime after a given number.
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// POST /primes/find-next-prime
        /// "29"
        /// </remarks>
        /// <param name="number">The number after which the next prime is searched for.
        /// Its size should be &lt; 128 bytes.</param>
        /// <returns>The next prime.</returns>
        /// <response code="400">If you do not provide a number, the number you provided is &gt; 128 bytes
        /// or the size of the next prime is &gt; 128 bytes.</response>
        /// <response code="200">
        /// Returns the next prime.
        /// Also, it shows the accuracy of the algorithm. The value could be from 0.0 (0%) to 1.0 (100%).
        /// </response>
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
