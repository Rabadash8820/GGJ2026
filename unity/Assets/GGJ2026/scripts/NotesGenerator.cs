#nullable enable

using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace GGJ2026
{
    public class NotesGenerator : MonoBehaviour
    {
        public UnityEvent<int> SpawnNote { get; private set; } = new();

        private NoteScript? _noteScript;
        private float _scriptStart;
        
        private void Start()
        {
            _scriptStart = Time.time;
            
            //initialize with test values
            var notes = new List<KeyValuePair<float, int>>()
            {
                new(1, 0),
                new(2, 1),
                new(3, 2),
                new(4, 3)
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
