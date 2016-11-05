namespace Bitretsmah.Core.Exceptions
{
    public class UnknownStoreException : BitretsmahException
    {
        public UnknownStoreException()
            : base()
        {
        }

        public UnknownStoreException(string message)
            : base(message)
        {
        }
    }
}