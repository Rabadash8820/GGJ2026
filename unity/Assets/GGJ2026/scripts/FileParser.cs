using System;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

namespace GGJ2026
{
    public static class FileParser
    {
        public static MusicScript Parse(string assetPath)
        {
            var json = AssetDatabase.LoadAssetAtPath<TextAsset>(assetPath);
            var script = JsonConvert.DeserializeObject<MusicScript>(json.text);
            if (script is null) { throw new InvalidOperationException(); }
            return script;
        }
    }
}