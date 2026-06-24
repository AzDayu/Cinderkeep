using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

// 5.00 direction: Provides editor-only setup or validation tooling for the 5.00 production workflow.
// 5.01+ note: Keep this out of runtime builds and use it to speed scene wiring, QA checks, and team handoff.
namespace Cinderkeep.EditorTools
{
    // 에디터를 열었을 때는 실제 게임 작업 씬을 보여주고,
    // Play 버튼을 누르면 메인 메뉴 씬부터 시작되도록 맞추는 편집기 전용 설정입니다.
    [InitializeOnLoad]
    public static class CinderkeepEditorStartupScene
    {
        private const string _mainMenuScenePath = "Assets/Scenes/Main_Lobby.unity";
        private const string _mainGameScenePath = "Assets/Scenes/MainGame/Cinderkeep_Game.unity";
        private const string _sessionOpenKey = "Cinderkeep.EditorStartupScene.Opened";

        static CinderkeepEditorStartupScene()
        {
            EditorApplication.delayCall += InitializeEditorSceneFlow;
        }

        private static void InitializeEditorSceneFlow()
        {
            EditorApplication.delayCall -= InitializeEditorSceneFlow;

            SetPlayModeStartScene();
            OpenMainGameSceneOnce();
        }

        private static void SetPlayModeStartScene()
        {
            SceneAsset mainMenuScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(_mainMenuScenePath);
            if (mainMenuScene == null)
            {
                return;
            }

            EditorSceneManager.playModeStartScene = mainMenuScene;
        }

        private static void OpenMainGameSceneOnce()
        {
            if (SessionState.GetBool(_sessionOpenKey, false))
            {
                return;
            }

            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                return;
            }

            if (IsAnyOpenSceneDirty())
            {
                return;
            }

            Scene activeScene = EditorSceneManager.GetActiveScene();
            if (activeScene.path == _mainGameScenePath)
            {
                SessionState.SetBool(_sessionOpenKey, true);
                return;
            }

            SceneAsset mainGameScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(_mainGameScenePath);
            if (mainGameScene == null)
            {
                return;
            }

            EditorSceneManager.OpenScene(_mainGameScenePath, OpenSceneMode.Single);
            SessionState.SetBool(_sessionOpenKey, true);
        }

        private static bool IsAnyOpenSceneDirty()
        {
            int sceneCount = SceneManager.sceneCount;
            for (int i = 0; i < sceneCount; i++)
            {
                Scene scene = SceneManager.GetSceneAt(i);
                if (scene.isDirty)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
