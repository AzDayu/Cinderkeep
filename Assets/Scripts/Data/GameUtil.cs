using UnityEngine;

namespace Cinderkeep.Gameplay
{
    // 공용 계산과 데이터 로드 시작점을 모아두는 순수 도구 클래스입니다.
    // 가능하면 씬 오브젝트를 직접 잡지 않고, 필요한 값만 파라미터로 받습니다.
    public static class GameUtil
    {
        public const string EnemyDataResourcePath = "Cinderkeep/data/enemies";

        public static void LoadFullData(GameDataManager gameDataManager)
        {
            if (gameDataManager == null)
            {
                Debug.LogWarning("GameUtil: GameDataManager reference is empty.");
                return;
            }

            gameDataManager.LoadEnemyData(EnemyDataResourcePath);
        }

        public static int GenerateNextInstanceId(int currentInstanceId)
        {
            return currentInstanceId + 1;
        }

        public static float GetRate(float currentValue, float maxValue)
        {
            if (maxValue <= 0f)
            {
                return 0f;
            }

            return Mathf.Clamp01(currentValue / maxValue);
        }
    }
}
