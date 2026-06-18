using UnityEngine;
using UnityEngine.Serialization;

namespace Cinderkeep.Gameplay
{
    // Enemy JSON 연결을 빠르게 확인하기 위한 테스트 컴포넌트입니다.
    // 실제 게임 로직이 아니라 검수용이므로, 필요한 오브젝트에만 직접 붙여서 사용합니다.
    public sealed class GameDataTester : MonoBehaviour
    {
        [FormerlySerializedAs("GameDataManager_GameDataManager")]
        [SerializeField] private GameDataManager _gameDataManager;
        [SerializeField] private string _enemyId = "ice_zombie";

        public void StartDataTest()
        {
            if (_gameDataManager == null)
            {
                Debug.LogWarning("GameDataTester: GameDataManager reference is empty.");
                return;
            }

            _gameDataManager.Initialize();

            EnemyData enemyData = _gameDataManager.GetEnemy(_enemyId);
            if (enemyData == null)
            {
                Debug.LogWarning("GameDataTester: enemy data was not found. id: " + _enemyId);
                return;
            }

            Debug.Log("Enemy Data Loaded: " + enemyData.Id + " / " + enemyData.DisplayName);
        }
    }
}
