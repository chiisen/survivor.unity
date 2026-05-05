using UnityEngine;
using System.Collections.Generic;

namespace SurvivorUnity.Core
{
    public class EnemyPool : MonoBehaviour
    {
        public static EnemyPool Instance { get; private set; }
        
        [Header("Pool Settings")]
        [SerializeField] private GameObject enemyPrefab;
        [SerializeField] private int initialPoolSize = 30;
        [SerializeField] private int maxPoolSize = 100;
        
        [Header("Spawn Settings")]
        [SerializeField] private float spawnInterval = 2f;
        [SerializeField] private float spawnDistance = 10f;
        
        private Queue<GameObject> pool = new Queue<GameObject>();
        private List<GameObject> activeEnemies = new List<GameObject>();
        private float spawnCooldown = 0f;
        
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
        
        private void Update()
        {
            UpdateSpawnCooldown();
            SpawnEnemy();
        }
        
        private void InitializePool()
        {
            for (int i = 0; i < initialPoolSize; i++)
            {
                GameObject enemy = Instantiate(enemyPrefab);
                enemy.SetActive(false);
                enemy.transform.SetParent(transform);
                pool.Enqueue(enemy);
            }
        }
        
        private void UpdateSpawnCooldown()
        {
            if (spawnCooldown > 0f)
            {
                spawnCooldown -= Time.deltaTime;
            }
        }
        
        private void SpawnEnemy()
        {
            if (spawnCooldown > 0f) return;
            if (activeEnemies.Count >= maxPoolSize) return;
            
            if (GameManager.Instance == null || GameManager.Instance.Player == null) return;
            
            Vector2 spawnPos = GetRandomSpawnPosition();
            
            GameObject enemy = GetFromPool();
            if (enemy == null)
            {
                enemy = Instantiate(enemyPrefab);
                enemy.transform.SetParent(transform);
            }
            
            enemy.SetActive(true);
            enemy.transform.position = spawnPos;
            
            EnemyController controller = enemy.GetComponent<EnemyController>();
            if (controller != null)
            {
                controller.Initialize(
                    GameManager.Instance.Player.transform.position,
                    moveSpeed: 50f,
                    health: 1,
                    attackDamage: 10
                );
            }
            
            activeEnemies.Add(enemy);
            spawnCooldown = spawnInterval;
        }
        
        private Vector2 GetRandomSpawnPosition()
        {
            Vector2 playerPos = GameManager.Instance.Player.transform.position;
            
            float angle = Random.Range(0f, 360f);
            Vector2 offset = new Vector2(
                Mathf.Cos(angle * Mathf.Deg2Rad) * spawnDistance,
                Mathf.Sin(angle * Mathf.Deg2Rad) * spawnDistance
            );
            
            return playerPos + offset;
        }
        
        public void ReturnEnemy(GameObject enemy)
        {
            if (enemy == null) return;
            
            enemy.SetActive(false);
            pool.Enqueue(enemy);
            activeEnemies.Remove(enemy);
            
            if (GameManager.Instance != null)
            {
                GameManager.Instance.AddKill();
            }
        }
        
        private GameObject GetFromPool()
        {
            if (pool.Count > 0)
            {
                return pool.Dequeue();
            }
            return null;
        }
        
        public List<GameObject> ActiveEnemies => activeEnemies;
    }
}