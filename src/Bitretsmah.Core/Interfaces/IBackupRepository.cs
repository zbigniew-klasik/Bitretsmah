using Bitretsmah.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bitretsmah.Core.Interfaces
{
    public interface IBackupRepository
    {
        Task<IEnumerable<Backup>> GetAllForTarget(string target);

        Task Add(Backup backup);
    }
}