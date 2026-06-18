using UnityEngine;

public class AttackHitbox : MonoBehaviour
{
    private void OnEnable()
    {
        if (!TryGetComponent<DamageDealer>(out var _dealer))
        {
            Debug.LogWarning($"{this.gameObject.name}에 DamageDealer 컴포넌트가 없습니다!");
        }
        _dealer = GetComponent<DamageDealer>();
    }
}
