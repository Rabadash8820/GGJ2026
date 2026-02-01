#nullable enable

using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace GGJ2026
{
    public class GameOverMenuController : MonoBehaviour
    {
        private VisualElement? _grpGameOver;
        private Label? _lblNotesHit;
        private Label? _lblNotesShown;
        private Label? _lblAccuracy;

        private string _lblNotesHitFormatString = "";
        private string _lblNotesShownFormatString = "";
        private string _lblAccuracyFormatString = "";

        [SerializeField, RequiredIn(PrefabKind.PrefabInstanceAndNonPrefabInstance)]
        private UIDocument? _uiDocument;

        [SerializeField, RequiredIn(PrefabKind.PrefabInstanceAndNonPrefabInstance)]
        private NoteHitter? _noteHitter;

        [SerializeField, RequiredIn(PrefabKind.PrefabInstanceAndNonPrefabInstance)]
        private NotesGenerator? _notesGenerator;

        [Header("Element Names")]

        [SerializeField] private string _grpGameOverName = "grp-game-over";
        [SerializeField] private string _lblNotesHitName = "lbl-notes-hit";
        [SerializeField] private string _lblNotesShownName = "lbl-notes-shown";
        [SerializeField] private string _lblAccuracyName = "lbl-accuracy";
        [SerializeField] private string _btnMainMenuName = "btn-main-menu";
        [SerializeField] private string _btnQuitName = "btn-quit";

        [Header("Events")]

        [field: SerializeField] public UnityEvent GoingToMainMenu { get; private set; } = new();
        [field: SerializeField] public UnityEvent Quitting { get; private set; } = new();

        private void Awake()
        {
            _grpGameOver = _uiDocument!.rootVisualElement.Query<VisualElement>(_grpGameOverName).First();
            _lblNotesHit = _uiDocument!.rootVisualElement.Query<Label>(_lblNotesHitName).First();
            _lblNotesShown = _uiDocument!.rootVisualElement.Query<Label>(_lblNotesShownName).First();
            _lblAccuracy = _uiDocument!.rootVisualElement.Query<Label>(_lblAccuracyName).First();

            _lblNotesHitFormatString = _lblNotesHit.text;
            _lblNotesShownFormatString = _lblNotesShown.text;
            _lblAccuracyFormatString = _lblAccuracy.text;

            Button btnMainMenu = _uiDocument!.rootVisualElement.Query<Button>(_btnMainMenuName).First();
            Button btnQuit = _uiDocument!.rootVisualElement.Query<Button>(_btnQuitName).First();

            btnMainMenu.clicked += GoingToMainMenu.Invoke;
            btnQuit.clicked += Quitting.Invoke;
        }

        [Button]
        public void ShowMenu()
        {
            _grpGameOver!.style.display = DisplayStyle.Flex;
            _lblNotesHit!.text = string.Format(_lblNotesHitFormatString, _noteHitter!.NotesHitCount);
            _lblNotesShown!.text = string.Format(_lblNotesShownFormatString, _notesGenerator!.GeneratedNoteCount);
            _lblAccuracy!.text = string.Format(_lblAccuracyFormatString, (float)_noteHitter.NotesHitCount / _notesGenerator.GeneratedNoteCount);
        }
    }
}
