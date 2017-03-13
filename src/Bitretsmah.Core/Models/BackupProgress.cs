using System;

namespace Bitretsmah.Core.Models
{
    public class BackupProgress
    {
        public enum BackupState
        {
            HashStart,
            HashFinished,
            UploadStart,
            UploadProgress,
            UploadFinished,
            DownloadStart,
            DownloadProgress,
            DownloadFinished,
            Error
        }

        private BackupProgress()
        {
        }

        public BackupState State { get; private set; }
        public int AllFilesCount { get; private set; }
        public int CurrentFileNumber { get; private set; }
        public double CurrentFileProgress { get; private set; }
        public File CurrentFile { get; private set; }
        public TimeSpan CurrentFileProcessingTime { get; private set; }
        public string Error { get; private set; }

        public static BackupProgress CreateHashStartReport(int allFilesCount, int currentFileNumber, File currentFile)
        {
            return new BackupProgress {State = BackupState.HashStart, AllFilesCount = allFilesCount, CurrentFileNumber = currentFileNumber, CurrentFile = currentFile};
        }

        public static BackupProgress CreateHashFinishedReport(int allFilesCount, int currentFileNumber, File currentFile)
        {
            return new BackupProgress { State = BackupState.HashFinished, AllFilesCount = allFilesCount, CurrentFileNumber = currentFileNumber, CurrentFile = currentFile};
        }

        public static BackupProgress CreateUploadStartReport(int allFilesCount, int currentFileNumber, File currentFile)
        {
            return new BackupProgress { State = BackupState.UploadStart, AllFilesCount = allFilesCount, CurrentFileNumber = currentFileNumber, CurrentFile = currentFile };
        }

        public static BackupProgress CreateUploadProgressReport(int allFilesCount, int currentFileNumber, File currentFile, double progress)
        {
            return new BackupProgress { State = BackupState.UploadProgress, AllFilesCount = allFilesCount, CurrentFileNumber = currentFileNumber, CurrentFile = currentFile, CurrentFileProgress = progress};
        }

        public static BackupProgress CreateUploadFinishedReport(int allFilesCount, int currentFileNumber, File currentFile)
        {
            return new BackupProgress { State = BackupState.UploadFinished, AllFilesCount = allFilesCount, CurrentFileNumber = currentFileNumber, CurrentFile = currentFile };
        }

        public static BackupProgress CreateDownloadStartReport(int allFilesCount, int currentFileNumber, File currentFile)
        {
            return new BackupProgress { State = BackupState.DownloadStart, AllFilesCount = allFilesCount, CurrentFileNumber = currentFileNumber, CurrentFile = currentFile };
        }

        public static BackupProgress CreateDownloadProgressReport(int allFilesCount, int currentFileNumber, File currentFile, double progress)
        {
            return new BackupProgress { State = BackupState.DownloadProgress, AllFilesCount = allFilesCount, CurrentFileNumber = currentFileNumber, CurrentFile = currentFile, CurrentFileProgress = progress };
        }

        public static BackupProgress CreateDownloadFinishedReport(int allFilesCount, int currentFileNumber, File currentFile)
        {
            return new BackupProgress { State = BackupState.DownloadFinished, AllFilesCount = allFilesCount, CurrentFileNumber = currentFileNumber, CurrentFile = currentFile };
        }

        public static BackupProgress CreateErrorReport(string error)
        {
            return new BackupProgress { State = BackupState.Error, Error = error };
        }
    }
}