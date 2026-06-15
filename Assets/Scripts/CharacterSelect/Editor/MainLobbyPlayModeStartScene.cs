using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace OODong.CharacterSelect.Editor
{
    [InitializeOnLoad]
    public static class MuckholdPlayModeStartScene
    {
        private const string StartScenePath = "Assets/Scenes/Main_Lobby.unity";

        static MuckholdPlayModeStartScene()
        {
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
                Debug.Log("MuckholdPlayModeStartScene: Main_Lobby was not auto-opened because the current scene has unsaved changes.");
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
            if (scene.path != StartScenePath || GameObject.Find("Main Camera") != null)
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
    }
}
