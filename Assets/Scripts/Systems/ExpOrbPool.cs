using UnityEngine;
using System.Collections.Generic;

namespace SurvivorUnity.Core
{
    public class ExpOrbPool : MonoBehaviour
    {
        public static ExpOrbPool Instance { get; private set; }
        
        [Header("Pool Settings")]
        [SerializeField] private GameObject expOrbPrefab;
        [SerializeField] private int initialPoolSize = 50;
        [SerializeField] private int maxPoolSize = 300;
        
        private Queue<GameObject> pool = new Queue<GameObject>();
        private List<GameObject> activeOrbs = new List<GameObject>();
        
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
                GameObject orb = Instantiate(expOrbPrefab);
                orb.SetActive(false);
                orb.transform.SetParent(transform);
                pool.Enqueue(orb);
            }
        }
        
        public GameObject SpawnExpOrb(Vector2 position, int expValue = 10)
        {
            GameObject orb = GetFromPool();
            
            if (orb == null)
            {
                if (activeOrbs.Count < maxPoolSize)
                {
                    orb = Instantiate(expOrbPrefab);
                    orb.transform.SetParent(transform);
                }
                else
                {
                    return null;
                }
            }
            
            orb.SetActive(true);
            orb.transform.position = position;
            
            ExperienceOrb controller = orb.GetComponent<ExperienceOrb>();
            if (controller != null)
            {
                controller.Initialize(expValue);
            }
            
            activeOrbs.Add(orb);
            return orb;
        }
        
        public void ReturnOrb(GameObject orb)
        {
            if (orb == null) return;

            var trail = orb.GetComponent<TrailRenderer>();
            if (trail != null) trail.Clear();

            orb.SetActive(false);
            pool.Enqueue(orb);
            activeOrbs.Remove(orb);
        }
        
        private GameObject GetFromPool()
        {
            if (pool.Count > 0)
            {
                return pool.Dequeue();
            }
            return null;
        }
        
        public List<GameObject> ActiveOrbs => activeOrbs;
    }
}