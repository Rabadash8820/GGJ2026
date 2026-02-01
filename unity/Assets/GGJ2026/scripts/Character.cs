#nullable enable

using UnityEngine;
using UnityUtil;

namespace GGJ2026
{
    public class Character : MonoBehaviour
    {
        [field: SerializeField] public string MistakeAnimStateName { get; private set; } = "mistake";
        [field: SerializeField] public Animator? Animator { get; private set; }
        [field: SerializeField] public AnimatorStateRestarter? AnimatorStateRestarter { get; private set; }
    }
}
