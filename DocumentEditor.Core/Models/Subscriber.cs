using System;

namespace DocumentEditor.Core.Models
{
    public class Subscriber
    {

        public void GiveUpdate(IRevision revision)
        {
            var notification = new SubcriberNotification
                {
                    Revision = revision
                };
            if (SubscriberNotifiedOfUpdate != null)
                SubscriberNotifiedOfUpdate(this, notification);
        }

        public event EventHandler<SubcriberNotification> SubscriberNotifiedOfUpdate;
    }
}