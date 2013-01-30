using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LayoutEditor.Core.Util;

namespace DocumentEditor.Tests.Core.Util
{
    public static class Patches
    {
        public static List<Patch> Make(string original, string revision)
        {
            return new diff_match_patch().patch_make(original, revision);
        }
    
    }
}
