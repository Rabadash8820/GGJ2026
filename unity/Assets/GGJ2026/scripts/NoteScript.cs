using System.Collections.Generic;
using System.Linq;

namespace GGJ2026
{
    /// <summary>
    /// 
    /// </summary>
    public class NoteScript
    {
        // TODO: Enforce ordering here. Unfortunately, no PrioriyQueue available.
        public List<KeyValuePair<float, NoteType>> Notes { get; set; }
    }
}