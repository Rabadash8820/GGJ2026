#nullable enable

using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

namespace GGJ2026
{
    public class GameOverMenuController : MonoBehaviour
    {
        private string _txtNotesHitFormatString = "";
        private string _txtNotesShownFormatString = "";
        private string _txtAccuracyFormatString = "";

        [SerializeField, RequiredIn(PrefabKind.PrefabInstanceAndNonPrefabInstance)]
        private NoteHitter? _noteHitter;

        [SerializeField, RequiredIn(PrefabKind.PrefabInstanceAndNonPrefabInstance)]
        private NotesGenerator? _notesGenerator;

        [Header("Audio")]

        [SerializeField, RequiredIn(PrefabKind.PrefabInstanceAndNonPrefabInstance)]
        private AudioSource? _musicWin;

        [SerializeField, RequiredIn(PrefabKind.PrefabInstanceAndNonPrefabInstance)]
        private AudioSource? _musicLose;

        [Header("UI")]

        [SerializeField, RequiredIn(PrefabKind.PrefabInstanceAndNonPrefabInstance)]
        private RectTransform? _rectWin;

        [SerializeField, RequiredIn(PrefabKind.PrefabInstanceAndNonPrefabInstance)]
        private RectTransform? _rectLose;

        [SerializeField, RequiredIn(PrefabKind.PrefabInstanceAndNonPrefabInstance)]
        private Text? _txtNotesHit;

        [SerializeField, RequiredIn(PrefabKind.PrefabInstanceAndNonPrefabInstance)]
        private Text? _txtNotesShown;

        [SerializeField, RequiredIn(PrefabKind.PrefabInstanceAndNonPrefabInstance)]
        private Text? _txtAccuracy;

        private void Awake()
        {
            _txtNotesHitFormatString = _txtNotesHit!.text;
            _txtNotesShownFormatString = _txtNotesShown!.text;
            _txtAccuracyFormatString = _txtAccuracy!.text;
        }

        [Button]
        public void ShowMenu(bool didWin)
        {
            _txtNotesHit!.text = string.Format(_txtNotesHitFormatString, _noteHitter!.NotesHitCount);
            _txtNotesShown!.text = string.Format(_txtNotesShownFormatString, _notesGenerator!.GeneratedNoteCount);
            _txtAccuracy!.text = string.Format(_txtAccuracyFormatString, (float)_noteHitter.NotesHitCount / _notesGenerator.GeneratedNoteCount);

            _rectWin!.gameObject.SetActive(didWin);
            _rectLose!.gameObject.SetActive(!didWin);

            _musicWin!.gameObject.SetActive(didWin);
            _musicLose!.gameObject.SetActive(!didWin);
        }
    }
}
