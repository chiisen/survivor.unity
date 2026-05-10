using UnityEngine;

namespace SurvivorUnity.Core
{
    public class PlayerController : MonoBehaviour
    {
        [Header("Player Stats")]
        [SerializeField] private int maxHP = 100;
        [SerializeField] private int currentHP = 100;
        [SerializeField] private float speed = 5f;
        [SerializeField] private int pickupRange = 80;
        [SerializeField] private float attackRange = 300f;
        [SerializeField] private float fireRate = 0.5f;
        [SerializeField] private int damage = 1;
        [SerializeField] private float projectileSpeed = 400f;
        [SerializeField] private int projectileCount = 3;

        [Header("Player State")]
        [SerializeField] private float fireCooldown = 0f;
        [SerializeField] private int level = 1;
        [SerializeField] private int currentExp = 0;
        [SerializeField] private int expToLevel = 100;

        [Header("Shield System")]
        [SerializeField] private int shield = 0;
        [SerializeField] private int maxShield = 50;

        [Header("Shooting")]
        public GameObject projectilePrefab;
        public bool autoFire = true;
        public float autoFireInterval = 0.5f;
        private float nextFireTime = 0f;
        
        [Header("Components")]
        private Rigidbody2D rb;
        private SpriteRenderer spriteRenderer;

        private Vector2 movement;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void Update()
        {
            HandleInput();
            UpdateCooldown();
            AutoFire();
        }

        private void FixedUpdate()
        {
            Move();
        }

        private void HandleInput()
        {
            movement.x = Input.GetAxisRaw("Horizontal");
            movement.y = Input.GetAxisRaw("Vertical");

            // 移除旋转逻辑 - 主角保持固定朝向
            // if (movement != Vector2.zero)
            // {
            //     UpdateDirection();
            // }
        }

        private void Move()
        {
            rb.MovePosition(rb.position + movement * speed * Time.fixedDeltaTime);
        }

        private void UpdateDirection()
        {
            float angle = Mathf.Atan2(movement.y, movement.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
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
            if (!autoFire)
            {
                Debug.LogWarning("[PlayerController.AutoFire] autoFire is disabled");
                return;
            }
            
            if (projectilePrefab == null)
            {
                Debug.LogError("[PlayerController.AutoFire] projectilePrefab is null! Cannot fire.");
                return;
            }
            
            if (Time.time >= nextFireTime)
            {
                Debug.Log($"[PlayerController.AutoFire] Time to fire! Time.time={Time.time}, nextFireTime={nextFireTime}");
                FireProjectile();
                nextFireTime = Time.time + autoFireInterval;
            }
        }
        
        private void FireProjectile()
        {
            Debug.Log($"[PlayerController.FireProjectile] Starting... projectilePrefab={projectilePrefab?.name ?? "NULL"}");
            
            GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
            projectile.name = "Projectile_" + Time.time;
            
            Debug.Log($"[PlayerController.FireProjectile] Projectile instantiated: {projectile.name} at {transform.position}");
            
            var rb = projectile.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                Vector2 direction = Vector2.right;
                rb.linearVelocity = direction * projectileSpeed;
                Debug.Log($"[PlayerController.FireProjectile] Rigidbody2D velocity set: {rb.linearVelocity}");
            }
            else
            {
                Debug.LogError("[PlayerController.FireProjectile] Rigidbody2D component not found on projectile!");
            }
            
            var sr = projectile.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                Debug.Log($"[PlayerController.FireProjectile] SpriteRenderer found: enabled={sr.enabled}, color={sr.color}, sortingOrder={sr.sortingOrder}");
            }
            else
            {
                Debug.LogError("[PlayerController.FireProjectile] SpriteRenderer component not found on projectile!");
            }
            
            Debug.Log($"[PlayerController.FireProjectile] ✅ Projectile fired successfully!");
        }

        public bool CanFire()
        {
            return fireCooldown <= 0f;
        }

        public void Fire()
        {
            fireCooldown = fireRate;
        }

        public void TakeDamage(int damage)
        {
            if (shield > 0)
            {
                int shieldDamage = Mathf.Min(damage, shield);
                shield -= shieldDamage;
                damage -= shieldDamage;
            }

            currentHP -= damage;

            if (currentHP <= 0)
            {
                Die();
            }
        }

        public void Heal(int amount)
        {
            currentHP = Mathf.Min(currentHP + amount, maxHP);
        }

        public void AddExp(int amount)
        {
            currentExp += amount;

            if (currentExp >= expToLevel)
            {
                LevelUp();
            }
        }

        private void LevelUp()
        {
            level++;
            currentExp = 0;
            expToLevel = Mathf.RoundToInt(expToLevel * 1.2f);
        }

        private void Die()
        {
            GameManager.Instance.GameOver();
        }

        public int MaxHP => maxHP;
        public int CurrentHP => currentHP;
        public float Speed => speed;
        public int PickupRange => pickupRange;
        public float AttackRange => attackRange;
        public int Damage => damage;
        public int Level => level;
        public int CurrentExp => currentExp;
        public int ExpToLevel => expToLevel;
        public int Shield => shield;
        public int MaxShield => maxShield;
        public float FireCooldown => fireCooldown;
    }
}