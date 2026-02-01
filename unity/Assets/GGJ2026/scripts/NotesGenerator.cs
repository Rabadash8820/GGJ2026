#nullable enable

using System.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace GGJ2026
{
    public class NotesGenerator : MonoBehaviour
    {
        private float _nextNoteTime;
        private float _beatsPerSec;
        private MusicScript? _musicScript;

        [field: Tooltip("Duration, in seconds, for which a note is visible until the player must hit it")]
        [field: SerializeField] public float NoteVisibilityDuration { get; private set; } = 0.5f;

        [Tooltip("Offset, in seconds, to sync up notes with the start of the actual music audio")]
        [SerializeField] private float _noteStartOffset = 3f;

        [SerializeField] private float _afterMusicDelaySeconds = 5f;

        [SerializeField] private UnityEvent<NoteData> _spawnNote = new();
        [SerializeField] private UnityEvent _endMusic = new();

        [SerializeField, RequiredIn(PrefabKind.PrefabInstanceAndNonPrefabInstance)]
        private TextAsset? _textAsset;

        private float _startTime;

        [SerializeField] private AudioSource _audioSource;

        public int GeneratedNoteCount { get; private set; }

        private void Awake()
        {
            _musicScript = FileParser.Parse(_textAsset!.text);
            _beatsPerSec = _musicScript.BeatsPerMinute / 60f;
            _nextNoteTime = getGenerateNoteTime(_musicScript.Notes[0]);
            Debug.Log(_nextNoteTime);
            Debug.Log(_noteStartOffset);
            Debug.Log(NoteVisibilityDuration);
            _startTime = Time.time;
            Debug.Log(_startTime);
            _audioSource!.Play();
        }

        private void Update()
        {
            var time = Time.time - _startTime;
            while (time >= _nextNoteTime) {
                Debug.Log(Time.time);
                _spawnNote.Invoke(_musicScript!.Notes[GeneratedNoteCount++]);

                if (GeneratedNoteCount < _musicScript.Notes.Count) {
                    _nextNoteTime = getGenerateNoteTime(_musicScript.Notes[GeneratedNoteCount]);
                }
                else {
                    Debug.Log($"End of music reached. Now delaying for {_afterMusicDelaySeconds} seconds...");
                    _nextNoteTime = float.PositiveInfinity;
                    _ = waitAfterMusicAsync();  // Fire and forget
                }
            }

            async Awaitable waitAfterMusicAsync()
            {
                await Awaitable.WaitForSecondsAsync(_afterMusicDelaySeconds, destroyCancellationToken);
                Debug.Log($"{_afterMusicDelaySeconds} second delay complete. Ending...");
                _endMusic.Invoke();
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
            var beat = noteData.Bar * Constants.BeatsPerMeasure + noteData.Beat - 4;
            return (beat - 1) / _beatsPerSec - _noteStartOffset - NoteVisibilityDuration;
        }
    }
}
