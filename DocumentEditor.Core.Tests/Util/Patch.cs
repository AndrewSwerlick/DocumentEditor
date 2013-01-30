using System.Collections.Generic;
using LayoutEditor.Core.Util;

namespace DocumentEditor.Core.Tests.Util
{
    public static class Patches
    {
        public static List<Patch> Make(string original, string revision)
        {
            return new diff_match_patch().patch_make(original, revision);
        }
    
    }
}
