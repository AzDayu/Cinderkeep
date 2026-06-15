using UnityEngine;

namespace OODong.Muckhold
{
    public sealed class MuckholdEnemy : MonoBehaviour
    {
        [SerializeField] private Transform _target;
        [SerializeField] private float _moveSpeed = 2.2f;
        [SerializeField] private int _health = 1;
        [SerializeField] private float _stopDistance = 1.4f;

        public bool IsAlive => _health > 0 && gameObject.activeInHierarchy;

        private void Update()
        {
            MoveToTarget();
        }

        public void SetTarget(Transform target)
        {
            _target = target;
        }

        public void TakeDamage(int damage)
        {
            if (damage <= 0 || _health <= 0)
            {
                return;
            }

            _health -= damage;
            if (_health <= 0)
            {
                Destroy(gameObject);
            }
        }

        private void MoveToTarget()
        {
            if (_target == null || _health <= 0)
            {
                return;
            }

            Vector3 toTarget = _target.position - transform.position;
            toTarget.y = 0f;
            if (toTarget.magnitude <= _stopDistance)
            {
                return;
            }

            Vector3 direction = toTarget.normalized;
            transform.position += direction * (_moveSpeed * Time.deltaTime);
            transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
        }
    }
}
