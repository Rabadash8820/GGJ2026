using System;

namespace GGJ2026
{
    [Serializable]
    public class NoteData
    {
        public int Bar { get; set; }
        public decimal Beat { get; set; }
        
        /// <summary>
        /// Duration in beats
        /// </summary>
        public int Duration { get; set; }
        
        public int NoteId { get; set; }
    }
}
