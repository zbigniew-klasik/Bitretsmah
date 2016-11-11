using Bitretsmah.Core.Models;
using System.Threading.Tasks;

namespace Bitretsmah.Core.Interfaces
{
    public interface INodeChangesFinder
    {
        Task<NodeChange> Find(Node initialNode, Node finalNode);
    }
}