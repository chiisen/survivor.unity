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
            hpBar.transform.SetParent(transform, false);
            hpBar.transform.localPosition = new Vector3(0, 0.15f, 0);
            hpBar.transform.localScale = new Vector3(0.5f, 0.08f, 1f);
            
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
            
            Debug.Log($"[EnemyController] HPBar created: Enemy.position={transform.position}, HPBar.localPosition={hpBar.transform.localPosition}, HPBar.localScale={hpBar.transform.localScale}, HPBar.worldPosition={hpBar.transform.position}");
        }
        
        private void UpdateHPBar()
        {
            if (hpBar != null && maxHP > 0)
            {
                float hpPercent = (float)hp / maxHP;
                
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
                hpBar.transform.localScale = new Vector3(0.5f * hpPercent, 0.08f, 1f);
                
                Debug.Log($"[EnemyController] HP updated: hp={hp}/{maxHP}, percent={hpPercent:P0}, color={hpColor}, localScale={hpBar.transform.localScale}, worldPosition={hpBar.transform.position}");
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