using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace GGJ2026
{
    public class NotesGenerator : MonoBehaviour
    {
        public UnityEvent<NoteType> SpawnNote { get; private set; } = new();

        private NoteScript? _noteScript;
        private float _scriptStart;
        
        private void Start()
        {
            _scriptStart = Time.time;
            
            //initialize with test values
            var notes = new List<KeyValuePair<float, NoteType>>()
            {
                new(1, NoteType.Note1),
                new(2, NoteType.Note2),
                new(3, NoteType.Note3),
                new(4, NoteType.Note4)
            };
            
            _noteScript = new NoteScript { Notes = notes };
        }
        
        // Update is called once per frame
        private void Update()
        {
            float time = Time.time - _scriptStart;
            if (_noteScript is null) { return; }
            if (_noteScript.Notes.Any() == false) { return; }
            while (_noteScript.Notes.First().Key <= time)
            {
                SpawnNote.Invoke(_noteScript.Notes.First().Value);
                _noteScript.Notes.RemoveAt(0);
            }
        }
    }
}
