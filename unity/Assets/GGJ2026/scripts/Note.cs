using UnityEngine;
using UnityEngine.Events;

namespace GGJ2026
{
    public class Note : MonoBehaviour
    {
        public int NoteType { get; set; }
        public UnityEvent<int> SpawnNote { get; set; }
        public UnityEvent NoteFailed { get; set; } = new();

        
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            // todo: wire up to input to listen for directional input
        }

        // Update is called once per frame
        void Update()
        {
            if (false) //TODO: Determine criteria for note failing
            {
                NoteFailed.Invoke();
            }  
        }
    }
}
