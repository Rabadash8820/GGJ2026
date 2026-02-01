using System;

namespace GGJ2026
{
    [Serializable]
    public class NoteData
    {
        public int Bar { get; set; }
        public float Beat { get; set; }
        
        /// <summary>
        /// Duration in beats
        /// </summary>
        public float Duration { get; set; }
        
        public int NoteId { get; set; }
    }
}
