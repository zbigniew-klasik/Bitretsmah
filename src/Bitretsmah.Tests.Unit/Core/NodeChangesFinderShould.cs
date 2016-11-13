using Bitretsmah.Core;
using Bitretsmah.Core.Models;
using FluentAssertions;
using NUnit.Framework;
using System;

namespace Bitretsmah.Tests.Unit.Core
{
    [TestFixture]
    public class NodeChangesFinderShould
    {
        private INodeChangesFinder _finder;
        private File _file1;
        private File _file2;
        private Directory _drectory1;
        private Directory _drectory2;

        [SetUp]
        public void SetUp()
        {
            _finder = new NodeChangesFinder();
            _file1 = new File("foo.txt", 11, "hash", newDate(0), newDate(1));
            _file2 = new File("foo.txt", 11, "hash", newDate(0), newDate(1));
            _drectory1 = new Directory("temp");
            _drectory2 = new Directory("temp");
        }

        #region FILE

        [Test]
        public void ReturnNoChangeForSameFiles()
        {
            var change = _finder.Find(_file1, _file2);
            change.Type.Should().Be(NodeState.None);
            change.Node.Should().Be(_file2);
            change.InnerChanges.Should().BeEmpty();
        }

        [Test]
        public void ReturnModifedForDifferentFileName()
        {
            _file2.Name = "foo.exe";
            var change = _finder.Find(_file1, _file2);
            change.Type.Should().Be(NodeState.Modified);
            change.Node.Should().Be(_file2);
            change.InnerChanges.Should().BeEmpty();
        }

        [Test]
        public void ReturnModifedForDifferentFileSize()
        {
            _file2.Size = 12;
            var change = _finder.Find(_file1, _file2);
            change.Type.Should().Be(NodeState.Modified);
            change.Node.Should().Be(_file2);
            change.InnerChanges.Should().BeEmpty();
        }

        [Test]
        public void ReturnModifedForDifferentFileCreationTime()
        {
            _file2.CreationTime = newDate(7);
            var change = _finder.Find(_file1, _file2);
            change.Type.Should().Be(NodeState.Modified);
            change.Node.Should().Be(_file2);
            change.InnerChanges.Should().BeEmpty();
        }

        [Test]
        public void ReturnModifedForDifferentFileModificationTime()
        {
            _file2.ModificationTime = newDate(7);
            var change = _finder.Find(_file1, _file2);
            change.Type.Should().Be(NodeState.Modified);
            change.Node.Should().Be(_file2);
            change.InnerChanges.Should().BeEmpty();
        }

        [Test]
        public void ReturnNoChangeForNullFileHashes()
        {
            _file1.Hash = null;
            _file2.Hash = null;
            var change = _finder.Find(_file1, _file2);
            change.Type.Should().Be(NodeState.None);
            change.Node.Should().Be(_file2);
            change.InnerChanges.Should().BeEmpty();
        }

        [Test]
        public void ReturnNoChangeForFirstNullFileHash()
        {
            _file1.Hash = null;
            var change = _finder.Find(_file1, _file2);
            change.Type.Should().Be(NodeState.None);
            change.Node.Should().Be(_file2);
            change.InnerChanges.Should().BeEmpty();
        }

        [Test]
        public void ReturnNoChangeForSecondNullFileHash()
        {
            _file2.Hash = null;
            var change = _finder.Find(_file1, _file2);
            change.Type.Should().Be(NodeState.None);
            change.Node.Should().Be(_file2);
            change.InnerChanges.Should().BeEmpty();
        }

        [Test]
        public void ReturnModifedForDifferentFileHashes()
        {
            _file2.Hash = "different";
            var change = _finder.Find(_file1, _file2);
            change.Type.Should().Be(NodeState.Modified);
            change.Node.Should().Be(_file2);
            change.InnerChanges.Should().BeEmpty();
        }

        #endregion FILE

        #region DIRECTORY

        [Test]
        public void ReturnNoChangeForSameEmptyDirectories()
        {
            var change = _finder.Find(_drectory1, _drectory2);
            change.Type.Should().Be(NodeState.None);
            change.Node.Should().Be(_drectory2);
            change.InnerChanges.Should().BeEmpty();
        }

        [Test]
        public void ReturnModifedForDifferentDirectoryName()
        {
            _drectory2.Name = "stuff";
            var change = _finder.Find(_drectory1, _drectory2);
            change.Type.Should().Be(NodeState.Modified);
            change.Node.Should().Be(_drectory2);
            change.InnerChanges.Should().BeEmpty();
        }

        #endregion DIRECTORY

        // add file/dir
        // remove file/dir
        // modif file/dir

        private DateTimeOffset newDate(int seconds)
        {
            return new DateTimeOffset(2016, 11, 12, 19, 34, seconds, new TimeSpan(0));
        }
    }
}