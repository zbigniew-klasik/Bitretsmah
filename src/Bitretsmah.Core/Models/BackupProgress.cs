namespace Bitretsmah.Core.Models
{
    public class BackupProgress
    {
        public BackupProgress(HashInfo hashInfo)
        {
            State = BackupState.ComputingHashes;
            Hash = hashInfo;
        }

        public BackupProgress(UploadInfo uploadInfo)
        {
            State = BackupState.UploadingFiles;
            Upload = uploadInfo;
        }

        public BackupProgress(string errorMessage)
        {
            State = BackupState.Error;
            ErrorMessage = ErrorMessage;
        }

        public enum BackupState
        {
            ComputingHashes,
            UploadingFiles,
            Error
        }

        public class HashInfo
        {
            public int AllFilesNumber { get; set; }
            public int ComputedHashesNumber { get; set; }
            public File CurrentFile { get; set; }
        }

        public class UploadInfo
        {
            public UploadInfo(int all, int processed, File file)
            {
                AllFilesNumber = all;
                ProcessedFilesNumber = processed;
                CurrentFileUploadProgress = 0;
                ComputingHash = true;
                CurrentFile = file;
            }

            public UploadInfo(int all, int processed, double progress, File file)
            {
                AllFilesNumber = all;
                ProcessedFilesNumber = processed;
                CurrentFileUploadProgress = progress;
                ComputingHash = false;
                CurrentFile = file;
            }

            public int AllFilesNumber { get; set; }
            public int ProcessedFilesNumber { get; set; }
            public double CurrentFileUploadProgress { get; set; }
            public bool ComputingHash { get; set; }
            public File CurrentFile { get; set; }
        }

        public BackupState State { get; set; }

        public HashInfo Hash { get; set; }

        public UploadInfo Upload { get; set; }

        public string ErrorMessage { get; set; }
    }
}