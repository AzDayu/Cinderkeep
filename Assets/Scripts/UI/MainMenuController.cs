using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Cinderkeep.UI
{
    // Cinderkeep의 게임 시작과 게임 종료만 담당하는 메인 메뉴 컨트롤러입니다.
    public sealed class MainMenuController : MonoBehaviour
    {
        [SerializeField] private string _gameSceneName = "Cinderkeep_Game";
        [SerializeField] private Button Button_StartGame;
        [SerializeField] private Button Button_QuitGame;

        private void Start()
        {
            InitializeButtons();
        }

        private void OnDestroy()
        {
            ReleaseButtons();
        }

        public void SetReferences(Button buttonStartGame, Button buttonQuitGame)
        {
            Button_StartGame = buttonStartGame;
            Button_QuitGame = buttonQuitGame;
        }

        public void StartGame()
        {
            if (string.IsNullOrEmpty(_gameSceneName))
            {
                Debug.LogWarning("MainMenuController: game scene name is empty.");
                return;
            }

            SceneManager.LoadScene(_gameSceneName);
        }

        public void QuitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        private void InitializeButtons()
        {
            if (Button_StartGame != null)
            {
                Button_StartGame.onClick.AddListener(StartGame);
            }

            if (Button_QuitGame != null)
            {
                Button_QuitGame.onClick.AddListener(QuitGame);
            }
        }

        private void ReleaseButtons()
        {
            if (Button_StartGame != null)
            {
                Button_StartGame.onClick.RemoveListener(StartGame);
            }

            if (Button_QuitGame != null)
            {
                Button_QuitGame.onClick.RemoveListener(QuitGame);
            }
        }
    }
}
