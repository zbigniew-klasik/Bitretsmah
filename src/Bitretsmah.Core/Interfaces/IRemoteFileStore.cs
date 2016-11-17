using Bitretsmah.Core.Models;
using System;
using System.Threading.Tasks;

namespace Bitretsmah.Core.Interfaces
{
    public interface IRemoteFileStore : IDisposable
    {
        string StoreId { get; }

        Task<Quota> GetQuota();

        Task<RemoteId> UploadFile(string localFilePath, IProgress<double> progress);

        Task DownloadFile(RemoteId remoteId, string localFilePath, IProgress<double> progress);
    }
}