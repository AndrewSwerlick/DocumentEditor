using System;
using System.Collections.Generic;
using LayoutEditor.Core.Util;

namespace DocumentEditor.Commands.DTOs
{
    public class DocumentEditRequest
    {
        public string DocumentId { get; set; }
        public Guid RevisionId { get; set; }
        public Guid ParentRevisionId { get; set; }
        public Patch[] Patches { get; set; }
    }
}