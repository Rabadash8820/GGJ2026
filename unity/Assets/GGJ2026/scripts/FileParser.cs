using System;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

namespace GGJ2026
{
    public static class FileParser
    {
        public static MusicScript Parse(string str)
        {
            //TODO: Hard-coded asset for now.
            var json = AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/GGJ2026/test-song-data.json");
            var script = JsonConvert.DeserializeObject<MusicScript>(json.text);
            if (script is null) { throw new InvalidOperationException(); }
            return script;
        }
    }
}