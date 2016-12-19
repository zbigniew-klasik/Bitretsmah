using Bitretsmah.Core.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Bitretsmah.Core.Interfaces
{
    public interface IRemoteFileStore : IDisposable
    {
        string StoreId { get; }

        Task<Quota> GetQuota();

        Task<ICollection<RemoteFile>> GetFilesList();

        Task<RemoteId> UploadFile(Stream stream, string remoteFileName, IProgress<double> progress);

        Task<Stream> DownloadFile(RemoteId remoteId, IProgress<double> progress);
    }
}