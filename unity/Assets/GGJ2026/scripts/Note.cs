using UnityEngine;
using UnityEngine.Events;

namespace GGJ2026
{
    public class Note : MonoBehaviour
    {
        public UnityEvent<int> SpawnNote { get; set; }
    }
}
