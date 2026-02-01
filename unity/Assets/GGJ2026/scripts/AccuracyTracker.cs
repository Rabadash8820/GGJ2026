#nullable enable

using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace GGJ2026
{
    public class AccuracyTracker : MonoBehaviour
    {
        [SerializeField, RequiredIn(PrefabKind.PrefabInstanceAndNonPrefabInstance)]
        private NoteHitter? _noteHitter;

        [SerializeField, RequiredIn(PrefabKind.PrefabInstanceAndNonPrefabInstance)]
        private NotesGenerator? _notesGenerator;

        [SerializeField, RequiredIn(PrefabKind.PrefabInstanceAndNonPrefabInstance)]
        private Image? _faceSpriteImage;

        [SerializeField] private Sprite[] _faceSprites = Array.Empty<Sprite>();

        [SerializeField] private float _minAccuracy = 0.15f;

        [SerializeField] private int _minNotesBeforeTooLow = 10;

        [SerializeField] private UnityEvent _accuracyTooLow = new();

        public void UpdateAccuracy()
        {
            float accuracy = (float)_noteHitter!.NotesHitCount / _notesGenerator!.GeneratedNoteCount;

            int faceSpriteIndex = (int)(accuracy * _faceSprites.Length);
            _faceSpriteImage!.sprite = _faceSprites[faceSpriteIndex];

            if (accuracy < _minAccuracy && _notesGenerator!.GeneratedNoteCount >= _minNotesBeforeTooLow) {
                Debug.Log($"Accuracy ({accuracy:P}) fell below the min ({_minAccuracy:P})");
                _accuracyTooLow.Invoke();
            }
        }
    }
}
