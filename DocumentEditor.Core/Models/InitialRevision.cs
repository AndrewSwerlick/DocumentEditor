using System;

namespace DocumentEditor.Core.Models
{
    public class InitialRevision : IRevision
    {
        private readonly string _content;
        public IRevision PreviousRevisionAppliedTo { get { return null; } }
        public IRevision NextRevisionApplied { get; set; }
        public Guid Id { get; private set; }

        public InitialRevision()
        {
            _content = string.Empty;
            Id = Guid.NewGuid();
        }

        public InitialRevision(string content)
        {
            _content = content;
        }

        public string GenerateEditedContent()
        {
            return _content;
        }
    }
}
