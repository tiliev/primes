namespace Primes.Core.Communication
{
    public class OperationResult<T>
    {
        public OperationResult(bool succeeded, T value)
        {
            Succeeded = succeeded;
            Value = value;
        }

        public OperationResult(bool succeeded, string error)
        {
            Succeeded = succeeded;
            Error = error;
        }

        /// <summary>
        /// Whether the operation is successfull.
        /// </summary>
        public bool Succeeded { get; }

        /// <summary>
        /// The result of the successfull operation.
        /// </summary>
        public T Value { get; }

        /// <summary>
        /// Holds the error message.
        /// </summary>
        public string Error { get; }
    }
}
