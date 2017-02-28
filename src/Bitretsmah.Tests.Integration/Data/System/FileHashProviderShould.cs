using Bitretsmah.Core.Interfaces;
using Bitretsmah.Data.System;
using FluentAssertions;
using NUnit.Framework;
using System;
using System.IO;

namespace Bitretsmah.Tests.Integration.Data.System
{
    [TestFixture]
    public class FileHashProviderShould
    {
        [TestCase("ABC", "3C01BDBB26F358BAB27F267924AA2C9A03FCFDB8")]
        [TestCase("aq2*sh&2uA^IBo%t4$fK#Z@H5!f", "A8499F0A32485CB682D5AB7050F967CFBA504D84")]
        [TestCase("{2DC278FA-7BF5-4B83-8CD9-FC5C700E86F9}", "F7031B995F0F2E9CED6D62156F3DEB756ADB7E1E")]
        [TestCase("{F8D2918E-8089-4B3A-804E-1BA1354F1528-2AC28CEA-C502-4103-AD78-E373CF428726}", "EF69707A3644F61EC4FF0C60483A8CC4E9103788")]
        [TestCase("sD0FMeoZkpeKZJkIFFpDqvWQ3Snw3XrQizfkfyOsKnCPlPzUdAkgXWeJ7pgTrCxnzgaN0RJszpB7Q3QOykfb6boh8rr2KJ3xfyMP", "A1BF0D5A8B5ADBCD4793447B9B10E1911EE50974")]
        public void ComputeProperFileHash(string fileContent, string expectedHash)
        {
            var fileName = Guid.NewGuid().ToString();
            File.WriteAllText(fileName, fileContent);
            IFileHashProvider provider = new FileHashProvider();
            var actualHash = provider.ComputeFileHash(fileName);
            File.Delete(fileName);
            actualHash.Should().Be(expectedHash);
        }

        [Test]
        public void ThrowExceptionForNotExistingFile()
        {
            var fileName = Guid.NewGuid().ToString();
            IFileHashProvider provider = new FileHashProvider();
            Assert.Throws<FileNotFoundException>(() => provider.ComputeFileHash(fileName));
        }
    }
}