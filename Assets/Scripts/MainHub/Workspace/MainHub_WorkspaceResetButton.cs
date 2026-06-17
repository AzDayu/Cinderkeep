using UnityEngine;
using UnityEngine.SceneManagement;

namespace MainHub.Workspace
{
    public sealed class MainHub_WorkspaceResetButton : MonoBehaviour
    {
        public void ResetCurrentWorkspace()
        {
            // 개인 작업 공간에서 테스트 중 만든 임시 상태를 버리고 현재 씬을 처음 상태로 다시 엽니다.
            Scene currentScene = SceneManager.GetActiveScene();
            if (string.IsNullOrWhiteSpace(currentScene.name))
            {
                Debug.LogWarning($"{nameof(MainHub_WorkspaceResetButton)}: current scene name is empty.");
                return;
            }

            SceneManager.LoadScene(currentScene.name);
        }
    }
}
