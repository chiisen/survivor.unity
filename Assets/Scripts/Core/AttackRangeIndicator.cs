using UnityEngine;

namespace SurvivorUnity.Core
{
    public class AttackRangeIndicator : MonoBehaviour
    {
        private PlayerController playerController;
        private GameObject rangeCircle;
        private SpriteRenderer spriteRenderer;
        
        [RuntimeInitializeOnLoadMethod]
        private static void AutoAddToPlayer()
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null && player.GetComponent<AttackRangeIndicator>() == null)
            {
                player.AddComponent<AttackRangeIndicator>();
                Debug.Log("[AttackRangeIndicator] Auto-added to Player");
            }
        }
        
        private void Start()
        {
            playerController = GetComponent<PlayerController>();
            CreateRangeCircle();
        }
        
        private void CreateRangeCircle()
        {
            float attackRange = playerController.AttackRange;
            
            rangeCircle = new GameObject("AttackRangeCircle");
            rangeCircle.transform.SetParent(transform, false);
            rangeCircle.transform.localPosition = new Vector3(0, 0, 0.1f);
            
            spriteRenderer = rangeCircle.AddComponent<SpriteRenderer>();
            
            Texture2D texture = new Texture2D(128, 128);
            Color[] colors = new Color[128 * 128];
            
            int centerX = 64;
            int centerY = 64;
            int radius = 60;
            
            for (int y = 0; y < 128; y++)
            {
                for (int x = 0; x < 128; x++)
                {
                    float dist = Mathf.Sqrt((x - centerX) * (x - centerX) + (y - centerY) * (y - centerY));
                    
                    if (dist <= radius && dist >= radius - 3)
                    {
                        colors[y * 128 + x] = new Color(0.5f, 0.5f, 1f, 0.3f);
                    }
                    else
                    {
                        colors[y * 128 + x] = new Color(0, 0, 0, 0);
                    }
                }
            }
            
            texture.SetPixels(colors);
            texture.Apply();
            
            Sprite sprite = Sprite.Create(
                texture,
                new Rect(0, 0, 128, 128),
                new Vector2(0.5f, 0.5f),
                128
            );
            
            spriteRenderer.sprite = sprite;
            spriteRenderer.color = new Color(0.5f, 0.5f, 1f, 0.5f);
            spriteRenderer.sortingOrder = -1;
            
            float diameter = attackRange * 2;
            rangeCircle.transform.localScale = new Vector3(diameter, diameter, 1);
            
            Debug.Log($"[AttackRangeIndicator] Circle created: range={attackRange}, diameter={diameter}");
        }
        
        private void Update()
        {
            if (playerController != null && rangeCircle != null)
            {
                float attackRange = playerController.AttackRange;
                float diameter = attackRange * 2;
                
                if (rangeCircle.transform.localScale.x != diameter)
                {
                    rangeCircle.transform.localScale = new Vector3(diameter, diameter, 1);
                    Debug.Log($"[AttackRangeIndicator] Range updated: {attackRange}");
                }
            }
        }
    }
}