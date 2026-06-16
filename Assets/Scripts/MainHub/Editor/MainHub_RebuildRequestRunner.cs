using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MainHub.CharacterSelect.Editor
{
    [InitializeOnLoad]
    public static class MainHub_RebuildRequestRunner
    {
        private const string RequestPath = "Assets/_Recovery/RebuildMainLobby.request";

        static MainHub_RebuildRequestRunner()
        {
            EditorApplication.delayCall += TryRunRequest;
            EditorApplication.playModeStateChanged += HandlePlayModeStateChanged;
        }

        private static void HandlePlayModeStateChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.EnteredEditMode)
            {
                EditorApplication.delayCall += TryRunRequest;
            }
        }

        private static void TryRunRequest()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode || !File.Exists(RequestPath))
            {
                return;
            }

            Scene activeScene = SceneManager.GetActiveScene();
            if (activeScene.isDirty)
            {
                Debug.LogWarning("MainHub_RebuildRequestRunner: current scene has unsaved changes. Save the scene, then run MainHub/Character Select/Rebuild Main Lobby Only.");
                return;
            }

            MainHub_SceneBuilder.RebuildMainLobbyAndBuildSettingsOnly();
            File.Delete(RequestPath);

            string metaPath = $"{RequestPath}.meta";
            if (File.Exists(metaPath))
            {
                File.Delete(metaPath);
            }

            AssetDatabase.Refresh();
            EditorSceneManager.OpenScene("Assets/Scenes/Main_Lobby.unity", OpenSceneMode.Single);
            Debug.Log("MainHub_RebuildRequestRunner: Main_Lobby was rebuilt from the pending request.");
        }
    }
}
