using UnityEngine;
using System.Collections.Generic;

namespace SurvivorUnity.Core
{
    public class WaveManager : MonoBehaviour
    {
        [Header("Wave Settings")]
        [SerializeField] private float waveDuration = 60f;
        [SerializeField] private float restDuration = 5f;
        [SerializeField] private int baseEnemyCount = 10;
        [SerializeField] private float enemyCountMultiplier = 1.3f;
        [SerializeField] private float baseSpawnInterval = 1.5f;
        [SerializeField] private float spawnIntervalDecrease = 0.05f;
        [SerializeField] private float minSpawnInterval = 0.3f;

        [Header("Boss Settings")]
        [SerializeField] private int bossWaveInterval = 5;
        [SerializeField] private GameObject bossPrefab;

        [Header("Current Wave State")]
        [SerializeField] private int currentWave = 1;
        [SerializeField] private float waveTimer = 0f;
        [SerializeField] private float spawnTimer = 0f;
        [SerializeField] private bool isRestTime = false;
        [SerializeField] private bool isBossWave = false;
        [SerializeField] private bool bossSpawned = false;

        [Header("Enemy Prefabs")]
        [SerializeField] private List<GameObject> enemyPrefabs = new List<GameObject>();

        private int enemiesSpawned = 0;
        private int enemiesToSpawn = 0;
        private float currentSpawnInterval = 1.5f;

        private void Start()
        {
            StartWave();
        }

        private void Update()
        {
            if (GameManager.Instance != null && GameManager.Instance.GameTime > 0)
            {
                UpdateWave();
            }
        }

        private void UpdateWave()
        {
            if (isRestTime)
            {
                UpdateRestTime();
            }
            else
            {
                UpdateWaveTime();
            }
        }

        private void UpdateWaveTime()
        {
            waveTimer += Time.deltaTime;

            if (!isBossWave && waveTimer >= waveDuration)
            {
                EndWave();
            }
            else if (isBossWave && waveTimer >= waveDuration * 0.5f && !bossSpawned)
            {
                SpawnBoss();
            }

            spawnTimer += Time.deltaTime;
            if (spawnTimer >= currentSpawnInterval && enemiesSpawned < enemiesToSpawn)
            {
                SpawnEnemy();
                spawnTimer = 0f;
            }
        }

        private void UpdateRestTime()
        {
            waveTimer += Time.deltaTime;

            if (waveTimer >= restDuration)
            {
                isRestTime = false;
                StartNextWave();
            }
        }

        private void StartWave()
        {
            waveTimer = 0f;
            spawnTimer = 0f;
            enemiesSpawned = 0;

            isBossWave = (currentWave % bossWaveInterval == 0);

            if (isBossWave)
            {
                enemiesToSpawn = Mathf.RoundToInt(baseEnemyCount * enemyCountMultiplier * (currentWave - 1) * 0.5f);
            }
            else
            {
                enemiesToSpawn = Mathf.RoundToInt(baseEnemyCount * enemyCountMultiplier * (currentWave - 1));
            }

            currentSpawnInterval = Mathf.Max(
                baseSpawnInterval - spawnIntervalDecrease * (currentWave - 1),
                minSpawnInterval
            );
        }

        private void EndWave()
        {
            isRestTime = true;
            waveTimer = 0f;
        }

        private void StartNextWave()
        {
            currentWave++;
            StartWave();
        }

        private void SpawnEnemy()
        {
            if (enemyPrefabs.Count == 0) return;

            int prefabIndex = Random.Range(0, enemyPrefabs.Count);
            GameObject enemyPrefab = enemyPrefabs[prefabIndex];

            Vector2 spawnPosition = GetSpawnPosition();

            GameObject enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
            GameManager.Instance.Enemies.Add(enemy);

            enemiesSpawned++;
        }

        private void SpawnBoss()
        {
            if (bossPrefab == null) return;

            Vector2 spawnPosition = GetSpawnPosition();

            GameObject boss = Instantiate(bossPrefab, spawnPosition, Quaternion.identity);
            GameManager.Instance.Enemies.Add(boss);

            bossSpawned = true;
        }

        private Vector2 GetSpawnPosition()
        {
            Camera mainCamera = Camera.main;
            if (mainCamera == null) return Vector2.zero;

            float cameraHeight = mainCamera.orthographicSize * 2f;
            float cameraWidth = cameraHeight * mainCamera.aspect;

            Vector2 playerPosition = GameManager.Instance.Player.transform.position;

            float spawnDistance = Mathf.Max(cameraWidth, cameraHeight) * 0.6f;

            float angle = Random.Range(0f, 360f);
            Vector2 spawnDirection = new Vector2(
                Mathf.Cos(angle * Mathf.Deg2Rad),
                Mathf.Sin(angle * Mathf.Deg2Rad)
            );

            return playerPosition + spawnDirection * spawnDistance;
        }

        public int CurrentWave => currentWave;
        public bool IsBossWave => isBossWave;
        public bool IsRestTime => isRestTime;
        public int EnemiesSpawned => enemiesSpawned;
        public int EnemiesToSpawn => enemiesToSpawn;
    }
}