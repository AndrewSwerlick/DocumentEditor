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
        private List<Patch> Patches { get; set; }

        /// <summary>
        /// Private parameterless constructor for JSON.Net to ensure the object can be deserialized
        /// </summary>
        // ReSharper disable UnusedMember.Local
        private BasicRevision()
        {
            
        }
        // ReSharper restore UnusedMember.Local

        public BasicRevision(IRevision revison, List<Patch> revisionPatches)
        {
            PreviousRevisionAppliedTo = revison;
            Patches = revisionPatches;
            Id = Guid.NewGuid();
        }

        public string GenerateEditedContent()
        {
            var currentContent = PreviousRevisionAppliedTo.GenerateEditedContent();
            return new diff_match_patch().patch_apply(Patches, currentContent)[0] as string;
        }

        public override string ToString()
        {
            return string.Join(",", Patches.SelectMany(p=> p.diffs).Select(d=>d.ToString()).ToArray());
        }
    }
}