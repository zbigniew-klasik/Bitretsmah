using Bitretsmah.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bitretsmah.Core.Interfaces
{
    public interface ITargetRepository
    {
        Task<List<Target>> GetAll();

        Task AddOrUpdate(Target target);
    }
}