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
        private string[] _noteInputActionNames = Enumerable.Range(0, Constants.NoteCount).Select(x => $"note{x}").ToArray();

        [SerializeField, RequiredIn(PrefabKind.PrefabInstanceAndNonPrefabInstance)]
        private NotesSpawner? _noteSpawner;

        [SerializeField, RequiredIn(PrefabKind.PrefabInstanceAndNonPrefabInstance)]
        private Transform? _hitBoxTransform;

        [SerializeField] private UnityEvent<int> _hitAttempted = new();
        [SerializeField] private UnityEvent<NoteIndicator> _noteHit = new();
        [SerializeField] private UnityEvent<int> _noteWrong = new();
        [SerializeField] private UnityEvent<NoteIndicator> _noteReleasedEarly = new();

        public int NotesHitCount { get; private set; }

        private void Awake()
        {
            for (int i = 0; i < Constants.NoteCount; i++) {
                InputAction action = InputSystem.actions[_noteInputActionNames[i]];
                int noteIndex = i;  // Don't accidentally capture the iteration var
                action.performed += ctx => tryHitNote(noteIndex);
                action.canceled += ctx => releaseHoldNote(noteIndex);
            }
        }

        private void tryHitNote(int noteIndex)
        {
            Debug.Log($"Hitting note of type {noteIndex}...");
            _hitAttempted.Invoke(noteIndex);

            foreach (NoteIndicator noteIndicator in _noteSpawner!.ShownNotes) 
            {
                if (noteIndicator.NoteIndex == noteIndex
                    && noteIndicator.State == NoteState.Active
                    && inHitRange(noteIndicator)
                )
                {
                    if (noteIndicator.RequiresHold)
                    {
                        noteIndicator.State = NoteState.Held;
                        return;
                    }

                    Debug.Log($"Successfully hit note of type {noteIndex}");
                    ++NotesHitCount;
                    _noteHit.Invoke(noteIndicator);
                    return;
                }
            }

            Debug.Log($"Hit wrong note of type {noteIndex}");
            _noteWrong.Invoke(noteIndex);
        }

        private bool inHitRange(NoteIndicator noteIndicator) =>
            noteIndicator.transform.position.x + noteIndicator.Width >= _hitBoxTransform!.position.x - _hitBoxTransform.localScale.x / 2f;

        private void releaseHoldNote(int noteIndex)
        {
            foreach (NoteIndicator noteIndicator in _noteSpawner!.ShownNotes) 
            {
                if (noteIndicator.NoteIndex == noteIndex
                    && noteIndicator.State == NoteState.Held) 
                {
                    if (inReleaseRange(noteIndicator))
                    {
                        Debug.Log($"Successfully held note of type {noteIndex}");
                        _noteHit.Invoke(noteIndicator);
                        return;   
                    }
                    
                    noteIndicator.State = NoteState.Missed;
                    _noteReleasedEarly.Invoke(noteIndicator);
                    return;
                }
            }
        }

        private bool inReleaseRange(NoteIndicator note) =>
            note.transform.position.x - note.Width / 2 >= _hitBoxTransform!.position.x - _hitBoxTransform.localScale.x / 2f;
    }
}
