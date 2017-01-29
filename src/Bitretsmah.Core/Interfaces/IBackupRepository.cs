using Bitretsmah.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bitretsmah.Core.Interfaces
{
    public interface IBackupRepository
    {
        Task<List<Backup>> GetAllForTarget(string targetName);

        Task Add(Backup backup);
    }
}