using System.Collections.Generic;
using System.Linq;

namespace GGJ2026
{
    /// <summary>
    /// 
    /// </summary>
    public class NoteScript
    {
        public IOrderedEnumerable<KeyValuePair<int, NoteType>> Notes { get; set; }
    }
}