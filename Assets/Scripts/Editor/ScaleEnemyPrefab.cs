using UnityEngine;
using UnityEditor;

namespace SurvivorUnity.Editor
{
    public class ScaleEnemyPrefab : EditorWindow
    {
        [MenuItem("Survivor/Scale Enemy Prefab 4x")]
        public static void Scale()
        {
            string prefabPath = "Assets/Prefabs/EnemyPrefab.prefab";
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            
            if (prefab == null)
            {
                Debug.LogError($"[ScaleEnemyPrefab] Prefab not found at {prefabPath}");
                return;
            }
            
            var transform = prefab.transform;
            transform.localScale = new Vector3(4f, 4f, 1f);
            
            var collider = prefab.GetComponent<CircleCollider2D>();
            if (collider != null)
            {
                collider.radius = 0.32f; // 0.08 * 4 = 0.32
            }
            
            EditorUtility.SetDirty(prefab);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            
            Debug.Log($"[ScaleEnemyPrefab] ✅ EnemyPrefab scaled to {transform.localScale}");
            Debug.Log($"[ScaleEnemyPrefab] Collider radius: {collider?.radius}");
            
            Selection.activeObject = prefab;
        }
        
        [MenuItem("Survivor/Scale All Enemy Prefabs 4x")]
        public static void ScaleAll()
        {
            string[] prefabPaths = {
                "Assets/Prefabs/EnemyPrefab.prefab",
                "Assets/Prefabs/FastEnemyPrefab.prefab",
                "Assets/Prefabs/TankEnemyPrefab.prefab",
                "Assets/Prefabs/RangedEnemyPrefab.prefab"
            };
            
            foreach (string path in prefabPaths)
            {
                var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (prefab == null) continue;
                
                prefab.transform.localScale = new Vector3(4f, 4f, 1f);
                
                var collider = prefab.GetComponent<CircleCollider2D>();
                if (collider != null)
                {
                    collider.radius = 0.32f;
                }
                
                EditorUtility.SetDirty(prefab);
                Debug.Log($"[ScaleEnemyPrefab] ✅ {prefab.name} scaled to (4, 4, 1)");
            }
            
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            
            Debug.Log("[ScaleEnemyPrefab] 🎮 All enemy prefabs scaled successfully!");
        }
    }
}