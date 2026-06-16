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
                EnsureStartSceneCamera(activeScene);
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
            EnsureStartSceneCamera(SceneManager.GetActiveScene());
        }

        private static void EnsureStartSceneCamera(Scene scene)
        {
            if (scene.path != StartScenePath || HasRootGameObject(scene, "Main Camera"))
            {
                return;
            }

            bool wasDirty = scene.isDirty;
            GameObject cameraObject = new GameObject("Main Camera", typeof(Camera), typeof(AudioListener));
            Camera camera = cameraObject.GetComponent<Camera>();
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color(0.08f, 0.18f, 0.12f);
            camera.orthographic = true;
            camera.orthographicSize = 13f;
            camera.transform.position = new Vector3(0f, 0f, -10f);

            EditorSceneManager.MarkSceneDirty(scene);
            if (!wasDirty)
            {
                EditorSceneManager.SaveScene(scene);
            }
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

        private static bool HasRootGameObject(Scene scene, string objectName)
        {
            GameObject[] rootObjects = scene.GetRootGameObjects();
            for (int i = 0; i < rootObjects.Length; i++)
            {
                if (rootObjects[i].name == objectName)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
