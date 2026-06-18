using UnityEngine;

// CinderHeart 오브젝트에 붙이는 표식 컴포넌트입니다.
// 몬스터 이동 목표 연결은 EnemyMovement.SetCinderHeartTarget() 또는 Inspector에서 명확히 처리합니다.
public sealed class CinderHeart : MonoBehaviour
{
    public Transform GetTargetTransform()
    {
        return transform;
    }
}
