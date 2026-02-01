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
            Enumerable.Range(0, Constants.NoteCount).Select(x => new Queue<NoteIndicator>()).ToArray();
        private readonly Queue<NoteIndicator>[] _noteHeldPools = 
            Enumerable.Range(0, Constants.NoteCount).Select(x => new Queue<NoteIndicator>()).ToArray();

        private readonly int[] _noteCounts = Enumerable.Repeat(0, Constants.NoteCount).ToArray();

        private readonly List<NoteIndicator> _shownNotes = new();
        public IReadOnlyList<NoteIndicator> ShownNotes => _shownNotes;

        [SerializeField, Min(0f)]
        private int _initialNoteCount = 5;

        [SerializeField, RequiredIn(PrefabKind.PrefabInstanceAndNonPrefabInstance)]
        private Transform? _notesParent;

        [SerializeField]
        private string _noteNameFormat = "note-{0}-{1}";

        [SerializeField] 
        private string _noteHeldNameFormat = "note-{0}-{1}-held";

        [SerializeField, ListDrawerSettings(ShowFoldout = false, IsReadOnly = true)]
        private GameObject?[] _notePrefabs = new GameObject[Constants.NoteCount];

        [SerializeField, ListDrawerSettings(ShowFoldout = false, IsReadOnly = true)]
        private GameObject?[] _noteHeldPrefabs = new GameObject[Constants.NoteCount];

        [SerializeField, ListDrawerSettings(ShowFoldout = false, IsReadOnly = true), Min(0f)]
        private Vector2[] _noteStartPositions = Enumerable
            .Range(0, Constants.NoteCount)
            .Select(x => new Vector2(-1920f / 2f, 1080f * ((float)x / Constants.NoteCount - 0.5f)))
            .ToArray();

        public UnityEvent<NoteIndicator> NoteSpawned = new();

        public void SpawnInitialNotes()
        {
            for (int n = 0; n < Constants.NoteCount; n++) {
                Debug.Log($"Spawning {_initialNoteCount} initial notes of type {n}...");
                for (int i = 0; i < _initialNoteCount; i++)
                {
                    _notePools[n].Enqueue(instantiateNote(n));
                    _noteHeldPools[n].Enqueue(instantiateNote(n, true));                    
                }
            }
        }

        public void ShowNoteFromPool(NoteData noteData)
        {
            Assert.IsTrue(noteData.NoteId >= 0);
            
            Debug.Log($"Showing note: {noteData.Bar}:{noteData.Beat} [Id-{noteData.NoteId}] {noteData.Duration}");        
            
            var noteIndicator = instantiateNote(noteData.NoteId);
            // NoteIndicator noteIndicator = _notePools[noteData.NoteId].TryDequeue(out noteIndicator) 
            //     ? noteIndicator : instantiateNote(noteData.NoteId);
            noteIndicator.gameObject.SetActive(true);
            _shownNotes.Add(noteIndicator);
            NoteSpawned.Invoke(noteIndicator);
            
            if (noteData.Duration > 1)
            {
                var heldNoteIndicator = instantiateNote(noteData.NoteId, true);
                // NoteIndicator heldNoteIndicator = _noteHeldPools[noteData.NoteId].TryDequeue(out heldNoteIndicator)
                //     ? heldNoteIndicator : instantiateNote(noteData.NoteId, true);
                heldNoteIndicator.gameObject.SetActive(true);
                heldNoteIndicator.transform.localScale = new Vector3((float)noteData.Duration, 1, 1);
                heldNoteIndicator.transform.position += new Vector3((float)-noteData.Duration * 64, 0, 0);
                _shownNotes.Add(noteIndicator);
                NoteSpawned.Invoke(heldNoteIndicator);
            }
        }

        public void ReturnNoteToPool(NoteIndicator noteIndicator)
        {
            noteIndicator.gameObject.SetActive(false);
            noteIndicator.transform.position = _noteStartPositions[noteIndicator.NoteIndex];

            _shownNotes.Remove(noteIndicator);  // Earlier shown notes are earlier in the list, so searching won't take long but removal is stil O(N)...
            _notePools[noteIndicator.NoteIndex].Enqueue(noteIndicator);
        }

        private NoteIndicator instantiateNote(int noteIndex, bool held = false)
        {
            Debug.Log($"Spawning note of type {noteIndex}...");

            string nameFormat = held ? _noteHeldNameFormat : _noteNameFormat;
            GameObject?[] prefabs = held ? _noteHeldPrefabs : _notePrefabs;

            Vector2 startPositions = _noteStartPositions[noteIndex];
            var position3 = new Vector3(startPositions.x, startPositions.y, held ? 0 : 1);
            
            GameObject noteObject = Instantiate(prefabs[noteIndex]!, position3, Quaternion.identity, _notesParent!);
            NoteIndicator noteIndicator = noteObject.GetComponentInChildren<NoteIndicator>();
            Assert.IsTrue(noteIndicator != null);

            noteObject.name = string.Format(nameFormat, noteIndex, _noteCounts[noteIndex]);
            noteIndicator.State = NoteState.Active;
            noteObject.SetActive(false);

            ++_noteCounts[noteIndex];

            return noteIndicator!;
        }
    }
}
