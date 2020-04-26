namespace Primes.Core.DTOs
{
    public class PrimalityTestResult
    {
        public PrimalityTestResult(bool isPrime, double accuracy)
        {
            IsPrime = isPrime;
            Accuracy = accuracy;
        }

        /// <summary>
        /// Whether the number is prime.
        /// </summary>
        public bool IsPrime { get; }

        /// <summary>
        /// Shows how accurate is the primality test.
        /// The value could be from 0.0 (0%) to 1.0 (100%).
        /// </summary>
        public double Accuracy { get; }
    }
}
