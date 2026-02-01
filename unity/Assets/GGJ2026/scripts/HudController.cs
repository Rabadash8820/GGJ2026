#nullable enable

using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UIElements;

namespace GGJ2026
{
    public class HudController : MonoBehaviour
    {
        private Label? _lblAccuracy;

        [SerializeField, RequiredIn(PrefabKind.PrefabInstanceAndNonPrefabInstance)]
        private UIDocument? _uiDocument;

        [SerializeField, RequiredIn(PrefabKind.PrefabInstanceAndNonPrefabInstance)]
        private NoteHitter? _noteHitter;

        [SerializeField, RequiredIn(PrefabKind.PrefabInstanceAndNonPrefabInstance)]
        private NotesGenerator? _notesGenerator;

        [SerializeField] private string _lblAccuracyName = "lbl-accuracy";
        [SerializeField] private string _lblAccuracyFormatString = "{0} / {1}";

        private void Awake() => _lblAccuracy = _uiDocument!.rootVisualElement.Query<Label>(_lblAccuracyName).First();

        public void UpdateAccuracyLabel() => _lblAccuracy!.text = string.Format(_lblAccuracyFormatString, _noteHitter!.NotesHitCount, _notesGenerator!.GeneratedNoteCount);
    }
}
