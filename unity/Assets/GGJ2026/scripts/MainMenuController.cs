#nullable enable

using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace GGJ2026
{
    public class MainMenuController : MonoBehaviour
    {
        private GroupBox? _grpMain;
        private Button? _btnPlay;
        private Button? _btnCredits;
        private Button? _btnQuit;
        private GroupBox? _grpCredits;
        private Button? _btnCreditsBack;

        [SerializeField, RequiredIn(PrefabKind.PrefabInstanceAndNonPrefabInstance)]
        private UIDocument? _uiDocument;

        [field: SerializeField] public UnityEvent Playing { get; private set; } = new();
        [field: SerializeField] public UnityEvent Quitting { get; private set; } = new();

        [SerializeField] private string _grpMainName = "grp-main";
        [SerializeField] private string _btnPlayName = "btn-play";
        [SerializeField] private string _btnCreditsName = "btn-credits";
        [SerializeField] private string _btnQuitName = "btn-quit";
        [SerializeField] private string _grpCreditsName = "grp-credits";
        [SerializeField] private string _btnCreditsBackName = "btn-credits-back";

        private void Awake()
        {
            _grpMain = _uiDocument!.rootVisualElement.Query<GroupBox>(_grpMainName).First();
            _btnPlay = _uiDocument!.rootVisualElement.Query<Button>(_btnPlayName).First();
            _btnCredits = _uiDocument!.rootVisualElement.Query<Button>(_btnCreditsName).First();
            _btnQuit = _uiDocument!.rootVisualElement.Query<Button>(_btnQuitName).First();
            _grpCredits = _uiDocument!.rootVisualElement.Query<GroupBox>(_grpCreditsName).First();
            _btnCreditsBack = _uiDocument!.rootVisualElement.Query<Button>(_btnCreditsBackName).First();

            _btnPlay.clicked += Playing.Invoke;
            _btnQuit.clicked += Quitting.Invoke;
            _btnCredits.clicked += () => {
                _grpMain.style.display = DisplayStyle.None;
                _grpCredits.style.display = DisplayStyle.Flex;
            };
            _btnCreditsBack.clicked += () => {
                _grpMain.style.display = DisplayStyle.Flex;
                _grpCredits.style.display = DisplayStyle.None;
            };
        }
    }
}
