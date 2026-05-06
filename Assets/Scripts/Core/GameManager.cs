using UnityEngine;
using System.Collections.Generic;
using Survivor.Systems;

namespace SurvivorUnity.Core
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("Game Settings")]
        [SerializeField] private float gameSpeed = 1f;
        [SerializeField] private bool isPaused = false;

        [Header("Player Reference")]
        [SerializeField] private GameObject player;
        
        [Header("Phase 2.1: Enemy Pool Manager")]
        [SerializeField] private EnemyPoolManager enemyPoolManager;
        [SerializeField] private bool usePoolManager = false;

        [Header("Enemy Management")]
        [SerializeField] private List<GameObject> enemies = new List<GameObject>();
        [SerializeField] private List<GameObject> projectiles = new List<GameObject>();
        [SerializeField] private List<GameObject> experienceOrbs = new List<GameObject>();

        private float gameTime = 0f;
        private int totalKills = 0;
        private int bossesKilled = 0;

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
        }

        private void Start()
        {
            InitializeGame();
        }

        private void Update()
        {
            if (isPaused) return;

            gameTime += Time.deltaTime;

            UpdatePhase1();
            UpdatePhase2();
            UpdatePhase3();
            UpdatePhase4();
        }

        private void InitializeGame()
        {
            gameTime = 0f;
            totalKills = 0;
            bossesKilled = 0;
            isPaused = false;
        }

        private void UpdatePhase1()
        {
            if (usePoolManager && enemyPoolManager != null)
            {
                enemyPoolManager.SpawnEnemy(gameTime);
            }
        }

        private void UpdatePhase2()
        {
        }

        private void UpdatePhase3()
        {
        }

        private void UpdatePhase4()
        {
        }

        public void TogglePause()
        {
            isPaused = !isPaused;
        }

        public void GameOver()
        {
            isPaused = true;
        }

        public void AddExp(int amount)
        {
            if (player != null)
            {
                PlayerController playerController = player.GetComponent<PlayerController>();
                if (playerController != null)
                {
                    playerController.AddExp(amount);
                }
            }
        }

        public void AddKill(bool isBoss = false)
        {
            totalKills++;
            if (isBoss)
            {
                bossesKilled++;
            }
        }

        public float GameTime => gameTime;
        public int TotalKills => totalKills;
        public int BossesKilled => bossesKilled;
        public GameObject Player => player;
        public List<GameObject> Enemies
        {
            get
            {
                if (EnemyPool.Instance != null)
                {
                    return EnemyPool.Instance.ActiveEnemies;
                }
                return new List<GameObject>();
            }
        }
        
        public List<GameObject> Projectiles
        {
            get
            {
                if (ProjectilePool.Instance != null)
                {
                    return ProjectilePool.Instance.ActiveProjectiles;
                }
                return new List<GameObject>();
            }
        }
        
        public List<GameObject> ExperienceOrbs
        {
            get
            {
                if (ExpOrbPool.Instance != null)
                {
                    return ExpOrbPool.Instance.ActiveOrbs;
                }
                return new List<GameObject>();
            }
        }
    }
}