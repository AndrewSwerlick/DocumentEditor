using System.Linq;
using LayoutEditor.Core.Util;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DocumentEditor.Core.Models
{
    public class Region
    {
        string _content;
        public string Content
        {
            get
            {
                return _content;
            }
        }
        public int Id
        {
            get;
            set;
        }

        public Region(string content, int Id)
        {
            _content = content;
        }

        public void Edit(JObject editData)
        {
            var reader = new JsonSerializer();
            var jsonPatches = editData["Content"]["Patches"];
            var patches = jsonPatches.Select(p=> reader.Deserialize<Patch>(new JTokenReader(p))).ToList();
            var patcher = new diff_match_patch();
            _content = patcher.patch_apply(patches.ToList(), Content)[0] as string;
        }
    }
}
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                          