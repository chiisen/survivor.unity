using UnityEngine;
using System.Collections.Generic;

namespace SurvivorUnity.Core
{
    public class SimpleEnemySpawner : MonoBehaviour
    {
        [Header("Spawn Settings")]
        public GameObject normalEnemyPrefab;
        public float spawnInterval = 2f;
        public float spawnDistance = 10f;
        public int maxEnemies = 50;
        
        [Header("Enemy Stats")]
        public float normalSpeed = 2f;
        public int normalHP = 1;
        public int normalDamage = 10;
        public int normalExpValue = 5;
        
        private float nextSpawnTime = 0f;
        private List<GameObject> activeEnemies = new List<GameObject>();
        private GameObject player;
        
        private void Start()
        {
            player = GameObject.FindGameObjectWithTag("Player");
            
            if (player == null)
            {
                Debug.LogError("[SimpleEnemySpawner] Player not found! Make sure Player has 'Player' tag.");
            }
            
            if (normalEnemyPrefab == null)
            {
                Debug.LogError("[SimpleEnemySpawner] normalEnemyPrefab is not assigned!");
            }
            
            Debug.Log("[SimpleEnemySpawner] Initialized. Player found: " + (player != null));
        }
        
        private void Update()
        {
            if (player == null || normalEnemyPrefab == null) return;
            
            if (Time.time >= nextSpawnTime && activeEnemies.Count < maxEnemies)
            {
                SpawnEnemy();
                nextSpawnTime = Time.time + spawnInterval;
            }
            
            CleanUpDestroyedEnemies();
        }
        
        private void SpawnEnemy()
        {
            Vector2 spawnPosition = GetRandomSpawnPosition();
            
            GameObject enemy = Instantiate(normalEnemyPrefab, spawnPosition, Quaternion.identity);
            enemy.name = "Enemy_" + activeEnemies.Count;
            
            var controller = enemy.GetComponent<EnemyController>();
            if (controller != null)
            {
                controller.Initialize(player.transform.position, normalSpeed, normalHP, normalDamage);
            }
            
            activeEnemies.Add(enemy);
            
            Debug.Log($"[SimpleEnemySpawner] Spawned enemy at {spawnPosition}. Active enemies: {activeEnemies.Count}");
        }
        
        private Vector2 GetRandomSpawnPosition()
        {
            Vector2 playerPos = player.transform.position;
            
            float angle = Random.Range(0f, 360f);
            float radians = angle * Mathf.Deg2Rad;
            
            Vector2 offset = new Vector2(
                Mathf.Cos(radians) * spawnDistance,
                Mathf.Sin(radians) * spawnDistance
            );
            
            return playerPos + offset;
        }
        
        private void CleanUpDestroyedEnemies()
        {
            activeEnemies.RemoveAll(e => e == null);
        }
        
        public int GetActiveEnemyCount()
        {
            return activeEnemies.Count;
        }
    }
}