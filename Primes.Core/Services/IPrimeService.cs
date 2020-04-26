using Primes.Core.Communication;
using Primes.Core.DTOs;
using System.Numerics;

namespace Primes.Core.Services
{
    public interface IPrimeService
    {
        OperationResult<PrimalityTestResult> IsPrime(BigInteger num);

        OperationResult<NextPrimeResult> FindNextPrime(BigInteger num);
    }
}
