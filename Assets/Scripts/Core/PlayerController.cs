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
        [SerializeField] private float projectileSpeed = 3f;
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

        private LineRenderer lineRenderer;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            CreateAttackRangeCircle();
        }

        private void CreateAttackRangeCircle()
        {
            GameObject rangeCircle = new GameObject("AttackRangeCircle");
            rangeCircle.transform.SetParent(transform, false);
            rangeCircle.transform.localPosition = Vector3.zero;

            lineRenderer = rangeCircle.AddComponent<LineRenderer>();
            lineRenderer.startColor = new Color(0.5f, 0.5f, 1f, 0.5f);
            lineRenderer.endColor = new Color(0.5f, 0.5f, 1f, 0.5f);
            lineRenderer.startWidth = 0.01f;
            lineRenderer.endWidth = 0.01f;
            lineRenderer.useWorldSpace = false;
            lineRenderer.loop = true;

            float diameter = attackRange;
            int segments = 64;
            Vector3[] points = new Vector3[segments + 1];

            for (int i = 0; i <= segments; i++)
            {
                float angle = i * 2f * Mathf.PI / segments;
                float x = Mathf.Cos(angle) * diameter * 0.5f;
                float y = Mathf.Sin(angle) * diameter * 0.5f;
                points[i] = new Vector3(x, y, 0);
            }

            lineRenderer.positionCount = segments + 1;
            lineRenderer.SetPositions(points);

            lineRenderer.sortingOrder = 1;

            Debug.Log($"[PlayerController] Attack range circle created: attackRange={attackRange}, diameter={diameter}");
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
                GameObject nearestEnemy = FindNearestEnemy();
                if (nearestEnemy != null)
                {
                    FireProjectile(nearestEnemy.transform.position);
                    nextFireTime = Time.time + autoFireInterval;
                }
            }
        }
        
        private GameObject FindNearestEnemy()
        {
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            
            if (enemies == null || enemies.Length == 0)
            {
                Debug.LogWarning("[PlayerController.FindNearestEnemy] No enemies found");
                return null;
            }
            
            float range = attackRange;
            GameObject nearest = null;
            float nearestDist = range;
            
            foreach (GameObject enemy in enemies)
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
            Debug.Log($"[PlayerController.FireProjectile] Targeting enemy at {targetPos}");
            
            GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
            projectile.name = "Projectile_" + Time.time;
            
            Vector2 direction = (targetPos - (Vector2)transform.position).normalized;
            
            var projectileCtrl = projectile.GetComponent<ProjectileController>();
            if (projectileCtrl != null)
            {
                projectileCtrl.Initialize(direction, projectileSpeed, damage);
                Debug.Log($"[PlayerController.FireProjectile] ProjectileController initialized: direction={direction}, speed={projectileSpeed}, damage={damage}");
            }
            else
            {
                Debug.LogError("[PlayerController.FireProjectile] ProjectileController not found!");
            }
            
            Debug.Log($"[PlayerController.FireProjectile] ✅ Projectile fired at enemy!");
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