using Bitretsmah.Core.Models;
using System.Threading.Tasks;

namespace Bitretsmah.Core
{
    internal interface INodeChangesFinder
    {
        Task<NodeChange> Find(Node initialNode, Node finalNode);
    }

    internal class NodeChangesFinder : INodeChangesFinder
    {
        public Task<NodeChange> Find(Node initialNode, Node finalNode)
        {
            return null;
        }
    }
}