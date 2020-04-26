using Primes.Core.Communication;
using Primes.Core.DTOs;
using System;
using System.Numerics;
using System.Security.Cryptography;

namespace Primes.Core.Services
{
    /// <summary>
    /// Service for primality check and finding the next prime.
    /// </summary>
    public class PrimeService : IPrimeService
    {
        /// <summary>
        /// Primality test.
        /// For 64-bit numbers it uses linear algorithm with 100% accuracy.
        /// For bigger numbers up to 128 bytes it uses Miller-Rabin test.
        /// </summary>
        /// <param name="num">The number for the primality test. Its size should be <= 128 bytes.</param>
        /// <returns>Whether the number is prime or not.</returns>
        public OperationResult<PrimalityTestResult> IsPrime(BigInteger num)
        {
            if (num.GetByteCount() > MILLER_RABIN_NUM_BYTES_LIMIT)
            {
                return new OperationResult<PrimalityTestResult>(
                    false, $"Cannot test numbers with size > {MILLER_RABIN_NUM_BYTES_LIMIT} bytes.");
            }

            if (num.Sign == -1)
            {
                return new OperationResult<PrimalityTestResult>(
                    true, new PrimalityTestResult(false, 1.0));
            }

            if (num <= new BigInteger(ulong.MaxValue))
            {
                return new OperationResult<PrimalityTestResult>(true,
                    new PrimalityTestResult(IsPrime((ulong)num), 1.0));
            }

            double accuracy = 1 - Math.Pow(4, -MILLER_RABIN_CERTAINTY);
            bool isProbablePrime = IsProbablePrime(num, MILLER_RABIN_CERTAINTY);
            PrimalityTestResult result;
            if (isProbablePrime)
            {
                result = new PrimalityTestResult(true, accuracy);
            }
            else
            {
                result = new PrimalityTestResult(false, 1.0);
            }

            return new OperationResult<PrimalityTestResult>(true, result);
        }

        /// <summary>
        /// Finds next prime after a given number.
        /// The size of the next prime should be <= 128 bytes.
        /// For 64-bit numbers it uses linear algorithm with 100% accuracy.
        /// For bigger numbers up to 128 bytes it uses Miller-Rabin test.
        /// </summary>
        /// <param name="num">The number after which the next prime is searched for.
        /// Its size should be < 128 bytes.
        /// </param>
        /// <returns>The next prime.</returns>
        public OperationResult<NextPrimeResult> FindNextPrime(BigInteger num)
        {
            if (num.GetByteCount() >= MILLER_RABIN_NUM_BYTES_LIMIT)
            {
                return new OperationResult<NextPrimeResult>(
                    false, $"The number size should be < {MILLER_RABIN_NUM_BYTES_LIMIT} bytes.");
            }

            if (num < 2)
            {
                return new OperationResult<NextPrimeResult>(
                    true, new NextPrimeResult(2, 1.0));
            }

            if (num < new BigInteger(ulong.MaxValue))
            {
                try
                {
                    ulong smallNum = (ulong)num;
                    ulong nextPrime = FindNextPrime(smallNum);
                    return new OperationResult<NextPrimeResult>(true,
                            new NextPrimeResult(nextPrime, 1.0));
                }
                catch (OverflowException)
                {
                    num = ulong.MaxValue;
                }
            }

            return FindNextProbablePrime(num);
        }

        /// <summary>
        /// Finds next prime.
        /// </summary>
        /// <exception cref="OverflowException">
        /// When the next prime overflows ulong
        /// </exception>
        /// <param name="num">The number after which the next prime is searched for.</param>
        /// <returns>Returns the next prime</returns>
        private ulong FindNextPrime(ulong num)
        {
            while (true)
            {
                checked
                {
                    num++;
                }

                if (IsPrime(num))
                {
                    return num;
                }
            }
        }

        /// <summary>
        /// Finds next prime. 
        /// The size of the next prime should be <= 128 bytes.
        /// </summary>
        /// <param name="num">The number after which the next prime is searched for.</param>
        /// <returns>Returns the next prime.</returns>
        private OperationResult<NextPrimeResult> FindNextProbablePrime(BigInteger num)
        {
            while (true)
            {
                num++;
                if (num.GetByteCount() > MILLER_RABIN_NUM_BYTES_LIMIT)
                {
                    return new OperationResult<NextPrimeResult>(false,
                        $"The size of the next prime should be <= than {MILLER_RABIN_NUM_BYTES_LIMIT} bytes");
                }

                if (IsProbablePrime(num, MILLER_RABIN_CERTAINTY))
                {
                    return new OperationResult<NextPrimeResult>(true,
                        new NextPrimeResult(num, 1 - Math.Pow(4, -MILLER_RABIN_CERTAINTY)));
                }
            }
        }

        /// <summary>
        /// Primality test with linear complexity for 64-bit numbers.
        /// Reference: https://en.wikipedia.org/wiki/Primality_test
        /// </summary>
        /// <param name="num">The number for the primality test.</param>
        /// <returns>Whether the number is prime or not.</returns>
        private bool IsPrime(ulong num)
        {
            if (num <= 3)
            {
                return num > 1;
            }
            else if (num % 2 == 0 || num % 3 == 0)
            {
                return false;
            }

            ulong limit = (ulong)Math.Floor(Math.Sqrt(num));


            for (ulong i = 5; i <= limit; i += 6)
            {
                if (num % i == 0 || num % (i + 2) == 0)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Miller-Rabin primality test.
        /// Reference: https://en.wikipedia.org/wiki/Miller%E2%80%93Rabin_primality_test
        /// Code: http://rosettacode.org/wiki/Miller-Rabin_primality_test#C.23
        /// Complexity: O(certainty*log3n)
        /// </summary>
        /// <param name="num">The number for the primality test.</param>
        /// <param name="certainty">How many random bases to try.</param>
        /// <returns>Whether the number is prime or not.</returns>
        private bool IsProbablePrime(BigInteger num, int certainty)
        {
            if (num == 2 || num == 3)
            {
                return true;
            }

            if (num < 2 || num % 2 == 0)
            {
                return false;
            }

            BigInteger d = num - 1;
            int s = 0;

            while (d % 2 == 0)
            {
                d /= 2;
                s += 1;
            }

            // There is no built-in method for generating random BigInteger values.
            // Instead, random BigIntegers are constructed from randomly generated
            // byte arrays of the same length as the source.
            RandomNumberGenerator rng = RandomNumberGenerator.Create();
            byte[] bytes = new byte[num.ToByteArray().LongLength];
            BigInteger a;

            for (int i = 0; i < certainty; i++)
            {
                do
                {
                    // This may raise an exception in Mono 2.10.8 and earlier.
                    // http://bugzilla.xamarin.com/show_bug.cgi?id=2761
                    rng.GetBytes(bytes);
                    a = new BigInteger(bytes);
                }
                while (a < 2 || a >= num - 2);

                BigInteger x = BigInteger.ModPow(a, d, num);
                if (x == 1 || x == num - 1)
                {
                    continue;
                }

                for (int r = 1; r < s; r++)
                {
                    x = BigInteger.ModPow(x, 2, num);
                    if (x == 1)
                    {
                        return false;
                    }

                    if (x == num - 1)
                    {
                        break;
                    }
                }

                if (x != num - 1)
                {
                    return false;
                }
            }

            return true;
        }

        private const int MILLER_RABIN_CERTAINTY = 10;
        private const int MILLER_RABIN_NUM_BYTES_LIMIT = 128;
    }
}
