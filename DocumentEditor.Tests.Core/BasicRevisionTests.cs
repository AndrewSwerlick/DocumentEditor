using DocumentEditor.Core.Models;
using LayoutEditor.Core.Util;
using NUnit.Framework;

namespace DocumentEditor.Tests.Core
{
    public class BasicRevisionTests
    {
        [Test]
        public void Ensure_We_Can_Construct_A_Basic_Revision()
        {
            var previousRevision = new TestRevision("Test");
            var patches = new diff_match_patch().patch_make("Test", "Test2");
            var revison = new BasicRevision(previousRevision, patches);
            Assert.That(revison != null);
        }

        [Test]
        public void Ensure_When_We_Construct_A_Revision_It_Contains_A_Reference_To_The_Previous_Revision()
        {
            var previousRevision = new TestRevision("Test");
            var patches = new diff_match_patch().patch_make("Test", "Test2");
            var revison = new BasicRevision(previousRevision, patches);
            Assert.That(revison.PreviousRevisionAppliedTo != null);
        }

        [Test]
        public void Ensure_When_We_Construct_A_Revision_We_Can_Retrieve_The_Current_Document_Content_From_The_Revision_Object
            ()
        {
            var previousRevision = new TestRevision("Test");
            var patches = new diff_match_patch().patch_make("Test", "Test2");
            var revison = new BasicRevision(previousRevision, patches);
            Assert.That(revison.GenerateEditedContent() != null);
        }

        [Test]
        public void
            Ensure_When_We_Construct_A_Revision_And_Retreive_The_Current_Document_That_It_Contains_Changes_From_The_Latest_Revision
            ()
        {
            var previousRevision = new TestRevision("Test");
            var patches = new diff_match_patch().patch_make("Test", "Test2");
            var revison = new BasicRevision(previousRevision,patches);
            var content = revison.GenerateEditedContent();
            Assert.That(content, Is.EqualTo("Test2"));
        }
    }
}