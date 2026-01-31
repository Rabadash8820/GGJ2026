using Sirenix.OdinInspector;
using UnityEngine;

#nullable enable

namespace GGJ2026
{
    public class NoteIndicatorMover : MonoBehaviour
    {
        private Transform[]? _noteTransforms;

        [RequiredIn(PrefabKind.PrefabInstanceAndNonPrefabInstance)]
        public Transform NotesParent;

        public Vector3 MoveDirection = Vector3.left;

        public float MoveSpeed = 5f;

        private void Awake() => _noteTransforms = NotesParent.GetComponentsInChildren<Transform>();

        private void Update()
        {
            float deltaTime = Time.deltaTime;
            foreach (Transform noteTransform in _noteTransforms!)
                noteTransform.position += MoveSpeed * deltaTime * MoveDirection;
        }
    }
}
