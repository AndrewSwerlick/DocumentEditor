using System;

namespace DocumentEditor.Core.Models
{
    public class Subscriber
    {
        public string Id { get; private set; }

        public Subscriber()
        {
            Id = Guid.NewGuid().ToString();
        }

        public Subscriber(string id)
        {
            Id = id;
        }

        public void GiveUpdate(IRevision revision)
        {
            var notification = new SubcriberNotification
                {
                    patches = revision.BuildPatch(),
                    id = revision.Id,
                    revisionAppliedTo = revision.PreviousRevisionAppliedTo.Id,
                    content = revision.GenerateEditedContent()
                };
            if (SubscriberNotifiedOfUpdate != null)
                SubscriberNotifiedOfUpdate(this, notification);
        }

        public event EventHandler<SubcriberNotification> SubscriberNotifiedOfUpdate;
    }
}