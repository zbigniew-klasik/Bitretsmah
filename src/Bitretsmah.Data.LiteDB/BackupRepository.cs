using Bitretsmah.Core.Interfaces;
using Bitretsmah.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bitretsmah.Data.LiteDB.Internal;

namespace Bitretsmah.Data.LiteDB
{
    public class BackupRepository : IBackupRepository
    {
        public Task<List<Backup>> GetAllForTarget(string targetName)
        {
            return Task.Run(() =>
            {
                using (var db = DbFactory.Create())
                {
                    return db.Backups.Find(x => x.TargetName.Equals(targetName)).ToList();
                }
            });
        }

        public Task Add(Backup backup)
        {
            return Task.Run(() =>
            {
                using (var db = DbFactory.Create())
                {
                    db.Backups.Insert(backup);
                }
            });
        }
    }
}