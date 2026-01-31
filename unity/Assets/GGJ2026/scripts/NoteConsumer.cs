using Sirenix.OdinInspector;
using UnityEngine;
using UnityUtil.Updating;

#nullable enable

namespace GGJ2026
{
    public class NoteIndicatorMover : Updatable
    {
        private Transform[]? _noteTransforms;

        [RequiredIn(PrefabKind.PrefabInstanceAndNonPrefabInstance)]
        public Transform NotesParent;

        public Vector3 MoveDirection = Vector3.left;

        public float MoveSpeed = 5f;

        protected override void Awake()
        {
            base.Awake();

            _noteTransforms = NotesParent.GetComponentsInChildren<Transform>();

            AddUpdate(updateNotes);
        }

        private void updateNotes(float deltaTime)
        {
            foreach (Transform noteTransform in _noteTransforms!)
                noteTransform.position += MoveSpeed * deltaTime * MoveDirection;
        }
    }
}
