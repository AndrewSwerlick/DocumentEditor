using System;
using System.Collections.Generic;
using DocumentEditor.Core.Models;
using LayoutEditor.Core.Util;

namespace DocumentEditor.Core
{
    public class SubcriberNotification
    {
        public Guid id { get; set; }
        public Guid revisionAppliedTo { get; set; }
        public IList<Patch> patches { get; set; }
        public string content { get; set; }
    }
}
