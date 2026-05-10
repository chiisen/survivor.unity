using UnityEngine;

namespace SurvivorUnity.Core
{
    public class ProjectileController : MonoBehaviour
    {
        private int damage;
        private Rigidbody2D rb;
        private float lifetime = 5f;
        
        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
        }
        
        private void Start()
        {
            lifetime = 5f;
        }
        
        private void Update()
        {
            lifetime -= Time.deltaTime;
            if (lifetime <= 0f)
            {
                ReturnToPool();
            }
            
            CheckBounds();
        }
        
        private void CheckBounds()
        {
            Vector2 pos = transform.position;
            float maxDistance = 20f;
            
            if (Mathf.Abs(pos.x) > maxDistance || Mathf.Abs(pos.y) > maxDistance)
            {
                Debug.Log($"[ProjectileController] Bullet out of bounds at {pos}, destroying");
                ReturnToPool();
            }
        }
        
        public void Initialize(Vector2 dir, float moveSpeed, int attackDamage)
        {
            damage = attackDamage;
            lifetime = 5f;
            
            if (rb != null)
            {
                rb.linearVelocity = dir * moveSpeed;
                Debug.Log($"[ProjectileController.Initialize] velocity set: {rb.linearVelocity}, speed={moveSpeed}");
            }
        }
        
        private void ReturnToPool()
        {
            if (ProjectilePool.Instance != null)
            {
                ProjectilePool.Instance.ReturnProjectile(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        public int Damage => damage;
    }
}