# Phase 2.1: Enemy Types Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** 实作 4 种敌人类型（Normal/Fast/Tank/Ranged）完整系统，包含 ScriptableObject 配置、多类型 Pool Manager、远程敌人射击机制。

**Architecture:** 4 层架构：GameManager → EnemyPoolManager → EnemyPool → EnemyController。ScriptableObject 配置驱动权重曲线与敌人属性。Ranged Enemy 独立 EnemyAutoFire component 处理射击逻辑。

**Tech Stack:** Unity 2022.3, C#, ScriptableObject, AnimationCurve, Object Pooling, Unity MCP Tools (ai-game-developer_*)

---

## 📁 File Structure Map

**新建文件：**
- `Assets/Scripts/Config/EnemyTypeConfig.cs` - ScriptableObject：单类型敌人配置
- `Assets/Scripts/Config/EnemySpawnSettings.cs` - ScriptableObject：全局生成设置
- `Assets/Scripts/Core/EnemyAutoFire.cs` - Component：Ranged Enemy 射击逻辑
- `Assets/Scripts/Systems/EnemyPoolManager.cs` - Component：多类型 Pool 管理器
- `Assets/Configs/EnemySpawnSettings.asset` - Asset：全局生成配置
- `Assets/Configs/EnemyTypes/NormalEnemyConfig.asset` - Asset：Normal 配置
- `Assets/Configs/EnemyTypes/FastEnemyConfig.asset` - Asset：Fast 配置
- `Assets/Configs/EnemyTypes/TankEnemyConfig.asset` - Asset：Tank 配置
- `Assets/Configs/EnemyTypes/RangedEnemyConfig.asset` - Asset：Ranged 配置
- `Assets/Prefabs/FastEnemyPrefab.prefab` - Prefab：Fast Enemy
- `Assets/Prefabs/TankEnemyPrefab.prefab` - Prefab：Tank Enemy
- `Assets/Prefabs/RangedEnemyPrefab.prefab` - Prefab：Ranged Enemy

**修改文件：**
- `Assets/Scripts/Core/EnemyController.cs` - 修改：读取 Config、类型特定行为
- `Assets/Scripts/Systems/EnemyPool.cs` - 修改：添加 enemyType 标识
- `Assets/Scripts/Core/GameManager.cs` - 修改：整合 EnemyPoolManager

---

## 🎯 Task 1: 创建 EnemyTypeConfig.cs ScriptableObject

**Files:**
- Create: `Assets/Scripts/Config/EnemyTypeConfig.cs`

- [ ] **Step 1: 创建 Config 目录**

Run: `ai-game-developer_assets-create-folder` tool
Parameters:
```
parentFolder: "Assets/Scripts/"
folderName: "Config"
```

Expected: 创建 `Assets/Scripts/Config/` 目录

- [ ] **Step 2: 编写 EnemyTypeConfig.cs**

Write to: `Assets/Scripts/Config/EnemyTypeConfig.cs`

```csharp
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
```

- [ ] **Step 3: Refresh AssetDatabase**

Run: `ai-game-developer_assets-refresh` tool
Parameters:
```
options: "ForceSynchronousImport"
```

Expected: Unity 重新编译，无错误

- [ ] **Step 4: Git commit**

```bash
git add Assets/Scripts/Config/EnemyTypeConfig.cs
git commit -m "feat(config): add EnemyTypeConfig ScriptableObject"
```

---

## 🎯 Task 2: 创建 EnemySpawnSettings.cs ScriptableObject

**Files:**
- Create: `Assets/Scripts/Config/EnemySpawnSettings.cs`

- [ ] **Step 1: 编写 EnemySpawnSettings.cs**

Write to: `Assets/Scripts/Config/EnemySpawnSettings.cs`

```csharp
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
```

- [ ] **Step 2: Refresh AssetDatabase**

Run: `ai-game-developer_assets-refresh` tool
Parameters:
```
options: "ForceSynchronousImport"
```

Expected: Unity 重新编译，无错误

- [ ] **Step 3: Git commit**

```bash
git add Assets/Scripts/Config/EnemySpawnSettings.cs
git commit -m "feat(config): add EnemySpawnSettings ScriptableObject"
```

---

## 🎯 Task 3: 创建 EnemyAutoFire.cs Component

**Files:**
- Create: `Assets/Scripts/Core/EnemyAutoFire.cs`

- [ ] **Step 1: 编写 EnemyAutoFire.cs**

Write to: `Assets/Scripts/Core/EnemyAutoFire.cs`

```csharp
using UnityEngine;

namespace Survivor.Core
{
    public class EnemyAutoFire : MonoBehaviour
    {
        private float attackRange;
        private float attackInterval;
        private GameObject projectilePrefab;
        private Transform playerTransform;
        private float lastAttackTime;

        public void Initialize(float range, float interval, GameObject prefab)
        {
            attackRange = range;
            attackInterval = interval;
            projectilePrefab = prefab;
            playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        }

        private void Update()
        {
            if (playerTransform == null)
                return;

            float distance = Vector2.Distance(transform.position, playerTransform.position);

            if (distance <= attackRange && Time.time - lastAttackTime >= attackInterval)
            {
                FireProjectile();
                lastAttackTime = Time.time;
            }
        }

        private void FireProjectile()
        {
            Vector2 direction = (playerTransform.position - transform.position).normalized;

            var projectile = Survivor.Systems.ProjectilePool.Instance.Spawn(transform.position);

            var projectileController = projectile.GetComponent<Survivor.Core.ProjectileController>();
            if (projectileController != null)
            {
                projectileController.SetDirection(direction);
            }
        }
    }
}
```

- [ ] **Step 2: Refresh AssetDatabase**

Run: `ai-game-developer_assets-refresh` tool
Parameters:
```
options: "ForceSynchronousImport"
```

Expected: Unity 重新编译，无错误

- [ ] **Step 3: Git commit**

```bash
git add Assets/Scripts/Core/EnemyAutoFire.cs
git commit -m "feat(enemy): add EnemyAutoFire component for ranged enemies"
```

---

## 🎯 Task 4: 创建 EnemyPoolManager.cs Component

**Files:**
- Create: `Assets/Scripts/Systems/EnemyPoolManager.cs`

- [ ] **Step 1: 编写 EnemyPoolManager.cs**

Write to: `Assets/Scripts/Systems/EnemyPoolManager.cs`

```csharp
using UnityEngine;
using System.Collections.Generic;
using Survivor.Config;
using Survivor.Core;

namespace Survivor.Systems
{
    public class EnemyPoolManager : MonoBehaviour
    {
        public static EnemyPoolManager Instance { get; private set; }

        [Header("Spawn Settings")]
        public EnemySpawnSettings spawnSettings;

        [Header("Enemy Prefabs")]
        public GameObject normalPrefab;
        public GameObject fastPrefab;
        public GameObject tankPrefab;
        public GameObject rangedPrefab;

        private Dictionary<EnemyType, EnemyPool> pools;
        private List<EnemyTypeConfig> configs;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void Start()
        {
            Initialize();
        }

        private void Initialize()
        {
            pools = new Dictionary<EnemyType, EnemyPool>();
            configs = spawnSettings.enemyConfigs;

            foreach (var config in configs)
            {
                var pool = CreatePoolForType(config.enemyType);
                pools.Add(config.enemyType, pool);
            }
        }

        public GameObject SpawnEnemy(float gameTime)
        {
            var weights = CalculateWeights(gameTime);
            var selectedType = SelectEnemyType(weights);

            var pool = pools[selectedType];
            var enemy = pool.Spawn(GetRandomSpawnPosition());

            var config = GetConfigForType(selectedType);
            var controller = enemy.GetComponent<EnemyController>();
            if (controller != null)
            {
                controller.Initialize(config);
            }

            return enemy;
        }

        public void ReturnEnemy(GameObject enemy)
        {
            var controller = enemy.GetComponent<EnemyController>();
            if (controller != null)
            {
                var pool = pools[controller.enemyType];
                pool.Return(enemy);
            }
        }

        private Dictionary<EnemyType, float> CalculateWeights(float gameTime)
        {
            var weights = new Dictionary<EnemyType, float>();

            foreach (var config in configs)
            {
                if (gameTime >= config.minSpawnTime)
                {
                    float weight = config.weightCurve.Evaluate(gameTime);
                    weights.Add(config.enemyType, weight);
                }
                else
                {
                    weights.Add(config.enemyType, 0f);
                }
            }

            return weights;
        }

        private EnemyType SelectEnemyType(Dictionary<EnemyType, float> weights)
        {
            float totalWeight = 0f;
            foreach (var weight in weights.Values)
            {
                totalWeight += weight;
            }

            if (totalWeight <= 0f)
                return EnemyType.Normal;

            float randomValue = Random.Range(0f, totalWeight);
            float cumulativeWeight = 0f;

            foreach (var kvp in weights)
            {
                cumulativeWeight += kvp.Value;
                if (randomValue <= cumulativeWeight)
                {
                    return kvp.Key;
                }
            }

            return EnemyType.Normal;
        }

        private EnemyPool CreatePoolForType(EnemyType type)
        {
            var poolGO = new GameObject($"EnemyPool_{type}");
            poolGO.transform.SetParent(transform);

            var pool = poolGO.AddComponent<EnemyPool>();
            pool.enemyType = type;
            pool.enemyPrefab = GetPrefabForType(type);
            pool.initialPoolSize = spawnSettings.initialPoolSize;

            return pool;
        }

        private GameObject GetPrefabForType(EnemyType type)
        {
            switch (type)
            {
                case EnemyType.Normal: return normalPrefab;
                case EnemyType.Fast: return fastPrefab;
                case EnemyType.Tank: return tankPrefab;
                case EnemyType.Ranged: return rangedPrefab;
                default: return normalPrefab;
            }
        }

        private EnemyTypeConfig GetConfigForType(EnemyType type)
        {
            foreach (var config in configs)
            {
                if (config.enemyType == type)
                    return config;
            }
            return null;
        }

        private Vector3 GetRandomSpawnPosition()
        {
            float spawnRadius = 15f;
            Vector2 randomDirection = Random.insideUnitCircle.normalized * spawnRadius;
            Vector3 playerPosition = GameObject.FindGameObjectWithTag("Player").transform.position;
            return playerPosition + new Vector3(randomDirection.x, randomDirection.y, 0);
        }
    }
}
```

- [ ] **Step 2: Refresh AssetDatabase**

Run: `ai-game-developer_assets-refresh` tool
Parameters:
```
options: "ForceSynchronousImport"
```

Expected: Unity 重新编译，无错误

- [ ] **Step 3: Git commit**

```bash
git add Assets/Scripts/Systems/EnemyPoolManager.cs
git commit -m "feat(pool): add EnemyPoolManager for multi-type enemy pooling"
```

---

## 🎯 Task 5: 修改 EnemyController.cs

**Files:**
- Modify: `Assets/Scripts/Core/EnemyController.cs`

- [ ] **Step 1: 读取现有 EnemyController.cs**

Run: `ai-game-developer_script-read` tool
Parameters:
```
filePath: "Assets/Scripts/Core/EnemyController.cs"
```

Expected: 获取现有代码内容

- [ ] **Step 2: 修改 EnemyController.cs（添加 Initialize 方法与类型特定行为）**

Write to: `Assets/Scripts/Core/EnemyController.cs`

```csharp
using UnityEngine;
using Survivor.Config;
using Survivor.Systems;

namespace Survivor.Core
{
    public class EnemyController : MonoBehaviour
    {
        public EnemyType enemyType;

        private EnemyTypeConfig config;
        private Rigidbody2D rb;
        private Transform playerTransform;
        private int currentHealth;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
        }

        private void Start()
        {
            playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        }

        public void Initialize(EnemyTypeConfig config)
        {
            this.config = config;
            enemyType = config.enemyType;
            currentHealth = config.maxHealth;

            ApplyTypeSpecificSettings();
        }

        private void ApplyTypeSpecificSettings()
        {
            if (enemyType == EnemyType.Tank)
            {
                GetComponent<CircleCollider2D>().radius = 0.8f;
            }

            if (enemyType == EnemyType.Ranged)
            {
                var autoFire = GetComponent<EnemyAutoFire>();
                if (autoFire != null)
                {
                    autoFire.Initialize(config.attackRange, config.attackInterval, config.projectilePrefab);
                }
            }
        }

        private void Update()
        {
            if (playerTransform == null || config == null)
                return;

            float distance = Vector2.Distance(transform.position, playerTransform.position);

            if (enemyType != EnemyType.Ranged)
            {
                Vector2 direction = (playerTransform.position - transform.position).normalized;
                rb.velocity = direction * config.moveSpeed;
            }
            else
            {
                if (distance > config.attackRange)
                {
                    Vector2 direction = (playerTransform.position - transform.position).normalized;
                    rb.velocity = direction * config.moveSpeed;
                }
                else
                {
                    rb.velocity = Vector2.zero;
                }
            }
        }

        public void TakeDamage(int damage)
        {
            currentHealth -= damage;
            if (currentHealth <= 0)
            {
                Die();
            }
        }

        private void Die()
        {
            ExperienceOrb.SpawnExpOrb(transform.position, config.expValue);
            EnemyPoolManager.Instance.ReturnEnemy(gameObject);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Projectile"))
            {
                var projectile = other.GetComponent<ProjectileController>();
                if (projectile != null)
                {
                    TakeDamage(projectile.damage);
                    ProjectilePool.Instance.Return(other.gameObject);
                }
            }
        }
    }
}
```

- [ ] **Step 3: Refresh AssetDatabase**

Run: `ai-game-developer_assets-refresh` tool
Parameters:
```
options: "ForceSynchronousImport"
```

Expected: Unity 重新编译，无错误

- [ ] **Step 4: Git commit**

```bash
git add Assets/Scripts/Core/EnemyController.cs
git commit -m "feat(enemy): modify EnemyController to support type-specific behaviors"
```

---

## 🎯 Task 6: 修改 EnemyPool.cs

**Files:**
- Modify: `Assets/Scripts/Systems/EnemyPool.cs`

- [ ] **Step 1: 读取现有 EnemyPool.cs**

Run: `ai-game-developer_script-read` tool
Parameters:
```
filePath: "Assets/Scripts/Systems/EnemyPool.cs"
```

Expected: 获取现有代码内容

- [ ] **Step 2: 修改 EnemyPool.cs（添加 enemyType 标识）**

Write to: `Assets/Scripts/Systems/EnemyPool.cs`

```csharp
using UnityEngine;
using System.Collections.Generic;
using Survivor.Config;

namespace Survivor.Systems
{
    public class EnemyPool : MonoBehaviour
    {
        public EnemyType enemyType;
        public GameObject enemyPrefab;
        public int initialPoolSize;

        private Queue<GameObject> pool;
        private List<GameObject> activeObjects;

        private void Awake()
        {
            pool = new Queue<GameObject>();
            activeObjects = new List<GameObject>();
        }

        private void Start()
        {
            for (int i = 0; i < initialPoolSize; i++)
            {
                var enemy = Instantiate(enemyPrefab, transform);
                enemy.SetActive(false);
                pool.Enqueue(enemy);
            }
        }

        public GameObject Spawn(Vector3 position)
        {
            GameObject enemy;

            if (pool.Count > 0)
            {
                enemy = pool.Dequeue();
            }
            else
            {
                enemy = Instantiate(enemyPrefab, transform);
            }

            enemy.transform.position = position;
            enemy.SetActive(true);
            activeObjects.Add(enemy);

            return enemy;
        }

        public void Return(GameObject enemy)
        {
            enemy.SetActive(false);
            enemy.transform.SetParent(transform);
            pool.Enqueue(enemy);
            activeObjects.Remove(enemy);
        }

        public int ActiveCount()
        {
            return activeObjects.Count;
        }
    }
}
```

- [ ] **Step 3: Refresh AssetDatabase**

Run: `ai-game-developer_assets-refresh` tool
Parameters:
```
options: "ForceSynchronousImport"
```

Expected: Unity 重新编译，无错误

- [ ] **Step 4: Git commit**

```bash
git add Assets/Scripts/Systems/EnemyPool.cs
git commit -m "feat(pool): modify EnemyPool to support enemyType identification"
```

---

## 🎯 Task 7: 修改 GameManager.cs

**Files:**
- Modify: `Assets/Scripts/Core/GameManager.cs`

- [ ] **Step 1: 读取现有 GameManager.cs**

Run: `ai-game-developer_script-read` tool
Parameters:
```
filePath: "Assets/Scripts/Core/GameManager.cs"
```

Expected: 获取现有代码内容

- [ ] **Step 2: 修改 GameManager.cs（整合 EnemyPoolManager）**

Write to: `Assets/Scripts/Core/GameManager.cs`

```csharp
using UnityEngine;
using Survivor.Systems;

namespace Survivor.Core
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("Game State")]
        public float gameTime;
        public bool isGameOver;

        [Header("Enemy Spawn Settings")]
        public EnemyPoolManager enemyPoolManager;
        public float spawnInterval = 2.0f;
        public int maxActiveEnemies = 50;

        private float nextSpawnTime;
        private int activeEnemies;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void Start()
        {
            nextSpawnTime = Time.time + spawnInterval;
        }

        private void Update()
        {
            if (isGameOver)
                return;

            gameTime += Time.deltaTime;

            if (Time.time >= nextSpawnTime && activeEnemies < maxActiveEnemies)
            {
                enemyPoolManager.SpawnEnemy(gameTime);
                nextSpawnTime = Time.time + spawnInterval;
                activeEnemies++;
            }
        }

        public void OnEnemyDeath()
        {
            activeEnemies--;
        }

        public void EndGame()
        {
            isGameOver = true;
        }
    }
}
```

- [ ] **Step 3: Refresh AssetDatabase**

Run: `ai-game-developer_assets-refresh` tool
Parameters:
```
options: "ForceSynchronousImport"
```

Expected: Unity 重新编译，无错误

- [ ] **Step 4: Git commit**

```bash
git add Assets/Scripts/Core/GameManager.cs
git commit -m "feat(game): integrate EnemyPoolManager into GameManager"
```

---

## 🎯 Task 8: 创建 Config Assets

**Files:**
- Create: `Assets/Configs/EnemySpawnSettings.asset`
- Create: `Assets/Configs/EnemyTypes/NormalEnemyConfig.asset`
- Create: `Assets/Configs/EnemyTypes/FastEnemyConfig.asset`
- Create: `Assets/Configs/EnemyTypes/TankEnemyConfig.asset`
- Create: `Assets/Configs/EnemyTypes/RangedEnemyConfig.asset`

- [ ] **Step 1: 创建 Configs 目录结构**

Run: `ai-game-developer_assets-create-folder` tool (call twice)

Parameters 1:
```
parentFolder: "Assets/"
folderName: "Configs"
```

Parameters 2:
```
parentFolder: "Assets/Configs/"
folderName: "EnemyTypes"
```

Expected: 创建 `Assets/Configs/EnemyTypes/` 目录

- [ ] **Step 2: 创建 NormalEnemyConfig.asset**

Run: `ai-game-developer_assets-find` tool to get existing EnemyPrefab
Parameters:
```
filter: "EnemyPrefab"
```

Expected: 获取 EnemyPrefab GUID（Phase 1 已存在）

Write ScriptableObject asset data to: `Assets/Configs/EnemyTypes/NormalEnemyConfig.asset`

使用 Unity Editor 手动创建（ScriptableObject 无法通过 MCP 直接创建）：
1. Unity Editor → Assets → Create → Survivor → EnemyTypeConfig
2. Rename to "NormalEnemyConfig"
3. Configure fields:
   - enemyType: Normal
   - moveSpeed: 2.0
   - maxHealth: 1
   - expValue: 10
   - weightCurve: Constant curve (value=0.6)
   - minSpawnTime: 0
   - enemyColor: Red (RGB: 1, 0, 0)

- [ ] **Step 3: 创建 FastEnemyConfig.asset**

手动创建：
1. Unity Editor → Assets → Create → Survivor → EnemyTypeConfig
2. Rename to "FastEnemyConfig"
3. Configure fields:
   - enemyType: Fast
   - moveSpeed: 4.0
   - maxHealth: 1
   - expValue: 12
   - weightCurve: Linear curve (0@30s → 0.3@60s)
   - minSpawnTime: 30
   - enemyColor: Green (RGB: 0, 1, 0)

- [ ] **Step 4: 创建 TankEnemyConfig.asset**

手动创建：
1. Unity Editor → Assets → Create → Survivor → EnemyTypeConfig
2. Rename to "TankEnemyConfig"
3. Configure fields:
   - enemyType: Tank
   - moveSpeed: 1.0
   - maxHealth: 3
   - expValue: 25
   - weightCurve: Linear curve (0@60s → 0.2@120s)
   - minSpawnTime: 60
   - enemyColor: Gray (RGB: 0.5, 0.5, 0.5)

- [ ] **Step 5: 创建 RangedEnemyConfig.asset**

手动创建：
1. Unity Editor → Assets → Create → Survivor → EnemyTypeConfig
2. Rename to "RangedEnemyConfig"
3. Configure fields:
   - enemyType: Ranged
   - moveSpeed: 2.0
   - maxHealth: 2
   - expValue: 15
   - weightCurve: Linear curve (0@45s → 0.25@90s)
   - minSpawnTime: 45
   - attackRange: 8.0
   - attackInterval: 2.0
   - projectilePrefab: ProjectilePrefab（Phase 1 已存在）
   - enemyColor: Purple (RGB: 0.5, 0, 0.5)

- [ ] **Step 6: 创建 EnemySpawnSettings.asset**

手动创建：
1. Unity Editor → Assets → Create → Survivor → EnemySpawnSettings
2. Rename to "EnemySpawnSettings"
3. Configure fields:
   - spawnInterval: 2.0
   - maxActiveEnemies: 50
   - enemyConfigs: Add 4 configs (Normal, Fast, Tank, Ranged)
   - initialPoolSize: 10

- [ ] **Step 7: Refresh AssetDatabase**

Run: `ai-game-developer_assets-refresh` tool
Parameters:
```
options: "ForceSynchronousImport"
```

Expected: Unity 重新编译，无错误

- [ ] **Step 8: Git commit**

```bash
git add Assets/Configs/
git commit -m "feat(config): create EnemyTypeConfig and EnemySpawnSettings assets"
```

---

## 🎯 Task 9: 创建 FastEnemyPrefab.prefab

**Files:**
- Create: `Assets/Prefabs/FastEnemyPrefab.prefab`

- [ ] **Step 1: 创建 FastEnemy GameObject**

Run: `ai-game-developer_gameobject-create` tool
Parameters:
```
name: "FastEnemy"
position: {x: 0, y: 0, z: 0}
primitiveType: "Sphere"（临时使用 Sphere，后期替换为 Sprite）
```

Expected: 创建 FastEnemy GameObject

- [ ] **Step 2: 添加 Rigidbody2D**

Run: `ai-game-developer_gameobject-component-add` tool
Parameters:
```
gameObjectRef: {instanceID: <FastEnemy instanceID>}
componentNames: ["UnityEngine.Rigidbody2D"]
```

Expected: 添加 Rigidbody2D component

- [ ] **Step 3: 配置 Rigidbody2D**

Run: `ai-game-developer_gameobject-component-modify` tool
Parameters:
```
gameObjectRef: {instanceID: <FastEnemy instanceID>}
componentRef: {index: 1, instanceID: <Rigidbody2D instanceID>}
componentDiff:
  fields:
    - name: "bodyType"
      typeName: "UnityEngine.RigidbodyType2D"
      value: "Dynamic"
    - name: "gravityScale"
      typeName: "System.Single"
      value: 0
```

Expected: Rigidbody2D 配置为 Dynamic, gravity=0

- [ ] **Step 4: 添加 CircleCollider2D**

Run: `ai-game-developer_gameobject-component-add` tool
Parameters:
```
gameObjectRef: {instanceID: <FastEnemy instanceID>}
componentNames: ["UnityEngine.CircleCollider2D"]
```

Expected: 添加 CircleCollider2D component

- [ ] **Step 5: 配置 CircleCollider2D（radius=0.3）**

Run: `ai-game-developer_gameobject-component-modify` tool
Parameters:
```
gameObjectRef: {instanceID: <FastEnemy instanceID>}
componentRef: {index: 2, instanceID: <CircleCollider2D instanceID>}
componentDiff:
  fields:
    - name: "radius"
      typeName: "System.Single"
      value: 0.3
```

Expected: CircleCollider2D radius=0.3（体型较小）

- [ ] **Step 6: 添加 EnemyController**

Run: `ai-game-developer_gameobject-component-add` tool
Parameters:
```
gameObjectRef: {instanceID: <FastEnemy instanceID>}
componentNames: ["Survivor.Core.EnemyController"]
```

Expected: 添加 EnemyController component

- [ ] **Step 7: 配置 SpriteRenderer 颜色（Green）**

Run: `ai-game-developer_gameobject-component-modify` tool
Parameters:
```
gameObjectRef: {instanceID: <FastEnemy instanceID>}
componentRef: {index: 0, instanceID: <MeshRenderer instanceID>}（Sphere 使用 MeshRenderer）
componentDiff:
  props:
    - name: "material.color"
      typeName: "UnityEngine.Color"
      value: {r: 0, g: 1, b: 0, a: 1}
```

Expected: Material color = Green

- [ ] **Step 8: 创建 Prefab**

Run: `ai-game-developer_assets-prefab-create` tool
Parameters:
```
prefabAssetPath: "Assets/Prefabs/FastEnemyPrefab.prefab"
gameObjectRef: {instanceID: <FastEnemy instanceID>}
replaceGameObjectWithPrefab: false
```

Expected: 创建 FastEnemyPrefab.prefab

- [ ] **Step 9: Git commit**

```bash
git add Assets/Prefabs/FastEnemyPrefab.prefab
git commit -m "feat(prefab): create FastEnemyPrefab"
```

---

## 🎯 Task 10: 创建 TankEnemyPrefab.prefab

**Files:**
- Create: `Assets/Prefabs/TankEnemyPrefab.prefab`

- [ ] **Step 1: 创建 TankEnemy GameObject**

Run: `ai-game-developer_gameobject-create` tool
Parameters:
```
name: "TankEnemy"
position: {x: 0, y: 0, z: 0}
primitiveType: "Sphere"
```

Expected: 创建 TankEnemy GameObject

- [ ] **Step 2: 添加 Rigidbody2D**

Run: `ai-game-developer_gameobject-component-add` tool
Parameters:
```
gameObjectRef: {instanceID: <TankEnemy instanceID>}
componentNames: ["UnityEngine.Rigidbody2D"]
```

Expected: 添加 Rigidbody2D component

- [ ] **Step 3: 配置 Rigidbody2D**

Run: `ai-game-developer_gameobject-component-modify` tool
Parameters:
```
gameObjectRef: {instanceID: <TankEnemy instanceID>}
componentRef: {index: 1, instanceID: <Rigidbody2D instanceID>}
componentDiff:
  fields:
    - name: "bodyType"
      typeName: "UnityEngine.RigidbodyType2D"
      value: "Dynamic"
    - name: "gravityScale"
      typeName: "System.Single"
      value: 0
```

Expected: Rigidbody2D 配置为 Dynamic, gravity=0

- [ ] **Step 4: 添加 CircleCollider2D**

Run: `ai-game-developer_gameobject-component-add` tool
Parameters:
```
gameObjectRef: {instanceID: <TankEnemy instanceID>}
componentNames: ["UnityEngine.CircleCollider2D"]
```

Expected: 添加 CircleCollider2D component

- [ ] **Step 5: 配置 CircleCollider2D（radius=0.8）**

Run: `ai-game-developer_gameobject-component-modify` tool
Parameters:
```
gameObjectRef: {instanceID: <TankEnemy instanceID>}
componentRef: {index: 2, instanceID: <CircleCollider2D instanceID>}
componentDiff:
  fields:
    - name: "radius"
      typeName: "System.Single"
      value: 0.8
```

Expected: CircleCollider2D radius=0.8（体型较大）

- [ ] **Step 6: 添加 EnemyController**

Run: `ai-game-developer_gameobject-component-add` tool
Parameters:
```
gameObjectRef: {instanceID: <TankEnemy instanceID>}
componentNames: ["Survivor.Core.EnemyController"]
```

Expected: 添加 EnemyController component

- [ ] **Step 7: 配置 Material 颜色（Gray）**

Run: `ai-game-developer_gameobject-component-modify` tool
Parameters:
```
gameObjectRef: {instanceID: <TankEnemy instanceID>}
componentRef: {index: 0, instanceID: <MeshRenderer instanceID>}
componentDiff:
  props:
    - name: "material.color"
      typeName: "UnityEngine.Color"
      value: {r: 0.5, g: 0.5, b: 0.5, a: 1}
```

Expected: Material color = Gray

- [ ] **Step 8: 创建 Prefab**

Run: `ai-game-developer_assets-prefab-create` tool
Parameters:
```
prefabAssetPath: "Assets/Prefabs/TankEnemyPrefab.prefab"
gameObjectRef: {instanceID: <TankEnemy instanceID>}
replaceGameObjectWithPrefab: false
```

Expected: 创建 TankEnemyPrefab.prefab

- [ ] **Step 9: Git commit**

```bash
git add Assets/Prefabs/TankEnemyPrefab.prefab
git commit -m "feat(prefab): create TankEnemyPrefab"
```

---

## 🎯 Task 11: 创建 RangedEnemyPrefab.prefab

**Files:**
- Create: `Assets/Prefabs/RangedEnemyPrefab.prefab`

- [ ] **Step 1: 创建 RangedEnemy GameObject**

Run: `ai-game-developer_gameobject-create` tool
Parameters:
```
name: "RangedEnemy"
position: {x: 0, y: 0, z: 0}
primitiveType: "Sphere"
```

Expected: 创建 RangedEnemy GameObject

- [ ] **Step 2: 添加 Rigidbody2D**

Run: `ai-game-developer_gameobject-component-add` tool
Parameters:
```
gameObjectRef: {instanceID: <RangedEnemy instanceID>}
componentNames: ["UnityEngine.Rigidbody2D"]
```

Expected: 添加 Rigidbody2D component

- [ ] **Step 3: 配置 Rigidbody2D**

Run: `ai-game-developer_gameobject-component-modify` tool
Parameters:
```
gameObjectRef: {instanceID: <RangedEnemy instanceID>}
componentRef: {index: 1, instanceID: <Rigidbody2D instanceID>}
componentDiff:
  fields:
    - name: "bodyType"
      typeName: "UnityEngine.RigidbodyType2D"
      value: "Dynamic"
    - name: "gravityScale"
      typeName: "System.Single"
      value: 0
```

Expected: Rigidbody2D 配置为 Dynamic, gravity=0

- [ ] **Step 4: 添加 CircleCollider2D**

Run: `ai-game-developer_gameobject-component-add` tool
Parameters:
```
gameObjectRef: {instanceID: <RangedEnemy instanceID>}
componentNames: ["UnityEngine.CircleCollider2D"]
```

Expected: 添加 CircleCollider2D component

- [ ] **Step 5: 配置 CircleCollider2D（radius=0.5）**

Run: `ai-game-developer_gameobject-component-modify` tool
Parameters:
```
gameObjectRef: {instanceID: <RangedEnemy instanceID>}
componentRef: {index: 2, instanceID: <CircleCollider2D instanceID>}
componentDiff:
  fields:
    - name: "radius"
      typeName: "System.Single"
      value: 0.5
```

Expected: CircleCollider2D radius=0.5

- [ ] **Step 6: 添加 EnemyController**

Run: `ai-game-developer_gameobject-component-add` tool
Parameters:
```
gameObjectRef: {instanceID: <RangedEnemy instanceID>}
componentNames: ["Survivor.Core.EnemyController"]
```

Expected: 添加 EnemyController component

- [ ] **Step 7: 添加 EnemyAutoFire**

Run: `ai-game-developer_gameobject-component-add` tool
Parameters:
```
gameObjectRef: {instanceID: <RangedEnemy instanceID>}
componentNames: ["Survivor.Core.EnemyAutoFire"]
```

Expected: 添加 EnemyAutoFire component

- [ ] **Step 8: 配置 Material 颜色（Purple）**

Run: `ai-game-developer_gameobject-component-modify` tool
Parameters:
```
gameObjectRef: {instanceID: <RangedEnemy instanceID>}
componentRef: {index: 0, instanceID: <MeshRenderer instanceID>}
componentDiff:
  props:
    - name: "material.color"
      typeName: "UnityEngine.Color"
      value: {r: 0.5, g: 0, b: 0.5, a: 1}
```

Expected: Material color = Purple

- [ ] **Step 9: 创建 Prefab**

Run: `ai-game-developer_assets-prefab-create` tool
Parameters:
```
prefabAssetPath: "Assets/Prefabs/RangedEnemyPrefab.prefab"
gameObjectRef: {instanceID: <RangedEnemy instanceID>}
replaceGameObjectWithPrefab: false
```

Expected: 创建 RangedEnemyPrefab.prefab

- [ ] **Step 10: Git commit**

```bash
git add Assets/Prefabs/RangedEnemyPrefab.prefab
git commit -m "feat(prefab): create RangedEnemyPrefab with EnemyAutoFire"
```

---

## 🎯 Task 12: 配置 GameManager GameObject

**Files:**
- Modify: MainScene GameManager GameObject

- [ ] **Step 1: 打开 MainScene**

Run: `ai-game-developer_scene-open` tool
Parameters:
```
sceneRef: {assetPath: "Assets/Scenes/MainScene.unity"}
loadSceneMode: "Single"
```

Expected: 打开 MainScene

- [ ] **Step 2: 查找 GameManager GameObject**

Run: `ai-game-developer_gameobject-find` tool
Parameters:
```
gameObjectRef: {name: "GameManager"}
```

Expected: 获取 GameManager GameObject 信息

- [ ] **Step 3: 添加 EnemyPoolManager component**

Run: `ai-game-developer_gameobject-component-add` tool
Parameters:
```
gameObjectRef: {instanceID: <GameManager instanceID>}
componentNames: ["Survivor.Systems.EnemyPoolManager"]
```

Expected: 添加 EnemyPoolManager component

- [ ] **Step 4: 配置 EnemyPoolManager component**

Run: `ai-game-developer_gameobject-component-modify` tool
Parameters:
```
gameObjectRef: {instanceID: <GameManager instanceID>}
componentRef: {index: <EnemyPoolManager component index>, instanceID: <EnemyPoolManager instanceID>}
componentDiff:
  fields:
    - name: "spawnSettings"
      typeName: "Survivor.Config.EnemySpawnSettings"
      value: {assetPath: "Assets/Configs/EnemySpawnSettings.asset"}
    - name: "normalPrefab"
      typeName: "UnityEngine.GameObject"
      value: {assetPath: "Assets/Prefabs/EnemyPrefab.prefab"}
    - name: "fastPrefab"
      typeName: "UnityEngine.GameObject"
      value: {assetPath: "Assets/Prefabs/FastEnemyPrefab.prefab"}
    - name: "tankPrefab"
      typeName: "UnityEngine.GameObject"
      value: {assetPath: "Assets/Prefabs/TankEnemyPrefab.prefab"}
    - name: "rangedPrefab"
      typeName: "UnityEngine.GameObject"
      value: {assetPath: "Assets/Prefabs/RangedEnemyPrefab.prefab"}
```

Expected: EnemyPoolManager 配置完成

- [ ] **Step 5: 修改 GameManager component（移除旧 enemyPool）**

Run: `ai-game-developer_gameobject-component-modify` tool
Parameters:
```
gameObjectRef: {instanceID: <GameManager instanceID>}
componentRef: {index: <GameManager component index>, instanceID: <GameManager instanceID>}
componentDiff:
  fields:
    - name: "enemyPoolManager"
      typeName: "Survivor.Systems.EnemyPoolManager"
      value: {instanceID: <EnemyPoolManager component instanceID>}
```

Expected: GameManager.enemyPoolManager 配置完成

- [ ] **Step 6: 删除旧的 EnemyPool component（如果有）**

Run: `ai-game-developer_gameobject-component-destroy` tool
Parameters:
```
gameObjectRef: {instanceID: <GameManager instanceID>}
destroyComponentRefs:
  - {typeName: "Survivor.Systems.EnemyPool", index: <EnemyPool component index>}
```

Expected: 删除旧 EnemyPool component

- [ ] **Step 7: 保存 MainScene**

Run: `ai-game-developer_scene-save` tool
Parameters:
```
openedSceneName: "MainScene"
```

Expected: MainScene 保存完成

- [ ] **Step 8: Git commit**

```bash
git add Assets/Scenes/MainScene.unity
git commit -m "feat(scene): configure GameManager with EnemyPoolManager"
```

---

## 🎯 Task 13: 测试验证

**Files:**
- Manual Testing in Unity Editor

- [ ] **Step 1: 启动 Unity Play Mode**

Run: `ai-game-developer_editor-application-set-state` tool
Parameters:
```
playmode: true
```

Expected: Unity 进入 Play Mode

- [ ] **Step 2: 观察敌人生成（30秒内应只有 Normal Enemy）**

手动观察：
- gameTime < 30秒：只有红色敌人（Normal）生成
- 检查敌人追踪玩家行为
- 检查敌人碰撞 Projectile 后死亡
- 检查经验球掉落

Expected: Normal Enemy 正常生成与行为

- [ ] **Step 3: 观察敌人生成（30-45秒应出现 Fast Enemy）**

手动观察：
- gameTime 30-45秒：出现绿色敌人（Fast）
- Fast Enemy 速度更快（约 ×2）
- 体型较小（radius=0.3）

Expected: Fast Enemy 正常生成与行为

- [ ] **Step 4: 观察敌人生成（45秒后应出现 Ranged Enemy）**

手动观察：
- gameTime > 45秒：出现紫色敌人（Ranged）
- Ranged Enemy 保持距离（attackRange=8.0）
- Ranged Enemy 每2秒射击紫色 Projectile

Expected: Ranged Enemy 正常生成与射击行为

- [ ] **Step 5: 观察敌人生成（60秒后应出现 Tank Enemy）**

手动观察：
- gameTime > 60秒：出现灰色敌人（Tank）
- Tank Enemy 速度较慢（约 ×0.5）
- Tank Enemy 生命值更高（需要多次碰撞 Projectile）
- 体型较大（radius=0.8）

Expected: Tank Enemy 正常生成与行为

- [ ] **Step 6: 停止 Unity Play Mode**

Run: `ai-game-developer_editor-application-set-state` tool
Parameters:
```
playmode: false
```

Expected: Unity 停止 Play Mode

- [ ] **Step 7: 检查 Console Logs（无错误）**

Run: `ai-game-developer_console-get-logs` tool
Parameters:
```
logTypeFilter: "Error"
maxEntries: 20
lastMinutes: 5
```

Expected: 无编译错误或运行时错误

---

## 🎯 Task 14: Git Final Commit

- [ ] **Step 1: 检查 git status**

```bash
git status
```

Expected: 所有文件已 commit

- [ ] **Step 2: 创建 Phase 2.1 完成 commit（如果有遗漏文件）**

```bash
git add .
git commit -m "feat(phase2.1): complete Enemy Types implementation"
```

- [ ] **Step 3: 更新 CHANGELOG.md**

Read: `CHANGELOG.md`

Edit: 添加 Phase 2.1 完成记录

```markdown
## [Phase 2.1] - 2026-05-06

### Added
- 4 种敌人类型完整实作（Normal/Fast/Tank/Ranged）
- EnemyTypeConfig ScriptableObject 配置系统
- EnemySpawnSettings 全局生成设置
- EnemyPoolManager 多类型 Pool 管理器
- EnemyAutoFire 远程敌人射击逻辑
- 加权随机算法（AnimationCurve 权重曲线）
- FastEnemyPrefab, TankEnemyPrefab, RangedEnemyPrefab

### Changed
- EnemyController 支持类型特定行为
- EnemyPool 支持 enemyType 标识
- GameManager 整合 EnemyPoolManager

### Technical
- Unity MCP Tools 自动化 Prefab 创建
- ScriptableObject 数据驱动设计
- Object Pooling 扩展为多类型 Pool
```

- [ ] **Step 4: Git commit CHANGELOG**

```bash
git add CHANGELOG.md
git commit -m "docs(changelog): add Phase 2.1 completion record"
```

---

## 📌 Plan Self-Review Checklist

**1. Spec Coverage Check:**
- ✅ Section 1 Architecture: Task 4 (EnemyPoolManager)
- ✅ Section 2 ScriptableObject Config: Task 1, 2, 8
- ✅ Section 3 Prefab Structure: Task 9, 10, 11
- ✅ Section 4 EnemyController Changes: Task 5
- ✅ Section 5 GameManager Integration: Task 7, 12
- ✅ Section 6 Weight Algorithm: Task 4 (CalculateWeights, SelectEnemyType)
- ✅ Section 7 Testing: Task 13

**2. Placeholder Scan:**
- ✅ No TBD/TODO placeholders
- ✅ No "implement later" statements
- ✅ All code steps have actual code
- ✅ All commands have exact parameters

**3. Type Consistency:**
- ✅ EnemyType enum used consistently across all files
- ✅ EnemyTypeConfig properties match EnemyController.Initialize parameters
- ✅ EnemyPoolManager.SpawnEnemy signature matches GameManager call
- ✅ AnimationCurve.Evaluate() used correctly in CalculateWeights

**4. Dependency Check:**
- ✅ Task 1-7: C# Scripts（无外部依赖）
- ✅ Task 8: Config Assets（依赖 Task 1, 2）
- ✅ Task 9-11: Prefabs（依赖 Task 3, 5）
- ✅ Task 12: Scene Configuration（依赖 Task 8, 9-11）
- ✅ Task 13: Testing（依赖 Task 1-12）
- ✅ Task 14: Git Final（依赖 Task 13）

---

**Plan Complete! Execution Options:**

**1. Subagent-Driven（推荐）** - 每个任务派遣独立 subagent，任务间 review，快速迭代

**2. Inline Execution** - 在当前 session 使用 executing-plans skill 执行，批次执行 + checkpoints

**选择哪种方式？**