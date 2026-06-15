using System.Collections.Generic;
using OODong.CharacterSelect;
using OODong.Shared;
using OODong.UI;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem.UI;
#endif

namespace OODong.Muckhold.Editor
{
    public static class MuckholdPlayableSceneBuilder
    {
        private const string MainLobbyScenePath = "Assets/Scenes/Main_Lobby.unity";
        private const string SharedWorkspaceScenePath = "Assets/Scenes/MainWorkspace/Muckhold_Workspace.unity";
        private const string BuildWorkspaceScenePath = "Assets/Scenes/MainWorkspace/MainWorkspaceRoom_ForBuild.unity";
        private const string GeneratedMaterialFolder = "Assets/MainAssets/Muckhold/GeneratedMaterials";
        private const string FontPath = "Assets/Fonts/ChosunCentennial.ttf";
        private const string ArrowModelPath = "Assets/MainAssets/Muckhold/ExternalAssets/PolygonParticles/Models/FX_Arrow_01.fbx";
        private const string PlantPrefabPath = "Assets/MainAssets/Muckhold/ExternalAssets/CarnivorousPlant/Prefabs/Carnivorous Plant-Green.prefab";
        private const string ApplePrefabPath = "Assets/MainAssets/Muckhold/ExternalAssets/FantasticFoodPack/prefabs/P_PROP_food_apple.prefab";
        private const string ForestBackgroundPath = "Assets/MainAssets/Muckhold/ExternalAssets/FantasyForest_Background.jpg";

        [MenuItem("OODong/Muckhold/Rebuild Playable Workspace Scenes")]
        public static void RebuildPlayableWorkspaceScenes()
        {
            EnsureFolders();
            AssetDatabase.Refresh();
            RebuildPlayableScene(SharedWorkspaceScenePath, "WorkspaceRoot_Shared", "Muckhold_Playable_V1");
            RebuildPlayableScene(BuildWorkspaceScenePath, "WorkspaceRoot_MainBuild", "Muckhold_Build_Review_Playable");
            UpdateBuildSettings();
            AssetDatabase.SaveAssets();
        }

        [MenuItem("OODong/Muckhold/Rebuild Muckhold Workspace Only")]
        public static void RebuildMuckholdWorkspaceOnly()
        {
            EnsureFolders();
            AssetDatabase.Refresh();
            RebuildPlayableScene(SharedWorkspaceScenePath, "WorkspaceRoot_Shared", "Muckhold_Playable_V1");
            UpdateBuildSettings();
            AssetDatabase.SaveAssets();
        }

        public static void ValidatePlayableWorkspaceScenes()
        {
            ValidatePlayableScene(SharedWorkspaceScenePath);
            ValidatePlayableScene(BuildWorkspaceScenePath);
            Debug.Log("MuckholdPlayableSceneBuilder: playable workspace scenes validated.");
        }

        private static void RebuildPlayableScene(string scenePath, string workspaceRootName, string title)
        {
            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            ConfigureMoodLighting();
            CreateEventSystem();

            GameObject workspaceRoot = new GameObject(workspaceRootName);
            GameObject player = CreatePlayer(workspaceRoot.transform);
            MuckholdInventory inventory = player.GetComponent<MuckholdInventory>();
            Camera playerCamera = player.GetComponentInChildren<Camera>();
            SceneCameraController sceneCameraController = playerCamera.GetComponent<SceneCameraController>();

            CreateField(workspaceRoot.transform);
            CreateHorizon(workspaceRoot.transform);
            CreateGatheringNodes(workspaceRoot.transform);
            MuckholdEnemy enemyTemplate = CreateEnemyTemplate(workspaceRoot.transform, player.transform);
            MuckholdProjectile projectileTemplate = CreateProjectileTemplate(workspaceRoot.transform);
            CreateInitialEnemies(workspaceRoot.transform, enemyTemplate, player.transform);
            CreateEnemySpawner(workspaceRoot.transform, enemyTemplate, player.transform);
            Transform placedItemRoot = CreatePlacedItemRoot(workspaceRoot.transform);

            MuckholdHudView hudView = CreateHud(title, inventory, sceneCameraController);
            MuckholdPickaxeView pickaxeView = player.GetComponentInChildren<MuckholdPickaxeView>();
            MuckholdFirstPersonPlayer firstPersonPlayer = player.GetComponent<MuckholdFirstPersonPlayer>();
            firstPersonPlayer.SetReferences(player.GetComponent<CharacterController>(), playerCamera, inventory, hudView, pickaxeView);
            EditorUtility.SetDirty(firstPersonPlayer);

            MuckholdPlaceableItemFactory placeableFactory = player.GetComponent<MuckholdPlaceableItemFactory>();
            placeableFactory.SetReferences(
                placedItemRoot,
                GetOrCreateColorMaterial("Pickup_Stone.mat", new Color(0.42f, 0.44f, 0.43f, 1f)),
                GetOrCreateColorMaterial("Mineable_Ore.mat", new Color(0.12f, 0.48f, 0.9f, 1f)));
            EditorUtility.SetDirty(placeableFactory);

            MuckholdAutoShooter autoShooter = player.GetComponent<MuckholdAutoShooter>();
            autoShooter.SetFireOrigin(playerCamera.transform);
            autoShooter.SetProjectileTemplate(projectileTemplate);
            EditorUtility.SetDirty(autoShooter);

            EditorSceneManager.SaveScene(scene, scenePath);
        }

        private static void ConfigureMoodLighting()
        {
            Material skybox = GetOrCreateSkyboxMaterial();
            if (skybox != null)
            {
                RenderSettings.skybox = skybox;
            }

            RenderSettings.ambientMode = AmbientMode.Trilight;
            RenderSettings.ambientSkyColor = new Color(0.48f, 0.62f, 0.74f, 1f);
            RenderSettings.ambientEquatorColor = new Color(0.42f, 0.5f, 0.43f, 1f);
            RenderSettings.ambientGroundColor = new Color(0.18f, 0.22f, 0.18f, 1f);
            RenderSettings.fog = true;
            RenderSettings.fogMode = FogMode.Linear;
            RenderSettings.fogColor = new Color(0.58f, 0.7f, 0.76f, 1f);
            RenderSettings.fogStartDistance = 70f;
            RenderSettings.fogEndDistance = 230f;

            GameObject lightObject = new GameObject("Sun_Directional_Light", typeof(Light));
            Light light = lightObject.GetComponent<Light>();
            light.type = LightType.Directional;
            light.color = new Color(1f, 0.87f, 0.68f, 1f);
            light.intensity = 1.15f;
            light.shadows = LightShadows.Soft;
            light.transform.rotation = Quaternion.Euler(36f, -42f, 0f);
        }

        private static GameObject CreatePlayer(Transform parent)
        {
            GameObject player = new GameObject(
                "FirstPerson_Player",
                typeof(CharacterController),
                typeof(MuckholdInventory),
                typeof(MuckholdFirstPersonPlayer),
                typeof(MuckholdAutoShooter),
                typeof(MuckholdPlaceableItemFactory));
            player.transform.SetParent(parent, false);
            player.transform.position = new Vector3(0f, 0.08f, 0f);

            CharacterController controller = player.GetComponent<CharacterController>();
            controller.height = 1.8f;
            controller.radius = 0.34f;
            controller.center = new Vector3(0f, 0.9f, 0f);

            GameObject cameraObject = new GameObject("FirstPerson_Camera", typeof(Camera), typeof(AudioListener), typeof(SceneCameraController));
            cameraObject.transform.SetParent(player.transform, false);
            cameraObject.transform.localPosition = new Vector3(0f, 1.62f, 0f);
            cameraObject.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);

            Camera camera = cameraObject.GetComponent<Camera>();
            camera.clearFlags = CameraClearFlags.Skybox;
            camera.fieldOfView = 68f;
            camera.nearClipPlane = 0.03f;
            camera.farClipPlane = 320f;

            SceneCameraController sceneCameraController = cameraObject.GetComponent<SceneCameraController>();
            sceneCameraController.SetCamera(camera);
            sceneCameraController.enabled = false;

            CreatePickaxeView(cameraObject.transform);
            return player;
        }

        private static void CreatePickaxeView(Transform cameraTransform)
        {
            GameObject pickaxeRoot = new GameObject("FirstPerson_Pickaxe_View", typeof(MuckholdPickaxeView));
            pickaxeRoot.transform.SetParent(cameraTransform, false);
            pickaxeRoot.transform.localPosition = new Vector3(0.48f, -0.46f, 0.88f);
            pickaxeRoot.transform.localRotation = Quaternion.Euler(8f, -35f, -8f);
            pickaxeRoot.transform.localScale = Vector3.one * 0.78f;

            Material handleMaterial = GetOrCreateColorMaterial("Pickaxe_Handle.mat", new Color(0.34f, 0.21f, 0.12f, 1f));
            Material headMaterial = GetOrCreateColorMaterial("Pickaxe_Head.mat", new Color(0.55f, 0.58f, 0.62f, 1f));

            GameObject handle = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            handle.name = "Pickaxe_Handle";
            handle.transform.SetParent(pickaxeRoot.transform, false);
            handle.transform.localPosition = Vector3.zero;
            handle.transform.localRotation = Quaternion.Euler(0f, 0f, 34f);
            handle.transform.localScale = new Vector3(0.025f, 0.3f, 0.025f);
            handle.GetComponent<Renderer>().sharedMaterial = handleMaterial;
            Object.DestroyImmediate(handle.GetComponent<Collider>());

            GameObject head = GameObject.CreatePrimitive(PrimitiveType.Cube);
            head.name = "Pickaxe_Head";
            head.transform.SetParent(pickaxeRoot.transform, false);
            head.transform.localPosition = new Vector3(0.11f, 0.13f, 0f);
            head.transform.localRotation = Quaternion.Euler(0f, 0f, 34f);
            head.transform.localScale = new Vector3(0.18f, 0.045f, 0.045f);
            head.GetComponent<Renderer>().sharedMaterial = headMaterial;
            Object.DestroyImmediate(head.GetComponent<Collider>());
        }

        private static Transform CreatePlacedItemRoot(Transform parent)
        {
            GameObject placedItemRoot = new GameObject("Placed_Item_Root");
            placedItemRoot.transform.SetParent(parent, false);
            return placedItemRoot.transform;
        }

        private static void CreateField(Transform parent)
        {
            Material grassMaterial = GetOrCreateColorMaterial("Muckhold_Field_Grass.mat", new Color(0.18f, 0.42f, 0.21f, 1f));
            GameObject field = GameObject.CreatePrimitive(PrimitiveType.Cube);
            field.name = "Muckhold_Green_Field";
            field.transform.SetParent(parent, false);
            field.transform.position = new Vector3(0f, -0.06f, 0f);
            field.transform.localScale = new Vector3(260f, 0.12f, 260f);
            field.GetComponent<Renderer>().sharedMaterial = grassMaterial;

            Material ridgeMaterial = GetOrCreateColorMaterial("Muckhold_Distant_Ridge.mat", new Color(0.21f, 0.34f, 0.28f, 1f));
            for (int i = 0; i < 14; i++)
            {
                float x = -92f + (i * 14f);
                float height = 8f + Mathf.Sin(i * 0.8f) * 3f;
                GameObject ridge = GameObject.CreatePrimitive(PrimitiveType.Cube);
                ridge.name = $"Horizon_Ridge_{i + 1:00}";
                ridge.transform.SetParent(parent, false);
                ridge.transform.position = new Vector3(x, height * 0.5f - 0.1f, 118f);
                ridge.transform.localScale = new Vector3(18f, height, 7f);
                ridge.GetComponent<Renderer>().sharedMaterial = ridgeMaterial;
            }

            Material treeTrunkMaterial = GetOrCreateColorMaterial("Tree_Trunk.mat", new Color(0.31f, 0.22f, 0.14f, 1f));
            Material treeLeafMaterial = GetOrCreateColorMaterial("Tree_Leaf.mat", new Color(0.13f, 0.33f, 0.18f, 1f));
            for (int i = 0; i < 18; i++)
            {
                float angle = i * 47f;
                float radius = 34f + (i % 5) * 9f;
                Vector3 position = new Vector3(Mathf.Cos(angle) * radius, 0f, Mathf.Sin(angle) * radius);
                CreateTree(parent, position, treeTrunkMaterial, treeLeafMaterial, i);
            }
        }

        private static void CreateTree(Transform parent, Vector3 position, Material trunkMaterial, Material leafMaterial, int index)
        {
            GameObject root = new GameObject($"Field_Tree_{index + 1:00}");
            root.transform.SetParent(parent, false);
            root.transform.position = position;

            GameObject trunk = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            trunk.name = "Trunk";
            trunk.transform.SetParent(root.transform, false);
            trunk.transform.localPosition = new Vector3(0f, 1.2f, 0f);
            trunk.transform.localScale = new Vector3(0.26f, 1.2f, 0.26f);
            trunk.GetComponent<Renderer>().sharedMaterial = trunkMaterial;

            GameObject leaf = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            leaf.name = "Leaf_Crown";
            leaf.transform.SetParent(root.transform, false);
            leaf.transform.localPosition = new Vector3(0f, 2.75f, 0f);
            leaf.transform.localScale = new Vector3(1.9f, 1.45f, 1.9f);
            leaf.GetComponent<Renderer>().sharedMaterial = leafMaterial;
            Object.DestroyImmediate(leaf.GetComponent<Collider>());
        }

        private static void CreateHorizon(Transform parent)
        {
            Texture2D forestTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(ForestBackgroundPath);
            if (forestTexture == null)
            {
                return;
            }

            Material backdropMaterial = GetOrCreateTextureMaterial("Fantasy_Forest_Backdrop.mat", forestTexture);
            GameObject backdrop = GameObject.CreatePrimitive(PrimitiveType.Quad);
            backdrop.name = "Distant_Fantasy_Forest_Backdrop";
            backdrop.transform.SetParent(parent, false);
            backdrop.transform.position = new Vector3(0f, 23f, 122f);
            backdrop.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
            backdrop.transform.localScale = new Vector3(96f, 55f, 1f);
            backdrop.GetComponent<Renderer>().sharedMaterial = backdropMaterial;
            Object.DestroyImmediate(backdrop.GetComponent<Collider>());
        }

        private static void CreateGatheringNodes(Transform parent)
        {
            Material stoneMaterial = GetOrCreateColorMaterial("Pickup_Stone.mat", new Color(0.42f, 0.44f, 0.43f, 1f));
            Material oreMaterial = GetOrCreateColorMaterial("Mineable_Ore.mat", new Color(0.12f, 0.48f, 0.9f, 1f));
            Material chestMaterial = GetOrCreateColorMaterial("Chest_Yellow.mat", new Color(0.86f, 0.62f, 0.16f, 1f));

            Vector3[] stonePositions =
            {
                new Vector3(1.8f, 0.25f, 2.8f),
                new Vector3(-2.4f, 0.25f, 3.1f),
                new Vector3(3.8f, 0.25f, -1.6f)
            };

            for (int i = 0; i < stonePositions.Length; i++)
            {
                GameObject stone = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                stone.name = $"Pickup_Stone_{i + 1:00}";
                stone.transform.SetParent(parent, false);
                stone.transform.position = stonePositions[i];
                stone.transform.localScale = new Vector3(0.46f, 0.32f, 0.42f);
                stone.GetComponent<Renderer>().sharedMaterial = stoneMaterial;
                stone.AddComponent<MuckholdPickupNode>().SetPickup(MuckholdItemId.Stone, 1, "Stone");
            }

            Vector3[] orePositions =
            {
                new Vector3(8f, 0.85f, 7f),
                new Vector3(-10f, 0.85f, 8f),
                new Vector3(14f, 0.85f, -9f),
                new Vector3(-16f, 0.85f, -6f)
            };

            for (int i = 0; i < orePositions.Length; i++)
            {
                GameObject ore = GameObject.CreatePrimitive(PrimitiveType.Cube);
                ore.name = $"Mineable_Blue_Ore_{i + 1:00}";
                ore.transform.SetParent(parent, false);
                ore.transform.position = orePositions[i];
                ore.transform.rotation = Quaternion.Euler(0f, 35f * i, 11f);
                ore.transform.localScale = new Vector3(1.2f, 1.5f, 1.2f);
                ore.GetComponent<Renderer>().sharedMaterial = oreMaterial;
                ore.AddComponent<MuckholdMineableNode>().SetMineable("Blue Ore", 3, MuckholdItemId.Ore, 1);
            }

            Vector3[] chestPositions =
            {
                new Vector3(6f, 0.55f, -5f),
                new Vector3(-8f, 0.55f, -8f)
            };

            for (int i = 0; i < chestPositions.Length; i++)
            {
                GameObject chest = GameObject.CreatePrimitive(PrimitiveType.Cube);
                chest.name = $"Chest_Yellow_Box_{i + 1:00}";
                chest.transform.SetParent(parent, false);
                chest.transform.position = chestPositions[i];
                chest.transform.localScale = new Vector3(1.4f, 1f, 1f);
                chest.GetComponent<Renderer>().sharedMaterial = chestMaterial;
                chest.AddComponent<MuckholdChestNode>().SetReward(i == 0 ? MuckholdItemId.Apple : MuckholdItemId.Stone, i == 0 ? 1 : 2);
                CreateAppleVisual(chest.transform, i);
            }
        }

        private static void CreateAppleVisual(Transform parent, int index)
        {
            GameObject applePrefab = AssetDatabase.LoadAssetAtPath<GameObject>(ApplePrefabPath);
            if (applePrefab == null || index != 0)
            {
                return;
            }

            GameObject apple = PrefabUtility.InstantiatePrefab(applePrefab) as GameObject;
            if (apple == null)
            {
                return;
            }

            apple.name = "External_Apple_Reward_Visual";
            apple.transform.SetParent(parent, false);
            apple.transform.localPosition = new Vector3(0f, 0.85f, 0f);
            apple.transform.localScale = Vector3.one * 2f;
        }

        private static MuckholdEnemy CreateEnemyTemplate(Transform parent, Transform target)
        {
            GameObject enemyRoot = new GameObject("Enemy_Template_CarnivorousPlant", typeof(CapsuleCollider), typeof(Rigidbody), typeof(MuckholdEnemy));
            enemyRoot.transform.SetParent(parent, false);
            enemyRoot.transform.position = new Vector3(0f, 0.05f, 0f);

            CapsuleCollider collider = enemyRoot.GetComponent<CapsuleCollider>();
            collider.height = 2.2f;
            collider.radius = 0.55f;
            collider.center = new Vector3(0f, 1.1f, 0f);

            Rigidbody rigidbody = enemyRoot.GetComponent<Rigidbody>();
            rigidbody.isKinematic = true;
            rigidbody.useGravity = false;

            MuckholdEnemy enemy = enemyRoot.GetComponent<MuckholdEnemy>();
            enemy.SetTarget(target);

            GameObject plantPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(PlantPrefabPath);
            if (plantPrefab != null)
            {
                GameObject visual = PrefabUtility.InstantiatePrefab(plantPrefab) as GameObject;
                if (visual != null)
                {
                    visual.name = "External_CarnivorousPlant_Visual";
                    visual.transform.SetParent(enemyRoot.transform, false);
                    visual.transform.localPosition = Vector3.zero;
                    FitVisualToHeight(visual, 2.1f);
                }
            }
            else
            {
                Material fallbackMaterial = GetOrCreateColorMaterial("Enemy_Red.mat", new Color(0.76f, 0.12f, 0.1f, 1f));
                GameObject fallback = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                fallback.name = "Fallback_Red_Enemy_Visual";
                fallback.transform.SetParent(enemyRoot.transform, false);
                fallback.transform.localPosition = new Vector3(0f, 1.05f, 0f);
                fallback.transform.localScale = new Vector3(1f, 1.1f, 1f);
                fallback.GetComponent<Renderer>().sharedMaterial = fallbackMaterial;
                Object.DestroyImmediate(fallback.GetComponent<Collider>());
            }

            enemyRoot.SetActive(false);
            return enemy;
        }

        private static void CreateInitialEnemies(Transform parent, MuckholdEnemy enemyTemplate, Transform target)
        {
            Vector3[] positions =
            {
                new Vector3(18f, 0.05f, 15f),
                new Vector3(-20f, 0.05f, 18f),
                new Vector3(22f, 0.05f, -14f)
            };

            for (int i = 0; i < positions.Length; i++)
            {
                MuckholdEnemy enemy = Object.Instantiate(enemyTemplate, positions[i], Quaternion.identity, parent);
                enemy.name = $"Enemy_CarnivorousPlant_{i + 1:00}";
                enemy.SetTarget(target);
                enemy.gameObject.SetActive(true);
            }
        }

        private static void CreateEnemySpawner(Transform parent, MuckholdEnemy enemyTemplate, Transform target)
        {
            GameObject spawnerObject = new GameObject("Enemy_Spawner_NightWave", typeof(MuckholdEnemySpawner));
            spawnerObject.transform.SetParent(parent, false);
            MuckholdEnemySpawner spawner = spawnerObject.GetComponent<MuckholdEnemySpawner>();
            spawner.SetEnemyTemplate(enemyTemplate);
            spawner.SetTarget(target);
            EditorUtility.SetDirty(spawner);
        }

        private static MuckholdProjectile CreateProjectileTemplate(Transform parent)
        {
            Material projectileMaterial = GetOrCreateColorMaterial("Projectile_Arrow.mat", new Color(0.68f, 0.56f, 0.38f, 1f));
            GameObject projectileObject = new GameObject(
                "Projectile_Template_Arrow",
                typeof(SphereCollider),
                typeof(Rigidbody),
                typeof(MuckholdProjectile));
            projectileObject.transform.SetParent(parent, false);

            SphereCollider collider = projectileObject.GetComponent<SphereCollider>();
            collider.radius = 0.16f;
            collider.isTrigger = true;

            Rigidbody rigidbody = projectileObject.GetComponent<Rigidbody>();
            rigidbody.isKinematic = true;
            rigidbody.useGravity = false;

            CreateArrowVisual(projectileObject.transform, projectileMaterial);

            MuckholdProjectile projectile = projectileObject.GetComponent<MuckholdProjectile>();
            projectileObject.SetActive(false);
            return projectile;
        }

        private static void CreateArrowVisual(Transform parent, Material projectileMaterial)
        {
            GameObject arrowModel = AssetDatabase.LoadAssetAtPath<GameObject>(ArrowModelPath);
            if (arrowModel != null)
            {
                GameObject visual = PrefabUtility.InstantiatePrefab(arrowModel) as GameObject;
                if (visual != null)
                {
                    visual.name = "External_FX_Arrow_Visual";
                    visual.transform.SetParent(parent, false);
                    visual.transform.localPosition = Vector3.zero;
                    visual.transform.localRotation = Quaternion.identity;
                    ApplyMaterialToRenderers(visual, projectileMaterial);
                    DestroyCollidersInChildren(visual);
                    FitVisualToLongestAxis(visual, 0.75f);
                    return;
                }

            }

            CreatePrimitiveArrowVisual(parent, projectileMaterial);
        }

        private static void CreatePrimitiveArrowVisual(Transform parent, Material projectileMaterial)
        {
            GameObject shaft = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            shaft.name = "Fallback_Arrow_Shaft";
            shaft.transform.SetParent(parent, false);
            shaft.transform.localPosition = Vector3.zero;
            shaft.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
            shaft.transform.localScale = new Vector3(0.025f, 0.32f, 0.025f);
            shaft.GetComponent<Renderer>().sharedMaterial = projectileMaterial;
            Object.DestroyImmediate(shaft.GetComponent<Collider>());

            GameObject head = GameObject.CreatePrimitive(PrimitiveType.Cube);
            head.name = "Fallback_Arrow_Head";
            head.transform.SetParent(parent, false);
            head.transform.localPosition = new Vector3(0f, 0f, 0.38f);
            head.transform.localRotation = Quaternion.Euler(0f, 45f, 0f);
            head.transform.localScale = new Vector3(0.12f, 0.12f, 0.12f);
            head.GetComponent<Renderer>().sharedMaterial = projectileMaterial;
            Object.DestroyImmediate(head.GetComponent<Collider>());
        }

        private static MuckholdHudView CreateHud(string title, MuckholdInventory inventory, SceneCameraController sceneCameraController)
        {
            GameObject canvasObject = new GameObject(
                "Canvas_Muckhold_HUD",
                typeof(RectTransform),
                typeof(Canvas),
                typeof(CanvasScaler),
                typeof(GraphicRaycaster),
                typeof(UIManager),
                typeof(MuckholdHudView));
            RectTransform root = canvasObject.GetComponent<RectTransform>();
            Stretch(root);

            Canvas canvas = canvasObject.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 20;

            CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.matchWidthOrHeight = 0.5f;

            UIManager uiManager = canvasObject.GetComponent<UIManager>();
            uiManager.SetCameraController(sceneCameraController);

            Font font = AssetDatabase.LoadAssetAtPath<Font>(FontPath);
            CreateText("HUD_Title", root, title, 28, new Color(0.92f, 0.95f, 0.9f, 1f), TextAnchor.MiddleLeft, FontStyle.Bold, font, new Vector2(0.03f, 0.92f), new Vector2(0.42f, 0.98f));
            Text statusText = CreateText("HUD_Status_Text", root, "Tab/I: Inventory, E: Interact, Left Click: Use quick slot", 20, new Color(0.95f, 0.82f, 0.48f, 1f), TextAnchor.MiddleLeft, FontStyle.Bold, font, new Vector2(0.03f, 0.86f), new Vector2(0.7f, 0.91f));

            Text crosshair = CreateText("HUD_Crosshair", root, "+", 32, Color.white, TextAnchor.MiddleCenter, FontStyle.Bold, font, new Vector2(0.49f, 0.48f), new Vector2(0.51f, 0.52f));
            crosshair.raycastTarget = false;
            Text promptText = CreateText("HUD_Interaction_Prompt", root, string.Empty, 24, Color.white, TextAnchor.MiddleCenter, FontStyle.Bold, font, new Vector2(0.34f, 0.18f), new Vector2(0.66f, 0.24f));

            Image miningBack = CreateImage("HUD_Mining_Back", root, new Color(0f, 0f, 0f, 0.58f), new Vector2(0.36f, 0.13f), new Vector2(0.64f, 0.16f));
            Image miningFill = CreateImage("HUD_Mining_Fill", miningBack.rectTransform, new Color(0.16f, 0.62f, 0.95f, 0.9f), Vector2.zero, Vector2.one);
            miningFill.type = Image.Type.Filled;
            miningFill.fillMethod = Image.FillMethod.Horizontal;
            miningFill.fillAmount = 0f;
            Text miningText = CreateText("HUD_Mining_Text", root, string.Empty, 18, Color.white, TextAnchor.MiddleCenter, FontStyle.Bold, font, new Vector2(0.36f, 0.13f), new Vector2(0.64f, 0.16f));

            MuckholdQuickSlotDropTarget[] quickSlots = CreateQuickSlots(root, font);
            MuckholdInventoryItemDragView[] itemRows = CreateInventoryPanel(root, font, out RectTransform inventoryPanel);
            CreateBackToLobbyButton(root, font);

            MuckholdHudView hudView = canvasObject.GetComponent<MuckholdHudView>();
            hudView.SetViewReferences(promptText, statusText, miningText, miningFill, inventoryPanel.gameObject, itemRows, quickSlots);
            hudView.SetInventory(inventory);
            hudView.SetInventoryOpen(false);
            EditorUtility.SetDirty(hudView);
            return hudView;
        }

        private static MuckholdQuickSlotDropTarget[] CreateQuickSlots(RectTransform root, Font font)
        {
            RectTransform panel = CreateRect("HUD_QuickSlot_Panel", root, new Vector2(0.28f, 0.03f), new Vector2(0.72f, 0.12f));
            MuckholdQuickSlotDropTarget[] quickSlots = new MuckholdQuickSlotDropTarget[MuckholdInventoryModel.QuickSlotCount];

            for (int i = 0; i < quickSlots.Length; i++)
            {
                float width = 1f / quickSlots.Length;
                RectTransform slotRoot = CreateRect($"QuickSlot_{i + 1}", panel, new Vector2(i * width + 0.005f, 0.04f), new Vector2((i + 1) * width - 0.005f, 0.96f));
                Image frame = CreateImage("Frame", slotRoot, new Color(0.08f, 0.12f, 0.14f, 0.9f), Vector2.zero, Vector2.one);
                Text label = CreateText("Label", slotRoot, $"{i + 1}\nEmpty", 17, Color.white, TextAnchor.MiddleCenter, FontStyle.Bold, font, Vector2.zero, Vector2.one);
                MuckholdQuickSlotDropTarget dropTarget = slotRoot.gameObject.AddComponent<MuckholdQuickSlotDropTarget>();
                dropTarget.SetSlotIndex(i);
                dropTarget.SetViewReferences(label, frame);
                quickSlots[i] = dropTarget;
            }

            return quickSlots;
        }

        private static MuckholdInventoryItemDragView[] CreateInventoryPanel(RectTransform root, Font font, out RectTransform panel)
        {
            panel = CreateRect("HUD_Inventory_Panel", root, new Vector2(0.77f, 0.42f), new Vector2(0.97f, 0.86f));
            CreateImage("Inventory_Background", panel, new Color(0.02f, 0.04f, 0.05f, 0.78f), Vector2.zero, Vector2.one);
            CreateText("Inventory_Title", panel, "Inventory / Drag to 1-7", 20, new Color(0.95f, 0.75f, 0.34f, 1f), TextAnchor.MiddleCenter, FontStyle.Bold, font, new Vector2(0.05f, 0.86f), new Vector2(0.95f, 0.98f));

            MuckholdItemId[] itemIds =
            {
                MuckholdItemId.Arrow,
                MuckholdItemId.Pickaxe,
                MuckholdItemId.Stone,
                MuckholdItemId.Ore,
                MuckholdItemId.Apple
            };

            MuckholdInventoryItemDragView[] rows = new MuckholdInventoryItemDragView[itemIds.Length];
            for (int i = 0; i < itemIds.Length; i++)
            {
                float top = 0.8f - (i * 0.14f);
                RectTransform row = CreateRect($"Inventory_Item_{itemIds[i]}", panel, new Vector2(0.07f, top - 0.1f), new Vector2(0.93f, top));
                Image frame = CreateImage("Frame", row, new Color(0.08f, 0.08f, 0.08f, 0.75f), Vector2.zero, Vector2.one);
                Text label = CreateText("Label", row, $"{MuckholdItemCatalog.GetDisplayName(itemIds[i])} x0", 18, Color.white, TextAnchor.MiddleLeft, FontStyle.Bold, font, new Vector2(0.08f, 0f), Vector2.one);
                CanvasGroup canvasGroup = row.gameObject.AddComponent<CanvasGroup>();
                MuckholdInventoryItemDragView dragView = row.gameObject.AddComponent<MuckholdInventoryItemDragView>();
                dragView.SetItem(itemIds[i]);
                dragView.SetDefaultQuickSlotIndex(GetDefaultQuickSlotIndex(itemIds[i]));
                dragView.SetViewReferences(label, frame, canvasGroup);
                rows[i] = dragView;
            }

            CreateText("Inventory_Hint", panel, "Double click: Arrow=1, Pickaxe=2", 15, new Color(0.74f, 0.82f, 0.8f, 1f), TextAnchor.MiddleCenter, FontStyle.Normal, font, new Vector2(0.05f, 0.02f), new Vector2(0.95f, 0.1f));
            return rows;
        }

        private static int GetDefaultQuickSlotIndex(MuckholdItemId itemId)
        {
            switch (itemId)
            {
                case MuckholdItemId.Arrow:
                    return 0;
                case MuckholdItemId.Pickaxe:
                    return 1;
                default:
                    return 0;
            }
        }

        private static void CreateBackToLobbyButton(RectTransform root, Font font)
        {
            RectTransform buttonRoot = CreateRect("Back_To_Lobby_Button", root, new Vector2(0.84f, 0.91f), new Vector2(0.97f, 0.97f));
            Image image = buttonRoot.gameObject.AddComponent<Image>();
            image.color = new Color(0.9f, 0.62f, 0.16f, 0.94f);
            Button button = buttonRoot.gameObject.AddComponent<Button>();
            Text label = CreateText("Label", buttonRoot, "Back To Hub", 18, new Color(0.06f, 0.06f, 0.06f, 1f), TextAnchor.MiddleCenter, FontStyle.Bold, font, Vector2.zero, Vector2.one);
            label.raycastTarget = false;

            CharacterSceneLoadButton loader = buttonRoot.gameObject.AddComponent<CharacterSceneLoadButton>();
            loader.SetSceneName("Main_Lobby");
            UnityEventTools.AddPersistentListener(button.onClick, loader.LoadScene);
        }

        private static void CreateEventSystem()
        {
            GameObject eventSystem = new GameObject("EventSystem", typeof(EventSystem));
#if ENABLE_INPUT_SYSTEM
            eventSystem.AddComponent<InputSystemUIInputModule>();
#else
            eventSystem.AddComponent<StandaloneInputModule>();
#endif
        }

        private static RectTransform CreateRect(string name, RectTransform parent, Vector2 anchorMin, Vector2 anchorMax)
        {
            RectTransform rect = CreateRect(name, parent);
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            return rect;
        }

        private static RectTransform CreateRect(string name, RectTransform parent)
        {
            GameObject gameObject = new GameObject(name, typeof(RectTransform));
            gameObject.transform.SetParent(parent, false);
            return gameObject.GetComponent<RectTransform>();
        }

        private static Image CreateImage(string name, RectTransform parent, Color color, Vector2 anchorMin, Vector2 anchorMax)
        {
            GameObject imageObject = new GameObject(name, typeof(RectTransform), typeof(Image));
            imageObject.transform.SetParent(parent, false);
            Image image = imageObject.GetComponent<Image>();
            image.color = color;
            image.rectTransform.anchorMin = anchorMin;
            image.rectTransform.anchorMax = anchorMax;
            image.rectTransform.offsetMin = Vector2.zero;
            image.rectTransform.offsetMax = Vector2.zero;
            return image;
        }

        private static Text CreateText(
            string name,
            RectTransform parent,
            string value,
            int size,
            Color color,
            TextAnchor alignment,
            FontStyle style,
            Font font,
            Vector2 anchorMin,
            Vector2 anchorMax)
        {
            GameObject textObject = new GameObject(name, typeof(RectTransform), typeof(Text));
            textObject.transform.SetParent(parent, false);
            Text text = textObject.GetComponent<Text>();
            text.text = value;
            text.font = font != null ? font : Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.fontSize = size;
            text.color = color;
            text.alignment = alignment;
            text.fontStyle = style;
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Truncate;
            text.rectTransform.anchorMin = anchorMin;
            text.rectTransform.anchorMax = anchorMax;
            text.rectTransform.offsetMin = Vector2.zero;
            text.rectTransform.offsetMax = Vector2.zero;
            return text;
        }

        private static void Stretch(RectTransform rectTransform)
        {
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;
        }

        private static Material GetOrCreateSkyboxMaterial()
        {
            string path = $"{GeneratedMaterialFolder}/Muckhold_Procedural_Skybox.mat";
            Material material = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (material == null)
            {
                Shader shader = Shader.Find("Skybox/Procedural");
                if (shader == null)
                {
                    return null;
                }

                material = new Material(shader);
                AssetDatabase.CreateAsset(material, path);
            }

            SetMaterialColor(material, "_SkyTint", new Color(0.54f, 0.68f, 0.82f, 1f));
            if (material.HasProperty("_GroundColor"))
            {
                material.SetColor("_GroundColor", new Color(0.33f, 0.48f, 0.36f, 1f));
            }

            if (material.HasProperty("_Exposure"))
            {
                material.SetFloat("_Exposure", 1.08f);
            }

            if (material.HasProperty("_SunSize"))
            {
                material.SetFloat("_SunSize", 0.045f);
            }

            EditorUtility.SetDirty(material);
            return material;
        }

        private static Material GetOrCreateColorMaterial(string fileName, Color color)
        {
            string path = $"{GeneratedMaterialFolder}/{fileName}";
            Material material = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (material == null)
            {
                Shader shader = Shader.Find("Universal Render Pipeline/Lit");
                if (shader == null)
                {
                    shader = Shader.Find("Standard");
                }

                material = new Material(shader);
                AssetDatabase.CreateAsset(material, path);
            }

            SetMaterialColor(material, color);
            EditorUtility.SetDirty(material);
            return material;
        }

        private static Material GetOrCreateTextureMaterial(string fileName, Texture2D texture)
        {
            string path = $"{GeneratedMaterialFolder}/{fileName}";
            Material material = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (material == null)
            {
                Shader shader = Shader.Find("Unlit/Texture");
                if (shader == null)
                {
                    shader = Shader.Find("Universal Render Pipeline/Unlit");
                }

                if (shader == null)
                {
                    shader = Shader.Find("Standard");
                }

                material = new Material(shader);
                AssetDatabase.CreateAsset(material, path);
            }

            if (material.HasProperty("_MainTex"))
            {
                material.SetTexture("_MainTex", texture);
            }

            if (material.HasProperty("_BaseMap"))
            {
                material.SetTexture("_BaseMap", texture);
            }

            EditorUtility.SetDirty(material);
            return material;
        }

        private static void SetMaterialColor(Material material, Color color)
        {
            SetMaterialColor(material, "_BaseColor", color);
            SetMaterialColor(material, "_Color", color);
        }

        private static void SetMaterialColor(Material material, string propertyName, Color color)
        {
            if (material != null && material.HasProperty(propertyName))
            {
                material.SetColor(propertyName, color);
            }
        }

        private static void ApplyMaterialToRenderers(GameObject visual, Material material)
        {
            if (visual == null || material == null)
            {
                return;
            }

            Renderer[] renderers = visual.GetComponentsInChildren<Renderer>();
            for (int i = 0; i < renderers.Length; i++)
            {
                renderers[i].sharedMaterial = material;
            }
        }

        private static void DestroyCollidersInChildren(GameObject visual)
        {
            if (visual == null)
            {
                return;
            }

            Collider[] colliders = visual.GetComponentsInChildren<Collider>();
            for (int i = colliders.Length - 1; i >= 0; i--)
            {
                Object.DestroyImmediate(colliders[i]);
            }
        }

        private static void FitVisualToHeight(GameObject visual, float targetHeight)
        {
            Renderer[] renderers = visual.GetComponentsInChildren<Renderer>();
            if (renderers.Length == 0)
            {
                return;
            }

            Bounds bounds = renderers[0].bounds;
            for (int i = 1; i < renderers.Length; i++)
            {
                bounds.Encapsulate(renderers[i].bounds);
            }

            if (bounds.size.y <= 0.01f)
            {
                return;
            }

            float scale = targetHeight / bounds.size.y;
            visual.transform.localScale *= scale;
        }

        private static void FitVisualToLongestAxis(GameObject visual, float targetSize)
        {
            Renderer[] renderers = visual.GetComponentsInChildren<Renderer>();
            if (renderers.Length == 0)
            {
                return;
            }

            Bounds bounds = renderers[0].bounds;
            for (int i = 1; i < renderers.Length; i++)
            {
                bounds.Encapsulate(renderers[i].bounds);
            }

            float longestAxis = Mathf.Max(bounds.size.x, bounds.size.y, bounds.size.z);
            if (longestAxis <= 0.01f)
            {
                return;
            }

            float scale = targetSize / longestAxis;
            visual.transform.localScale *= scale;
        }

        private static void EnsureFolders()
        {
            if (!AssetDatabase.IsValidFolder("Assets/MainAssets"))
            {
                AssetDatabase.CreateFolder("Assets", "MainAssets");
            }

            if (!AssetDatabase.IsValidFolder("Assets/MainAssets/Muckhold"))
            {
                AssetDatabase.CreateFolder("Assets/MainAssets", "Muckhold");
            }

            if (!AssetDatabase.IsValidFolder(GeneratedMaterialFolder))
            {
                AssetDatabase.CreateFolder("Assets/MainAssets/Muckhold", "GeneratedMaterials");
            }

            if (!AssetDatabase.IsValidFolder("Assets/Scenes"))
            {
                AssetDatabase.CreateFolder("Assets", "Scenes");
            }

            if (!AssetDatabase.IsValidFolder("Assets/Scenes/MainWorkspace"))
            {
                AssetDatabase.CreateFolder("Assets/Scenes", "MainWorkspace");
            }
        }

        private static void UpdateBuildSettings()
        {
            List<EditorBuildSettingsScene> scenes = new List<EditorBuildSettingsScene>();
            AddBuildScene(scenes, MainLobbyScenePath);
            AddBuildScene(scenes, "Assets/Scenes/CharacterScenes/SampleScene.unity");
            AddBuildScene(scenes, SharedWorkspaceScenePath);
            AddBuildScene(scenes, BuildWorkspaceScenePath);

            EditorBuildSettingsScene[] currentScenes = EditorBuildSettings.scenes;
            for (int i = 0; i < currentScenes.Length; i++)
            {
                AddBuildScene(scenes, currentScenes[i].path);
            }

            EditorBuildSettings.scenes = scenes.ToArray();
        }

        private static void AddBuildScene(List<EditorBuildSettingsScene> scenes, string scenePath)
        {
            if (string.IsNullOrWhiteSpace(scenePath))
            {
                return;
            }

            for (int i = 0; i < scenes.Count; i++)
            {
                if (scenes[i].path == scenePath)
                {
                    return;
                }
            }

            scenes.Add(new EditorBuildSettingsScene(scenePath, true));
        }

        private static void ValidatePlayableScene(string scenePath)
        {
            EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
            if (Object.FindFirstObjectByType<MuckholdFirstPersonPlayer>() == null)
            {
                throw new System.InvalidOperationException($"{scenePath} has no first person player.");
            }

            if (Object.FindFirstObjectByType<MuckholdHudView>() == null)
            {
                throw new System.InvalidOperationException($"{scenePath} has no HUD view.");
            }

            if (Object.FindFirstObjectByType<MuckholdEnemySpawner>() == null)
            {
                throw new System.InvalidOperationException($"{scenePath} has no enemy spawner.");
            }

            if (GameObject.Find("Sun_Directional_Light") == null)
            {
                throw new System.InvalidOperationException($"{scenePath} has no mood directional light.");
            }

            if (GameObject.Find("Muckhold_Green_Field") == null)
            {
                throw new System.InvalidOperationException($"{scenePath} has no green field.");
            }

            if (!HasSceneGameObject("Projectile_Template_Arrow"))
            {
                throw new System.InvalidOperationException($"{scenePath} has no arrow projectile template.");
            }
        }

        private static bool HasSceneGameObject(string objectName)
        {
            GameObject[] gameObjects = Resources.FindObjectsOfTypeAll<GameObject>();
            for (int i = 0; i < gameObjects.Length; i++)
            {
                GameObject gameObject = gameObjects[i];
                if (gameObject.name == objectName && gameObject.scene.IsValid())
                {
                    return true;
                }
            }

            return false;
        }
    }
}
