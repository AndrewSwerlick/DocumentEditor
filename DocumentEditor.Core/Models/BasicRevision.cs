using System;
using System.Collections.Generic;
using System.Linq;
using LayoutEditor.Core.Util;
namespace DocumentEditor.Core.Models
{
    public class BasicRevision : IRevision
    {
        public IRevision PreviousRevisionAppliedTo { get; private set; }
        public IRevision NextRevisionApplied { get; set; }
        public Guid Id { get; private set; }
        private readonly List<Patch> _patches;

        public BasicRevision(IRevision revison, List<Patch> revisionPatches)
        {
            PreviousRevisionAppliedTo = revison;
            _patches = revisionPatches;
            Id = Guid.NewGuid();
        }

        public string GenerateEditedContent()
        {
            var currentContent = PreviousRevisionAppliedTo.GenerateEditedContent();
            return new diff_match_patch().patch_apply(_patches, currentContent)[0] as string;
        }

        public override string ToString()
        {
            return string.Join(",", _patches.SelectMany(p=> p.diffs).Select(d=>d.ToString()).ToArray());
        }
    }
}