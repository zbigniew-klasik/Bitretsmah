namespace Bitretsmah.Core.Exceptions
{
    public class InvalidTargetPath : BitretsmahException
    {
        public InvalidTargetPath(string path)
            : base($"Provided target path '{path}' is not valid.")
        {
            TargetPath = path;
        }

        public string TargetPath { get; private set; }
    }
}