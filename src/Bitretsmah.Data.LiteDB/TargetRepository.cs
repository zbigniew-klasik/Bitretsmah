using Bitretsmah.Core.Interfaces;
using Bitretsmah.Core.Models;
using Bitretsmah.Data.LiteDB.Internal;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bitretsmah.Data.LiteDB
{
    public class TargetRepository : ITargetRepository
    {
        public Task<List<Target>> GetAll()
        {
            return Task.Run(() =>
            {
                using (var db = DbFactory.Create())
                {
                    return db.Targets.FindAll().ToList();
                }
            });
        }

        public Task AddOrUpdate(Target target)
        {
            return Task.Run(() =>
            {
                using (var db = DbFactory.Create())
                {
                    var existingTarget = db.Targets.FindOne(x => x.Name.Equals(target.Name));

                    if (existingTarget == null)
                    {
                        db.Targets.Insert(target);
                    }
                    else
                    {
                        existingTarget.LocalPath = target.LocalPath;
                        db.Targets.Update(existingTarget);
                    }
                }
            });
        }
    }
}