namespace Bitretsmah.Core.Interfaces
{
    public interface IFileHashProvider
    {
        string ComputeFileHash(string filePath);
    }
}