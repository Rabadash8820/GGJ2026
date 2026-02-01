#nullable enable

using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GGJ2026
{
    public enum CharacterType
    {
        OldLady,
        DoorDasher,
        Businessman,
    }

    [Serializable]
    public class CharacterSpawnInfo
    {
        [field: SerializeField] public float Time { get; set; }
        [field: SerializeField] public CharacterType CharacterType { get; set; }
        [field: SerializeField] public bool HideCurrentlyShownCharacters { get; set; }
    }

    public class CharacterSpawner : MonoBehaviour
    {
        private readonly Dictionary<CharacterType, Character> _shownCharacters = new();
        private float _tElapsed;
        private int _nextSpawnDataIndex = 0;

        [SerializeField, RequiredIn(PrefabKind.PrefabInstanceAndNonPrefabInstance)]
        private Character? _oldLady;
        [SerializeField, RequiredIn(PrefabKind.PrefabInstanceAndNonPrefabInstance)]
        private Character? _doorDasher;
        [SerializeField, RequiredIn(PrefabKind.PrefabInstanceAndNonPrefabInstance)]
        private Character? _businessman;

        [SerializeField] private string _characterEnterAnimStateName = "enter";
        [SerializeField] private string _characterExitAnimStateName = "exit";

        [SerializeField] private CharacterSpawnInfo[] _characterSpawnInfos = Array.Empty<CharacterSpawnInfo>();

        [Header("Test Spawning")]

        [SerializeField] private string _spawnOldLadyInputActionName = "spawn-old-lady";
        [SerializeField] private string _spawnDoorDasherInputActionName = "spawn-door-dasher";
        [SerializeField] private string _spawnBusinessManInputActionName = "spawn-businessman";

        private void Awake()
        {
            InputSystem.actions[_spawnOldLadyInputActionName].performed += ctx => ShowCharacter(CharacterType.OldLady);
            InputSystem.actions[_spawnDoorDasherInputActionName].performed += ctx => ShowCharacter(CharacterType.DoorDasher);
            InputSystem.actions[_spawnBusinessManInputActionName].performed += ctx => ShowCharacter(CharacterType.Businessman);
        }

        private void Update()
        {
            if (_nextSpawnDataIndex == _characterSpawnInfos.Length)
                return;

            _tElapsed += Time.deltaTime;
            CharacterSpawnInfo nextCharacterSpawnInfo = _characterSpawnInfos[_nextSpawnDataIndex];
            if (_tElapsed > _characterSpawnInfos[_nextSpawnDataIndex].Time) {
                if (nextCharacterSpawnInfo.HideCurrentlyShownCharacters) {
                    foreach (CharacterType characterType in Enum.GetValues(typeof(CharacterType)))
                        HideCharacter(characterType);
                }
                ShowCharacter(nextCharacterSpawnInfo.CharacterType);
                ++_nextSpawnDataIndex;
            }
        }

        [Button]
        public void ShowCharacter(CharacterType type)
        {
            if (_shownCharacters.ContainsKey(type))
                return;

            Debug.Log($"Showing character '{type}'...");

            Character character = type switch {
                CharacterType.OldLady => _oldLady!,
                CharacterType.DoorDasher => _doorDasher!,
                CharacterType.Businessman => _businessman!,
                _ => throw new NotImplementedException(),
            };

            character.gameObject.SetActive(true);
            character.Animator!.Play(_characterEnterAnimStateName);
            _shownCharacters[type] = character;
        }

        [Button]
        public void ShowCharacterMistakeEffects()
        {
            foreach (Character character in _shownCharacters.Values) {
                character.AnimatorStateRestarter!.RestartCurrentState();
                character.Animator!.Play(character.MistakeAnimStateName);
            }
        }

        [Button]
        public void HideCharacter(CharacterType type)
        {
            if (!_shownCharacters.ContainsKey(type))
                return;

            Debug.Log($"Hiding character '{type}'...");


            Character character = _shownCharacters[type];
            character.Animator!.Play(_characterExitAnimStateName);
            _shownCharacters.Remove(type);
        }
    }
}
