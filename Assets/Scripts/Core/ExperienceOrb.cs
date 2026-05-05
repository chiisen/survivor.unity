using UnityEngine;

namespace SurvivorUnity.Core
{
    public class ExperienceOrb : MonoBehaviour
    {
        private int expValue;
        private Rigidbody2D rb;
        private bool isBeingPickedUp = false;
        private float pickupSpeed = 300f;
        
        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
        }
        
        private void Update()
        {
            CheckPickupRange();
        }
        
        private void FixedUpdate()
        {
            if (isBeingPickedUp)
            {
                MoveTowardsPlayer();
            }
        }
        
        public void Initialize(int value)
        {
            expValue = value;
            isBeingPickedUp = false;
        }
        
        private void CheckPickupRange()
        {
            if (GameManager.Instance == null || GameManager.Instance.Player == null) return;
            
            PlayerController player = GameManager.Instance.Player.GetComponent<PlayerController>();
            if (player == null) return;
            
            float distance = Vector2.Distance(transform.position, GameManager.Instance.Player.transform.position);
            
            if (distance <= player.PickupRange)
            {
                isBeingPickedUp = true;
            }
        }
        
        private void MoveTowardsPlayer()
        {
            if (GameManager.Instance == null || GameManager.Instance.Player == null) return;
            
            Vector2 direction = (GameManager.Instance.Player.transform.position - transform.position).normalized;
            rb.MovePosition(rb.position + direction * pickupSpeed * Time.fixedDeltaTime);
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                PlayerController player = other.GetComponent<PlayerController>();
                if (player != null)
                {
                    player.AddExp(expValue);
                }
                
                if (ExpOrbPool.Instance != null)
                {
                    ExpOrbPool.Instance.ReturnOrb(gameObject);
                }
            }
        }
    }
}