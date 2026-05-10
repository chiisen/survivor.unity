using UnityEngine;
using Survivor.Config;
using Survivor.Systems;

namespace SurvivorUnity.Core
{
    public class EnemyController : MonoBehaviour
    {
        public EnemyType enemyType;
        
        private EnemyTypeConfig config;
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
        
        public void Initialize(EnemyTypeConfig config)
        {
            this.config = config;
            enemyType = config.enemyType;
            hp = config.maxHealth;
            speed = config.moveSpeed;
            
            ApplyTypeSpecificSettings();
        }
        
        private void ApplyTypeSpecificSettings()
        {
            if (enemyType == EnemyType.Tank)
            {
                GetComponent<CircleCollider2D>().radius = 0.8f;
            }
            
            if (enemyType == EnemyType.Ranged)
            {
                var autoFire = GetComponent<EnemyAutoFire>();
                if (autoFire != null)
                {
                    autoFire.Initialize(config.attackRange, config.attackInterval, config.projectilePrefab);
                }
            }
        }
        
        private void MoveTowardsPlayer()
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player == null) return;
            
            Vector2 direction = (player.transform.position - transform.position).normalized;
            
            if (enemyType == EnemyType.Ranged && config != null)
            {
                float distance = Vector2.Distance(transform.position, GameManager.Instance.Player.transform.position);
                if (distance <= config.attackRange)
                {
                    rb.linearVelocity = Vector2.zero;
                    return;
                }
            }
            
            rb.MovePosition(rb.position + direction * speed * Time.fixedDeltaTime);
        }
        
        public void TakeDamage(int amount)
        {
            hp -= amount;
            
            StartCoroutine(FlashRed());
            
            if (hp <= 0)
            {
                Die();
            }
        }
        
        private System.Collections.IEnumerator FlashRed()
        {
            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                Color originalColor = sr.color;
                sr.color = Color.red;
                yield return new WaitForSeconds(0.1f);
                sr.color = originalColor;
            }
        }
        
        private void Die()
        {
            Debug.Log($"[EnemyController.Die] Enemy dying at {transform.position}");
            
            if (ExpOrbPool.Instance != null)
            {
                int expValue = config != null ? config.expValue : 10;
                ExpOrbPool.Instance.SpawnExpOrb(transform.position, expValue);
            }
            
            if (EnemyPoolManager.Instance != null)
            {
                EnemyPoolManager.Instance.ReturnEnemy(gameObject);
            }
            else if (EnemyPool.Instance != null)
            {
                EnemyPool.Instance.ReturnEnemy(gameObject);
            }
            else
            {
                Debug.Log("[EnemyController.Die] No pool found, destroying enemy");
                Destroy(gameObject);
            }
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            Debug.Log($"[EnemyController.OnTriggerEnter2D] Collision detected with: {other.name}, tag={other.tag}");
            
            if (other.CompareTag("Projectile"))
            {
                ProjectileController projectile = other.GetComponent<ProjectileController>();
                if (projectile != null)
                {
                    Debug.Log($"[EnemyController.OnTriggerEnter2D] Taking damage: {projectile.Damage}");
                    TakeDamage(projectile.Damage);
                    if (ProjectilePool.Instance != null)
                    {
                        ProjectilePool.Instance.ReturnProjectile(other.gameObject);
                    }
                    else
                    {
                        Destroy(other.gameObject);
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