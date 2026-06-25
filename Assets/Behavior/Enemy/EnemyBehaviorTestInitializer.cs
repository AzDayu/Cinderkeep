//테스트용 초기화 코드

using System.Collections;
using Cinderkeep.Gameplay;
using UnityEngine;


public sealed class EnemyBehaviorTestInitializer : MonoBehaviour
{
    [Header("Test Enemy Data")]
    [Tooltip("enemies.json의 _id입니다. 기본 테스트는 ice_zombie를 사용합니다.")]
    [SerializeField] private string _enemyDataId = "ice_zombie";

    [Header("Behavior Test")]
    [Tooltip("테스트 중 기존 EnemyBrain이 이동 명령을 중복으로 내리지 않도록 끕니다.")]
    [SerializeField] private bool _disableEnemyBrain = true;

    private IEnumerator Start()
    {
        yield return null;

        GameDataManager gameDataManager = GetGameDataManager();
        if (gameDataManager == null)
        {
            Debug.LogWarning("EnemyBehaviorTestInitializer: GameDataManager를 찾지 못했습니다.");
            yield break;
        }

        gameDataManager.Initialize();

        EnemyData enemyData = gameDataManager.GetEnemy(_enemyDataId);
        if (enemyData == null)
        {
            Debug.LogWarning("EnemyBehaviorTestInitializer: EnemyData를 찾지 못했습니다. id=" + _enemyDataId);
            yield break;
        }

        InitializeEnemyComponents(enemyData);
    }

    private GameDataManager GetGameDataManager()
    {
        if (GameManager.Inst != null)
        {
            GameDataManager managerFromGameManager = GameManager.Inst.GetGameDataManager();
            if (managerFromGameManager != null)
            {
                return managerFromGameManager;
            }
        }

        return FindFirstObjectByType<GameDataManager>();
    }

    private void InitializeEnemyComponents(EnemyData enemyData)
    {
        EnemyStatus enemyStatus = GetComponent<EnemyStatus>();
        EnemyDetector enemyDetector = GetComponent<EnemyDetector>();
        EnemyAttack enemyAttack = GetComponent<EnemyAttack>();
        EnemyMovement enemyMovement = GetComponent<EnemyMovement>();
        EnemyBrain enemyBrain = GetComponent<EnemyBrain>();

        if (enemyStatus != null)
        {
            enemyStatus.Initialize(enemyData);
        }

        if (enemyDetector != null)
        {
            enemyDetector.Initialize(enemyData);
        }

        if (enemyAttack != null)
        {
            enemyAttack.Initialize(enemyData);
        }

        if (enemyMovement != null)
        {
            enemyMovement.Initialize(enemyData, enemyDetector);
        }
        else
        {
            Debug.LogWarning("EnemyBehaviorTestInitializer: EnemyMovement가 없습니다. object=" + gameObject.name);
        }

        if (_disableEnemyBrain && enemyBrain != null)
        {
            enemyBrain.enabled = false;
        }

        Debug.Log("EnemyBehaviorTestInitializer: Enemy 초기화 완료. id=" + enemyData.Id + ", object=" + gameObject.name);
    }
}