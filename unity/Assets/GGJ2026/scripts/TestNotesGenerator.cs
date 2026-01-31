#nullable enable

using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace GGJ2026
{
    public class TestNotesGenerator : MonoBehaviour
    {
        [SerializeField, ListDrawerSettings(ShowFoldout = false, IsReadOnly = true)]
        private string[] _noteInputActionNames = Enumerable.Range(0, NoteConstants.NoteCount).Select(x => $"test-note{x}").ToArray();

        public UnityEvent<NoteData> SpawnNote = new();

        private void Awake()
        {
            for (int i = 0; i < NoteConstants.NoteCount; i++) {
                InputAction action = InputSystem.actions[_noteInputActionNames[i]];
                int noteIndex = i;  // Don't accidentally capture the iteration var
                action.performed += ctx => generateNote(new NoteData { NoteId = noteIndex });
            }
        }

        private void generateNote(NoteData noteData)
        {
            Debug.Log($"Generating test note of type {noteData.NoteId}...");
            SpawnNote.Invoke(noteData);
        }
    }
}
