using UnityEngine;
using Survivor.Config;
using Survivor.Systems;
using System.Collections;
using System.Collections;

namespace SurvivorUnity.Core
{
    public class EnemyController : MonoBehaviour
    {
        public EnemyType enemyType;
        
        private EnemyTypeConfig config;
        private Vector2 targetPosition;
        private float speed;
        private int hp;
        private int maxHP;
        private int damage;
        private Rigidbody2D rb;
        private GameObject hpBar;
        private SpriteRenderer hpBarRenderer;
        
        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            CreateHPBar();
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
            maxHP = health;
            damage = attackDamage;
            UpdateHPBar();
        }
        
        public void Initialize(EnemyTypeConfig config)
        {
            this.config = config;
            enemyType = config.enemyType;
            hp = config.maxHealth;
            maxHP = config.maxHealth;
            speed = config.moveSpeed;
            
            ApplyTypeSpecificSettings();
            UpdateHPBar();
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
            
            UpdateHPBar();
            
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
        
        private void CreateHPBar()
        {
            hpBar = new GameObject("HPBar");
            hpBar.transform.SetParent(transform);
            hpBar.transform.localPosition = new Vector3(0, 1.2f, 0);
            hpBar.transform.localScale = new Vector3(1f, 0.15f, 1f);
            
            hpBarRenderer = hpBar.AddComponent<SpriteRenderer>();
            
            Sprite circleSprite = Sprite.Create(
                Texture2D.whiteTexture,
                new Rect(0, 0, 1, 1),
                new Vector2(0.5f, 0.5f)
            );
            
            hpBarRenderer.sprite = circleSprite;
            hpBarRenderer.color = Color.green;
            hpBarRenderer.sortingOrder = 50;
            hpBarRenderer.sortingLayerID = 0;
            
            Debug.Log($"[EnemyController] HPBar created for {gameObject.name}, scale={hpBar.transform.localScale}, color={hpBarRenderer.color}");
        }
        
        private void UpdateHPBar()
        {
            if (hpBar != null && maxHP > 0)
            {
                float hpPercent = (float)hp / maxHP;
                hpBar.transform.localScale = new Vector3(1f * hpPercent, 0.15f, 1f);
                
                Color hpColor;
                if (hpPercent <= 0.3f)
                {
                    hpColor = new Color(1f, 0f, 0f); // 红色
                }
                else if (hpPercent <= 0.6f)
                {
                    hpColor = new Color(1f, 0.5f, 0f); // 橙色
                }
                else
                {
                    hpColor = new Color(0f, 1f, 0f); // 绿色
                }
                
                hpBarRenderer.color = hpColor;
                
                Debug.Log($"[EnemyController] HP updated: hp={hp}/{maxHP}, percent={hpPercent:P0}, scale={hpBar.transform.localScale}, color={hpColor}");
            }
        }
        
        private void OnDestroy()
        {
            if (hpBar != null)
            {
                Destroy(hpBar);
            }
        }
    }
}