#nullable enable

using Sirenix.OdinInspector;
using UnityEngine;

namespace GGJ2026
{
    public class MusicPlayer : MonoBehaviour
    {
        [SerializeField, RequiredIn(PrefabKind.PrefabInstanceAndNonPrefabInstance)]
        private AudioSource? _audioSource;

        public void PlayMusic() => _audioSource!.Play();
    }
}
