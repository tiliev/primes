using System.Numerics;

namespace Primes.Core.DTOs
{
    public class NextPrimeResult
    {
        public NextPrimeResult(BigInteger nextPrime, double accuracy)
        {
            NextPrime = nextPrime.ToString();
            Accuracy = accuracy;
        }

        /// <summary>
        /// The next prime.
        /// </summary>
        public string NextPrime { get; }

        /// <summary>
        /// Shows how accurate is the algorithm of finding the next prime.
        /// The value could be from 0.0 (0%) to 1.0 (100%).
        /// </summary>
        public double Accuracy { get; }
    }
}
