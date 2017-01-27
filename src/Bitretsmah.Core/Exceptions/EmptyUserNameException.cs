namespace Bitretsmah.Core.Exceptions
{
    public class EmptyUserNameException : BitretsmahException
    {
        public EmptyUserNameException()
            : base($"User name cannot be empty.")
        {
        }
    }
}
