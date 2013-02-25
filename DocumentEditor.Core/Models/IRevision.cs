using System;
using System.Collections.Generic;
using LayoutEditor.Core.Util;

namespace DocumentEditor.Core.Models
{
    public interface IRevision
    {
        IRevision PreviousRevisionAppliedTo { get; }
        IRevision NextRevisionApplied { get; set; }
        Guid Id { get; }

        string GenerateEditedContent();
        IList<Patch> BuildPatch();
    }
}
