using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

#nullable enable

namespace GGJ2026
{
    public class NoteIndicatorMover : MonoBehaviour
    {
        private readonly List<NoteIndicator> _noteIndicators = new();

        [SerializeField] private Vector3 _moveDirection = Vector3.right;
        [SerializeField] private float _moveSpeed = 5f;
        [SerializeField] private float _missedX = 1920f / 2f;

        public UnityEvent<NoteIndicator> NoteMissed = new();

        private void Update()
        {
            float deltaTime = Time.deltaTime;
            for (int i = 0; i < _noteIndicators!.Count; i++) {
                NoteIndicator noteIndicator = _noteIndicators![i];
                noteIndicator.transform.position += _moveSpeed * deltaTime * _moveDirection;
                if (noteIndicator.transform.position.x - (noteIndicator.transform.localScale.x * 64) > _missedX) {
                    _noteIndicators[i] = _noteIndicators[^1];
                    _noteIndicators.RemoveAt(_noteIndicators.Count - 1);
                    Debug.Log($"Missed note of type {noteIndicator.NoteIndex}");
                    NoteMissed.Invoke(noteIndicator);
                }
            }
        }

        public void StartMoving(NoteIndicator noteIndicator) => _noteIndicators.Add(noteIndicator);
    }
}
