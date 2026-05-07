using UnityEngine;
using UnityEditor;
using SurvivorUnity.Core;

namespace SurvivorUnity.Editor
{
    public class SetupEnemyPrefab : EditorWindow
    {
        [MenuItem("Survivor/Setup Enemy Prefabs")]
        public static void Setup()
        {
            string playerSpritePath = "Assets/Sprites/player_character.png";
            var playerSprite = AssetDatabase.LoadAssetAtPath<Sprite>(playerSpritePath);
            
            if (playerSprite == null)
            {
                Debug.LogError($"Failed to load sprite at {playerSpritePath}");
                return;
            }
            
            ConfigureEnemyPrefab("Assets/Prefabs/EnemyPrefab.prefab", playerSprite, Color.red, 3f);
            ConfigureEnemyPrefab("Assets/Prefabs/FastEnemyPrefab.prefab", playerSprite, Color.yellow, 2.5f);
            ConfigureEnemyPrefab("Assets/Prefabs/TankEnemyPrefab.prefab", playerSprite, new Color(0.5f, 0f, 0.5f, 1f), 4f);
            ConfigureEnemyPrefab("Assets/Prefabs/RangedEnemyPrefab.prefab", playerSprite, Color.cyan, 3f);
            
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            
            Debug.Log("Enemy prefabs setup completed!");
        }
        
        private static void ConfigureEnemyPrefab(string prefabPath, Sprite sprite, Color color, float scale)
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            
            if (prefab == null)
            {
                Debug.LogError($"Failed to load prefab at {prefabPath}");
                return;
            }
            
            var spriteRenderer = prefab.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                spriteRenderer.sprite = sprite;
                spriteRenderer.color = color;
                spriteRenderer.sortingOrder = 5;
            }
            
            var transform = prefab.transform;
            transform.localScale = new Vector3(scale, scale, 1f);
            
            var collider = prefab.GetComponent<CircleCollider2D>();
            if (collider != null)
            {
                collider.radius = 0.08f;
            }
            
            var enemyController = prefab.GetComponent<EnemyController>();
            if (enemyController != null)
            {
                EditorUtility.SetDirty(prefab);
            }
            
            EditorUtility.SetDirty(prefab);
            Debug.Log($"Setup {prefabPath} with scale {scale}, color {color}");
        }
    }
}