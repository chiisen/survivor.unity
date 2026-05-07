using UnityEngine;
using UnityEditor;
using Survivor.Config;
using System.Collections.Generic;

namespace SurvivorUnity.Editor
{
    public class CreateEnemyConfigs : EditorWindow
    {
        [MenuItem("Survivor/Create Enemy Configs")]
        public static void CreateConfigs()
        {
            string configPath = "Assets/Config/Enemy";
            
            if (!AssetDatabase.IsValidFolder("Assets/Config"))
                AssetDatabase.CreateFolder("Assets", "Config");
            
            if (!AssetDatabase.IsValidFolder(configPath))
                AssetDatabase.CreateFolder("Assets/Config", "Enemy");
            
            var normalConfig = CreateNormalEnemyConfig(configPath);
            var fastConfig = CreateFastEnemyConfig(configPath);
            var tankConfig = CreateTankEnemyConfig(configPath);
            var rangedConfig = CreateRangedEnemyConfig(configPath);
            
            var spawnSettings = CreateInstance<EnemySpawnSettings>();
            spawnSettings.spawnInterval = 2.0f;
            spawnSettings.maxActiveEnemies = 50;
            spawnSettings.initialPoolSize = 10;
            spawnSettings.enemyConfigs = new List<EnemyTypeConfig>
            {
                normalConfig, fastConfig, tankConfig, rangedConfig
            };
            
            AssetDatabase.CreateAsset(spawnSettings, $"{configPath}/EnemySpawnSettings.asset");
            
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            
            Debug.Log("Enemy configs created successfully!");
            
            Selection.activeObject = spawnSettings;
        }
        
        private static EnemyTypeConfig CreateNormalEnemyConfig(string path)
        {
            var config = CreateInstance<EnemyTypeConfig>();
            config.enemyType = EnemyType.Normal;
            config.moveSpeed = 2f;
            config.maxHealth = 10;
            config.expValue = 5;
            config.minSpawnTime = 0f;
            config.enemyColor = Color.red;
            
            var curve = new AnimationCurve();
            curve.AddKey(0f, 1f);
            curve.AddKey(60f, 0.5f);
            curve.AddKey(300f, 0.2f);
            config.weightCurve = curve;
            
            AssetDatabase.CreateAsset(config, $"{path}/NormalEnemyConfig.asset");
            return config;
        }
        
        private static EnemyTypeConfig CreateFastEnemyConfig(string path)
        {
            var config = CreateInstance<EnemyTypeConfig>();
            config.enemyType = EnemyType.Fast;
            config.moveSpeed = 4f;
            config.maxHealth = 5;
            config.expValue = 3;
            config.minSpawnTime = 30f;
            config.enemyColor = Color.yellow;
            
            var curve = new AnimationCurve();
            curve.AddKey(0f, 0f);
            curve.AddKey(30f, 0.3f);
            curve.AddKey(60f, 0.6f);
            curve.AddKey(300f, 0.4f);
            config.weightCurve = curve;
            
            AssetDatabase.CreateAsset(config, $"{path}/FastEnemyConfig.asset");
            return config;
        }
        
        private static EnemyTypeConfig CreateTankEnemyConfig(string path)
        {
            var config = CreateInstance<EnemyTypeConfig>();
            config.enemyType = EnemyType.Tank;
            config.moveSpeed = 1f;
            config.maxHealth = 30;
            config.expValue = 15;
            config.minSpawnTime = 60f;
            config.enemyColor = new Color(0.5f, 0f, 0.5f);
            
            var curve = new AnimationCurve();
            curve.AddKey(0f, 0f);
            curve.AddKey(60f, 0.2f);
            curve.AddKey(120f, 0.5f);
            curve.AddKey(300f, 0.6f);
            config.weightCurve = curve;
            
            AssetDatabase.CreateAsset(config, $"{path}/TankEnemyConfig.asset");
            return config;
        }
        
        private static EnemyTypeConfig CreateRangedEnemyConfig(string path)
        {
            var config = CreateInstance<EnemyTypeConfig>();
            config.enemyType = EnemyType.Ranged;
            config.moveSpeed = 1.5f;
            config.maxHealth = 8;
            config.expValue = 8;
            config.minSpawnTime = 90f;
            config.attackRange = 5f;
            config.attackInterval = 2f;
            config.enemyColor = Color.cyan;
            
            var curve = new AnimationCurve();
            curve.AddKey(0f, 0f);
            curve.AddKey(90f, 0.1f);
            curve.AddKey(180f, 0.3f);
            curve.AddKey(300f, 0.5f);
            config.weightCurve = curve;
            
            AssetDatabase.CreateAsset(config, $"{path}/RangedEnemyConfig.asset");
            return config;
        }
    }
}