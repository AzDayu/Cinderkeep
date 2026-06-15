using UnityEngine;

namespace OODong.Muckhold
{
    public sealed class MuckholdEnemySpawner : MonoBehaviour
    {
        [SerializeField] private MuckholdEnemy _enemyTemplate;
        [SerializeField] private Transform _target;
        [SerializeField] private float _spawnRadius = 28f;
        [SerializeField] private float _spawnInterval = 4f;
        [SerializeField] private int _maxAliveCount = 6;

        private float _timer;

        private void Update()
        {
            if (_enemyTemplate == null || _target == null)
            {
                return;
            }

            _timer += Time.deltaTime;
            if (_timer < _spawnInterval)
            {
                return;
            }

            _timer = 0f;
            if (CountAliveEnemies() >= _maxAliveCount)
            {
                return;
            }

            SpawnEnemy();
        }

        public void SetTarget(Transform target)
        {
            _target = target;
        }

        public void SetEnemyTemplate(MuckholdEnemy enemyTemplate)
        {
            _enemyTemplate = enemyTemplate;
        }

        private void SpawnEnemy()
        {
            Vector2 circle = Random.insideUnitCircle.normalized * _spawnRadius;
            Vector3 spawnPosition = _target.position + new Vector3(circle.x, 0f, circle.y);
            MuckholdEnemy enemy = Instantiate(_enemyTemplate, spawnPosition, Quaternion.identity);
            enemy.gameObject.SetActive(true);
            enemy.SetTarget(_target);
        }

        private int CountAliveEnemies()
        {
            int aliveCount = 0;
            MuckholdEnemy[] enemies = FindObjectsByType<MuckholdEnemy>(FindObjectsSortMode.None);
            for (int i = 0; i < enemies.Length; i++)
            {
                if (enemies[i] != _enemyTemplate && enemies[i].IsAlive)
                {
                    aliveCount++;
                }
            }

            return aliveCount;
        }
    }
}
