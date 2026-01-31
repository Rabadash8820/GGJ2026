#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.TextCore.Text;

namespace GGJ2026
{
    public class CharacterSpawner : MonoBehaviour
    {
        [SerializeField, ListDrawerSettings(ShowFoldout = false, IsReadOnly = true)]
        private readonly Character?[] _characters = Enumerable.Repeat((Character?)null, Constants.CharacterCount).ToArray();

        [SerializeField, RequiredIn(PrefabKind.PrefabInstanceAndNonPrefabInstance)]
        private Transform? _charactersParent;

        [SerializeField] private Vector2 _characterStartPosition = new(-1000f, -200f);

        [SerializeField] private string _characterExitAnimStateName = "exit";
        [SerializeField] private string _characterMistakeAnimStateName = "mistake";

        public void ShowCharacter(int index)
        {
            Character character = _characters[index]!;
            Debug.Log($"Showing character named '{character.name}'...");

            character.transform.position = _characterStartPosition;
            character.gameObject.SetActive(true);
        }

        public void ShowCharacterWrongEffects()
        {
            foreach (Character? character in _characters) {
                if (character == null || !character.gameObject.activeSelf)
                    continue;

                character!.AnimatorStateRestarter.RestartCurrentState();
                character.Animator.Play(_characterMistakeAnimStateName);
            }
        }

        public void HideCharacter(int index)
        {
            Character character = _characters[index]!;
            character.Animator.Play(_characterExitAnimStateName);
        }
    }
}
