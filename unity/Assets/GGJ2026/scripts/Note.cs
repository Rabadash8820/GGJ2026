using UnityEngine;
using UnityEngine.Events;

namespace GGJ2026
{
    public class Note : MonoBehaviour
    {
        public UnityEvent<NoteType> SpawnNote { get; set; }
        
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start() { }

        // Update is called once per frame
        void Update()
        {
            SpawnNote.Invoke(NoteType.Note1);
        }
    }
}
