using Bitretsmah.Core;
using Bitretsmah.Core.Models;
using FluentAssertions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using static Bitretsmah.Tests.Unit.Core.NodesTestHelper;

namespace Bitretsmah.Tests.Unit.Core
{
    [TestFixture]
    public class NodeChangesApplierShould
    {
        [Test]
        public void ApplyFileModification()
        {
            var initialFile = new File
            {
                Name = "foo.txt",
                State = NodeState.None,
                Hash = "hash_1",
                Size = 1234,
                CreationTime = new DateTimeOffset(2015, 9, 10, 17, 32, 13, new TimeSpan(0)),
                ModificationTime = new DateTimeOffset(2015, 9, 11, 18, 33, 14, new TimeSpan(0)),
                AbsolutePath = @"C:\Temp\foo.txt"
            };

            var change = new File
            {
                Name = "foo.txt",
                State = NodeState.Modified,
                Hash = "hash_2",
                Size = 4321,
                CreationTime = new DateTimeOffset(2016, 10, 11, 18, 33, 14, new TimeSpan(0)),
                ModificationTime = new DateTimeOffset(2016, 11, 12, 19, 34, 15, new TimeSpan(0)),
                AbsolutePath = @"C:\Temp\foo.txt"
            };

            var expectedFile = change.DeepCopy();
            expectedFile.State = NodeState.None;

            INodeChangesApplier applier = new NodeChangesApplier();
            var actualFile = applier.Apply(initialFile, change);

            actualFile.ShouldBeEquivalentTo(expectedFile);
            actualFile.ShouldSerializeSameAs(expectedFile);
        }

        [Test]
        public void ApplyCreatedFileInDirectory()
        {
            var initialDirectory =
                CreateDirectory("root");

            var change =
                CreateDirectory("root", NodeState.Modified,
                    CreateFile("F1", NodeState.Created));

            var expectedDirectory =
                CreateDirectory("root", NodeState.None,
                    CreateFile("F1", NodeState.None));

            INodeChangesApplier applier = new NodeChangesApplier();
            var actualDirectory = applier.Apply(initialDirectory, change);

            actualDirectory.ShouldBeEquivalentTo(expectedDirectory);
            actualDirectory.ShouldSerializeSameAs(expectedDirectory);
        }

        [Test]
        public void ApplyModifiedFileInDirectory()
        {
            var initialDirectory =
                CreateDirectory("root",
                    CreateFile("F1", "hash"));

            var change =
                CreateDirectory("root", NodeState.Modified,
                    CreateFile("F1", NodeState.Modified, "different_hash"));

            var expectedDirectory =
                CreateDirectory("root", NodeState.None,
                    CreateFile("F1", NodeState.None, "different_hash"));

            INodeChangesApplier applier = new NodeChangesApplier();
            var actualDirectory = applier.Apply(initialDirectory, change);

            actualDirectory.ShouldBeEquivalentTo(expectedDirectory);
            actualDirectory.ShouldSerializeSameAs(expectedDirectory);
        }

        [Test]
        public void ApplyDeletedFileInDirectory()
        {
            var initialDirectory =
                CreateDirectory("root",
                    CreateFile("F1"));

            var change =
                CreateDirectory("root", NodeState.Modified,
                    CreateFile("F1", NodeState.Deleted));

            var expectedDirectory =
                CreateDirectory("root");

            INodeChangesApplier applier = new NodeChangesApplier();
            var actualDirectory = applier.Apply(initialDirectory, change);

            actualDirectory.ShouldBeEquivalentTo(expectedDirectory);
            actualDirectory.ShouldSerializeSameAs(expectedDirectory);
        }

        [Test]
        public void ApplyCreatedDirectoryInDirectory()
        {
            var initialDirectory =
                CreateDirectory("D0");

            var change =
                CreateDirectory("D0", NodeState.Modified,
                    CreateDirectory("D1", NodeState.Created,
                        CreateDirectory("D2", NodeState.Created,
                            CreateFile("F", NodeState.Created))));

            var expectedDirectory =
                CreateDirectory("D0", NodeState.None,
                    CreateDirectory("D1", NodeState.None,
                        CreateDirectory("D2", NodeState.None,
                            CreateFile("F", NodeState.None))));

            INodeChangesApplier applier = new NodeChangesApplier();
            var actualDirectory = applier.Apply(initialDirectory, change);

            actualDirectory.ShouldBeEquivalentTo(expectedDirectory);
            actualDirectory.ShouldSerializeSameAs(expectedDirectory);
        }

        [Test]
        public void ApplyDeletedDirectoryInDirectory()
        {
            var initialDirectory =
                CreateDirectory("D0",
                    CreateDirectory("D1",
                        CreateDirectory("D2",
                            CreateFile("F"))));

            var change =
                CreateDirectory("D0", NodeState.Modified,
                    CreateDirectory("D1", NodeState.Modified,
                        CreateDirectory("D2", NodeState.Deleted)));

            var expectedDirectory =
                CreateDirectory("D0", NodeState.None,
                    CreateDirectory("D1", NodeState.None));

            INodeChangesApplier applier = new NodeChangesApplier();
            var actualDirectory = applier.Apply(initialDirectory, change);

            actualDirectory.ShouldBeEquivalentTo(expectedDirectory);
            actualDirectory.ShouldSerializeSameAs(expectedDirectory);
        }

        [Test]
        public void ApplyManyChanges()
        {
            var initialDirectory =
                CreateDirectory("root");

            var change1 =
                CreateDirectory("root", NodeState.Modified,
                    CreateDirectory("D0", NodeState.Created,
                        CreateDirectory("D00", NodeState.Created,
                            CreateFile("F000", NodeState.Created))));

            var change2 =
                CreateDirectory("root", NodeState.Modified,
                    CreateDirectory("D0", NodeState.Modified,
                        CreateDirectory("D00", NodeState.Deleted,
                            CreateFile("F000", NodeState.Deleted))),
                    CreateDirectory("D1", NodeState.Created,
                        CreateDirectory("D10", NodeState.Created,
                            CreateFile("F1000", NodeState.Created))));

            var change3 =
                CreateDirectory("root", NodeState.Modified,
                    CreateDirectory("D1", NodeState.Modified,
                        CreateDirectory("D10", NodeState.Modified,
                            CreateFile("F1000", NodeState.Modified, "different_hash_for_F1000"))),
                    CreateFile("F2", NodeState.Created, "hash_for_F2"));

            var expectedDirectory =
                CreateDirectory("root", NodeState.None,
                    CreateDirectory("D0", NodeState.None),
                    CreateDirectory("D1", NodeState.None,
                        CreateDirectory("D10", NodeState.None,
                            CreateFile("F1000", NodeState.None, "different_hash_for_F1000"))),
                    CreateFile("F2", NodeState.None, "hash_for_F2"));

            var changes = new List<Node>() { change1, change2, change3 };

            INodeChangesApplier applier = new NodeChangesApplier();
            var actualDirectory = applier.Apply(initialDirectory, changes);

            actualDirectory.ShouldBeEquivalentTo(expectedDirectory);
            actualDirectory.ShouldSerializeSameAs(expectedDirectory);
        }
    }
}