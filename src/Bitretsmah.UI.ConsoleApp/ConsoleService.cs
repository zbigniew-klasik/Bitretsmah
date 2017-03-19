using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Security;
using Bitretsmah.Core.Models;

namespace Bitretsmah.UI.ConsoleApp
{
    public interface IConsoleService
    {
        void WriteHelp();

        void WriteVersion();

        SecureString ReadPassword();

        void AccountSetSuccessfully();

        void ListAccounts(IEnumerable<Account> accounts);

        void TargetSetSuccessfully();

        void TargetRemovedSuccessfully();

        void ListTargets(IEnumerable<Target> targets);

        void WriteErrorMessage(string message);

        void WriteUnexpectedException(Exception exception);

        void WriteProgress(BackupProgress progress);
    }

    internal class ConsoleService : IConsoleService
    {
        public void WriteHelp()
        {
            Console.WriteLine("Usage:");
            Console.WriteLine("\t--backup target");
            Console.WriteLine("\t--restore target");
            Console.WriteLine("\t--hash");
            Console.WriteLine("\t--accounts");
            Console.WriteLine("\t--set-account user [--password password]");
            Console.WriteLine("\t--targets");
            Console.WriteLine("\t--set-target target --path path");
            Console.WriteLine("\t--remove-target target");
            Console.WriteLine("\t--forced");
            Console.WriteLine("\t--help");
            Console.WriteLine("\t--version");
        }

        public void WriteVersion()
        {
            var version = "unknown";

            var location = Assembly.GetExecutingAssembly().Location;
            if (!string.IsNullOrWhiteSpace(location))
                version = FileVersionInfo.GetVersionInfo(location).FileVersion;

            Console.WriteLine(version);
        }

        public SecureString ReadPassword()
        {
            Console.Write("Enter password: ");

            var pwd = new SecureString();
            while (true)
            {
                var i = Console.ReadKey(true);
                if (i.Key == ConsoleKey.Enter)
                    break;

                if (i.Key == ConsoleKey.Backspace)
                {
                    if (pwd.Length > 0)
                    {
                        pwd.RemoveAt(pwd.Length - 1);
                        Console.Write("\b \b");
                    }
                }
                else
                {
                    pwd.AppendChar(i.KeyChar);
                    Console.Write("*");
                }
            }
            Console.WriteLine();
            return pwd;
        }

        public void AccountSetSuccessfully()
        {
            Console.WriteLine("Account configured successfully.");
        }

        public void ListAccounts(IEnumerable<Account> accounts)
        {
            var accountsList = accounts.ToList();

            if (accountsList.Any())
            {
                var totalQuota = accountsList.Sum(x => x.Quota.Total);
                var usedQuota = accountsList.Sum(x => x.Quota.Used);
                var freeQuota = accountsList.Sum(x => x.Quota.Free);

                Console.WriteLine("Accounts:");
                accountsList.ForEach(x => Console.WriteLine($"\t{x.Credential.UserName}\t{x.Quota.Free}"));
                Console.WriteLine($"Quota: {totalQuota}\tUsed: {usedQuota}\tFree: {freeQuota}");
            }
            else
            {
                Console.WriteLine("No accounts configured.");
            }
        }

        public void TargetSetSuccessfully()
        {
            Console.WriteLine("Target configured successfully.");
        }

        public void TargetRemovedSuccessfully()
        {
            Console.WriteLine("Target removed successfully.");
        }

        public void ListTargets(IEnumerable<Target> targets)
        {
            var targetsList = targets.ToList();

            if (targetsList.Any())
            {
                Console.WriteLine("Targets:");
                targetsList.ForEach(x => Console.WriteLine($"\t{x.Name}\t{x.LocalPath}"));
            }
            else
            {
                Console.WriteLine("No targets configured.");
            }
        }

        public void WriteErrorMessage(string message)
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        public void WriteUnexpectedException(Exception exception)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"UNEXPECTED EXCEPTION:");
            Console.WriteLine($"{exception.GetType()}:\t{exception.Message}");
            Console.WriteLine($"SEE PROGRAM LOG FOR DETAILS.");
            Console.ResetColor();
        }

        public void WriteProgress(BackupProgress progress)
        {
            switch (progress.State)
            {
                case BackupProgress.BackupState.HashStart:
                    Console.Write(progress.CurrentFile.AbsolutePath + " computing...");
                    break;

                case BackupProgress.BackupState.HashFinished:
                    Console.WriteLine(" done.");
                    Console.WriteLine();
                    break;

                case BackupProgress.BackupState.UploadStart:
                    Console.WriteLine(progress.CurrentFileNumber + 1 + "/" + progress.AllFilesCount + ": " + progress.CurrentFile.AbsolutePath);
                    break;

                case BackupProgress.BackupState.UploadProgress:
                    Console.Write($"\r uploading... {progress.CurrentFileProgress:0.00}% ");
                    break;

                case BackupProgress.BackupState.UploadFinished:
                    Console.WriteLine("done.");
                    Console.WriteLine();
                    break;

                case BackupProgress.BackupState.DownloadStart:
                    Console.WriteLine(progress.CurrentFileNumber + 1 + "/" + progress.AllFilesCount + ": " + progress.CurrentFile.AbsolutePath);
                    break;

                case BackupProgress.BackupState.DownloadProgress:
                    Console.Write($"\r downloading... {progress.CurrentFileProgress:0.00}% ");
                    break;

                case BackupProgress.BackupState.DownloadFinished:
                    Console.WriteLine("done.");
                    Console.WriteLine();
                    break;

                //TODO: deleting report

                case BackupProgress.BackupState.Error:
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.WriteLine("ERROR: " + progress.Error);
                    Console.ResetColor();
                    Console.WriteLine();
                    break;
            }
            Console.Out.Flush();
        }
    }
}