using Bitretsmah.Core.Models;
using System.IO;

namespace Bitretsmah.Core.Interfaces
{
    public interface ILocalFilesService
    {
        Node GetNodeStructure(string nodePath);

        Stream ReadFileStream(string filePath);
    }
}