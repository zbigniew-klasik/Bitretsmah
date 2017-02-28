using StructureMap;
using Bitretsmah.Core.Interfaces;

namespace Bitretsmah.Data.Mega
{
    public class ContainerRegistry : Registry
    {
        public ContainerRegistry()
        {
            For<ICredentialVerifier>().Use<MegaCredentialVerifier>();
            For<IRemoteFileStore>().Use<MegaStore>();
            For<IRemoteFileStoreFactory>().Use<MegaStoreFactory>();
        }
    }
}