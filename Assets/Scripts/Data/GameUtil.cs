using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class GameUtil
{
    public static void LoadFullData()
    {
        GameDataManager.Instance.LoadEnemyData("Cinderkeep/Data/enemies");
    }

    public static string GetFullDataPath(string dataTableName)
    {
        if (string.IsNullOrEmpty(dataTableName))
        {
            Debug.Log("테이블 이름이 올바르지 않습니다!");
            return string.Empty;
        }

        
        string relativePath = $"Assets/Resources/JsonOutput/{dataTableName}.json";
        string fullPath = Path.GetFullPath(relativePath);
        return fullPath;
    }
}