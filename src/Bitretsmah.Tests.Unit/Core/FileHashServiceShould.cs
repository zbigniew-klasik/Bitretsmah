using Bitretsmah.Core;
using Bitretsmah.Core.Interfaces;
using Bitretsmah.Core.Models;
using Moq;
using NUnit.Framework;
using System.Threading.Tasks;
using System;

namespace Bitretsmah.Tests.Unit.Core
{
    public class FileHashServiceShould
    {
        private Mock<IFileHashProvider> _fileHashProviderMock;
        private Mock<ILogger> _loggerMock;
        private Mock<IProgress<BackupProgress>> _progressMock;
        private IFileHashService _fileHashService;

        [SetUp]
        public void Setup()
        {
            _fileHashProviderMock = new Mock<IFileHashProvider>();
            _loggerMock = new Mock<ILogger>();
            _progressMock = new Mock<IProgress<BackupProgress>>();
            _fileHashService = new FileHashService(_fileHashProviderMock.Object, _loggerMock.Object);
        }

        [Test]
        public async Task TryEnsureFileHasComputedHash()
        {
            var file = new File { Name = "foo.txt", AbsolutePath = @"C:\Temp\foo.txt" };

            await _fileHashService.TryEnsureFileHasComputedHash(file, _progressMock.Object);

            _fileHashProviderMock.Verify(x => x.ComputeFileHash(@"C:\Temp\foo.txt"));
            _progressMock.Verify(x => x.Report(It.IsAny<BackupProgress>()), Times.Exactly(2));
        }

        [Test]
        public async Task TryEnsureFileHasComputedHash_HandleException()
        {
            var exception = new Exception();
            _fileHashProviderMock.Setup(x => x.ComputeFileHash(It.IsAny<string>())).Throws(exception);
            var file = new File { Name = "foo.txt", AbsolutePath = @"C:\Temp\foo.txt" };

            await _fileHashService.TryEnsureFileHasComputedHash(file, _progressMock.Object);

            _fileHashProviderMock.Verify(x => x.ComputeFileHash(@"C:\Temp\foo.txt"), Times.Once);
            _progressMock.Verify(x => x.Report(It.IsAny<BackupProgress>()), Times.Exactly(2));
            _loggerMock.Verify(x => x.Error(exception, It.IsAny<string>(), file.AbsolutePath));
        }

        [Test]
        public async Task TryEnsureEachFileHasComputedHash()
        {
            var file1 = new File { Name = "foo.txt", AbsolutePath = @"C:\D1\foo.txt" };
            var file2 = new File { Name = "bar.txt", AbsolutePath = @"C:\D1\D2\bar.txt" };

            var dir1 = new Directory { Name = "D1", AbsolutePath = @"C:\D1" };
            var dir2 = new Directory { Name = "D2", AbsolutePath = @"C:\D1\D2" };

            dir1.InnerNodes.Add(file1);
            dir1.InnerNodes.Add(dir2);
            dir2.InnerNodes.Add(file2);

            await _fileHashService.TryEnsureEachFileHasComputedHash(dir1, _progressMock.Object);

            _fileHashProviderMock.Verify(x => x.ComputeFileHash(@"C:\D1\foo.txt"), Times.Once);
            _fileHashProviderMock.Verify(x => x.ComputeFileHash(@"C:\D1\D2\bar.txt"), Times.Once);
            _progressMock.Verify(x => x.Report(It.IsAny<BackupProgress>()), Times.Exactly(4));
        }
    }
}