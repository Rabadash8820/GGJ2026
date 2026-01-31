#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;

namespace GGJ2026
{
    public class NotesSpawner : MonoBehaviour
    {
        private readonly Queue<NoteIndicator>[] _notePools =
            Enumerable.Range(0, NoteConstants.NoteCount).Select(x => new Queue<NoteIndicator>()).ToArray();
        private readonly int[] _noteCounts = Enumerable.Repeat(0, NoteConstants.NoteCount).ToArray();

        private readonly List<NoteIndicator> _shownNotes = new();
        public IReadOnlyList<NoteIndicator> ShownNotes => _shownNotes;

        [SerializeField, Min(0f)]
        private int _initialNoteCount = 5;

        [SerializeField, RequiredIn(PrefabKind.PrefabInstanceAndNonPrefabInstance)]
        private Transform? _notesParent;

        [SerializeField]
        private string _noteNameFormat = "note-{0}-{1}";

        [SerializeField, ListDrawerSettings(ShowFoldout = false, IsReadOnly = true)]
        private GameObject?[] _notePrefabs = new GameObject[NoteConstants.NoteCount];

        [SerializeField, ListDrawerSettings(ShowFoldout = false, IsReadOnly = true), Min(0f)]
        private Vector2[] _noteStartPositions = Enumerable
            .Range(0, NoteConstants.NoteCount)
            .Select(x => new Vector2(-1920f / 2f, 1080f * ((float)x / NoteConstants.NoteCount - 0.5f)))
            .ToArray();

        public UnityEvent<NoteIndicator> NoteSpawned = new();

        public void SpawnInitialNotes()
        {
            for (int n = 0; n < NoteConstants.NoteCount; n++) {
                Debug.Log($"Spawning {_initialNoteCount} initial notes of type {n}...");
                for (int i = 0; i < _initialNoteCount; i++)
                    _notePools[n].Enqueue(instantiateNote(n));
            }
        }

        public void ShowNoteFromPool(NoteData noteData)
        {
            Assert.IsTrue(noteData.NoteId >= 0);

            NoteIndicator noteIndicator = _notePools[noteData.NoteId].TryDequeue(out noteIndicator) 
                ? noteIndicator : instantiateNote(noteData.NoteId);
            noteIndicator.gameObject.SetActive(true);

            _shownNotes.Add(noteIndicator);

            NoteSpawned.Invoke(noteIndicator!);
        }

        public void ReturnNoteToPool(NoteIndicator noteIndicator)
        {
            noteIndicator.gameObject.SetActive(false);
            noteIndicator.transform.position = _noteStartPositions[noteIndicator.NoteIndex];

            _shownNotes.Remove(noteIndicator);  // Earlier shown notes are earlier in the list, so searching won't take long but removal is stil O(N)...
            _notePools[noteIndicator.NoteIndex].Enqueue(noteIndicator);
        }

        private NoteIndicator instantiateNote(int noteIndex)
        {
            Debug.Log($"Spawning note of type {noteIndex}...");

            GameObject noteObject = Instantiate(_notePrefabs[noteIndex]!, _noteStartPositions[noteIndex], Quaternion.identity, _notesParent!);
            NoteIndicator noteIndicator = noteObject.GetComponentInChildren<NoteIndicator>();
            Assert.IsTrue(noteIndicator != null);

            noteObject.name = string.Format(_noteNameFormat, noteIndex, _noteCounts[noteIndex]);
            noteObject.SetActive(false);

            ++_noteCounts[noteIndex];

            return noteIndicator!;
        }
    }
}
