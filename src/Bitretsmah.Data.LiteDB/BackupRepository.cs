using Bitretsmah.Core.Interfaces;
using Bitretsmah.Core.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bitretsmah.Data.LiteDB
{
    public class BackupRepository : IBackupRepository
    {
        public Task<IEnumerable<Backup>> GetAllForTarget(string target)
        {
            throw new NotImplementedException();
        }

        public Task Add(Backup backup)
        {
            throw new NotImplementedException();
        }
    }
}