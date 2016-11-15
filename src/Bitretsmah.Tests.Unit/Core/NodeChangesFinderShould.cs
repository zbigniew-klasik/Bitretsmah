using Bitretsmah.Core;
using Bitretsmah.Core.Models;
using FluentAssertions;
using NUnit.Framework;
using System;
using System.Collections.Generic;

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
            result.InnerNodes.Should().BeEmpty();
        }

        [Test]
        public void FindCreatedFileInDirectory()
        {
            _drectory1.InnerNodes.Add(_file1);
            _drectory2.InnerNodes.Add(_file2);
            _drectory2.InnerNodes.Add(new File("bar.txt", 987, "HASH", newDate(7), newDate(8)));
            var result = (Directory)_finder.Find(_drectory1, _drectory2);
            result.State.Should().Be(NodeState.Modified);
            result.InnerNodes.Count.Should().Be(1);
            result.InnerNodes[0].State.Should().Be(NodeState.Created);
            result.InnerNodes[0].Name.Should().Be("bar.txt");
        }

        [Test]
        public void FindModifiedFileInDirectory()
        {
            _drectory1.InnerNodes.Add(_file1);
            _drectory2.InnerNodes.Add(_file2);
            _drectory1.InnerNodes.Add(new File("bar.txt", 987, "HASH_OLD", newDate(7), newDate(8)));
            _drectory2.InnerNodes.Add(new File("bar.txt", 987, "HASH_NEW", newDate(7), newDate(8)));
            var result = (Directory)_finder.Find(_drectory1, _drectory2);
            result.State.Should().Be(NodeState.Modified);
            result.InnerNodes.Count.Should().Be(1);
            result.InnerNodes[0].State.Should().Be(NodeState.Modified);
            result.InnerNodes[0].Name.Should().Be("bar.txt");
        }

        [Test]
        public void FindDeletedFileInDirectory()
        {
            _drectory1.InnerNodes.Add(_file1);
            _drectory2.InnerNodes.Add(_file2);
            _drectory1.InnerNodes.Add(new File("bar.txt", 987, "HASH_OLD", newDate(7), newDate(8)));
            var result = (Directory)_finder.Find(_drectory1, _drectory2);
            result.State.Should().Be(NodeState.Modified);
            result.InnerNodes.Count.Should().Be(1);
            result.InnerNodes[0].State.Should().Be(NodeState.Deleted);
            result.InnerNodes[0].Name.Should().Be("bar.txt");
        }

        #endregion SINGLE DIRECTORY

        #region NESTED DIRECTORIES

        [Test]
        public void FindCreatedDirectoryAndMarkInnerElements()
        {
            var initialNode =
                CreateD("D_0",
                    CreateF("F_1"));

            var finalNode =
                CreateD("D_0",
                    CreateF("F_1"),
                    CreateD("D_1", // has been created
                        CreateF("F_2"),
                        CreateD("D_2",
                            CreateF("F_3"))));

            var expectedResult =
                CreateD("D_0", NodeState.Modified,
                    CreateD("D_1", NodeState.Created,
                        CreateF("F_2", NodeState.Created),
                        CreateD("D_2", NodeState.Created,
                            CreateF("F_3", NodeState.Created))));

            var actualResult = (Directory)_finder.Find(initialNode, finalNode);
            actualResult.ShouldBeEquivalentTo(expectedResult);
            actualResult.ToJson().Should().Be(expectedResult.ToJson());
        }

        [Test]
        public void FindDeletedDirectoryAndMarkInnerElements()
        {
            var initialNode =
                CreateD("D_0",
                    CreateF("F_1"),
                    CreateD("D_1", // will be deleted
                        CreateF("F_2"),
                        CreateD("D_2",
                            CreateF("F_3"))));

            var finalNode =
                CreateD("D_0",
                    CreateF("F_1"));

            var expectedResult =
                CreateD("D_0", NodeState.Modified,
                    CreateD("D_1", NodeState.Deleted,
                        CreateF("F_2", NodeState.Deleted),
                        CreateD("D_2", NodeState.Deleted,
                            CreateF("F_3", NodeState.Deleted))));

            var actualResult = (Directory)_finder.Find(initialNode, finalNode);
            actualResult.ShouldBeEquivalentTo(expectedResult);
            actualResult.ToJson().Should().Be(expectedResult.ToJson());
        }

        [Test]
        public void FindModifiedInnerDirectory()
        {
            var initialNode =
                CreateD("root",
                    CreateF("F_0"),
                    CreateF("F_1"),                 // will be deleted
                    CreateD("D_2",
                        CreateF("F_2_0"),
                        CreateF("F_2_1"),
                        CreateD("D_2_2",
                            CreateF("F_2_2_0"),
                            CreateF("F_2_2_1")),    // will be deleted
                        CreateD("D_2_3",            // will be deleted
                            CreateF("F_2_3_0"),
                            CreateF("F_2_3_1"))),
                    CreateD("D_3",                  // will be deleted
                        CreateF("F_3_0"),
                        CreateF("F_3_1"),
                        CreateD("D_3_2",
                            CreateF("F_3_2_0"),
                            CreateF("F_3_2_1")),
                        CreateD("D_3_3",
                            CreateF("F_3_3_0"),
                            CreateF("F_3_3_1"))));

            var finalNode =
                CreateD("root",
                    CreateF("F_0"),
                    CreateD("D_2",
                        CreateF("F_2_0"),
                        CreateF("F_2_1", NodeState.None, "different_hash_1"),
                        CreateD("D_2_2",
                            CreateF("F_2_2_0", NodeState.None, "different_hash_2")),
                            CreateD("D_2_2_2",      // has been created
                                CreateF("F_2_2_2_0"),
                                CreateD("F_2_2_2_1",
                                    CreateF("F_2_2_2_1_0")))));

            var expectedResult =
                CreateD("root", NodeState.Modified,
                    CreateF("F_1", NodeState.Deleted),
                    CreateD("D_2", NodeState.Modified,
                        CreateF("F_2_1", NodeState.Modified, "different_hash_1"),
                        CreateD("D_2_2", NodeState.Modified,
                            CreateF("F_2_2_0", NodeState.Modified, "different_hash_2"),
                            CreateF("F_2_2_1", NodeState.Deleted)),
                            CreateD("D_2_2_2", NodeState.Created,
                                CreateF("F_2_2_2_0", NodeState.Created),
                                CreateD("F_2_2_2_1", NodeState.Created,
                                    CreateF("F_2_2_2_1_0", NodeState.Created))),
                        CreateD("D_2_3", NodeState.Deleted,
                            CreateF("F_2_3_0", NodeState.Deleted),
                            CreateF("F_2_3_1", NodeState.Deleted))),
                    CreateD("D_3", NodeState.Deleted,
                        CreateF("F_3_0", NodeState.Deleted),
                        CreateF("F_3_1", NodeState.Deleted),
                        CreateD("D_3_2", NodeState.Deleted,
                            CreateF("F_3_2_0", NodeState.Deleted),
                            CreateF("F_3_2_1", NodeState.Deleted)),
                        CreateD("D_3_3", NodeState.Deleted,
                            CreateF("F_3_3_0", NodeState.Deleted),
                            CreateF("F_3_3_1", NodeState.Deleted))));

            var actualResult = (Directory)_finder.Find(initialNode, finalNode);

            actualResult.ShouldBeEquivalentTo(expectedResult);
            actualResult.ToJson().Should().Be(expectedResult.ToJson());
        }

        #endregion NESTED DIRECTORIES

        #region HELP METHODS

        private DateTimeOffset newDate(int seconds)
        {
            return new DateTimeOffset(2016, 11, 12, 19, 34, seconds, new TimeSpan(0));
        }

        private File CreateF(string name, NodeState state = NodeState.None, string hash = "hash")
        {
            return new File
            {
                Name = name,
                State = state,
                Hash = hash,
            };
        }

        private Directory CreateD(string name, NodeState state, params Node[] nodes)
        {
            return new Directory
            {
                Name = name,
                State = state,
                InnerNodes = new List<Node>(nodes)
            };
        }

        private Directory CreateD(string name, params Node[] nodes)
        {
            return new Directory
            {
                Name = name,
                InnerNodes = new List<Node>(nodes)
            };
        }

        #endregion HELP METHODS
    }
}