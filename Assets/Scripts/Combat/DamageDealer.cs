using UnityEngine;

public class DamageDealer : MonoBehaviour
{
    private int test_damage;

    private void Awake()
    {
        test_damage = 40;   // 임시 데미지
    }

    public int GetDamageValue()
    {
        return test_damage;
    }
}
