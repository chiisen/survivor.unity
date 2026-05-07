using UnityEngine;
using UnityEditor;
using Survivor.Systems;
using Survivor.Config;
using SurvivorUnity.Core;

namespace SurvivorUnity.Editor
{
    public class FixGameManager : EditorWindow
    {
        [MenuItem("Survivor/Fix GameManager Setup")]
        public static void Fix()
        {
            var gameManagerGO = GameObject.Find("GameManager");
            
            if (gameManagerGO == null)
            {
                Debug.LogError("GameManager GameObject not found!");
                return;
            }
            
            var gameManagers = gameManagerGO.GetComponents<GameManager>();
            
            if (gameManagers.Length > 1)
            {
                for (int i = 1; i < gameManagers.Length; i++)
                {
                    Object.DestroyImmediate(gameManagers[i]);
                }
                Debug.Log($"Removed {gameManagers.Length - 1} duplicate GameManager components");
            }
            
            var gm = gameManagerGO.GetComponent<GameManager>();
            var poolManager = gameManagerGO.GetComponent<EnemyPoolManager>();
            
            if (gm != null && poolManager != null)
            {
                gm.enemyPoolManager = poolManager;
                gm.usePoolManager = true;
                // player reference is auto-found in Awake
            }
            
            var spawnSettings = AssetDatabase.LoadAssetAtPath<EnemySpawnSettings>("Assets/Config/Enemy/EnemySpawnSettings.asset");
            
            if (poolManager != null && spawnSettings != null)
            {
                poolManager.spawnSettings = spawnSettings;
            }
            else if (poolManager != null && spawnSettings == null)
            {
                Debug.LogWarning("EnemySpawnSettings.asset not found. Please create it first.");
            }
            
            EditorUtility.SetDirty(gameManagerGO);
            
            Debug.Log("GameManager setup fixed!");
        }
    }
}