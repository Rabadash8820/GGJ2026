using System.Collections.Generic;
using System.Linq;

namespace GGJ2026
{
    /// <summary>
    /// 
    /// </summary>
    public class MusicScript
    {
        public float BeatsPerMinute { get; set; }
        
        // TODO: Enforce ordering here. Unfortunately, no PrioriyQueue available.
        public List<NoteData> Notes { get; set; } = new();
    }
}
