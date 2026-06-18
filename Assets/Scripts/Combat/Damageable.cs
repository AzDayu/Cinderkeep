using UnityEngine;

public class Damageable : MonoBehaviour
{
    private float test_maxHealth;   // 임시체력
    private float test_currentHealth;

    private void Awake()
    {
        test_maxHealth = 100;
        test_currentHealth = test_maxHealth;
    }

    public void TakeDamage(int damage)
    {
        test_currentHealth -= damage;

        if (test_currentHealth <= 0){
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Dead");
    }
}
