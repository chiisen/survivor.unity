using UnityEngine;
using UnityEditor;

public class ManualSetupPlayer : EditorWindow
{
    [MenuItem("Tools/Manual Setup Player (Fixed)")]
    public static void Setup()
    {
        // 1. Force refresh assets
        AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
        Debug.Log("[1/6] AssetDatabase refreshed");
        
        // 2. Wait for assets to load
        System.Threading.Thread.Sleep(500);
        
        // 3. Find sprite
        Sprite playerSprite = null;
        string[] possiblePaths = {
            "Assets/Sprites/player_character.png",
            "Assets/Sprites/player_warrior.png",
            "Assets/Sprites/player_mage.png"
        };
        
        foreach (string path in possiblePaths)
        {
            playerSprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
            if (playerSprite != null)
            {
                Debug.Log("[2/6] Found sprite: " + path);
                break;
            }
        }
        
        if (playerSprite == null)
        {
            Debug.LogError("[ERROR] No player sprites found!");
            Debug.Log("Checked paths:");
            foreach (string path in possiblePaths)
            {
                Debug.Log("  " + path);
            }
            return;
        }
        
        // 4. Find Player
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("[ERROR] Player GameObject not found!");
            return;
        }
        Debug.Log("[3/6] Found Player GameObject");
        
        // 5. Update SpriteRenderer
        SpriteRenderer sr = player.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.sprite = playerSprite;
            Debug.Log("[4/6] SpriteRenderer updated");
        }
        
        // 6. Update scale
        player.transform.localScale = new Vector3(5f, 5f, 1f);
        Debug.Log("[5/6] Scale set to (5, 5, 1)");
        
        // 7. Update collider
        CircleCollider2D collider = player.GetComponent<CircleCollider2D>();
        if (collider != null)
        {
            collider.radius = 0.1f; // Original 0.2, scale 5x = 1.0, so use 0.1
            Debug.Log("[6/6] Collider radius = 0.1");
        }
        
        EditorUtility.SetDirty(player);
        EditorApplication.SaveScene();
        
        Debug.Log("\n[SUCCESS] Player setup complete!");
        Debug.Log("Sprite: " + playerSprite.name);
        Debug.Log("Check Game View now!");
    }
}
