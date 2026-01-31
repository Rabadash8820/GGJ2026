using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace GGJ2026
{
    public class NotesGenerator : MonoBehaviour
    {
        public UnityEvent<NoteType> SpawnNote { get; set; }

        private NoteScript _noteScript = null;
        private float _scriptStart;
        
        void Start()
        {
            _scriptStart = Time.time;
            _noteScript = new NoteScript();
        }
        
        // Update is called once per frame
        void Update()
        {
            var time = Time.time - _scriptStart;
            if (_noteScript.Notes.Any() == false) { return; }
            while (_noteScript.Notes.First().Key < time)
            {
                SpawnNote.Invoke(_noteScript.Notes.First().Value);
            }
        }
    }
}
