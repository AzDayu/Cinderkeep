using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MainHub.CharacterSelect.Editor
{
    [InitializeOnLoad]
    public static class MainHub_PlayModeStartScene
    {
        private const string StartScenePath = "Assets/Scenes/Main_Lobby.unity";

        static MainHub_PlayModeStartScene()
        {
            if (IsCommandLineTestRun())
            {
                return;
            }

            SetPlayModeStartScene();
            EditorApplication.delayCall += OpenStartSceneIfNeeded;
        }

        private static void SetPlayModeStartScene()
        {
            SceneAsset startScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(StartScenePath);
            if (startScene == null)
            {
                return;
            }

            if (EditorSceneManager.playModeStartScene == startScene)
            {
                return;
            }

            EditorSceneManager.playModeStartScene = startScene;
        }

        private static void OpenStartSceneIfNeeded()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                return;
            }

            Scene activeScene = SceneManager.GetActiveScene();
            if (activeScene.path == StartScenePath)
            {
                return;
            }

            if (activeScene.isDirty)
            {
                Debug.Log("MainHub_PlayModeStartScene: Main_Lobby was not auto-opened because the current scene has unsaved changes.");
                return;
            }

            SceneAsset startScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(StartScenePath);
            if (startScene == null)
            {
                return;
            }

            EditorSceneManager.OpenScene(StartScenePath, OpenSceneMode.Single);
        }

        private static bool IsCommandLineTestRun()
        {
            string[] arguments = Environment.GetCommandLineArgs();
            for (int i = 0; i < arguments.Length; i++)
            {
                if (string.Equals(arguments[i], "-runTests", StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
