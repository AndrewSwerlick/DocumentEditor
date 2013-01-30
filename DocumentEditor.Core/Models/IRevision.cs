using System;

namespace DocumentEditor.Core.Models
{
    public interface IRevision
    {
        IRevision PreviousRevisionAppliedTo { get; }
        IRevision NextRevisionApplied { get; set; }
        Guid Id { get; }

        string GenerateEditedContent();
    }
}
