using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using Survivor.Systems;

namespace SurvivorUnity.Core
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        public static bool IsPaused => Instance != null && Instance.isPaused;

        [Header("Game Settings")]
        [SerializeField] private float gameSpeed = 1f;
        [SerializeField] private bool isPaused = false;

        [Header("Player Reference")]
        [SerializeField] private GameObject player;
        
        [Header("Phase 2.1: Enemy Pool Manager")]
        public EnemyPoolManager enemyPoolManager;
        public bool usePoolManager = false;

        [Header("Enemy Management")]
        [SerializeField] private List<GameObject> enemies = new List<GameObject>();
        [SerializeField] private List<GameObject> projectiles = new List<GameObject>();
        [SerializeField] private List<GameObject> experienceOrbs = new List<GameObject>();

        private float gameTime = 0f;
        private int totalKills = 0;
        private int bossesKilled = 0;
        private bool gameOverShown = false;
        private GameObject gameOverCanvas;

        private void Awake()
        {
            Debug.LogWarning($"[GameManager.Awake] Called! Instance before={Instance}");
            if (Instance == null)
            {
                Instance = this;
                Debug.LogWarning("[GameManager.Awake] Instance set to this");
            }
            else
            {
                Debug.LogWarning("[GameManager.Awake] Instance already exists, destroying this");
                Destroy(gameObject);
            }
            Debug.LogWarning($"[GameManager.Awake] Instance after={Instance}");
        }

        private void Start()
        {
            InitializeGame();
        }

        private void Update()
        {
            if (isPaused)
            {
                if (Input.GetKeyDown(KeyCode.R) && gameOverShown)
                {
                    RestartGame();
                }
                return;
            }

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
            if (isPaused && gameOverShown) return;

            Debug.LogError("=============================");
            Debug.LogError("[GameManager.GameOver] GAME OVER CALLED!");
            isPaused = true;
            Time.timeScale = 0f;

            if (gameOverShown) return;
            gameOverShown = true;

            var canvas = GameObject.Find("GameOverCanvas");
            GameObject panel = canvas != null ? canvas.transform.Find("GameOverPanel")?.gameObject : null;

            if (panel != null)
            {
                panel.SetActive(true);
                Debug.LogError("[GameManager.GameOver] GameOverPanel activated!");

                var statsText = panel.transform.Find("StatsText")?.GetComponent<UnityEngine.UI.Text>();
                if (statsText != null)
                {
                    statsText.text = $"存活時間: {gameTime:F1} 秒\n擊殺數: {totalKills}";
                }
            }
            else
            {
                Debug.LogError("[GameManager.GameOver] GameOverPanel not found!");
            }

            Debug.LogError("[GameManager.GameOver] 按 R 键重新开始");
            Debug.LogError("=============================");
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

        private void RestartGame()
        {
            Debug.LogWarning("[GameManager.RestartGame] Restarting game...");

            var canvas = GameObject.Find("GameOverCanvas");
            var panel = canvas != null ? canvas.transform.Find("GameOverPanel")?.gameObject : null;
            if (panel != null) panel.SetActive(false);

            Time.timeScale = 1f;
            isPaused = false;
            gameOverShown = false;
            gameTime = 0f;
            totalKills = 0;
            bossesKilled = 0;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}