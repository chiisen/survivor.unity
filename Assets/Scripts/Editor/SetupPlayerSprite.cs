using UnityEngine;
using UnityEditor;
using System.IO;

public class SetupPlayerSprite : EditorWindow
{
    [MenuItem("Tools/Setup Player Sprite")]
    public static void Setup()
    {
        // 1. Refresh AssetDatabase to generate .meta files
        AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
        
        // 2. Load player sprite
        string spritePath = "Assets/Sprites/player_character.png";
        Sprite playerSprite = AssetDatabase.LoadAssetAtPath<Sprite>(spritePath);
        
        if (playerSprite == null)
        {
            Debug.LogError("[ERROR] Player sprite not found: " + spritePath);
            Debug.Log("\n[SOLUTION] Please ensure PNG files exist in Assets/Sprites/");
            return;
        }
        
        // 3. Set sprite Pixels Per Unit
        string metaPath = spritePath + ".meta";
        if (File.Exists(metaPath))
        {
            TextureImporter importer = AssetImporter.GetAtPath(spritePath) as TextureImporter;
            if (importer != null)
            {
                importer.spritePixelsToUnits = 32;
                importer.textureType = TextureImporterType.Sprite;
                importer.spriteImportMode = SpriteImportMode.Single;
                EditorUtility.SetDirty(importer);
                importer.SaveAndReimport();
                Debug.Log("[OK] Sprite Pixels Per Unit = 32");
            }
        }
        
        // 4. Update Player SpriteRenderer
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            SpriteRenderer sr = player.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.sprite = playerSprite;
                Debug.Log("[OK] Player sprite updated to: player_character.png");
                
                // 5. Adjust scale
                player.transform.localScale = new Vector3(1f, 1f, 1f);
                Debug.Log("[OK] Player scale = (1, 1, 1)");
                
                // 6. Update collider
                CircleCollider2D collider = player.GetComponent<CircleCollider2D>();
                if (collider != null)
                {
                    // Sprite is 64x64, PixelsPerUnit=32, so sprite is 2x2 units
                    // Collider radius should match sprite size
                    collider.radius = 1f; // radius = 1 unit, diameter = 2 units
                    Debug.Log("[OK] Collider radius = 1 (matches sprite 2x2 units)");
                }
                
                EditorUtility.SetDirty(player);
            }
        }
        
        // 7. Save scene
        SceneView sv = SceneView.lastActiveSceneView;
        if (sv != null)
        {
            EditorApplication.SaveScene();
            Debug.Log("[OK] Scene saved");
        }
        
        Debug.Log("\n[COMPLETE] Player sprite setup finished!");
        Debug.Log("\n[RESULT]");
        Debug.Log("  Sprite: player_character.png (64x64 -> 2x2 units)");
        Debug.Log("  Scale: (1, 1, 1)");
        Debug.Log("  Collider radius: 1 unit");
        Debug.Log("\n[VIEW] Check Game View to see your new player!");
    }
}
