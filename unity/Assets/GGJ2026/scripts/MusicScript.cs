using System;
using System.Collections.Generic;

namespace GGJ2026
{
    [Serializable]
    public class MusicScript
    {
        public int BeatsPerMinute { get; set; }
        public List<NoteData> Notes { get; set; } = new();
    }
}
