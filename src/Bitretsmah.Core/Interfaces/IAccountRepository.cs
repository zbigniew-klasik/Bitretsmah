using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Bitretsmah.Core.Models;

namespace Bitretsmah.Core.Interfaces
{
    public interface IAccountRepository
    {
        Task<List<Account>> GetAll();

        Task AddOrUpdate(NetworkCredential credential);
    }
}