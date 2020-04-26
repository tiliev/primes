using Microsoft.VisualStudio.TestTools.UnitTesting;
using Primes.Core.Communication;
using Primes.Core.DTOs;
using Primes.Core.Services;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Primes.Core.Tests
{
    [TestClass]
    public class PrimeServiceTests
    {
        [TestInitialize]
        public void Init()
        {
            primeService = new PrimeService();
        }

        [TestMethod]
        public void IsPrime_NumBiggerThan128Bytes()
        {
            // https://primes.utm.edu/curios/page.php?number_id=1757 132 bytes
            string primeNum = new string('1', 317);

            bool expectedOperationSuccess = false;
            bool actual = primeService.IsPrime(BigInteger.Parse(primeNum)).Succeeded;

            Assert.AreEqual(expectedOperationSuccess, actual);
        }

        [TestMethod]
        public void IsPrime_NegativeNumber()
        {
            var result = primeService.IsPrime(new BigInteger(int.MinValue));
            TestNonPrimeWithFullAccuracy(result);
        }

        [TestMethod]
        public void IsPrime_Zero()
        {
            var result = primeService.IsPrime(new BigInteger(0));
            TestNonPrimeWithFullAccuracy(result);
        }

        [TestMethod]
        public void IsPrime_One()
        {
            var result = primeService.IsPrime(new BigInteger(1));
            TestNonPrimeWithFullAccuracy(result);
        }

        [TestMethod]
        public void IsPrime_First30Primes()
        {
            int maxPrimeFromRange = FIRST_30_PRIMES.Max();
            for (int i = 2; i <= maxPrimeFromRange; i++)
            {
                var result = primeService.IsPrime(new BigInteger(i));
                if (FIRST_30_PRIMES.Contains(i))
                {
                    TestPrimeWithFullAccuracy(result);
                }
                else
                {
                    TestNonPrimeWithFullAccuracy(result);
                }
            }
        }

        [TestMethod]
        public void IsPrime_BigPrime()
        {
            string bigPrime = new string(BIG_PRIME.ToCharArray()
                .Where(c => !char.IsWhiteSpace(c))
                .ToArray());

            var result = primeService.IsPrime(BigInteger.Parse(bigPrime));

            // is operation successful
            Assert.AreEqual(true, result.Succeeded, $"Error: {result.Error}");

            // shouldn't be 100% accurate
            Assert.AreEqual(true, result.Value.Accuracy < 1.0, "Shouldn't have 100% accuracy.");
        }

        [TestMethod]
        public void IsPrime_BigCompositeNum()
        {
            string semiPrime = new string(SEMI_PRIME.ToCharArray()
                .Where(c => !char.IsWhiteSpace(c))
                .ToArray());

            var result = primeService.IsPrime(BigInteger.Parse(semiPrime));

            // is operation successful
            Assert.AreEqual(true, result.Succeeded, $"Error: {result.Error}");

            // if Miller-Rabin primality test managed to determine this is not a prime,
            // the accuracy should be 100%.
            if (!result.Value.IsPrime)
            {
                Assert.AreEqual(1.0, result.Value.Accuracy, "Should have 100% accuracy.");
            }
        }

        [TestMethod]
        public void FindNextPrime_NumBiggerThan128Bytes()
        {
            // https://primes.utm.edu/curios/page.php?number_id=1757 132 bytes
            string primeNum = new string('1', 317);

            bool expectedOperationSuccess = false;
            bool actual = primeService.FindNextPrime(BigInteger.Parse(primeNum)).Succeeded;

            Assert.AreEqual(expectedOperationSuccess, actual);
        }

        [TestMethod]
        public void FindNextPrime_NegativeNumber()
        {
            BigInteger negativeNum = int.MinValue;

            BigInteger expectedPrime = 2;
            double expectedAccuracy = 1.0;

            var result = primeService.FindNextPrime(negativeNum);
            TestNextPrime(expectedPrime, expectedAccuracy, result);
        }

        [TestMethod]
        public void FindNextPrime_Zero()
        {
            BigInteger zero = 0;

            BigInteger expectedPrime = 2;
            double expectedAccuracy = 1.0;

            var result = primeService.FindNextPrime(zero);
            TestNextPrime(expectedPrime, expectedAccuracy, result);
        }

        [TestMethod]
        public void FindNextPrime_One()
        {
            BigInteger one = 1;

            BigInteger expectedPrime = 2;
            double expectedAccuracy = 1.0;

            var result = primeService.FindNextPrime(one);
            TestNextPrime(expectedPrime, expectedAccuracy, result);
        }

        [TestMethod]
        public void FindNextPrime_Range()
        {
            int maxPrimeRange = FIRST_30_PRIMES.Max();
            for (int i = 2; i < maxPrimeRange; i++)
            {
                var result = primeService.FindNextPrime(i);
                int expectedPrime = FIRST_30_PRIMES.Where(p => p > i).Min();
                double expectedAccuracy = 1.0;
                TestNextPrime(expectedPrime, expectedAccuracy, result);
            }
        }

        [TestMethod]
        public void FindNextPrime_BigUlongNum()
        {
            BigInteger num = ulong.MaxValue - 1;
            var result = primeService.FindNextPrime(num);

            Assert.AreEqual(true, result.Succeeded, $"Error: {result.Error}");
            Assert.AreEqual(true, result.Value.Accuracy < 1.0, "Accuracy shouldn't be 100%.");
        }

        [TestMethod]
        public void FindNextPrime_BigNum()
        {
            string semiPrime = new string(SEMI_PRIME.ToCharArray()
                .Where(c => !char.IsWhiteSpace(c))
                .ToArray());

            var result = primeService.FindNextPrime(BigInteger.Parse(semiPrime));

            Assert.AreEqual(true, result.Succeeded, $"Error: {result.Error}");
            Assert.AreEqual(true, result.Value.Accuracy < 1.0, "Accuracy shouldn't be 100%.");
        }

        public void TestPrimeWithFullAccuracy(OperationResult<PrimalityTestResult> result)
        {
            // is operation successfull
            Assert.AreEqual(true, result.Succeeded, $"Error: {result.Error}");

            // is prime
            Assert.AreEqual(true, result.Value.IsPrime, "Number should be prime.");

            // is 100% accurate
            Assert.AreEqual(1.0, result.Value.Accuracy, "The result should be 100% accurate.");
        }

        public void TestNonPrimeWithFullAccuracy(OperationResult<PrimalityTestResult> result)
        {
            // is operation successfull
            Assert.AreEqual(true, result.Succeeded, $"Error: {result.Error}");

            // is prime
            Assert.AreEqual(false, result.Value.IsPrime, "Number shouldn't be prime.");

            // is 100% accurate
            Assert.AreEqual(1.0, result.Value.Accuracy, "The result should be 100% accurate.");
        }

        public void TestNextPrime(BigInteger expectedNextPrime, double expectedAccuracy,
            OperationResult<NextPrimeResult> result)
        {
            // is operation successfull
            Assert.AreEqual(true, result.Succeeded, $"Error: {result.Error}");

            // next prime test
            Assert.AreEqual(expectedNextPrime,
                result.Value.NextPrime, $"Next prime should be {expectedNextPrime}.");

            // accuracy
            Assert.AreEqual(expectedAccuracy,
                result.Value.Accuracy, $"The accuracy should be {expectedAccuracy}");
        }

        private PrimeService primeService;
        private static readonly HashSet<int> FIRST_30_PRIMES = new HashSet<int>
        {
            2, 3, 5, 7, 11, 13, 17, 19, 23, 29,
            31, 37, 41, 43, 47, 53, 59, 61, 67, 71,
            73, 79, 83, 89, 97, 101, 103, 107, 109, 113
        };

        // https://primes.utm.edu/curios/page.php?number_id=9455 125 bytes
        private const string BIG_PRIME = @"13401855797820303090291422858454857480734067787022709387
            554841473183824203380878344068289557141870056546402570384957965451554022800559870762517
            045579946375897267127098893120428018580440395901554076504716679079958882921239092780465
            63998441725881316702608454953284969473141146885140822683049274853701491";

        // https://primes.utm.edu/curios/page.php?number_id=10103 125 bytes
        private const string SEMI_PRIME = @"1000000000000000000000000000000000000000000000000000000
            000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
            000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
            00000000000000000000000000000000000000000000000000000000000000000000059";
    }
}
