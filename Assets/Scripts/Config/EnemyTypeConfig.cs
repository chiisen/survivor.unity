using UnityEngine;

namespace Survivor.Config
{
    public enum EnemyType
    {
        Normal,
        Fast,
        Tank,
        Ranged
    }

    [CreateAssetMenu(fileName = "EnemyTypeConfig", menuName = "Survivor/EnemyTypeConfig")]
    public class EnemyTypeConfig : ScriptableObject
    {
        [Header("Enemy Identity")]
        public EnemyType enemyType;

        [Header("Base Stats")]
        public float moveSpeed;
        public int maxHealth;
        public int expValue;

        [Header("Spawn Weight")]
        public AnimationCurve weightCurve;
        public float minSpawnTime;

        [Header("Type-specific Stats (Ranged)")]
        public float attackRange;
        public float attackInterval;
        public GameObject projectilePrefab;

        [Header("Visual")]
        public Color enemyColor;
    }
}