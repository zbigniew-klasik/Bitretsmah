using Bitretsmah.Core.Models;
using System.Threading.Tasks;

namespace Bitretsmah.Core.Interfaces
{
    public interface IBackupRepository
    {
        Task<Backup> GetLastForPath(string path);

        Task Add(Backup backup);
    }
}