namespace Bitretsmah.Core.Exceptions
{
    public class EmptyTargetNameException : BitretsmahException
    {
        public EmptyTargetNameException()
            : base($"Name of the target cannot be empty.")
        {
        }
    }
}