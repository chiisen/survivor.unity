using UnityEngine;

namespace SurvivorUnity.Core
{
    /// <summary>
    /// Diagnostic script to test player movement directly
    /// </summary>
    public class DiagnosticPlayer : MonoBehaviour
    {
        private Rigidbody2D rb;
        private Vector2 movement;
        private float testSpeed = 5f;
        
        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            Debug.Log($"[DiagnosticPlayer] Awake - Rigidbody2D: {rb != null}");
            Debug.Log($"[DiagnosticPlayer] Rigidbody2D.simulated: {rb?.simulated}");
            Debug.Log($"[DiagnosticPlayer] Rigidbody2D.bodyType: {rb?.bodyType}");
            Debug.Log($"[DiagnosticPlayer] Rigidbody2D.constraints: {rb?.constraints}");
            Debug.Log($"[DiagnosticPlayer] Rigidbody2D.gravityScale: {rb?.gravityScale}");
        }
        
        private void Update()
        {
            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");
            
            movement = new Vector2(h, v);
            
            Debug.Log($"[DiagnosticPlayer] Update - Input: ({h}, {v})");
            Debug.Log($"[DiagnosticPlayer] Update - movement: {movement}");
            Debug.Log($"[DiagnosticPlayer] Update - Position: {rb?.position}");
            
            if (h != 0 || v != 0)
            {
                Debug.Log($"[DiagnosticPlayer] ✅ MOVEMENT DETECTED: ({h}, {v})");
            }
        }
        
        private void FixedUpdate()
        {
            if (rb == null)
            {
                Debug.LogError("[DiagnosticPlayer] ❌ Rigidbody2D is NULL!");
                return;
            }
            
            if (!rb.simulated)
            {
                Debug.LogError("[DiagnosticPlayer] ❌ Rigidbody2D.simulated is FALSE!");
                return;
            }
            
            Vector2 newPos = rb.position + movement * testSpeed * Time.fixedDeltaTime;
            rb.MovePosition(newPos);
            
            Debug.Log($"[DiagnosticPlayer] FixedUpdate - Moved from {rb.position} to {newPos}");
        }
        
        private void OnDisable()
        {
            Debug.LogWarning("[DiagnosticPlayer] Script DISABLED!");
        }
    }
}