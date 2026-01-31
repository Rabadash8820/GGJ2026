using System;
using Newtonsoft.Json;

namespace GGJ2026
{
    public class FileParser
    {
        public NoteScript Parse(string str)
        {
            var script = JsonConvert.DeserializeObject<NoteScript>(str);
            if (script is null) { throw new InvalidOperationException(); }
            return script;
        }
    }
}