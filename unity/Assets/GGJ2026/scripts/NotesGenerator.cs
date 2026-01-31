#nullable enable

using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace GGJ2026
{
    public class NotesGenerator : MonoBehaviour
    {
        public UnityEvent<int, float> SpawnNote = new();
        
        private void Start()
        {
            var asset = "Assets/GGJ2026/test-song-data.json";
            var script = FileParser.Parse(asset);

            foreach (var note in script.Notes)
            {
                var absoluteBeat = note.Bar * NoteConstants.BeatsPerMeasure + note.Beat;
                var time = absoluteBeat / (script.BeatsPerMinute / 60);

                StartCoroutine(spawnNote(note.NoteId, note.Duration, (float)time));
            }
        }
        
        private IEnumerator spawnNote(int id, float duration, float delay)
        {
            yield return new WaitForSeconds(delay);

            SpawnNote.Invoke(id, duration);
        }
    }
}
