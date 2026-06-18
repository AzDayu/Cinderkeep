using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class CinderkeepHierarchyOrganizer
{
    private const string ScenePath = "Assets/Scenes/MainGame/Cinderkeep_Game.unity";

    [MenuItem("Cinderkeep/Main Game/Organize Hierarchy")]
    public static void OrganizeHierarchy()
    {
        Scene scene = EditorSceneManager.OpenScene(ScenePath);
        OrganizeScene(scene);
        EditorSceneManager.SaveScene(scene);
        AssetDatabase.SaveAssets();
        Debug.Log("CinderkeepHierarchyOrganizer: hierarchy organized.");
    }

    public static void OrganizeHierarchyBatch()
    {
        OrganizeHierarchy();
    }

    private static void OrganizeScene(Scene scene)
    {
        SetRootOrder(scene, new string[]
        {
            "MainGame_Managers",
            "MainGame_LoopConnector",
            "MainGame_RuntimeManagers",
            "Player",
            "CinderHeart",
            "EnemySpawnPoints",
            "MainGame_RuntimeObjects",
            "Canvas_GameHUD",
            "EnemySpawnCandidatePoints"
        });

        SetChildOrder(scene, "MainGame_Managers", new string[]
        {
            "GameManager",
            "GameDataManager",
            "GameObjectManager",
            "UIManager",
            "SoundManager",
            "MapManager",
            "Transform_RuntimeObjectRoot"
        });

        SetChildOrder(scene, "MainGame_RuntimeManagers", new string[]
        {
            "GameFlowController",
            "EnemyLoopConnector"
        });

        SetChildOrder(scene, "Player", new string[]
        {
            "Transform_CameraRoot_FirstPerson",
            "PlayerVisual_DebugCapsule",
            "Transform_GroundCheck"
        });

        SetChildOrder(scene, "CinderHeart", new string[]
        {
            "Visual_CinderHeart_Core",
            "Light_CinderHeart_Beacon"
        });

        SetChildOrder(scene, "EnemySpawnPoints", new string[]
        {
            "EnemySpawnPoint_Near_Required",
            "EnemySpawnPoint_Outer_Random"
        });

        SetChildOrder(scene, "MainGame_RuntimeObjects", new string[]
        {
            "Enemies_GameLoop",
            "Resources_GameLoop",
            "BuildingPreview_GameLoop"
        });

        SetChildOrder(scene, "Canvas_GameHUD", new string[]
        {
            "Panel_HUDRoot",
            "Panel_CinderHeartHUD",
            "Panel_EnemyTargetHUD",
            "Panel_InventoryRoot_Disabled",
            "Panel_GameOver_Disabled"
        });

        SetChildOrder(scene, "EnemySpawnCandidatePoints", new string[]
        {
            "EnemySpawnCandidate_01",
            "EnemySpawnCandidate_02",
            "EnemySpawnCandidate_03",
            "EnemySpawnCandidate_04",
            "EnemySpawnCandidate_05",
            "EnemySpawnCandidate_06",
            "EnemySpawnCandidate_07",
            "EnemySpawnCandidate_08"
        });
    }

    private static void SetRootOrder(Scene scene, IReadOnlyList<string> orderedNames)
    {
        Dictionary<string, GameObject> rootsByName = GetRootObjectsByName(scene);
        for (int i = 0; i < orderedNames.Count; i++)
        {
            GameObject rootObject;
            if (rootsByName.TryGetValue(orderedNames[i], out rootObject))
            {
                rootObject.transform.SetSiblingIndex(i);
                EditorUtility.SetDirty(rootObject);
            }
        }
    }

    private static void SetChildOrder(Scene scene, string rootName, IReadOnlyList<string> orderedNames)
    {
        GameObject rootObject = FindRootObject(scene, rootName);
        if (rootObject == null)
        {
            return;
        }

        Dictionary<string, Transform> childrenByName = new Dictionary<string, Transform>();
        for (int i = 0; i < rootObject.transform.childCount; i++)
        {
            Transform child = rootObject.transform.GetChild(i);
            if (childrenByName.ContainsKey(child.name) == false)
            {
                childrenByName.Add(child.name, child);
            }
        }

        for (int i = 0; i < orderedNames.Count; i++)
        {
            Transform child;
            if (childrenByName.TryGetValue(orderedNames[i], out child))
            {
                child.SetSiblingIndex(i);
                EditorUtility.SetDirty(child.gameObject);
            }
        }
    }

    private static Dictionary<string, GameObject> GetRootObjectsByName(Scene scene)
    {
        Dictionary<string, GameObject> rootsByName = new Dictionary<string, GameObject>();
        GameObject[] rootObjects = scene.GetRootGameObjects();
        for (int i = 0; i < rootObjects.Length; i++)
        {
            if (rootsByName.ContainsKey(rootObjects[i].name) == false)
            {
                rootsByName.Add(rootObjects[i].name, rootObjects[i]);
            }
        }

        return rootsByName;
    }

    private static GameObject FindRootObject(Scene scene, string objectName)
    {
        GameObject[] rootObjects = scene.GetRootGameObjects();
        for (int i = 0; i < rootObjects.Length; i++)
        {
            if (rootObjects[i].name == objectName)
            {
                return rootObjects[i];
            }
        }

        return null;
    }
}
