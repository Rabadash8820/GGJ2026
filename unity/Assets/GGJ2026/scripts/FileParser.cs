using System;
using Newtonsoft.Json;

namespace GGJ2026
{
    public static class FileParser
    {
        public static MusicScript Parse(string text)
        {
            var script = JsonConvert.DeserializeObject<MusicScript>(text);
            if (script is null) { throw new InvalidOperationException(); }
            return script;
        }
    }
}