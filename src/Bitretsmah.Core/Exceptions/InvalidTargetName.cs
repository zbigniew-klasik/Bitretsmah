namespace Bitretsmah.Core.Exceptions
{
    public class InvalidTargetName : BitretsmahException
    {
        public InvalidTargetName(string targetName)
            : base($"Provided target name '{targetName}' is not valid.")
        {
            TargetName = targetName;
        }

        public string TargetName { get; private set; }
    }
}