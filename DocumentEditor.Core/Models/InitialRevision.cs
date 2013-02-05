using System;

namespace DocumentEditor.Core.Models
{
    public class InitialRevision : IRevision
    {
        public string Content { get; private set; }
        public IRevision PreviousRevisionAppliedTo { get { return null; } }
        public IRevision NextRevisionApplied { get; set; }
        public Guid Id { get; private set; }

        public InitialRevision()
        {
            Content = string.Empty;
            Id = Guid.NewGuid();
        }

        public InitialRevision(string content)
        {
            Content = content;
            Id = Guid.NewGuid();
        }

        public string GenerateEditedContent()
        {
            return Content;
        }
    }
}
