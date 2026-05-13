using UnityEngine;
using UnityEngine.Rendering;

namespace SurvivorUnity.Core
{
    public class ProjectileController : MonoBehaviour
    {
        [Header("📝 備註")]
        [Tooltip("子彈本體：Scale=0.75（原本1.5的一半）\n尾跡寬度：TrailRenderer widthCurve 起点=0.15（原本0.3的一半）\n尾跡比例：TrailRenderer widthCurve 终点=0.025（原本0.05的一半）")]
        public string note = "子彈小_尾跡小";

        private int damage;
        private Rigidbody2D rb;
        private float lifetime = 5f;
        private TrailRenderer trailRenderer;
        
        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            SetupTrailRenderer();
        }
        
        private void SetupTrailRenderer()
        {
            trailRenderer = GetComponent<TrailRenderer>();
            if (trailRenderer == null)
            {
                trailRenderer = gameObject.AddComponent<TrailRenderer>();
            }
            
            trailRenderer.time = 0.4f;
            trailRenderer.minVertexDistance = 0.1f;
            trailRenderer.widthMultiplier = 1f;
            
            AnimationCurve widthCurve = new AnimationCurve();
            widthCurve.AddKey(0f, 0.15f);
            widthCurve.AddKey(1f, 0.025f);
            trailRenderer.widthCurve = widthCurve;
            
            Gradient gradient = new Gradient();
            GradientColorKey[] colorKeys = new GradientColorKey[2];
            colorKeys[0] = new GradientColorKey(new Color(1f, 0.8f, 0.4f), 0f);
            colorKeys[1] = new GradientColorKey(new Color(1f, 0.6f, 0.2f), 1f);
            
            GradientAlphaKey[] alphaKeys = new GradientAlphaKey[2];
            alphaKeys[0] = new GradientAlphaKey(1f, 0f);
            alphaKeys[1] = new GradientAlphaKey(0f, 1f);
            
            gradient.SetKeys(colorKeys, alphaKeys);
            trailRenderer.colorGradient = gradient;
            
            trailRenderer.material = new Material(Shader.Find("Sprites/Default"));
            trailRenderer.sortingOrder = 15;
            
            Debug.Log($"[ProjectileController] TrailRenderer created: time={trailRenderer.time}, width={trailRenderer.widthMultiplier}");
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
            if (Camera.main == null) return;

            Vector3 pos = transform.position;
            Vector3 screenPos = Camera.main.WorldToScreenPoint(pos);

            if (screenPos.x < 0 || screenPos.x > Screen.width ||
                screenPos.y < 0 || screenPos.y > Screen.height)
            {
                Debug.Log($"[ProjectileController] Bullet out of screen at {pos}, destroying");
                if (ProjectilePool.Instance != null)
                {
                    ProjectilePool.Instance.ReturnProjectile(gameObject);
                }
                Destroy(gameObject);
            }
        }
        
        public void Initialize(Vector2 dir, float moveSpeed, int attackDamage)
        {
            damage = attackDamage;
            lifetime = 5f;
            
            if (trailRenderer != null)
            {
                trailRenderer.Clear();
            }
            
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