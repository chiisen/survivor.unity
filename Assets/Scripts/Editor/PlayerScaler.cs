#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

public class PlayerScaler : EditorWindow
{
    [MenuItem("Tools/Scale Player x5")]
    public static void ScalePlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            player.transform.localScale = new Vector3(5f, 5f, 1f);
            EditorUtility.SetDirty(player);
            Debug.Log("✅ Player scale = (5, 5, 1). Size ≈ 1x1 units");
        }
    }
}
#endif
