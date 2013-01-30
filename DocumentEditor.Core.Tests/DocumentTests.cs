using System.Collections.Generic;
using System.Linq;
using DocumentEditor.Core.Models;
using DocumentEditor.Core.Tests.Util;
using NUnit.Framework;

namespace DocumentEditor.Core.Tests
{
    public class DocumentTests
    {
        [Test]
        public void Ensure_That_We_Can_Construct_A_Document()
        {
            var document = new Document();
            Assert.That(document != null);
        }

        [Test]
        public void Ensure_That_When_We_Construct_A_Document_That_It_Has_An_Initial_Revision()
        {
            var document = new Document();
            Assert.That(document.CurrentRevision != null);
        }

        [Test]
        public void Ensure_We_Can_Add_A_Revision_To_A_Document_And_Then_Retrive_The_New_Document_Contents()
        {
            var document = new Document();
            var patch = Patches.Make("", "This is a test");
            var revision = new BasicRevision(document.CurrentRevision, patch);

            document.Edit(revision);
            Assert.That(document.CurrentRevision.GenerateEditedContent(), Is.EqualTo("This is a test"));
        }

        [Test]
        public void Ensure_That_If_We_Add_Two_Revisions_With_The_Same_Previous_Revision_The_Document_Handles_The_Merge()
        {
            var document = new Document("I went to the store to get mom some milk");
           
            var revisionOne = new BasicRevision(document.CurrentRevision,
                                                Patches.Make("I went to the store to get mom some milk",
                                                             "I went to the store to get mom some milk and crackers"));
            var revisionTwo = new BasicRevision(document.CurrentRevision,
                                                Patches.Make("I went to the store to get mom some milk",
                                                             "I ran to the store to get mom some milk"));

            document.Edit(revisionTwo);
            document.Edit(revisionOne);

            Assert.That(document.CurrentRevision.GenerateEditedContent(),
                        Is.EqualTo("I ran to the store to get mom some milk and crackers"));
        }

        [Test]
        public void Ensure_That_If_We_Add_Three_Revisions_With_The_Same_Previous_Revision_The_Document_Handles_The_Merge
            ()
        {
            var document = new Document("I went to the store to get mom some milk");
            var revisionStrings = new[]
                {
                    "I went to the store to get mom some milk and crackers",
                    "I ran to the store to get mom some milk",
                    "I went to the corner store to get mom some milk"
                };
            var revisions = revisionStrings.Select(s => new BasicRevision(document.CurrentRevision,
                                                                          Patches.Make(
                                                                              "I went to the store to get mom some milk",
                                                                              s))).ToList();
            foreach (var revision in revisions)
            {
                document.Edit(revision);
            }

            Assert.That(document.CurrentRevision.GenerateEditedContent(),
                        Is.EqualTo("I ran to the corner store to get mom some milk and crackers"));
        }

        [Test]
        public void Ensure_That_If_We_Add_Four_Revisions_With_The_Same_Previous_Revision_The_Document_Handles_The_Merge()
        {
            var document = new Document("I went to the store to get mom some milk");
            var revisionStrings = new[]
                {
                    "I went to the store to get mom some milk and crackers",
                    "I ran to the store to get mom some milk",
                    "I went to the corner store to get mom some milk",
                    "I went to the store to get mom and dad some milk"
                };
            var revisions = revisionStrings.Select(s => new BasicRevision(document.CurrentRevision,
                                                                          Patches.Make(
                                                                              "I went to the store to get mom some milk",
                                                                              s))).ToList();
            foreach (var revision in revisions)
                document.Edit(revision);

            Assert.That(document.CurrentRevision.GenerateEditedContent(),
                        Is.EqualTo("I ran to the corner store to get mom and dad some milk and crackers"));            
        }

        [Test]
        public void
            Given_That_We_Add_Two_Revisions_Where_One_Revision_Deletes_What_Another_Edits_And_They_Have_The_Same_Previous_Revision_Ensure_The_Document_Throws_A_Conflict_Exception
            ()
        {
            var document = new Document("I went to the store to get mom some milk");
            var revisionStrings = new[]
                {
                    "I went to the store",
                    "I went to the store to get mom some orange juice"
                };
            var revisions = revisionStrings.Select(s => new BasicRevision(document.CurrentRevision,
                                                                          Patches.Make(
                                                                              "I went to the store to get mom some milk",
                                                                              s))).ToList();
            foreach (var revision in revisions)
                document.Edit(revision);

            Assert.Throws<ConflictException>(()=> document.CurrentRevision.GenerateEditedContent()); 
        }

        [Test]
        public void
            Ensure_That_If_We_Add_Two_Revisions_Where_Both_Revisions_Change_The_Same_Word_In_The_Previous_Revisions_The_Document_Throws_A_Conflict_Exception
            ()
        {
            var document = new Document("I went to the store to get mom some milk");
            var revisionStrings = new[]
                {
                    "I went to the store to get mom some cheese",
                    "I went to the store to get mom some orange juice"
                };
            var revisions = revisionStrings.Select(s => new BasicRevision(document.CurrentRevision,
                                                                          Patches.Make(
                                                                              "I went to the store to get mom some milk",
                                                                              s))).ToList();
            foreach (var revision in revisions)
                document.Edit(revision);

            Assert.Throws<ConflictException>(() => document.CurrentRevision.GenerateEditedContent()); 
        }

        [Test]
        public void
            Ensure_That_Revisions_With_Identical_Previous_Revisions_Can_Be_Applied_To_Documents_In_Different_Orders_For_The_Same_Result
            ()
        {
            var documents = new List<Document>
                {
                    new Document("I went to the store to get mom some milk"),
                    new Document("I went to the store to get mom some milk")
                };
            foreach (var it in documents.Select((x,i) => new {Document =x, Index =i}))
            {
                var revisionStrings = new[]
                    {
                        "I went to the store to get mom some milk and crackers",
                        "I ran to the store to get mom some milk",
                        "I went to the corner store to get mom some milk"
                    };
                var revisions = revisionStrings.Select(s => new BasicRevision(it.Document.CurrentRevision,
                                                                              Patches.Make(
                                                                                  "I went to the store to get mom some milk",
                                                                                  s))).ToList();
                if (it.Index % 2 == 0)
                    revisions.Reverse();

                foreach (var revision in revisions)
                    it.Document.Edit(revision);

            }
            Assert.That(documents[0].CurrentRevision.GenerateEditedContent(),
                        Is.EqualTo(documents[1].CurrentRevision.GenerateEditedContent()));
        }

        [Test]
        public void Given_A_Revision_Guid_Ensure_We_Can_Load_A_Specific_Revision()
        {
            var document = new Document("Test");
            document.Edit(new BasicRevision(document.CurrentRevision, Patches.Make(document.Contents, "Test2")));
            var revisionIdToLoad = document.CurrentRevision.Id;
            document.Edit(new BasicRevision(document.CurrentRevision, Patches.Make(document.Contents, "Test3")));

            var revision = document.LoadRevision(revisionIdToLoad);

            Assert.That(revision.GenerateEditedContent(), Is.EqualTo("Test2"));
        }

        [Test]
        public void Given_A_Revision_Guid_For_A_Revision_That_Was_Part_Of_A_Merge_Ensure_We_Can_Load_That_Revision()
        {
            var document = new Document("Test");
            var sharedParentRevision = document.CurrentRevision;
            document.Edit(new BasicRevision(sharedParentRevision, Patches.Make(document.Contents, "Test2")));
            var revisionIdToLoad = document.CurrentRevision.Id;
            document.Edit(new BasicRevision(sharedParentRevision, Patches.Make(document.Contents, "Test3")));

            var revision = document.LoadRevision(revisionIdToLoad);

            Assert.That(revision.GenerateEditedContent(), Is.EqualTo("Test2"));
        }

        [Test]
        public void
            Given_We_Have_Added_A_Subscriber_To_A_Document_Ensure_That_When_A_Document_Is_Edited_The_Subscriber_Is_Notified
            ()
        {
            var document = new Document("Test");
            var subscriber = new Subscriber();
            document.AddSubscriber(subscriber);

            var revision = new BasicRevision(document.CurrentRevision, Patches.Make(document.Contents, "Test 2"));
            var eventFired = false;
            subscriber.SubscriberNotifiedOfUpdate += (o, e) => eventFired = true;
            document.Edit(revision);

            Assert.That(eventFired);
        }

        [Test]
        public void
            Given_We_Have_Added_A_Subscriber_To_A_Document_Ensure_That_When_The_Document_Merges_Two_Revisions_The_Subscriber_Is_Notified_With_The_Merging_Revision
            ()
        {
            var document = new Document("Test");
            var subscriber = new Subscriber();
            document.AddSubscriber(subscriber);

            var revision1 = new BasicRevision(document.CurrentRevision, Patches.Make(document.Contents, "Test 2"));
            var revision2 = new BasicRevision(document.CurrentRevision, Patches.Make(document.Contents, "Test 3"));
            
            var notificationContainsMergingRevision = false;
            subscriber.SubscriberNotifiedOfUpdate += (o, e) => notificationContainsMergingRevision = e.Revision is MergingRevision;
            document.Edit(revision1);
            document.Edit(revision2);

            Assert.That(notificationContainsMergingRevision);
        }

        [Test]
        public void
            Given_We_Have_A_Document_With_A_Merging_Revision_As_The_Current_Revision_Ensure_That_When_The_Document_Is_Edited_With_A_Revision_Targeting_One_Of_The_Merged_Revisions_The_New_Revision_Is_Merged_In
            ()
        {
            var document = new Document("I went to the store to get some milk");
            var revisionsToMerge = new[]
                {
                    "I ran to the store to get some milk",
                    "I went to the store to get some milk and crackers"
                }.Select(s => new BasicRevision(document.CurrentRevision, Patches.Make(document.Contents, s))).ToList();

            foreach (var basicRevision in revisionsToMerge)
                document.Edit(basicRevision);

            var lateRevisionsParent = revisionsToMerge.First();
            var lateRevision = new BasicRevision(lateRevisionsParent,
                                                 Patches.Make(lateRevisionsParent.GenerateEditedContent(),
                                                              "I ran to the corner store to get some milk"));

            document.Edit(lateRevision);

            Assert.That(document.Contents, Is.EqualTo("I ran to the corner store to get some milk and crackers"));
        }

        [Test]
        public void
            Given_We_Have_A_Document_With_A_Merging_Revision_As_The_Parent_Of_The_Current_Revision_Ensure_That_When_The_Document_Is_Edited_With_A_Revision_Targeting_One_Of_The_Merged_Revisions_The_New_Revision_Is_Merged_In
            ()
        {
            var document = new Document("I went to the store to get some milk");
            var revisionsToMerge = new[]
                {
                    "I ran to the store to get some milk",
                    "I went to the store to get some milk and crackers"
                }.Select(s => new BasicRevision(document.CurrentRevision, Patches.Make(document.Contents, s))).ToList();

            foreach (var basicRevision in revisionsToMerge)
                document.Edit(basicRevision);

            var postMergeRevision = new BasicRevision(document.CurrentRevision,
                                                      Patches.Make(document.Contents,
                                                                   "I ran to the store to get mom some milk and crackers"));
            document.Edit(postMergeRevision);

            var lateRevisionsParent = revisionsToMerge.First();
            var lateRevision = new BasicRevision(lateRevisionsParent,
                                                 Patches.Make(lateRevisionsParent.GenerateEditedContent(),
                                                              "I ran to the corner store to get some milk"));

            document.Edit(lateRevision);

            Assert.That(document.Contents, Is.EqualTo("I ran to the corner store to get mom some milk and crackers"));
        }
    }
}