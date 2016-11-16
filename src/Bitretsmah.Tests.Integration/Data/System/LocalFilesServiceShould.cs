using Bitretsmah.Core;
using Bitretsmah.Core.Interfaces;
using Bitretsmah.Core.Models;
using Bitretsmah.Data.System;
using FluentAssertions;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using SystemDirectory = System.IO.Directory;
using SystemFile = System.IO.File;
using SystemPath = System.IO.Path;

namespace Bitretsmah.Tests.Integration.Data.System
{
    [TestFixture]
    public class LocalFilesServiceShould
    {
        private DateTimeOffset _creationTime;
        private DateTimeOffset _writeTime;
        private string _d0Path, _d1Path, _d2Path, _f1Path, _f2Path;

        [SetUp]
        public void SetUp()
        {
            _creationTime = DateTimeOffset.Now.AddMinutes(-5);
            _writeTime = DateTimeOffset.Now.AddMinutes(-1);

            _d0Path = SystemPath.Combine(Environment.CurrentDirectory, "LocalFilesService Test D0");
            _d1Path = SystemPath.Combine(_d0Path, "LocalFilesService Test D1");
            _d2Path = SystemPath.Combine(_d1Path, "LocalFilesService Test D2");
            _f1Path = SystemPath.Combine(_d0Path, "LocalFilesService Test F1.txt");
            _f2Path = SystemPath.Combine(_d1Path, "LocalFilesService Test F2.dat");

            SystemDirectory.CreateDirectory(_d0Path);
            SystemDirectory.CreateDirectory(_d1Path);
            SystemDirectory.CreateDirectory(_d2Path);

            SystemFile.WriteAllText(_f1Path, "text");
            SystemFile.WriteAllText(_f2Path, "longer text");

            SystemFile.SetCreationTime(_f1Path, _creationTime.DateTime);
            SystemFile.SetCreationTime(_f2Path, _creationTime.DateTime);
            SystemFile.SetLastWriteTime(_f1Path, _writeTime.DateTime);
            SystemFile.SetLastWriteTime(_f2Path, _writeTime.DateTime);
        }

        [Test]
        public void GetNodeStructure()
        {
            var f2 = new File
            {
                Name = "LocalFilesService Test F2.dat",
                Size = 11,
                Hash = null,
                CreationTime = _creationTime,
                ModificationTime = _writeTime,
                AbsolutePath = _f2Path,
                State = NodeState.None
            };

            var d2 = new Directory
            {
                Name = "LocalFilesService Test D2",
                AbsolutePath = _d2Path,
                State = NodeState.None
            };

            var d1 = new Directory
            {
                Name = "LocalFilesService Test D1",
                AbsolutePath = _d1Path,
                State = NodeState.None
            };

            d1.InnerNodes.Add(f2);
            d1.InnerNodes.Add(d2);

            var f1 = new File
            {
                Name = "LocalFilesService Test F1.txt",
                Size = 4,
                Hash = null,
                CreationTime = _creationTime,
                ModificationTime = _writeTime,
                AbsolutePath = _f1Path,
                State = NodeState.None
            };

            var d0 = new Directory
            {
                Name = "LocalFilesService Test D0",
                AbsolutePath = _d0Path,
                State = NodeState.None
            };

            d0.InnerNodes.Add(f1);
            d0.InnerNodes.Add(d1);

            ILocalFilesService service = new LocalFilesService();
            var actualNodeStructure = service.GetNodeStructure(_d0Path);

            actualNodeStructure.ShouldBeEquivalentTo(d0);
            ShouldSerializeSameAs(actualNodeStructure, d0);
        }

        [TearDown]
        public void TearDown()
        {
            SystemDirectory.Delete(_d0Path, true);
        }

        public static void ShouldSerializeSameAs(object actual, object expected)
        {
            if (expected == actual) return;

            string expectedJson = expected.ToJson();
            string actualJson = actual.ToJson();

            if (expectedJson == actualJson) return;

            string path = string.Empty;

            try
            {
                path = SystemPath.Combine(Environment.CurrentDirectory, Guid.NewGuid().ToString());
                SystemDirectory.CreateDirectory(path);
                SystemFile.WriteAllText(SystemPath.Combine(path, "expected.json"), expectedJson);
                SystemFile.WriteAllText(SystemPath.Combine(path, "actual.json"), actualJson);
            }
            finally
            {
                throw new JsonException($"The actual JSON does not match the expected. Compare files in directory: '{path}'.");
            }
        }
    }
}