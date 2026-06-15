using UnityEngine;

namespace OODong.Muckhold
{
    public sealed class MuckholdAutoShooter : MonoBehaviour
    {
        [SerializeField] private Transform _fireOrigin;
        [SerializeField] private MuckholdProjectile _projectileTemplate;
        [SerializeField] private float _range = 24f;
        [SerializeField] private float _cooldown = 0.55f;

        private float _timer;

        private void Update()
        {
            _timer += Time.deltaTime;
        }

        public void SetProjectileTemplate(MuckholdProjectile projectileTemplate)
        {
            _projectileTemplate = projectileTemplate;
        }

        public void SetFireOrigin(Transform fireOrigin)
        {
            _fireOrigin = fireOrigin;
        }

        public bool TryFireNearestEnemy()
        {
            if (_projectileTemplate == null || _timer < _cooldown)
            {
                return false;
            }

            MuckholdEnemy target = FindNearestEnemy();
            if (target == null)
            {
                return false;
            }

            _timer = 0f;
            FireAt(target.transform);
            return true;
        }

        private MuckholdEnemy FindNearestEnemy()
        {
            MuckholdEnemy nearestEnemy = null;
            float nearestDistanceSqr = _range * _range;
            MuckholdEnemy[] enemies = FindObjectsByType<MuckholdEnemy>(FindObjectsSortMode.None);

            for (int i = 0; i < enemies.Length; i++)
            {
                if (!enemies[i].IsAlive)
                {
                    continue;
                }

                float distanceSqr = (enemies[i].transform.position - transform.position).sqrMagnitude;
                if (distanceSqr < nearestDistanceSqr)
                {
                    nearestDistanceSqr = distanceSqr;
                    nearestEnemy = enemies[i];
                }
            }

            return nearestEnemy;
        }

        private void FireAt(Transform target)
        {
            Transform origin = _fireOrigin != null ? _fireOrigin : transform;
            MuckholdProjectile projectile = Instantiate(_projectileTemplate, origin.position, Quaternion.identity);
            projectile.gameObject.SetActive(true);
            Vector3 direction = (target.position + Vector3.up - origin.position).normalized;
            projectile.Launch(direction);
        }
    }
}
