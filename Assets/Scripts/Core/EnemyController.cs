using UnityEngine;

namespace SurvivorUnity.Core
{
    public class EnemyController : MonoBehaviour
    {
        private Vector2 targetPosition;
        private float speed;
        private int hp;
        private int damage;
        private Rigidbody2D rb;
        
        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
        }
        
        private void FixedUpdate()
        {
            MoveTowardsPlayer();
        }
        
        public void Initialize(Vector2 target, float moveSpeed, int health, int attackDamage)
        {
            targetPosition = target;
            speed = moveSpeed;
            hp = health;
            damage = attackDamage;
        }
        
        private void MoveTowardsPlayer()
        {
            if (GameManager.Instance == null || GameManager.Instance.Player == null) return;
            
            Vector2 direction = (GameManager.Instance.Player.transform.position - transform.position).normalized;
            rb.MovePosition(rb.position + direction * speed * Time.fixedDeltaTime);
        }
        
        public void TakeDamage(int amount)
        {
            hp -= amount;
            if (hp <= 0)
            {
                Die();
            }
        }
        
        private void Die()
        {
            if (ExpOrbPool.Instance != null)
            {
                ExpOrbPool.Instance.SpawnExpOrb(transform.position, expValue: 10);
            }
            
            if (EnemyPool.Instance != null)
            {
                EnemyPool.Instance.ReturnEnemy(gameObject);
            }
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Projectile"))
            {
                ProjectileController projectile = other.GetComponent<ProjectileController>();
                if (projectile != null)
                {
                    TakeDamage(projectile.Damage);
                    if (ProjectilePool.Instance != null)
                    {
                        ProjectilePool.Instance.ReturnProjectile(other.gameObject);
                    }
                }
            }
            
            if (other.CompareTag("Player"))
            {
                PlayerController player = other.GetComponent<PlayerController>();
                if (player != null)
                {
                    player.TakeDamage(damage);
                }
            }
        }
    }
}