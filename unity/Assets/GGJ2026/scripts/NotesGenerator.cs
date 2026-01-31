#nullable enable

using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace GGJ2026
{
    //TODO: Behavior commented out for now. Is this even needed?
    public class NotesGenerator : MonoBehaviour
    {
        public UnityEvent<int> SpawnNote { get; private set; } = new();
        
        private MusicScript? _noteScript;
        private float _scriptStart;
        
        private void Start()
        {
            // _scriptStart = Time.time;
            //
            // //initialize with test values
            // var notes = new List<NoteData>();
            //
            // _noteScript = new MusicScript { Notes = notes };
        }
        
        // Update is called once per frame
        private void Update()
        {
            // float time = Time.time - _scriptStart;
            // if (_noteScript is null) { return; }
            // if (_noteScript.Notes.Any() == false) { return; }
            // while (_noteScript.Notes.First().Bar <= time)
            // {
            //     SpawnNote.Invoke(_noteScript.Notes.First().NoteId);
            //     _noteScript.Notes.RemoveAt(0);
            // }
        }
    }
}
