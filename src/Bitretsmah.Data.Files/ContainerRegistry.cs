using StructureMap;
using Bitretsmah.Core.Interfaces;

namespace Bitretsmah.Data.Mega
{
    public class ContainerRegistry : Registry
    {
        public ContainerRegistry()
        {
            For<ICredentialVerifier>().Use<MegaCredentialVerifier>().Singleton();
            For<IRemoteFileStore>().Use<MegaStore>().Singleton();
            For<IRemoteFileStoreFactory>().Use<MegaStoreFactory>().Singleton();
        }
    }
}