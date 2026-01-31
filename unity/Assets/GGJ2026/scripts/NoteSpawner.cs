#nullable enable

using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GGJ2026
{
    public class NotesSpawner : MonoBehaviour
    {
        [SerializeField, Min(0f)]
        private int _initialNoteCount = 5;

        [SerializeField, RequiredIn(PrefabKind.PrefabInstanceAndNonPrefabInstance)]
        private Transform _notesParent;

        [SerializeField]
        private string _noteNameFormat = "note-{0}-{1}";

        [SerializeField, ListDrawerSettings(ShowFoldout = false, IsReadOnly = true)]
        private GameObject[] _notePrefabs = new GameObject[NoteConstants.NoteCount];

        [SerializeField, ListDrawerSettings(ShowFoldout = false, IsReadOnly = true), Min(0f)]
        private Vector2[] _noteStartPositions =
            Enumerable.Range(0, NoteConstants.NoteCount).Select(x => new Vector2(1920f, 1080f / NoteConstants.NoteCount * x)).ToArray();

        public void SpawnInitialNotes()
        {
            for (int n = 0; n < NoteConstants.NoteCount; n++) {
                Debug.Log($"Spawning {_initialNoteCount} initial notes of type {n}...");

                for (int i = 0; i < _initialNoteCount; i++) {
                    GameObject noteObject = Instantiate(_notePrefabs[n], _noteStartPositions[n], Quaternion.identity, _notesParent);
                    noteObject.name = string.Format(_noteNameFormat, n, i);
                    noteObject.SetActive(false);
                }
            }
        }
    }
}
