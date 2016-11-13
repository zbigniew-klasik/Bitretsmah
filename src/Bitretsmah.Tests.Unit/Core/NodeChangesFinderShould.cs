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

        #region SINGLE FILE

        [Test]
        public void FindNoChangeForSameFiles()
        {
            var result = _finder.Find(_file1, _file2);
            result.Should().BeOfType<File>();
            result.ShouldBeEquivalentTo(_file2);
            result.State.Should().Be(NodeState.None);
        }

        [Test]
        public void FindModifedForDifferentFileSize()
        {
            _file2.Size = 12;
            var result = _finder.Find(_file1, _file2);
            result.State.Should().Be(NodeState.Modified);
        }

        [Test]
        public void FindModifedForDifferentFileCreationTime()
        {
            _file2.CreationTime = newDate(7);
            var result = _finder.Find(_file1, _file2);
            result.State.Should().Be(NodeState.Modified);
        }

        [Test]
        public void FindModifedForDifferentFileModificationTime()
        {
            _file2.ModificationTime = newDate(7);
            var result = _finder.Find(_file1, _file2);
            result.State.Should().Be(NodeState.Modified);
        }

        [Test]
        public void FindNoChangeForNullFileHashes()
        {
            _file1.Hash = null;
            _file2.Hash = null;
            var result = _finder.Find(_file1, _file2);
            result.State.Should().Be(NodeState.None);
        }

        [Test]
        public void FindNoChangeForFirstNullFileHash()
        {
            _file1.Hash = null;
            var result = _finder.Find(_file1, _file2);
            result.State.Should().Be(NodeState.None);
        }

        [Test]
        public void FindNoChangeForSecondNullFileHash()
        {
            _file2.Hash = null;
            var result = _finder.Find(_file1, _file2);
            result.State.Should().Be(NodeState.None);
        }

        [Test]
        public void FindModifedForDifferentFileHashes()
        {
            _file2.Hash = "different";
            var result = _finder.Find(_file1, _file2);
            result.State.Should().Be(NodeState.Modified);
        }

        #endregion SINGLE FILE

        #region SINGLE DIRECTORY

        [Test]
        public void FindNoChangesInDirectory()
        {
            _drectory1.InnerNodes.Add(_file1);
            _drectory2.InnerNodes.Add(_file2);
            var result = (Directory)_finder.Find(_drectory1, _drectory2);
            result.State.Should().Be(NodeState.None);
            result.InnerNodes[0].State.Should().Be(NodeState.None);
        }

        [Test]
        public void FindCreatedFileInDirectory()
        {
            _drectory2.InnerNodes.Add(_file1);
            var result = (Directory)_finder.Find(_drectory1, _drectory2);
            result.State.Should().Be(NodeState.Modified);
            result.InnerNodes[0].State.Should().Be(NodeState.Created);
        }

        [Test]
        public void FindModifiedFileInDirectory()
        {
            _drectory1.InnerNodes.Add(_file1);
            _drectory2.InnerNodes.Add(_file2);
            _file2.Hash = "different";
            var result = (Directory)_finder.Find(_drectory1, _drectory2);
            result.State.Should().Be(NodeState.Modified);
            result.InnerNodes[0].State.Should().Be(NodeState.Modified);
        }

        [Test]
        public void FindDeletedFileInDirectory()
        {
            _drectory1.InnerNodes.Add(_file1);
            var result = (Directory)_finder.Find(_drectory1, _drectory2);
            result.State.Should().Be(NodeState.Modified);
            result.InnerNodes[0].State.Should().Be(NodeState.Deleted);
        }

        #endregion SINGLE DIRECTORY

        #region NESTED DIRECTORIES

        // TODO

        #endregion NESTED DIRECTORIES

        // add file/dir
        // remove file/dir
        // modif file/dir

        private DateTimeOffset newDate(int seconds)
        {
            return new DateTimeOffset(2016, 11, 12, 19, 34, seconds, new TimeSpan(0));
        }
    }
}