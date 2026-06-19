using System.Collections.Generic;
using UnityEngine;

namespace Cinderkeep.Gameplay
{
    // 변하지 않는 기획 데이터를 JSON에서 읽어 보관하는 매니저입니다.
    // 적, 도구, 무기, 방어구, 제작법처럼 엑셀에서 JSON으로 옮길 데이터를 이곳에서 관리합니다.
    public sealed class GameDataManager : MonoBehaviour, IGameInitializable
    {
        [SerializeField] private string _enemyDataResourcePath = GameUtil.EnemyDataResourcePath;
        [SerializeField] private string _toolDataResourcePath = GameUtil.ToolDataResourcePath;
        [SerializeField] private string _weaponDataResourcePath = GameUtil.WeaponDataResourcePath;
        [SerializeField] private string _armorDataResourcePath = GameUtil.ArmorDataResourcePath;
        [SerializeField] private string _buildingDataResourcePath = GameUtil.BuildingDataResourcePath;
        [SerializeField] private string _craftingRecipeDataResourcePath = GameUtil.CraftingRecipeDataResourcePath;

        private readonly Dictionary<string, EnemyData> _enemyDataList = new Dictionary<string, EnemyData>();
        private readonly Dictionary<string, ToolData> _toolDataList = new Dictionary<string, ToolData>();
        private readonly Dictionary<string, WeaponData> _weaponDataList = new Dictionary<string, WeaponData>();
        private readonly Dictionary<string, ArmorData> _armorDataList = new Dictionary<string, ArmorData>();
        private readonly Dictionary<string, BuildingData> _buildingDataList = new Dictionary<string, BuildingData>();
        private readonly Dictionary<string, CraftingRecipeData> _craftingRecipeDataList = new Dictionary<string, CraftingRecipeData>();
        private bool _isInitialized;

        public IReadOnlyDictionary<string, EnemyData> EnemyDataList
        {
            get
            {
                return _enemyDataList;
            }
        }

        public bool IsInitialized
        {
            get
            {
                return _isInitialized;
            }
        }

        public IReadOnlyDictionary<string, ToolData> ToolDataList
        {
            get
            {
                return _toolDataList;
            }
        }

        public IReadOnlyDictionary<string, WeaponData> WeaponDataList
        {
            get
            {
                return _weaponDataList;
            }
        }

        public IReadOnlyDictionary<string, ArmorData> ArmorDataList
        {
            get
            {
                return _armorDataList;
            }
        }

        public IReadOnlyDictionary<string, BuildingData> BuildingDataList
        {
            get
            {
                return _buildingDataList;
            }
        }

        public IReadOnlyDictionary<string, CraftingRecipeData> CraftingRecipeDataList
        {
            get
            {
                return _craftingRecipeDataList;
            }
        }

        public void Initialize()
        {
            if (_isInitialized)
            {
                return;
            }

            GameUtil.LoadFullData(this);
            _isInitialized = true;
        }

        public void LoadEnemyData(string resourcePath)
        {
            _enemyDataList.Clear();

            TextAsset jsonAsset = Resources.Load<TextAsset>(resourcePath);
            if (jsonAsset == null)
            {
                Debug.LogWarning("GameDataManager: enemy JSON was not found at Resources/" + resourcePath + ".json");
                return;
            }

            EnemyDataCatalog catalog = JsonUtility.FromJson<EnemyDataCatalog>(jsonAsset.text);
            if (catalog == null || catalog.Items == null)
            {
                Debug.LogWarning("GameDataManager: enemy JSON format is invalid.");
                return;
            }

            for (int i = 0; i < catalog.Items.Count; i++)
            {
                AddData(_enemyDataList, catalog.Items[i]);
            }
        }

        public EnemyData GetEnemy(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return null;
            }

            if (!_enemyDataList.ContainsKey(id))
            {
                return null;
            }

            return _enemyDataList[id];
        }

        public void LoadToolData(string resourcePath)
        {
            _toolDataList.Clear();

            TextAsset jsonAsset = Resources.Load<TextAsset>(resourcePath);
            if (jsonAsset == null)
            {
                Debug.LogWarning("GameDataManager: tool JSON was not found at Resources/" + resourcePath + ".json");
                return;
            }

            ToolDataCatalog catalog = JsonUtility.FromJson<ToolDataCatalog>(jsonAsset.text);
            if (catalog == null || catalog.Items == null)
            {
                Debug.LogWarning("GameDataManager: tool JSON format is invalid.");
                return;
            }

            for (int i = 0; i < catalog.Items.Count; i++)
            {
                AddData(_toolDataList, catalog.Items[i]);
            }
        }

        public ToolData GetTool(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return null;
            }

            if (!_toolDataList.ContainsKey(id))
            {
                return null;
            }

            return _toolDataList[id];
        }

        public void LoadWeaponData(string resourcePath)
        {
            _weaponDataList.Clear();

            TextAsset jsonAsset = Resources.Load<TextAsset>(resourcePath);
            if (jsonAsset == null)
            {
                Debug.LogWarning("GameDataManager: weapon JSON was not found at Resources/" + resourcePath + ".json");
                return;
            }

            WeaponDataCatalog catalog = JsonUtility.FromJson<WeaponDataCatalog>(jsonAsset.text);
            if (catalog == null || catalog.Items == null)
            {
                Debug.LogWarning("GameDataManager: weapon JSON format is invalid.");
                return;
            }

            for (int i = 0; i < catalog.Items.Count; i++)
            {
                AddData(_weaponDataList, catalog.Items[i]);
            }
        }

        public WeaponData GetWeapon(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return null;
            }

            if (!_weaponDataList.ContainsKey(id))
            {
                return null;
            }

            return _weaponDataList[id];
        }

        public void LoadArmorData(string resourcePath)
        {
            _armorDataList.Clear();

            TextAsset jsonAsset = Resources.Load<TextAsset>(resourcePath);
            if (jsonAsset == null)
            {
                Debug.LogWarning("GameDataManager: armor JSON was not found at Resources/" + resourcePath + ".json");
                return;
            }

            ArmorDataCatalog catalog = JsonUtility.FromJson<ArmorDataCatalog>(jsonAsset.text);
            if (catalog == null || catalog.Items == null)
            {
                Debug.LogWarning("GameDataManager: armor JSON format is invalid.");
                return;
            }

            for (int i = 0; i < catalog.Items.Count; i++)
            {
                AddData(_armorDataList, catalog.Items[i]);
            }
        }

        public ArmorData GetArmor(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return null;
            }

            if (!_armorDataList.ContainsKey(id))
            {
                return null;
            }

            return _armorDataList[id];
        }

        public void LoadBuildingData(string resourcePath)
        {
            _buildingDataList.Clear();

            TextAsset jsonAsset = Resources.Load<TextAsset>(resourcePath);
            if (jsonAsset == null)
            {
                Debug.LogWarning("GameDataManager: building JSON was not found at Resources/" + resourcePath + ".json");
                return;
            }

            BuildingDataCatalog catalog = JsonUtility.FromJson<BuildingDataCatalog>(jsonAsset.text);
            if (catalog == null || catalog.Items == null)
            {
                Debug.LogWarning("GameDataManager: building JSON format is invalid.");
                return;
            }

            for (int i = 0; i < catalog.Items.Count; i++)
            {
                AddData(_buildingDataList, catalog.Items[i]);
            }
        }

        public BuildingData GetBuilding(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return null;
            }

            if (!_buildingDataList.ContainsKey(id))
            {
                return null;
            }

            return _buildingDataList[id];
        }

        public void LoadCraftingRecipeData(string resourcePath)
        {
            _craftingRecipeDataList.Clear();

            TextAsset jsonAsset = Resources.Load<TextAsset>(resourcePath);
            if (jsonAsset == null)
            {
                Debug.LogWarning("GameDataManager: crafting recipe JSON was not found at Resources/" + resourcePath + ".json");
                return;
            }

            CraftingRecipeDataCatalog catalog = JsonUtility.FromJson<CraftingRecipeDataCatalog>(jsonAsset.text);
            if (catalog == null || catalog.Items == null)
            {
                Debug.LogWarning("GameDataManager: crafting recipe JSON format is invalid.");
                return;
            }

            for (int i = 0; i < catalog.Items.Count; i++)
            {
                AddData(_craftingRecipeDataList, catalog.Items[i]);
            }
        }

        public CraftingRecipeData GetCraftingRecipe(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return null;
            }

            if (!_craftingRecipeDataList.ContainsKey(id))
            {
                return null;
            }

            return _craftingRecipeDataList[id];
        }

        public string GetEnemyDataResourcePath()
        {
            return _enemyDataResourcePath;
        }

        public string GetToolDataResourcePath()
        {
            return _toolDataResourcePath;
        }

        public string GetWeaponDataResourcePath()
        {
            return _weaponDataResourcePath;
        }

        public string GetArmorDataResourcePath()
        {
            return _armorDataResourcePath;
        }

        public string GetBuildingDataResourcePath()
        {
            return _buildingDataResourcePath;
        }

        public string GetCraftingRecipeDataResourcePath()
        {
            return _craftingRecipeDataResourcePath;
        }

        private void AddData<TData>(Dictionary<string, TData> target, TData data)
            where TData : GameDataBase
        {
            if (target == null || data == null)
            {
                return;
            }

            if (string.IsNullOrEmpty(data.Id))
            {
                return;
            }

            if (target.ContainsKey(data.Id))
            {
                target[data.Id] = data;
                return;
            }

            target.Add(data.Id, data);
        }
    }
}
