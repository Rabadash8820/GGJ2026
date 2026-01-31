#nullable enable

using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UIElements;

namespace GGJ2026
{
    public class HudController : MonoBehaviour
    {
        private Label? _lblAccuracy;
        private int _hitNoteCount;

        [SerializeField, RequiredIn(PrefabKind.PrefabInstanceAndNonPrefabInstance)]
        private UIDocument? _uiDocument;

        [field: SerializeField] public int TotalNoteCount { get; private set; }

        [SerializeField] private string _lblAccuracyName = "lbl-accuracy";
        [SerializeField] private string _lblAccuracyFormatString = "{0} / {1}";

        private void Awake()
        {
            _lblAccuracy = _uiDocument!.rootVisualElement.Query<Label>(_lblAccuracyName).First();
        }

        public void HitNote() => _lblAccuracy!.text = string.Format(_lblAccuracyFormatString, ++_hitNoteCount, TotalNoteCount);
    }
}
