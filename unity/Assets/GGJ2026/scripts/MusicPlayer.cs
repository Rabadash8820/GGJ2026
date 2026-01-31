using UnityEngine;

namespace GGJ2026
{
    public class MusicPlayer : MonoBehaviour
    {
        private AudioSource _source;
        
        void Awake()
        {
            _source = GetComponent<AudioSource>();
        }

        public void PlayMusic()
        {
            _source.Play();
        }
    }
}