using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
 
public static class GameDataTester
{
    public static void StartDataTest()
    {
        GameUtil.LoadFullData();
        foreach(var Enemy in GameDataManager.Instance.EnemyDataList)
        {
            string key = Enemy.Key;
            var data = Enemy.Value;
            Debug.Log($"키: {key}. 데이터의 이름: {data._displayName}");
        }
    }
}
