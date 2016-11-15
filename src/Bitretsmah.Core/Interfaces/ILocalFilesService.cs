using Bitretsmah.Core.Models;

namespace Bitretsmah.Core.Interfaces
{
    public interface ILocalFilesService
    {
        Node GetNodeStructure(string nodePath);
    }
}