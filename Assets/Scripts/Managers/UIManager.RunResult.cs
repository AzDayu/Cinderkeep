using UnityEngine;
using UnityEngine.UI;

namespace Cinderkeep.Gameplay
{
    public sealed partial class UIManager
    {
        // Run Result 화면의 입력과 fallback 텍스트 표시를 담당합니다.
        // 화면 열기/닫기 결정은 UIManager 본체가 하고, 결과 데이터 구성은 RunResultTracker가 담당합니다.
        private void UpdateRunResultInput()
        {
            if (CinderkeepInput.WasKeyPressedThisFrame(KeyCode.R))
            {
                RestartRunFromResult();
                return;
            }

            if (CinderkeepInput.WasKeyPressedThisFrame(KeyCode.Escape))
            {
                ReturnToMainLobbyFromResult();
            }
        }

        private void RestartRunFromResult()
        {
            if (GameManager.Inst == null)
            {
                return;
            }

            CloseGameOverPanel();
            PlayUiClickSfx();
            GameManager.Inst.RestartRun();
        }

        private void ReturnToMainLobbyFromResult()
        {
            if (GameManager.Inst == null)
            {
                return;
            }

            PlayUiBackSfx();
            GameManager.Inst.ReturnToMainLobby();
        }

        private void RefreshRunResultTextFromTracker(bool isClear)
        {
            ConnectRunResultText();
            if (_runResultText == null)
            {
                return;
            }

            GameRunModel gameRunModel = GameManager.Inst == null ? null : GameManager.Inst.GameRunModel;
            RunResultTracker tracker = RunResultTracker.Instance;
            RunResultSnapshot snapshot = tracker == null
                ? RunResultTextFormatter.CreateFallbackSnapshot(isClear, gameRunModel)
                : tracker.CreateSnapshot(isClear, gameRunModel);
            _runResultText.text = RunResultTextFormatter.BuildText(snapshot);
        }

        private void OpenRunResultUI(bool isClear)
        {
            ConnectRunResultUI();
            if (_runResultUI != null)
            {
                _runResultUI.Open(isClear);
                return;
            }

            RefreshRunResultTextFromTracker(isClear);
            SetActive(_gameOverPanel, true);
        }

        private void ConnectRunResultUI()
        {
            if (_runResultUI != null || _gameOverPanel == null)
            {
                return;
            }

            _runResultUI = _gameOverPanel.GetComponent<RunResultUI>();
            if (_runResultUI == null)
            {
                _runResultUI = _gameOverPanel.AddComponent<RunResultUI>();
            }
        }

        private void ConnectRunResultText()
        {
            if (_runResultText != null || _gameOverPanel == null)
            {
                return;
            }

            _runResultText = _gameOverPanel.GetComponentInChildren<Text>(true);
        }
    }
}
