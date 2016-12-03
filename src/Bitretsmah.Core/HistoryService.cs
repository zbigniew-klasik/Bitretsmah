using Bitretsmah.Core.Models;
using System.Threading.Tasks;

namespace Bitretsmah.Core
{
    public interface IHistoryService
    {
        Task<Node> GetLastStructure(string target);
    }

    public class HistoryService : IHistoryService
    {
        public async Task<Node> GetLastStructure(string target)
        {
            return await Task.FromResult(new Directory());
        }
    }
}