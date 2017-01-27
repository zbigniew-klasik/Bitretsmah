namespace Bitretsmah.Core.Exceptions
{
    public class InvalidTargetPathException : BitretsmahException
    {
        public InvalidTargetPathException(string path)
            : base($"Target path '{path}' is not valid.")
        {
            TargetPath = path;
        }

        public string TargetPath { get; private set; }
    }
}