#nullable enable

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace GGJ2026
{
    public class NotesGenerator : MonoBehaviour
    {
        public UnityEvent<NoteData> SpawnNote = new();
        public UnityEvent StartMusic = new();
        
        [SerializeField]
        public TextAsset _textAsset;
        
        private void Start()
        {
            var script = FileParser.Parse(_textAsset.text);
            
            StartMusic.Invoke();
            foreach (var note in script.Notes)
            {
                var absoluteBeat = note.Bar * Constants.BeatsPerMeasure + note.Beat;
                var time = absoluteBeat / (script.BeatsPerMinute / 60);

                StartCoroutine(spawnNote(note, (float)time));
            }
        }
        
        private IEnumerator spawnNote(NoteData noteData, float delay)
        {
            yield return new WaitForSeconds(delay);

            SpawnNote.Invoke(noteData);
        }
    }
}
