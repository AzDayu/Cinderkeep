using UnityEngine;
using UnityEngine.AI;

// 보스 프리팹이 없을 때 3일차 루프가 끊기지 않도록 임시 보스 오브젝트를 만듭니다.
// 스폰 포인트는 생성된 오브젝트에 데이터, 모드, 사망 이벤트를 이어 붙이는 역할만 유지합니다.
public static class EnemyRuntimeBossFactory
{
    public static GameObject Create(Vector3 spawnPosition, Quaternion spawnRotation)
    {
        GameObject createdEnemy = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        createdEnemy.SetActive(false);
        createdEnemy.transform.SetPositionAndRotation(spawnPosition, spawnRotation);

        EnsureComponents(createdEnemy);
        ApplyMaterial(createdEnemy);
        return createdEnemy;
    }

    private static void EnsureComponents(GameObject createdEnemy)
    {
        GetOrAddComponent<Rigidbody>(createdEnemy);
        GetOrAddComponent<NavMeshAgent>(createdEnemy);
        GetOrAddComponent<Damageable>(createdEnemy);
        GetOrAddComponent<EnemyStatus>(createdEnemy);
        GetOrAddComponent<EnemyAttack>(createdEnemy);
        GetOrAddComponent<EnemyDetector>(createdEnemy);
        GetOrAddComponent<EnemyMovement>(createdEnemy);
        GetOrAddComponent<EnemyBrain>(createdEnemy);
    }

    private static TComponent GetOrAddComponent<TComponent>(GameObject targetObject)
        where TComponent : Component
    {
        TComponent component = targetObject.GetComponent<TComponent>();
        if (component != null)
        {
            return component;
        }

        return targetObject.AddComponent<TComponent>();
    }

    private static void ApplyMaterial(GameObject createdEnemy)
    {
        Renderer renderer = createdEnemy.GetComponentInChildren<Renderer>();
        if (renderer == null)
        {
            return;
        }

        Shader shader = Shader.Find("Standard");
        if (shader == null)
        {
            return;
        }

        Material material = new Material(shader);
        material.color = new Color(0.45f, 0.85f, 1f, 1f);
        renderer.sharedMaterial = material;
    }
}
