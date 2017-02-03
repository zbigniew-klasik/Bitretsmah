using System;

namespace Bitretsmah.Core.Models
{
    [Serializable]
    public class RemoteId
    {
        public RemoteId()
        {
        }

        public RemoteId(string storeId, string nodeId)
        {
            StoreId = storeId;
            NodeId = nodeId;
        }

        public string StoreId { get; set; }
        public string NodeId { get; set; }

        public override string ToString()
        {
            return $"{StoreId}:{NodeId}";
        }
    }
}