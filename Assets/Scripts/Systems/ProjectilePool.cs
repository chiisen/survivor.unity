using UnityEngine;
using System.Collections.Generic;

namespace SurvivorUnity.Core
{
    public class ProjectilePool : MonoBehaviour
    {
        public static ProjectilePool Instance { get; private set; }
        
        [Header("Pool Settings")]
        [SerializeField] private GameObject projectilePrefab;
        [SerializeField] private int initialPoolSize = 50;
        [SerializeField] private int maxPoolSize = 200;
        
        private Queue<GameObject> pool = new Queue<GameObject>();
        private List<GameObject> activeProjectiles = new List<GameObject>();
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
            
            InitializePool();
        }
        
        private void InitializePool()
        {
            for (int i = 0; i < initialPoolSize; i++)
            {
                GameObject projectile = Instantiate(projectilePrefab);
                projectile.SetActive(false);
                projectile.transform.SetParent(transform);
                pool.Enqueue(projectile);
            }
        }
        
        public GameObject SpawnProjectile(Vector2 position, Vector2 direction, float speed, int damage)
        {
            GameObject projectile = GetFromPool();
            
            if (projectile == null)
            {
                if (activeProjectiles.Count < maxPoolSize)
                {
                    projectile = Instantiate(projectilePrefab);
                    projectile.transform.SetParent(transform);
                }
                else
                {
                    return null;
                }
            }
            
            projectile.SetActive(true);
            projectile.transform.position = position;
            
            ProjectileController controller = projectile.GetComponent<ProjectileController>();
            if (controller != null)
            {
                controller.Initialize(direction, speed, damage);
            }
            
            activeProjectiles.Add(projectile);
            return projectile;
        }
        
        public void ReturnProjectile(GameObject projectile)
        {
            if (projectile == null) return;

            var trail = projectile.GetComponent<TrailRenderer>();
            if (trail != null) trail.Clear();

            projectile.SetActive(false);
            pool.Enqueue(projectile);
            activeProjectiles.Remove(projectile);
        }
        
        private GameObject GetFromPool()
        {
            if (pool.Count > 0)
            {
                return pool.Dequeue();
            }
            return null;
        }
        
        public List<GameObject> ActiveProjectiles => activeProjectiles;
    }
}