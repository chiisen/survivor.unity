using UnityEngine;
using UnityEditor;

public class SetupPuzzleFloor
{
    [MenuItem("Tools/Setup Puzzle Floor")]
    public static void Setup()
    {
        GameObject puzzleFloor = GameObject.Find("PuzzleFloor");
        if (puzzleFloor == null)
        {
            Debug.LogError("PuzzleFloor not found!");
            return;
        }

        Sprite floorSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/floor_tileset.png");
        if (floorSprite == null)
        {
            Debug.LogError("Floor sprite not found!");
            return;
        }

        int index = 0;
        foreach (Transform child in puzzleFloor.transform)
        {
            SpriteRenderer sr = child.GetComponent<SpriteRenderer>();
            if (sr != null && sr.sprite == null)
            {
                sr.sprite = floorSprite;
                
                // 計算網格位置 (10x5)
                int col = index % 10;
                int row = index / 10;
                float x = -9 + col * 2;
                float y = 4 - row * 2;
                
                child.localPosition = new Vector3(x, y, 0);
                child.localScale = new Vector3(2, 2, 1);
                
                index++;
            }
        }

        EditorUtility.SetDirty(puzzleFloor);
        AssetDatabase.SaveAssets();
        Debug.Log($"Setup complete! {index} tiles configured.");
    }
}
