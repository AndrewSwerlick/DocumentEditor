using System;
using System.Collections.Generic;
using LayoutEditor.Core.Util;

namespace DocumentEditor.Commands.DTOs
{
    public class RevisionDTO
    {
        public string DocumentId { get; set; }
        public Guid RevisionId { get; set; }
        public List<Patch> Patches { get; set; }
    }
}