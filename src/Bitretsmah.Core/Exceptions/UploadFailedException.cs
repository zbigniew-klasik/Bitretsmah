namespace Bitretsmah.Core.Exceptions
{
    public class UploadFailedException : BitretsmahException
    {
        public UploadFailedException()
        {
        }

        public UploadFailedException(string message)
            : base(message)
        {
        }
    }
}