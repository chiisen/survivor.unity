using UnityEngine;
using System.Collections.Generic;
using Survivor.Config;
using Survivor.Core;

namespace Survivor.Systems
{
    public class EnemyPoolManager : MonoBehaviour
    {
        public static EnemyPoolManager Instance { get; private set; }

        [Header("Spawn Settings")]
        public EnemySpawnSettings spawnSettings;

        [Header("Enemy Prefabs")]
        public GameObject normalPrefab;
        public GameObject fastPrefab;
        public GameObject tankPrefab;
        public GameObject rangedPrefab;

        private Dictionary<EnemyType, EnemyPool> pools;
        private List<EnemyTypeConfig> configs;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void Start()
        {
            Initialize();
        }

        private void Initialize()
        {
            pools = new Dictionary<EnemyType, EnemyPool>();
            configs = spawnSettings.enemyConfigs;

            foreach (var config in configs)
            {
                var pool = CreatePoolForType(config.enemyType);
                pools.Add(config.enemyType, pool);
            }
        }

        public GameObject SpawnEnemy(float gameTime)
        {
            var weights = CalculateWeights(gameTime);
            var selectedType = SelectEnemyType(weights);

            var pool = pools[selectedType];
            var enemy = pool.Spawn(GetRandomSpawnPosition());

            var config = GetConfigForType(selectedType);
            var controller = enemy.GetComponent<EnemyController>();
            if (controller != null)
            {
                controller.Initialize(config);
            }

            return enemy;
        }

        public void ReturnEnemy(GameObject enemy)
        {
            var controller = enemy.GetComponent<EnemyController>();
            if (controller != null)
            {
                var pool = pools[controller.enemyType];
                pool.Return(enemy);
            }
        }

        private Dictionary<EnemyType, float> CalculateWeights(float gameTime)
        {
            var weights = new Dictionary<EnemyType, float>();

            foreach (var config in configs)
            {
                if (gameTime >= config.minSpawnTime)
                {
                    float weight = config.weightCurve.Evaluate(gameTime);
                    weights.Add(config.enemyType, weight);
                }
                else
                {
                    weights.Add(config.enemyType, 0f);
                }
            }

            return weights;
        }

        private EnemyType SelectEnemyType(Dictionary<EnemyType, float> weights)
        {
            float totalWeight = 0f;
            foreach (var weight in weights.Values)
            {
                totalWeight += weight;
            }

            if (totalWeight <= 0f)
                return EnemyType.Normal;

            float randomValue = Random.Range(0f, totalWeight);
            float cumulativeWeight = 0f;

            foreach (var kvp in weights)
            {
                cumulativeWeight += kvp.Value;
                if (randomValue <= cumulativeWeight)
                {
                    return kvp.Key;
                }
            }

            return EnemyType.Normal;
        }

        private EnemyPool CreatePoolForType(EnemyType type)
        {
            var poolGO = new GameObject($"EnemyPool_{type}");
            poolGO.transform.SetParent(transform);

            var pool = poolGO.AddComponent<EnemyPool>();
            pool.enemyType = type;
            pool.enemyPrefab = GetPrefabForType(type);
            pool.initialPoolSize = spawnSettings.initialPoolSize;

            return pool;
        }

        private GameObject GetPrefabForType(EnemyType type)
        {
            switch (type)
            {
                case EnemyType.Normal: return normalPrefab;
                case EnemyType.Fast: return fastPrefab;
                case EnemyType.Tank: return tankPrefab;
                case EnemyType.Ranged: return rangedPrefab;
                default: return normalPrefab;
            }
        }

        private EnemyTypeConfig GetConfigForType(EnemyType type)
        {
            foreach (var config in configs)
            {
                if (config.enemyType == type)
                    return config;
            }
            return null;
        }

        private Vector3 GetRandomSpawnPosition()
        {
            float spawnRadius = 15f;
            Vector2 randomDirection = Random.insideUnitCircle.normalized * spawnRadius;
            Vector3 playerPosition = GameObject.FindGameObjectWithTag("Player").transform.position;
            return playerPosition + new Vector3(randomDirection.x, randomDirection.y, 0);
        }
    }
}