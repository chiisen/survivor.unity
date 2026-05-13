using UnityEngine;

namespace SurvivorUnity.Core
{
    public class PlayerController : MonoBehaviour
    {
        [Header("📝 備註")]
        [Tooltip("攻擊範圍：attackRange=300（用於判斷敵人是否在射擊範圍內）\n射擊半徑：實際射擊範圍 = attackRange * 0.5（等於視覺圈圈半徑）\n子彈冷卻：autoFireInterval=0.5秒")]
        public string note = "射擊範圍等於視覺圈圈半徑";

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
        private GameObject hpBar;
        private SpriteRenderer hpBarRenderer;
        private int lastHP;

        [Header("Directional Sprites")]
        public Sprite spriteUp;
        public Sprite spriteDown;
        public Sprite spriteLeft;
        public Sprite spriteRight;

        private Vector2 movement;

        private LineRenderer lineRenderer;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            lastHP = maxHP;
            CreateAttackRangeCircle();
            CreatePlayerHPBar();
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

        private void CreatePlayerHPBar()
        {
            hpBar = new GameObject("PlayerHPBar");
            hpBar.transform.SetParent(transform, false);
            hpBar.transform.localPosition = new Vector3(0, 0.6f, 0);
            hpBar.transform.localScale = new Vector3(1.2f, 0.12f, 1f);

            hpBarRenderer = hpBar.AddComponent<SpriteRenderer>();

            Texture2D texture = new Texture2D(64, 16);
            Color[] colors = new Color[64 * 16];
            for (int i = 0; i < colors.Length; i++)
            {
                colors[i] = Color.white;
            }
            texture.SetPixels(colors);
            texture.Apply();

            Sprite sprite = Sprite.Create(
                texture,
                new Rect(0, 0, 64, 16),
                new Vector2(0.5f, 0.5f),
                64
            );

            hpBarRenderer.sprite = sprite;
            hpBarRenderer.color = Color.green;
            hpBarRenderer.sortingOrder = 100;

            Debug.Log($"[PlayerController] PlayerHPBar created: localPosition={hpBar.transform.localPosition}, localScale={hpBar.transform.localScale}");
        }

        private void UpdatePlayerHPBar()
        {
            if (hpBar != null && maxHP > 0)
            {
                float hpPercent = (float)currentHP / maxHP;

                Color hpColor;
                if (hpPercent <= 0.3f)
                {
                    hpColor = new Color(1f, 0f, 0f);
                }
                else if (hpPercent <= 0.6f)
                {
                    hpColor = new Color(1f, 0.5f, 0f);
                }
                else
                {
                    hpColor = new Color(0f, 1f, 0f);
                }

                hpBarRenderer.color = hpColor;
                hpBar.transform.localScale = new Vector3(1.2f * hpPercent, 0.12f, 1f);

                if (currentHP != lastHP)
                {
                    Debug.Log($"[PlayerController] HP changed: {lastHP} -> {currentHP} ({hpPercent:P0})");
                    lastHP = currentHP;
                }
            }
        }

        private void OnDestroy()
        {
            if (hpBar != null)
            {
                Destroy(hpBar);
            }
        }

        private void Update()
        {
            HandleInput();
            UpdateCooldown();
            AutoFire();
            UpdatePlayerHPBar();
        }

        private void FixedUpdate()
        {
            Move();
        }

        private void HandleInput()
        {
            movement.x = Input.GetAxisRaw("Horizontal");
            movement.y = Input.GetAxisRaw("Vertical");
            UpdateDirectionalSprite();
        }

        private void UpdateDirectionalSprite()
        {
            if (spriteRenderer == null) return;

            if (movement.x > 0 && spriteRight != null)
            {
                spriteRenderer.sprite = spriteRight;
            }
            else if (movement.x < 0 && spriteLeft != null)
            {
                spriteRenderer.sprite = spriteLeft;
            }
            else if (movement.y > 0 && spriteUp != null)
            {
                spriteRenderer.sprite = spriteUp;
            }
            else if (movement.y < 0 && spriteDown != null)
            {
                spriteRenderer.sprite = spriteDown;
            }
        }

        private void Move()
        {
            Vector2 newPos = rb.position + movement * speed * Time.fixedDeltaTime;

            if (Camera.main != null)
            {
                float halfHeight = Camera.main.orthographicSize;
                float halfWidth = halfHeight * Camera.main.aspect;

                float minX = Camera.main.transform.position.x - halfWidth + 0.5f;
                float maxX = Camera.main.transform.position.x + halfWidth - 0.5f;
                float minY = Camera.main.transform.position.y - halfHeight + 0.5f;
                float maxY = Camera.main.transform.position.y + halfHeight - 0.5f;

                newPos.x = Mathf.Clamp(newPos.x, minX, maxX);
                newPos.y = Mathf.Clamp(newPos.y, minY, maxY);
            }

            rb.MovePosition(newPos);
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
            
            float range = attackRange * 0.5f;
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
            
            GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity, transform);
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
            if (GameManager.IsPaused) return;

            if (shield > 0)
            {
                int shieldDamage = Mathf.Min(damage, shield);
                shield -= shieldDamage;
                damage -= shieldDamage;
            }

            currentHP -= damage;
            Debug.LogWarning($"[PlayerController.TakeDamage] currentHP={currentHP}, damage={damage}");

            if (currentHP <= 0)
            {
                currentHP = 0;
                Debug.LogWarning("[PlayerController.TakeDamage] HP <= 0, calling Die()");
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
            Debug.LogError("[PlayerController.Die] PLAYER DIED!");
            Debug.LogError($"[PlayerController.Die] GameManager.Instance = {GameManager.Instance}");

            if (GameManager.Instance != null)
            {
                Debug.LogError("[PlayerController.Die] Calling GameManager.Instance.GameOver()...");
                GameManager.Instance.GameOver();
                Debug.LogError("[PlayerController.Die] GameOver called.");
            }
            else
            {
                Debug.LogError("[PlayerController.Die] GameManager.Instance is NULL! Cannot call GameOver!");
            }
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