# Primes
Service that performs primality tests and can find the next prime. 

## What algorithm do we use for 64-bit numbers?
For 64-bit numbers we use a linear algorithm with 100% accuracy.

## What algorithm do we use for 128-byte numbers?
For numbers up to 128 bytes we use Miller-Rabin test. It is a test for compositeness, rather than primality - it has 100% accuracy when it tests positive for compositeness and 4<sup>-k</sup> accuracy rate when it comes back positive for primality. The `k` is the number of random bases we try. In our implementation `k = 10`, which is `99.99990%` confidence or 1 failure in a million. The complexity of the algorithm is O(<i>k</i>log<sup>3</sup><i>n</i>).

# Documentation
To see the API documentation of the service, run the project and open `index.html`.
