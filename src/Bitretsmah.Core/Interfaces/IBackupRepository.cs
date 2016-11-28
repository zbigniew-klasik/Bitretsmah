using Bitretsmah.Core.Models;
using System.Threading.Tasks;

namespace Bitretsmah.Core.Interfaces
{
    public interface IBackupRepository
    {
        Task<Backup> GetLastForTarget(string path);

        Task Add(Backup backup);
    }
}