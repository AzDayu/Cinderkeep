using System;
using System.Collections.Generic;
using UnityEngine;

namespace Cinderkeep.Gameplay
{
    // GameDataManager의 반복 JSON 로드 과정을 담당하는 공용 로더입니다.
    // 각 Catalog는 GameDataManager가 소유하고, Resources/JsonUtility/ID 등록 규칙은 여기서 통일합니다.
    public static class GameDataCatalogLoader
    {
        public static void LoadCatalog<TData, TCatalog>(
            Dictionary<string, TData> target,
            string resourcePath,
            string catalogName,
            Func<TCatalog, List<TData>> getItems)
            where TData : GameDataBase
        {
            if (target == null)
            {
                Debug.LogWarning("GameDataCatalogLoader: target dictionary is null for " + catalogName + ".");
                return;
            }

            target.Clear();

            TextAsset jsonAsset = Resources.Load<TextAsset>(resourcePath);
            if (jsonAsset == null)
            {
                Debug.LogWarning("GameDataManager: " + catalogName + " JSON was not found at Resources/" + resourcePath + ".json");
                return;
            }

            TCatalog catalog = JsonUtility.FromJson<TCatalog>(jsonAsset.text);
            List<TData> items = catalog == null || getItems == null ? null : getItems(catalog);
            if (items == null)
            {
                Debug.LogWarning("GameDataManager: " + catalogName + " JSON format is invalid.");
                return;
            }

            for (int i = 0; i < items.Count; i++)
            {
                AddData(target, items[i], catalogName, i);
            }
        }

        private static void AddData<TData>(Dictionary<string, TData> target, TData data, string catalogName, int index)
            where TData : GameDataBase
        {
            if (target == null || data == null)
            {
                return;
            }

            if (string.IsNullOrEmpty(data.Id))
            {
                Debug.LogWarning("GameDataCatalogLoader: empty id skipped. catalog=" + catalogName + ", index=" + index);
                return;
            }

            if (target.ContainsKey(data.Id))
            {
                Debug.LogWarning("GameDataCatalogLoader: duplicate id overwritten. catalog=" + catalogName + ", id=" + data.Id);
                target[data.Id] = data;
                return;
            }

            target.Add(data.Id, data);
        }
    }
}
