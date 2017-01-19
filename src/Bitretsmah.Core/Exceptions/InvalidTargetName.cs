namespace Bitretsmah.Core.Exceptions
{
    public class InvalidTargetName : BitretsmahException
    {
        public InvalidTargetName(string targetName)
        {
            TargetName = targetName;
        }

        public string TargetName { get; private set; }
    }
}