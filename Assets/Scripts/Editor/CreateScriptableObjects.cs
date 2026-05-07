using UnityEngine;
using UnityEditor;
using Survivor.Config;
using System.Collections.Generic;

namespace SurvivorUnity.Editor
{
    public class CreateScriptableObjects : EditorWindow
    {
        [MenuItem("Survivor/Create ScriptableObjects")]
        public static void Create()
        {
            if (!AssetDatabase.IsValidFolder("Assets/Config"))
                AssetDatabase.CreateFolder("Assets", "Config");
            
            if (!AssetDatabase.IsValidFolder("Assets/Config/Enemy"))
                AssetDatabase.CreateFolder("Assets/Config", "Enemy");
            
            var normalConfig = CreateEnemyConfig(EnemyType.Normal, 2f, 10, 5, Color.red, 0f);
            var fastConfig = CreateEnemyConfig(EnemyType.Fast, 4f, 5, 3, Color.yellow, 30f);
            var tankConfig = CreateEnemyConfig(EnemyType.Tank, 1f, 30, 15, new Color(0.5f, 0f, 0.5f, 1f), 60f);
            var rangedConfig = CreateEnemyConfig(EnemyType.Ranged, 1.5f, 8, 8, Color.cyan, 90f);
            
            var spawnSettings = CreateInstance<EnemySpawnSettings>();
            spawnSettings.spawnInterval = 2.0f;
            spawnSettings.maxActiveEnemies = 50;
            spawnSettings.initialPoolSize = 10;
            spawnSettings.enemyConfigs = new List<EnemyTypeConfig> { normalConfig, fastConfig, tankConfig, rangedConfig };
            
            AssetDatabase.CreateAsset(spawnSettings, "Assets/Config/Enemy/EnemySpawnSettings.asset");
            
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            
            Debug.Log("ScriptableObjects created successfully!");
            Selection.activeObject = spawnSettings;
        }
        
        private static EnemyTypeConfig CreateEnemyConfig(EnemyType type, float moveSpeed, int maxHealth, int expValue, Color color, float minSpawnTime)
        {
            var config = CreateInstance<EnemyTypeConfig>();
            config.enemyType = type;
            config.moveSpeed = moveSpeed;
            config.maxHealth = maxHealth;
            config.expValue = expValue;
            config.enemyColor = color;
            config.minSpawnTime = minSpawnTime;
            
            var curve = new AnimationCurve();
            
            if (type == EnemyType.Normal)
            {
                curve.AddKey(0f, 1f);
                curve.AddKey(60f, 0.5f);
                curve.AddKey(300f, 0.2f);
            }
            else if (type == EnemyType.Fast)
            {
                curve.AddKey(0f, 0f);
                curve.AddKey(30f, 0.3f);
                curve.AddKey(60f, 0.6f);
                curve.AddKey(300f, 0.4f);
            }
            else if (type == EnemyType.Tank)
            {
                curve.AddKey(0f, 0f);
                curve.AddKey(60f, 0.2f);
                curve.AddKey(120f, 0.5f);
                curve.AddKey(300f, 0.6f);
            }
            else if (type == EnemyType.Ranged)
            {
                curve.AddKey(0f, 0f);
                curve.AddKey(90f, 0.1f);
                curve.AddKey(180f, 0.3f);
                curve.AddKey(300f, 0.5f);
            }
            
            config.weightCurve = curve;
            
            string path = $"Assets/Config/Enemy/{type}EnemyConfig.asset";
            AssetDatabase.CreateAsset(config, path);
            
            return config;
        }
    }
}