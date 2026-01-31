#nullable enable

using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace GGJ2026
{
    public class NoteHitter : MonoBehaviour
    {
        [SerializeField, ListDrawerSettings(ShowFoldout = false, IsReadOnly = true)]
        private string[] _noteInputActionNames = Enumerable.Range(0, NoteConstants.NoteCount).Select(x => $"note{x}").ToArray();

        [SerializeField, RequiredIn(PrefabKind.PrefabInstanceAndNonPrefabInstance)]
        private NotesSpawner? _noteSpawner;

        [SerializeField, RequiredIn(PrefabKind.PrefabInstanceAndNonPrefabInstance)]
        private SpriteRenderer? _hitBoxSpriteRenderer;

        [SerializeField] private UnityEvent<int> _hitAttempted = new();
        [SerializeField] private UnityEvent<NoteIndicator> _noteHit = new();
        [SerializeField] private UnityEvent<int> _noteWrong = new();

        private void Awake()
        {
            for (int i = 0; i < NoteConstants.NoteCount; i++) {
                InputAction action = InputSystem.actions[_noteInputActionNames[i]];
                int noteIndex = i;  // Don't accidentally capture the iteration var
                action.performed += ctx => tryHitNote(noteIndex);
            }
        }

        private void tryHitNote(int noteIndex)
        {
            Debug.Log($"Hitting note of type {noteIndex}...");
            _hitAttempted.Invoke(noteIndex);

            foreach (NoteIndicator noteIndicator in _noteSpawner!.ShownNotes) {
                if (noteIndicator.NoteIndex == noteIndex
                    && noteIndicator.transform.position.x + noteIndicator.Width >= _hitBoxSpriteRenderer!.transform.position.x - _hitBoxSpriteRenderer.transform.localScale.x / 2f
                ) {
                    Debug.Log($"Successfully hit note of type {noteIndex}");
                    _noteHit.Invoke(noteIndicator);
                    return;
                }
            }

            Debug.Log($"Hit wrong note of type {noteIndex}");
            _noteWrong.Invoke(noteIndex);
        }
    }
}
