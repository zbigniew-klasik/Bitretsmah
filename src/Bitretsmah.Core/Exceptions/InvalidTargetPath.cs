namespace Bitretsmah.Core.Exceptions
{
    public class InvalidTargetPath : BitretsmahException
    {
        public InvalidTargetPath(string path)
        {
            TargetPath = path;
        }

        public string TargetPath { get; private set; }
    }
}