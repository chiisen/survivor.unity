using UnityEngine;

namespace SurvivorUnity.Core
{
    public class ProjectileController : MonoBehaviour
    {
        private Vector2 direction;
        private float speed;
        private int damage;
        private Rigidbody2D rb;
        private float lifetime = 5f;
        
        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
        }
        
        private void FixedUpdate()
        {
            if (rb != null && direction != Vector2.zero)
            {
                rb.MovePosition(rb.position + direction * speed * Time.fixedDeltaTime);
            }
        }
        
        private void Update()
        {
            lifetime -= Time.deltaTime;
            if (lifetime <= 0f)
            {
                ReturnToPool();
            }
        }
        
        public void Initialize(Vector2 dir, float moveSpeed, int attackDamage)
        {
            direction = dir;
            speed = moveSpeed;
            damage = attackDamage;
            lifetime = 5f;
        }
        
        private void ReturnToPool()
        {
            if (ProjectilePool.Instance != null)
            {
                ProjectilePool.Instance.ReturnProjectile(gameObject);
            }
        }
        
        public int Damage => damage;
    }
}