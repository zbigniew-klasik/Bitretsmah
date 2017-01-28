using Bitretsmah.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bitretsmah.Core.Interfaces
{
    public interface ITargetRepository
    {
        Task<List<Target>> GetAll();

        Task<Target> GetByName(string name);

        Task AddOrUpdate(Target target);
    }
}