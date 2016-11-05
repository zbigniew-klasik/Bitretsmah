namespace Bitretsmah.Core.Exceptions
{
    public class InvalidCredentialException : BitretsmahException
    {
        public InvalidCredentialException()
            : base()
        {
        }

        public InvalidCredentialException(string message)
            : base(message)
        {
        }
    }
}