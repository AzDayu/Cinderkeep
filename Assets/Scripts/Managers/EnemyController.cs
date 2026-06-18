using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private int _spawnPointId;
    private int _hp;

    public int SpawnPointId
    {
        get { return _spawnPointId; }
    }

    public void InitializeEnemy(int spawnPointId, int hp)
    {
        _spawnPointId = spawnPointId;
        _hp = hp;
    }

    public void TakeDamage(int damage)
    {
        _hp -= damage;

        if (_hp <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Destroy(gameObject);
    }

}
