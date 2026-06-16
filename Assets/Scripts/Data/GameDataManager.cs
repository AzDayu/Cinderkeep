using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class GameDataManager : MonoBehaviour
{
    public static GameDataManager Instance { get; set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.Log($"중복된 GameDataManager가 발견되어 파괴합니다: {gameObject.name}");
            Destroy(gameObject);
            return;
        }
        GameUtil.LoadFullData();
        

    }

    [Serializable]
    private class SerializationWrapper<T>
    {
        public List<T> Items;
    }

    public Dictionary<string, EnemyData> EnemyDataList { get; private set; } = new Dictionary<string, EnemyData>();

    private Dictionary<string, T> LoadData<T>(string resourcePath) where T : GameDataBase
    {
        TextAsset jsonAsset = Resources.Load<TextAsset>(resourcePath);
        if (jsonAsset == null)
        {
            Debug.LogError($"[Error] 파일을 찾을 수 없습니다: Resources/{resourcePath}.json 파일이 존재하는지 확인하세요.");
            return new Dictionary<string, T>();
        }

        try
        {
            string jsonString = jsonAsset.text;

            //string wrappedJson = "{\"items\":" + jsonString + "}";
            //SerializationWrapper<T> wrapper = JsonUtility.FromJson<SerializationWrapper<T>>(wrappedJson);
            SerializationWrapper<T> wrapper = JsonUtility.FromJson<SerializationWrapper<T>>(jsonString);
            if (wrapper != null && wrapper.Items != null)
            {
                Debug.Log($"{typeof(T).Name} 데이터를 {wrapper.Items.Count}개 로드했습니다.");
                return wrapper.Items.ToDictionary(item => item._id);
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"[{typeof(T).Name} JSON 로드 오류] {ex.Message}");
        }

        return new Dictionary<string, T>();
    }


    public void LoadEnemyData(string resourcePath)
    {
        EnemyDataList = LoadData<EnemyData>(resourcePath);
    }


   public EnemyData GetEnemy(string id)
    {
        if (EnemyDataList == null || string.IsNullOrEmpty(id))
        {
            return null;
        }

        return EnemyDataList.TryGetValue(id,out var data) ? data : null;
    }


}