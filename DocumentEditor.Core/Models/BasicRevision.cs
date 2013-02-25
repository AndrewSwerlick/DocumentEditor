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
        public List<Patch> Patches { get; private set; }

        /// <summary>
        /// Private parameterless constructor for JSON.Net to ensure the object can be deserialized
        /// </summary>
        // ReSharper disable UnusedMember.Local
        private BasicRevision()
        {
            
        }
        // ReSharper restore UnusedMember.Local

        public BasicRevision(IRevision revison, List<Patch> revisionPatches, Guid id)
        {
            PreviousRevisionAppliedTo = revison;
            Patches = revisionPatches;
            Id = id;
        }

        public BasicRevision(IRevision revision, List<Patch> revisionPatches ) : this(revision,revisionPatches, Guid.NewGuid()){}

        public string GenerateEditedContent()
        {
            var currentContent = PreviousRevisionAppliedTo.GenerateEditedContent();
            return new diff_match_patch().patch_apply(Patches, currentContent)[0] as string;
        }

        public IList<Patch> BuildPatch()
        {
            return Patches;
        }

        public override string ToString()
        {
            return string.Join(",", Patches.SelectMany(p=> p.diffs).Select(d=>d.ToString()).ToArray());
        }
    }
}