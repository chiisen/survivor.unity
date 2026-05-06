using UnityEngine;
using System.Collections.Generic;

namespace Survivor.Config
{
    [CreateAssetMenu(fileName = "EnemySpawnSettings", menuName = "Survivor/EnemySpawnSettings")]
    public class EnemySpawnSettings : ScriptableObject
    {
        [Header("Spawn Control")]
        public float spawnInterval = 2.0f;
        public int maxActiveEnemies = 50;

        [Header("Enemy Type Configs")]
        public List<EnemyTypeConfig> enemyConfigs = new List<EnemyTypeConfig>();

        [Header("Pool Settings")]
        public int initialPoolSize = 10;
    }
}