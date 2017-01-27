namespace Bitretsmah.Core.Exceptions
{
    public class EmptyPasswordException : BitretsmahException
    {
        public EmptyPasswordException()
            : base($"Password cannot be empty.")
        {
        }
    }
}
