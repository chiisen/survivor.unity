using UnityEngine;

namespace Survivor.Core
{
    public class EnemyAutoFire : MonoBehaviour
    {
        private float attackRange;
        private float attackInterval;
        private GameObject projectilePrefab;
        private Transform playerTransform;
        private float lastAttackTime;

        public void Initialize(float range, float interval, GameObject prefab)
        {
            attackRange = range;
            attackInterval = interval;
            projectilePrefab = prefab;
            playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        }

        private void Update()
        {
            if (playerTransform == null)
                return;

            float distance = Vector2.Distance(transform.position, playerTransform.position);

            if (distance <= attackRange && Time.time - lastAttackTime >= attackInterval)
            {
                FireProjectile();
                lastAttackTime = Time.time;
            }
        }

        private void FireProjectile()
        {
            Vector2 direction = (playerTransform.position - transform.position).normalized;

            var projectile = Survivor.Systems.ProjectilePool.Instance.Spawn(transform.position);

            var projectileController = projectile.GetComponent<Survivor.Core.ProjectileController>();
            if (projectileController != null)
            {
                projectileController.SetDirection(direction);
            }
        }
    }
}