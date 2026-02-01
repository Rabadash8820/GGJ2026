#nullable enable

using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace GGJ2026
{
    public class NotesGenerator : MonoBehaviour
    {
        private int _nextNoteIndex;
        private float _nextNoteTime;
        private float _beatsPerSec;
        private MusicScript? _musicScript;

        [field: Tooltip("Duration, in seconds, for which a note is visible until the player must hit it")]
        [field: SerializeField] public float NoteVisibilityDuration { get; private set; } = 0.5f;

        [Tooltip("Offset, in seconds, to sync up notes with the start of the actual music audio")]
        [SerializeField] private float _noteStartOffset = 3f;

        [SerializeField] private UnityEvent<NoteData> _spawnNote = new();
        [SerializeField] private UnityEvent _endMusic = new();

        [SerializeField, RequiredIn(PrefabKind.PrefabInstanceAndNonPrefabInstance)]
        private TextAsset? _textAsset;

        private void Awake()
        {
            _musicScript = FileParser.Parse(_textAsset!.text);
            _beatsPerSec = _musicScript.BeatsPerMinute / 60f;
            _nextNoteTime = getGenerateNoteTime(_musicScript.Notes[0]);
        }

        private void Update()
        {
            float time = Time.timeSinceLevelLoad;
            while (time >= _nextNoteTime) {
                _spawnNote.Invoke(_musicScript!.Notes[_nextNoteIndex]);

                ++_nextNoteIndex;
                if (_nextNoteIndex < _musicScript.Notes.Count) {
                    _nextNoteTime = getGenerateNoteTime(_musicScript.Notes[_nextNoteIndex]);
                }
                else {
                    Debug.Log("End of music reached");
                    _nextNoteTime = float.PositiveInfinity;
                    _endMusic.Invoke();
                }
            }
        }

        /// <summary>
        /// Gets the absolute time when <paramref name="noteData"/> should be generated
        /// (given by its beat and the delay necessary for it to travel to the player's hitbox).
        /// </summary>
        /// <param name="noteData"></param>
        /// <returns></returns>
        private float getGenerateNoteTime(NoteData noteData)
        {
            float beat = noteData.Bar * Constants.BeatsPerMeasure + noteData.Beat;
            return beat / _beatsPerSec - _noteStartOffset - NoteVisibilityDuration;
        }
    }
}
