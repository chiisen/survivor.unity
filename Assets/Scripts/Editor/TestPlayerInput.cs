using UnityEngine;
using UnityEditor;

namespace SurvivorUnity.Editor
{
    public class TestPlayerInput : EditorWindow
    {
        [MenuItem("Survivor/Test Player Input")]
        public static void Test()
        {
            Debug.Log("=== Testing Player Input System ===");
            
            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");
            
            Debug.Log($"Horizontal Input: {h}");
            Debug.Log($"Vertical Input: {v}");
            Debug.Log($"Combined Movement: ({h}, {v})");
            
            var player = GameObject.Find("Player");
            if (player != null)
            {
                var controller = player.GetComponent<SurvivorUnity.Core.PlayerController>();
                if (controller != null)
                {
                    Debug.Log($"Player Speed: {controller.Speed}");
                    Debug.Log($"Player Position: {player.transform.position}");
                }
            }
            
            if (h != 0 || v != 0)
            {
                Debug.Log("✅ Input system is working! Movement detected.");
            }
            else
            {
                Debug.LogWarning("⚠️ No input detected. Press WASD or arrow keys while testing.");
            }
        }
    }
}