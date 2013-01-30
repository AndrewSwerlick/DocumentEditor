using System;
using System.Collections.Generic;
using DocumentEditor.Core;
using DocumentEditor.Tests.Core.Util;
using NUnit.Framework;
using DocumentEditor.Core.Models;

namespace DocumentEditor.Tests.Core
{
    public class MergingRevisionTests
    {
        [Test]
        public void Ensure_We_Can_Construct_A_Merging_Revision()
        {
            var originalRevision = new TestRevision("Test\nTest");
            var revisionOne = new BasicRevision(originalRevision,
                                                Patches.Make(originalRevision.GenerateEditedContent(), "Test\nTest3"));

            var revisionTwo = new BasicRevision(originalRevision,
                                                Patches.Make(originalRevision.GenerateEditedContent(), "Test2\nTest"));
            var revision = new MergingRevision(originalRevision, new List<IRevision> { revisionOne, revisionTwo });
            Assert.That(revision != null);
        }

        [Test]
        public void
            Given_A_Merging_Revision_With_Two_Mergeable_Revisions_Ensure_That_When_We_Retrieve_The_Current_Document_It_Contains_The_Merge
            ()
        {
            var originalRevision = new TestRevision("Test\nTest");
            var revisionOne = new BasicRevision(originalRevision, Patches.Make(originalRevision.GenerateEditedContent(), "Test\nTest3"));
            var revisionTwo = new BasicRevision(originalRevision, Patches.Make(originalRevision.GenerateEditedContent(), "Test2\nTest"));
            var revision = new MergingRevision(originalRevision, new List<IRevision> {revisionOne, revisionTwo});
            Assert.That(revision.GenerateEditedContent(), Is.EqualTo("Test2\nTest3"));
        }

        [Test]
        public void
            Given_A_Merging_Revision_With_Two_Un_Mergeable_Revisions_Ensure_that_When_We_Retrieve_The_Current_Document_We_Recieve_An_Exception
            ()
        {
            var originalRevision = new TestRevision("Test\nTest");
            var revisionOne = new BasicRevision(originalRevision,
                                                Patches.Make(originalRevision.GenerateEditedContent(),
                                                             "The quick brown fox jumped over the lazy dog\nTest"));
            var revisionTwo = new BasicRevision(originalRevision,
                                                Patches.Make(originalRevision.GenerateEditedContent(),
                                                             "Waiter, there's a fly in my soup\nTest"));
            var revision = new MergingRevision(originalRevision, new List<IRevision> {revisionOne, revisionTwo});
            Assert.Throws<ConflictException>(() => revision.GenerateEditedContent());
        }

        [Test]
        public void
            Given_Two_Un_Mergeable_Revisions_Ensure_that_When_We_Construct_The_MergingRevision_Ensure_It_Is_Marked_As_In_Conflict
            ()
        {
            var originalRevision = new TestRevision("Test\nTest");
            var revisionOne = new BasicRevision(originalRevision,
                                                Patches.Make(originalRevision.GenerateEditedContent(),
                                                             "The quick brown fox jumped over the lazy dog\nTest"));
            var revisionTwo = new BasicRevision(originalRevision,
                                                Patches.Make(originalRevision.GenerateEditedContent(),
                                                             "Waiter, there's a fly in my soup\nTest"));
            var revision = new MergingRevision(originalRevision, new List<IRevision> { revisionOne, revisionTwo });
            Assert.That(revision.RevisionsConflict);
        }

        [Test]
        public void
            Given_A_Merging_Revision_With_Two_Un_Mergeable_Revisions_Ensure_that_If_We_Set_A_Revision_For_Resolution_And_Then_Call_GenerateEditedContent_It_Returns_The_Resolution_Revision
            ()
        {
            var originalRevision = new TestRevision("Test\nTest");
            var revisionOne = new BasicRevision(originalRevision,
                                                Patches.Make(originalRevision.GenerateEditedContent(),
                                                             "The quick brown fox jumped over the lazy dog\nTest"));
            var revisionTwo = new BasicRevision(originalRevision,
                                                Patches.Make(originalRevision.GenerateEditedContent(),
                                                             "Waiter, there's a fly in my soup\nTest"));
            var revision = new MergingRevision(originalRevision, new List<IRevision> { revisionOne, revisionTwo });
            revision.ResolveConflict(new TestRevision("The quick brown fox jumped over the lazy dog\nWaiter, there's a fly in my soup\nTest"));
            Assert.That(revision.GenerateEditedContent(),
                        Is.EqualTo(
                            "The quick brown fox jumped over the lazy dog\nWaiter, there's a fly in my soup\nTest"));
        }
    }
}