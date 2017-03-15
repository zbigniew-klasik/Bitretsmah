namespace Bitretsmah.Core.Exceptions
{
    public class InvalidFileHashException : BitretsmahException
    {
        public InvalidFileHashException(string filePath, string expectedHash, string actualHash)
            : base($"The file '{filePath}' has invalid hash. Expected: '{expectedHash}'. Actual: '{actualHash}'.")
        {
        }
    }
}