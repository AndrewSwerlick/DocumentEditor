using System;
using DocumentEditor.Core.Models;

namespace DocumentEditor.Core.Tests
{
    public class TestRevision : IRevision
    {
        public IRevision PreviousRevisionAppliedTo { get { return null; } }
        public IRevision NextRevisionApplied { get; set; }
        public Guid Id { get; private set; }
        public string RevisionContent { get; private set; }

        public TestRevision(string revisionContent)
        {
            RevisionContent = revisionContent;
            Id = Guid.NewGuid();
        }

        public string GenerateEditedContent()
        {
            return RevisionContent;
        }
    }
}