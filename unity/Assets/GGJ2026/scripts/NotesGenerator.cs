using UnityEngine;
using UnityEngine.Events;

namespace GGJ2026
{
    public class NotesGenerator : MonoBehaviour
    {
        public UnityEvent<NoteType> SpawnNote { get; set; }
        
        // Update is called once per frame
        void Update()
        {
            SpawnNote.Invoke(NoteType.Note1);
        }
    }
}
