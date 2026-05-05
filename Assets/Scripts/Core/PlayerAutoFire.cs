using UnityEngine;

namespace SurvivorUnity.Core
{
    public class PlayerAutoFire : MonoBehaviour
    {
        [Header("Fire Settings")]
        [SerializeField] private float fireRate = 0.5f;
        [SerializeField] private int projectileCount = 3;
        [SerializeField] private float projectileSpeed = 400f;
        [SerializeField] private int damage = 1;
        
        private float fireCooldown = 0f;
        private PlayerController player;
        
        private void Awake()
        {
            player = GetComponent<PlayerController>();
        }
        
        private void Update()
        {
            UpdateCooldown();
            AutoFire();
        }
        
        private void UpdateCooldown()
        {
            if (fireCooldown > 0f)
            {
                fireCooldown -= Time.deltaTime;
            }
        }
        
        private void AutoFire()
        {
            if (!CanFire()) return;
            
            GameObject nearestEnemy = FindNearestEnemy();
            if (nearestEnemy == null) return;
            
            FireProjectile(nearestEnemy.transform.position);
            fireCooldown = fireRate;
        }
        
        private bool CanFire()
        {
            return fireCooldown <= 0f;
        }
        
        private GameObject FindNearestEnemy()
        {
            if (player == null || EnemyPool.Instance == null) return null;
            
            float range = player.AttackRange;
            GameObject nearest = null;
            float nearestDist = range;
            
            foreach (GameObject enemy in EnemyPool.Instance.ActiveEnemies)
            {
                if (enemy == null || !enemy.activeInHierarchy) continue;
                
                float dist = Vector2.Distance(transform.position, enemy.transform.position);
                if (dist < nearestDist)
                {
                    nearestDist = dist;
                    nearest = enemy;
                }
            }
            
            return nearest;
        }
        
        private void FireProjectile(Vector2 targetPos)
        {
            if (ProjectilePool.Instance == null) return;
            
            Vector2 direction = (targetPos - (Vector2)transform.position).normalized;
            
            for (int i = 0; i < projectileCount; i++)
            {
                float spreadAngle = (i - (projectileCount - 1) / 2f) * 22.5f;
                Vector2 spreadDir = RotateDirection(direction, spreadAngle);
                
                ProjectilePool.Instance.SpawnProjectile(
                    transform.position,
                    spreadDir,
                    projectileSpeed,
                    damage
                );
            }
        }
        
        private Vector2 RotateDirection(Vector2 dir, float angle)
        {
            float rad = angle * Mathf.Deg2Rad;
            return new Vector2(
                dir.x * Mathf.Cos(rad) - dir.y * Mathf.Sin(rad),
                dir.x * Mathf.Sin(rad) + dir.y * Mathf.Cos(rad)
            );
        }
    }
}