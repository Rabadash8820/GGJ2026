using UnityEngine;

namespace GGJ2026
{
    public class NoteIndicator : MonoBehaviour
    {
        [field: SerializeField] public int NoteIndex { get; private set; }
        [field: SerializeField] public float Width { get; private set; }
        
        public bool RequiresHold { get; set; }
        public NoteIndicator? HeldNoteIndicator { get; set; }
        
        public NoteState State { get; set; } = NoteState.Inactive;
    }
}
