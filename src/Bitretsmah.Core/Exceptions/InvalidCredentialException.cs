namespace Bitretsmah.Core.Exceptions
{
    public class InvalidCredentialException : BitretsmahException
    {
        public InvalidCredentialException(string username)
            : base($"Cannot log in as '{username}'. Invalid user name or password.")
        {
        }
    }
}