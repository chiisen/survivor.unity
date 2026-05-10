using UnityEngine;
using UnityEditor;

namespace SurvivorUnity.Editor
{
    public class SetupPlayerShooting : EditorWindow
    {
        [MenuItem("Survivor/Setup Player Shooting")]
        public static void Setup()
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            
            if (player == null)
            {
                Debug.LogError("[SetupPlayerShooting] Player not found! Make sure Player GameObject has 'Player' tag.");
                return;
            }
            
            var controller = player.GetComponent<SurvivorUnity.Core.PlayerController>();
            
            if (controller == null)
            {
                Debug.LogError("[SetupPlayerShooting] PlayerController component not found on Player!");
                return;
            }
            
            var projectilePrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/ProjectilePrefab.prefab");
            
            if (projectilePrefab == null)
            {
                Debug.LogError("[SetupPlayerShooting] ProjectilePrefab not found at Assets/Prefabs/ProjectilePrefab.prefab");
                return;
            }
            
            controller.projectilePrefab = projectilePrefab;
            controller.autoFire = true;
            controller.autoFireInterval = 0.5f;
            
            EditorUtility.SetDirty(player);
            
            Debug.Log($"[SetupPlayerShooting] ✅ Player shooting configured successfully!");
            Debug.Log($"  - Projectile Prefab: {projectilePrefab.name}");
            Debug.Log($"  - Auto Fire: {controller.autoFire}");
            Debug.Log($"  - Fire Interval: {controller.autoFireInterval}s");
            Debug.Log($"  - Damage: {controller.Damage}");
            
            Selection.activeGameObject = player;
            
            Debug.Log("[SetupPlayerShooting] 🎮 Ready! Enter Play Mode to test auto-shooting.");
        }
        
        [MenuItem("Survivor/Test Player Shooting (Play Mode)")]
        public static void Test()
        {
            if (!EditorApplication.isPlaying)
            {
                Debug.LogWarning("[SetupPlayerShooting] Please enter Play Mode first (Ctrl+P)");
                return;
            }
            
            var player = GameObject.FindGameObjectWithTag("Player");
            var controller = player?.GetComponent<SurvivorUnity.Core.PlayerController>();
            
            if (controller == null)
            {
                Debug.LogError("[SetupPlayerShooting] Player not found!");
                return;
            }
            
            if (controller.projectilePrefab == null)
            {
                Debug.LogError("[SetupPlayerShooting] projectilePrefab is null! Run 'Survivor/Setup Player Shooting' first.");
                return;
            }
            
            Debug.Log($"[SetupPlayerShooting] ✅ Shooting configuration verified:");
            Debug.Log($"  - Projectile Prefab: {controller.projectilePrefab.name}");
            Debug.Log($"  - Auto Fire: {controller.autoFire}");
            Debug.Log($"  - Fire Interval: {controller.autoFireInterval}s");
            
            var projectiles = GameObject.FindGameObjectsWithTag("Projectile");
            Debug.Log($"  - Active Projectiles: {projectiles.Length}");
        }
    }
}