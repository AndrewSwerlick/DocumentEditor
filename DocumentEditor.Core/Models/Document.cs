using System;
using System.Collections.Generic;
using System.Linq;

namespace DocumentEditor.Core.Models
{
    public class Document
    {
        private readonly IList<Subscriber> _subcribers;
        private IDictionary<Guid, Tuple<IRevision, RevisionStatus>> _revisionMap = new Dictionary<Guid, Tuple<IRevision, RevisionStatus>>();

        public IDictionary<Guid, Tuple<IRevision, RevisionStatus>> Revisions
        {
            get { return new Dictionary<Guid, Tuple<IRevision, RevisionStatus>>(_revisionMap); }
            private set { _revisionMap = value.ToDictionary(pair => pair.Key, pair=> pair.Value); }
        }
        public string Id { get; private set; }
        public string Name { get; set; }
        public IRevision CurrentRevision {get; private set; }
        public string Contents {get { return CurrentRevision.GenerateEditedContent(); }}

        public IEnumerable<Subscriber> Subcribers
        {
            get { return _subcribers; }
        }

        public Document()
        {
            CurrentRevision = new InitialRevision();
            _subcribers = new List<Subscriber>();
        }
        public Document(string contents) : this()
        {
            IRevision intialRevision = new InitialRevision(contents);
            CurrentRevision = intialRevision;
            _revisionMap.Add(intialRevision.Id, Tuple.Create(intialRevision, RevisionStatus.Applied));
        }
        public void Edit(IRevision revision)
        {
            var appliedRevision = revision;
            var parentAlreadyHasNewChildRevision = revision.PreviousRevisionAppliedTo.NextRevisionApplied != null;
            var parentMerged = _revisionMap.ContainsKey(revision.PreviousRevisionAppliedTo.Id) &&
                               _revisionMap[revision.PreviousRevisionAppliedTo.Id].Item2 == RevisionStatus.Merged;

            if (parentAlreadyHasNewChildRevision)
            {
                var mergeParent = FindCommonAncestor(revision, CurrentRevision);
              
                appliedRevision = new MergingRevision(mergeParent, new List<IRevision>
                    {                        
                        revision,
                        CurrentRevision
                    });
                var mergeRevision = (MergingRevision) appliedRevision;
                foreach (var mergedRevision in mergeRevision.RevisionsToMerge)
                {
                    var tuple = Tuple.Create(mergedRevision, RevisionStatus.Merged);
                    UpdateRevisionMap(tuple);
                    mergedRevision.NextRevisionApplied = mergeRevision;
                }
            }
            else
                CurrentRevision.NextRevisionApplied = revision;

            UpdateRevisionMap(Tuple.Create(appliedRevision, RevisionStatus.Applied));
            CurrentRevision = appliedRevision;
            NotifySubscribers(appliedRevision);
        }
        public IRevision LoadRevision(Guid id)
        {
            return _revisionMap.ContainsKey(id) ? _revisionMap[id].Item1 : null;
        }
        public void AddSubscriber(Subscriber subscriber)
        {
            _subcribers.Add(subscriber);
        }
       
        private void NotifySubscribers(IRevision revision)
        {
            foreach (var subscriber in Subcribers)
            {
                subscriber.GiveUpdate(revision);
            }
        }
        private void UpdateRevisionMap(Tuple<IRevision, RevisionStatus> status)
        {
            var revision = status.Item1;
            if (_revisionMap.ContainsKey(revision.Id))
                _revisionMap[revision.Id] = status;
            else
                _revisionMap.Add(revision.Id, status);
        }

        private static IRevision FindCommonAncestor(IRevision revision1, IRevision revision2)
        {
            var revision1Parents = new List<IRevision>();
            var revision2Parents = new List<IRevision>();
            var revision1AncestorCurrentPtr = revision1;
            var revision2AncestorCurrentPtr = revision2;

            while (!revision1Parents.Intersect(revision2Parents).Any() && !(
                   revision1AncestorCurrentPtr.PreviousRevisionAppliedTo == null &&
                   revision2AncestorCurrentPtr.PreviousRevisionAppliedTo == null))
            {
                if (revision1AncestorCurrentPtr.PreviousRevisionAppliedTo != null)
                {
                    revision1Parents.Add(revision1AncestorCurrentPtr.PreviousRevisionAppliedTo);
                    revision1AncestorCurrentPtr = revision1AncestorCurrentPtr.PreviousRevisionAppliedTo;
                }

                if (revision2AncestorCurrentPtr.PreviousRevisionAppliedTo != null)
                {
                    revision2Parents.Add(revision2AncestorCurrentPtr.PreviousRevisionAppliedTo);
                    revision2AncestorCurrentPtr = revision2AncestorCurrentPtr.PreviousRevisionAppliedTo;
                }
            }

            return revision1Parents.Intersect(revision2Parents).Any() ? revision1Parents.Intersect(revision2Parents).Single() : null;
        }

        public enum RevisionStatus
        {
            Applied,
            Merged,
        }
    }

    
}