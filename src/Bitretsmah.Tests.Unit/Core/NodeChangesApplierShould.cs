﻿using Bitretsmah.Core;
using Bitretsmah.Core.Models;
using FluentAssertions;
using NUnit.Framework;
using System;
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
        }

        [Test]
        public void ApplyModifiedDirectoryInDirectory()
        {
        }

        [Test]
        public void ApplyDeletedDirectoryInDirectory()
        {
        }
    }
}