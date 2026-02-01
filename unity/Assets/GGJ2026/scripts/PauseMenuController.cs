#nullable enable

using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace GGJ2026
{
    public class PauseMenuController : MonoBehaviour
    {
        private bool _isPaused;

        private VisualElement? _grpPaused;
        private Button? _btnResume;
        private Button? _btnMainMenu;
        private Button? _btnQuit;

        [SerializeField, RequiredIn(PrefabKind.PrefabInstanceAndNonPrefabInstance)]
        private UIDocument? _uiDocument;

        [SerializeField] private string _pauseInputActionName = "pause";

        [field: SerializeField] public UnityEvent Pausing { get; private set; } = new();
        [field: SerializeField] public UnityEvent Resuming { get; private set; } = new();
        [field: SerializeField] public UnityEvent GoingToMainMenu { get; private set; } = new();
        [field: SerializeField] public UnityEvent Quitting { get; private set; } = new();

        [SerializeField] private string _grpPausedName = "grp-paused";
        [SerializeField] private string _btnResumeName = "btn-resume";
        [SerializeField] private string _btnMainMenuName = "btn-main-menu";
        [SerializeField] private string _btnQuitName = "btn-quit";

        private void Awake()
        {
            InputAction pauseAction = InputSystem.actions[_pauseInputActionName];
            pauseAction.performed += ctx => togglePause(!_isPaused);

            _grpPaused = _uiDocument!.rootVisualElement.Query<VisualElement>(_grpPausedName).First();
            _btnResume = _uiDocument!.rootVisualElement.Query<Button>(_btnResumeName).First();
            _btnMainMenu = _uiDocument!.rootVisualElement.Query<Button>(_btnMainMenuName).First();
            _btnQuit = _uiDocument!.rootVisualElement.Query<Button>(_btnQuitName).First();

            _btnResume.clicked += () => togglePause(isPaused: false);
            _btnMainMenu.clicked += () => {
                Time.timeScale = 1f;    // Otherwise, next play will start paused
                GoingToMainMenu.Invoke();
            };
            _btnQuit.clicked += () => Quitting.Invoke();
        }

        private void togglePause(bool isPaused)
        {
            _isPaused = isPaused;
            Time.timeScale = isPaused ? 0f : 1f;
            _grpPaused!.style.display = isPaused ? DisplayStyle.Flex : DisplayStyle.None;
            (isPaused ? Pausing : Resuming).Invoke();
        }
    }
}
