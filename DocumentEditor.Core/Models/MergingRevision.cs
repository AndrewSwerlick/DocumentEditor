using System;
using System.Collections.Generic;
using LayoutEditor.Core.Util;
using System.Linq;

namespace DocumentEditor.Core.Models
{
    public class MergingRevision : IRevision
    {
        private object[] _patchResult;

        public IList<IRevision> RevisionsToMerge { get; private set; }
        public IRevision PreviousRevisionAppliedTo { get; private set; }
        public IRevision NextRevisionApplied { get; set; }
        public Guid Id { get; private set; }
        public bool RevisionsConflict { get; private set; }
        public IRevision RevisionChosenForResolution { get; private set; }

        public MergingRevision(IRevision revisionAppliedTo, IList<IRevision> revisionsToMerge)
        {
            revisionAppliedTo.NextRevisionApplied = this;
            RevisionsToMerge = revisionsToMerge;
            PreviousRevisionAppliedTo = revisionAppliedTo;
            CheckForConflicts();
            Id = Guid.NewGuid();
        }     

        public string GenerateEditedContent()
        {
            if(RevisionsConflict && RevisionChosenForResolution == null)
                throw  new ConflictException(_patchResult);

            if (RevisionChosenForResolution != null)
                return RevisionChosenForResolution.GenerateEditedContent();

            var dmp = new diff_match_patch { Match_Distance = 200, Match_Threshold = 0.2f };

            var patches = dmp.patch_make(PreviousRevisionAppliedTo.GenerateEditedContent(), RevisionsToMerge.ElementAt(0).GenerateEditedContent());

            var patchResult = dmp.patch_apply(patches, RevisionsToMerge.ElementAt(1).GenerateEditedContent());          

            return patchResult[0] as string;
        }

        public IList<Patch> BuildPatch()
        {
            var lastContent = PreviousRevisionAppliedTo.GenerateEditedContent();
            return new diff_match_patch().patch_make(lastContent, GenerateEditedContent());
        }

        public void ResolveConflict(IRevision revision)
        {
            if(!RevisionsConflict || RevisionChosenForResolution != null)
                throw new InvalidOperationException("The revision is not currently in conflict, no resolution necessary");

            RevisionChosenForResolution = revision;

        }

        private void CheckForConflicts()
        {
             var dmp = new diff_match_patch {Match_Distance = 200, Match_Threshold = 0.2f};

            var patches = dmp.patch_make(RevisionsToMerge[0].PreviousRevisionAppliedTo.GenerateEditedContent(), RevisionsToMerge[0].GenerateEditedContent());

            _patchResult = dmp.patch_apply(patches, RevisionsToMerge.ElementAt(1).GenerateEditedContent());
            var patchSucessIndicators = (bool[])_patchResult[1];

            if (patchSucessIndicators.Any(sucess => !sucess))
                RevisionsConflict = true;
        }
    }
}