#nullable enable

using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;

namespace GGJ2026
{
    public class NotesSpawner : MonoBehaviour
    {
        private readonly Queue<Transform>[] _notePools = Enumerable.Range(0, NoteConstants.NoteCount).Select(x => new Queue<Transform>()).ToArray();
        private readonly int[] _noteCounts = Enumerable.Repeat(0, NoteConstants.NoteCount).ToArray();

        [SerializeField, Min(0f)]
        private int _initialNoteCount = 5;

        [SerializeField, RequiredIn(PrefabKind.PrefabInstanceAndNonPrefabInstance)]
        private Transform _notesParent;

        [SerializeField]
        private string _noteNameFormat = "note-{0}-{1}";

        [SerializeField, ListDrawerSettings(ShowFoldout = false, IsReadOnly = true)]
        private GameObject?[] _notePrefabs = new GameObject[NoteConstants.NoteCount];

        [SerializeField, ListDrawerSettings(ShowFoldout = false, IsReadOnly = true), Min(0f)]
        private Vector2[] _noteStartPositions =
            Enumerable.Range(0, NoteConstants.NoteCount).Select(x => new Vector2(1920f, 1080f / NoteConstants.NoteCount * x)).ToArray();

        public void SpawnInitialNotes()
        {
            for (int n = 0; n < NoteConstants.NoteCount; n++) {
                Debug.Log($"Spawning {_initialNoteCount} initial notes of type {n}...");
                for (int i = 0; i < _initialNoteCount; i++)
                    _notePools[n].Enqueue(instantiateNote(n));
            }
        }

        public void SpawnNoteFromPool(int noteIndex)
        {
            Assert.IsTrue(noteIndex >= 0);

            Transform noteTrans = _notePools[noteIndex].TryDequeue(out noteTrans) ? noteTrans : instantiateNote(noteIndex);
            noteTrans.gameObject.SetActive(true);
        }

        private Transform instantiateNote(int noteIndex)
        {
            Debug.Log($"Spawning note of type {noteIndex}...");

            GameObject noteObject = Instantiate(_notePrefabs[noteIndex]!, _noteStartPositions[noteIndex], Quaternion.identity, _notesParent);
            noteObject.name = string.Format(_noteNameFormat, noteIndex, _noteCounts[noteIndex]);
            noteObject.SetActive(false);

            ++_noteCounts[noteIndex];

            return noteObject.transform;
        }
    }
}
