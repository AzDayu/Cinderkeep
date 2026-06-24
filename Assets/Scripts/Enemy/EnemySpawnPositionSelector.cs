using UnityEngine;

// 5.00 direction: Supports enemy spawning, sensing, movement, attack, or boss-clear behavior for the 5.00 loop.
// 5.01+ note: Keep AI decisions separated from movement, detection, and attack so 5.01+ behavior can grow safely.
// 적이 실제로 생성될 위치와 회전을 계산하는 클래스입니다.
// EnemySpawnPoint는 스폰 지점 설정을 보관하고, 위치 계산은 이 클래스에 맡깁니다.
public sealed class EnemySpawnPositionSelector
{
    private const float GoldenAngle = 137.50776f;
    private const float MinimumSpawnSpacing = 0.8f;

    public Vector3 GetSpawnPosition(
        Transform centerTransform,
        Transform[] candidatePoints,
        float spawnSpacing,
        int index,
        int totalCount)
    {
        Vector3 spawnPosition = GetSpawnCenterPosition(centerTransform, candidatePoints);
        spawnPosition += GetSpreadOffset(spawnSpacing, index, totalCount);
        return spawnPosition;
    }

    public Quaternion GetSpawnRotation(Transform centerTransform)
    {
        if (centerTransform == null)
        {
            return Quaternion.identity;
        }

        return centerTransform.rotation;
    }

    public Vector3 GetGizmoPosition(Vector3 centerPosition, float spawnSpacing, int index, int totalCount)
    {
        Vector3 position = centerPosition;
        position += GetSpreadOffset(spawnSpacing, index, totalCount);
        return position;
    }

    private Vector3 GetSpreadOffset(float spawnSpacing, int index, int totalCount)
    {
        if (totalCount <= 1)
        {
            return Vector3.zero;
        }

        float safeSpacing = Mathf.Max(MinimumSpawnSpacing, spawnSpacing);
        float radius = safeSpacing * Mathf.Sqrt(index + 1);
        float angle = index * GoldenAngle * Mathf.Deg2Rad;
        return new Vector3(Mathf.Cos(angle) * radius, 0f, Mathf.Sin(angle) * radius);
    }

    private Vector3 GetSpawnCenterPosition(Transform centerTransform, Transform[] candidatePoints)
    {
        Transform candidatePoint = GetRandomCandidatePoint(candidatePoints);
        if (candidatePoint != null)
        {
            return candidatePoint.position;
        }

        if (centerTransform == null)
        {
            return Vector3.zero;
        }

        return centerTransform.position;
    }

    private Transform GetRandomCandidatePoint(Transform[] candidatePoints)
    {
        if (candidatePoints == null)
        {
            return null;
        }

        if (candidatePoints.Length == 0)
        {
            return null;
        }

        int randomStartIndex = Random.Range(0, candidatePoints.Length);
        for (int i = 0; i < candidatePoints.Length; i++)
        {
            int candidateIndex = (randomStartIndex + i) % candidatePoints.Length;
            Transform candidatePoint = candidatePoints[candidateIndex];
            if (candidatePoint != null)
            {
                return candidatePoint;
            }
        }

        return null;
    }
}
