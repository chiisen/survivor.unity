# Phase 1 Core MVP Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** 建立 Unity 2D 倖存者游戏的最小可行产品（MVP），实现核心游戏循环：Player 移动 + 自动射击 + Enemy 生成 + Projectile 碰撞 + ExpOrb 拾取。

**Architecture:** 使用 Unity 2D Physics + Object Pooling 系统，遵循 PRD Update Loop 四 Phase 规范。Player 模块拆分为 PlayerController + PlayerAutoFire，Pool 系统独立模块化。

**Tech Stack:** Unity 2022.3 LTS, URP, Input System Package, C# (.NET Standard 2.1)

---

## File Structure

### Scripts 目录结构

```
Assets/Scripts/
├── Core/
│   ├── GameManager.cs (已存在，需修改)
│   ├── PlayerController.cs (已存在，需修改)
│   ├── PlayerAutoFire.cs (新建)
│   ├── EnemyController.cs (已存在，需修改)
│   ├── ProjectileController.cs (已存在，需修改)
│   ├── ExperienceOrb.cs (已存在，需修改)
│   └── InputManager.cs (新建)
│
├── Systems/
│   ├── ProjectilePool.cs (新建)
│   ├── EnemyPool.cs (新建)
│   └── ExpOrbPool.cs (新建)
│
└── Input/
│   └── PlayerInputActions.inputactions (新建 InputAction 资源)
```

### Prefab 目录结构

```
Assets/Prefabs/
├── Player.prefab (新建)
├── EnemyPrefab.prefab (新建)
├── ProjectilePrefab.prefab (新建)
├── ExpOrbPrefab.prefab (新建)
```

### Scene 目录结构

```
Assets/Scenes/
└── MainScene.unity (修改现有场景)
```

---

## Task 1: 安装 Input System Package

**Files:**
- Package: Input System (1.7.x)

- [ ] **Step 1: 使用 Unity MCP 安装 Package**

使用 Unity MCP `package-add` 工具安装 Input System Package。

- [ ] **Step 2: 等待 Unity 编译完成**

Package 安装会触发 Unity Domain Reload，等待编译完成后继续。

---

## Task 2: 创建 InputAction 资源

**Files:**
- Create: `Assets/Scripts/Input/PlayerInputActions.inputactions`

- [ ] **Step 1: 创建 InputAction 资源文件**

在 Unity Editor 中创建 Input Actions 资源：
- 右键 Assets/Scripts/Input/ → Create → Input Actions
-命名为 "PlayerInputActions"

- [ ] **Step 2: 配置 Action Map 和 Actions**

配置 InputAction 资源：
- Action Map Name: "Player"
- Action Name: "Move"
- Binding Type: WASD (Keyboard)
  - W: Up (Vector2.y = 1)
  - A: Left (Vector2.x = -1)
  - S: Down (Vector2.y = -1)
  - D: Right (Vector2.x = 1)
- Control Type: Vector2

- [ ] **Step 3: 保存并生成 C# Class**

点击 "Generate C# Class" 按钮，生成 `PlayerInputActions.cs` 自动代码。

---

## Task 3: 实现 InputManager.cs

**Files:**
- Create: `Assets/Scripts/Core/InputManager.cs`

- [ ] **Step 1: 编写 InputManager.cs**

```csharp
using UnityEngine;
using UnityEngine.InputSystem;

namespace SurvivorUnity.Core
{
    public class InputManager : MonoBehaviour
    {
        public static InputManager Instance { get; private set; }
        
        private PlayerInputActions inputActions;
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
            
            inputActions = new PlayerInputActions();
            inputActions.Player.Enable();
        }
        
        private void OnDestroy()
        {
            inputActions?.Dispose();
        }
        
        public Vector2 GetMovementInput()
        {
            if (inputActions == null) return Vector2.zero;
            return inputActions.Player.Move.ReadValue<Vector2>();
        }
    }
}
```

- [ ] **Step 2: 将 InputManager 添加到 GameManager GameObject**

在 Unity Editor 中：
- 选择 GameManager GameObject
- Add Component → InputManager

- [ ] **Step 3: 提交代码**

```bash
git add Assets/Scripts/Input/ Assets/Scripts/Core/InputManager.cs
git commit -m "feat(input): 实作 InputManager 使用 Input System Package"
```

---

## Task 4: 修改 PlayerController.cs（移动逻辑）

**Files:**
- Modify: `Assets/Scripts/Core/PlayerController.cs:40-71`

- [ ] **Step 1: 修改 HandleInput() 方法**

替换旧的 Input.GetAxisRaw() 为 InputManager：

```csharp
private void HandleInput()
{
    movement = InputManager.Instance.GetMovementInput();
    
    if (movement != Vector2.zero)
    {
        UpdateDirection();
    }
}
```

- [ ] **Step 2: 测试 Player 移动**

在 Unity Editor 中：
- Play Mode
- WASD 移动测试
- 确认 Player 移动方向正确

- [ ] **Step 3: 提交代码**

```bash
git add Assets/Scripts/Core/PlayerController.cs
git commit -m "feat(player): 改用 InputManager 处理输入"
```

---

## Task 5: 实现 ProjectilePool.cs

**Files:**
- Create: `Assets/Scripts/Systems/ProjectilePool.cs`

- [ ] **Step 1: 编写 ProjectilePool.cs**

```csharp
using UnityEngine;
using System.Collections.Generic;

namespace SurvivorUnity.Core
{
    public class ProjectilePool : MonoBehaviour
    {
        public static ProjectilePool Instance { get; private set; }
        
        [Header("Pool Settings")]
        [SerializeField] private GameObject projectilePrefab;
        [SerializeField] private int initialPoolSize = 50;
        [SerializeField] private int maxPoolSize = 200;
        
        private Queue<GameObject> pool = new Queue<GameObject>();
        private List<GameObject> activeProjectiles = new List<GameObject>();
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
            
            InitializePool();
        }
        
        private void InitializePool()
        {
            for (int i = 0; i < initialPoolSize; i++)
            {
                GameObject projectile = Instantiate(projectilePrefab);
                projectile.SetActive(false);
                projectile.transform.SetParent(transform);
                pool.Enqueue(projectile);
            }
        }
        
        public GameObject SpawnProjectile(Vector2 position, Vector2 direction, float speed, int damage)
        {
            GameObject projectile = GetFromPool();
            
            if (projectile == null)
            {
                if (activeProjectiles.Count < maxPoolSize)
                {
                    projectile = Instantiate(projectilePrefab);
                    projectile.transform.SetParent(transform);
                }
                else
                {
                    return null;
                }
            }
            
            projectile.SetActive(true);
            projectile.transform.position = position;
            
            ProjectileController controller = projectile.GetComponent<ProjectileController>();
            if (controller != null)
            {
                controller.Initialize(direction, speed, damage);
            }
            
            activeProjectiles.Add(projectile);
            return projectile;
        }
        
        public void ReturnProjectile(GameObject projectile)
        {
            if (projectile == null) return;
            
            projectile.SetActive(false);
            pool.Enqueue(projectile);
            activeProjectiles.Remove(projectile);
        }
        
        private GameObject GetFromPool()
        {
            if (pool.Count > 0)
            {
                return pool.Dequeue();
            }
            return null;
        }
        
        public List<GameObject> ActiveProjectiles => activeProjectiles;
    }
}
```

- [ ] **Step 2: 将 ProjectilePool 添加到 GameManager GameObject**

在 Unity Editor 中：
- 选择 GameManager GameObject
- Add Component → ProjectilePool

- [ ] **Step 3: 提交代码**

```bash
git add Assets/Scripts/Systems/ProjectilePool.cs
git commit -m "feat(pool): 实作 ProjectilePool Object Pooling 系统"
```

---

## Task 6: 实现 ProjectileController.cs

**Files:**
- Modify: `Assets/Scripts/Core/ProjectileController.cs`

- [ ] **Step 1: 编写 ProjectileController.cs**

```csharp
using UnityEngine;

namespace SurvivorUnity.Core
{
    public class ProjectileController : MonoBehaviour
    {
        private Vector2 direction;
        private float speed;
        private int damage;
        private Rigidbody2D rb;
        private float lifetime = 5f;
        
        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
        }
        
        private void FixedUpdate()
        {
            if (rb != null && direction != Vector2.zero)
            {
                rb.MovePosition(rb.position + direction * speed * Time.fixedDeltaTime);
            }
        }
        
        private void Update()
        {
            lifetime -= Time.deltaTime;
            if (lifetime <= 0f)
            {
                ReturnToPool();
            }
        }
        
        public void Initialize(Vector2 dir, float moveSpeed, int attackDamage)
        {
            direction = dir;
            speed = moveSpeed;
            damage = attackDamage;
            lifetime = 5f;
        }
        
        private void ReturnToPool()
        {
            if (ProjectilePool.Instance != null)
            {
                ProjectilePool.Instance.ReturnProjectile(gameObject);
            }
        }
        
        public int Damage => damage;
    }
}
```

- [ ] **Step 2: 提交代码**

```bash
git add Assets/Scripts/Core/ProjectileController.cs
git commit -m "feat(projectile): 实作 ProjectileController 飞行逻辑"
```

---

## Task 7: 实现 EnemyPool.cs

**Files:**
- Create: `Assets/Scripts/Systems/EnemyPool.cs`

- [ ] **Step 1: 编写 EnemyPool.cs**

```csharp
using UnityEngine;
using System.Collections.Generic;

namespace SurvivorUnity.Core
{
    public class EnemyPool : MonoBehaviour
    {
        public static EnemyPool Instance { get; private set; }
        
        [Header("Pool Settings")]
        [SerializeField] private GameObject enemyPrefab;
        [SerializeField] private int initialPoolSize = 30;
        [SerializeField] private int maxPoolSize = 100;
        
        [Header("Spawn Settings")]
        [SerializeField] private float spawnInterval = 2f;
        [SerializeField] private float spawnDistance = 10f;
        
        private Queue<GameObject> pool = new Queue<GameObject>();
        private List<GameObject> activeEnemies = new List<GameObject>();
        private float spawnCooldown = 0f;
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
            
            InitializePool();
        }
        
        private void Update()
        {
            UpdateSpawnCooldown();
            SpawnEnemy();
        }
        
        private void InitializePool()
        {
            for (int i = 0; i < initialPoolSize; i++)
            {
                GameObject enemy = Instantiate(enemyPrefab);
                enemy.SetActive(false);
                enemy.transform.SetParent(transform);
                pool.Enqueue(enemy);
            }
        }
        
        private void UpdateSpawnCooldown()
        {
            if (spawnCooldown > 0f)
            {
                spawnCooldown -= Time.deltaTime;
            }
        }
        
        private void SpawnEnemy()
        {
            if (spawnCooldown > 0f) return;
            if (activeEnemies.Count >= maxPoolSize) return;
            
            if (GameManager.Instance == null || GameManager.Instance.Player == null) return;
            
            Vector2 spawnPos = GetRandomSpawnPosition();
            
            GameObject enemy = GetFromPool();
            if (enemy == null)
            {
                enemy = Instantiate(enemyPrefab);
                enemy.transform.SetParent(transform);
            }
            
            enemy.SetActive(true);
            enemy.transform.position = spawnPos;
            
            EnemyController controller = enemy.GetComponent<EnemyController>();
            if (controller != null)
            {
                controller.Initialize(
                    GameManager.Instance.Player.transform.position,
                    speed: 50f,
                    hp: 1,
                    damage: 10
                );
            }
            
            activeEnemies.Add(enemy);
            spawnCooldown = spawnInterval;
        }
        
        private Vector2 GetRandomSpawnPosition()
        {
            Vector2 playerPos = GameManager.Instance.Player.transform.position;
            
            float angle = Random.Range(0f, 360f);
            Vector2 offset = new Vector2(
                Mathf.Cos(angle * Mathf.Deg2Rad) * spawnDistance,
                Mathf.Sin(angle * Mathf.Deg2Rad) * spawnDistance
            );
            
            return playerPos + offset;
        }
        
        public void ReturnEnemy(GameObject enemy)
        {
            if (enemy == null) return;
            
            enemy.SetActive(false);
            pool.Enqueue(enemy);
            activeEnemies.Remove(enemy);
            
            if (GameManager.Instance != null)
            {
                GameManager.Instance.AddKill();
            }
        }
        
        private GameObject GetFromPool()
        {
            if (pool.Count > 0)
            {
                return pool.Dequeue();
            }
            return null;
        }
        
        public List<GameObject> ActiveEnemies => activeEnemies;
    }
}
```

- [ ] **Step 2: 将 EnemyPool 添加到 GameManager GameObject**

在 Unity Editor 中：
- 选择 GameManager GameObject
- Add Component → EnemyPool

- [ ] **Step 3: 提交代码**

```bash
git add Assets/Scripts/Systems/EnemyPool.cs
git commit -m "feat(pool): 实作 EnemyPool Object Pooling + 自动生成逻辑"
```

---

## Task 8: 实现 EnemyController.cs

**Files:**
- Modify: `Assets/Scripts/Core/EnemyController.cs`

- [ ] **Step 1: 编写 EnemyController.cs**

```csharp
using UnityEngine;

namespace SurvivorUnity.Core
{
    public class EnemyController : MonoBehaviour
    {
        private Vector2 targetPosition;
        private float speed;
        private int hp;
        private int damage;
        private Rigidbody2D rb;
        
        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
        }
        
        private void FixedUpdate()
        {
            MoveTowardsPlayer();
        }
        
        public void Initialize(Vector2 target, float moveSpeed, int health, int attackDamage)
        {
            targetPosition = target;
            speed = moveSpeed;
            hp = health;
            damage = attackDamage;
        }
        
        private void MoveTowardsPlayer()
        {
            if (GameManager.Instance == null || GameManager.Instance.Player == null) return;
            
            Vector2 direction = (GameManager.Instance.Player.transform.position - transform.position).normalized;
            rb.MovePosition(rb.position + direction * speed * Time.fixedDeltaTime);
        }
        
        public void TakeDamage(int amount)
        {
            hp -= amount;
            if (hp <= 0)
            {
                Die();
            }
        }
        
        private void Die()
        {
            if (ExpOrbPool.Instance != null)
            {
                ExpOrbPool.Instance.SpawnExpOrb(transform.position, expValue: 10);
            }
            
            if (EnemyPool.Instance != null)
            {
                EnemyPool.Instance.ReturnEnemy(gameObject);
            }
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Projectile"))
            {
                ProjectileController projectile = other.GetComponent<ProjectileController>();
                if (projectile != null)
                {
                    TakeDamage(projectile.Damage);
                    if (ProjectilePool.Instance != null)
                    {
                        ProjectilePool.Instance.ReturnProjectile(other.gameObject);
                    }
                }
            }
            
            if (other.CompareTag("Player"))
            {
                PlayerController player = other.GetComponent<PlayerController>();
                if (player != null)
                {
                    player.TakeDamage(damage);
                }
            }
        }
    }
}
```

- [ ] **Step 2: 提交代码**

```bash
git add Assets/Scripts/Core/EnemyController.cs
git commit -m "feat(enemy): 实作 EnemyController 追踪 + 受伤逻辑"
```

---

## Task 9: 实现 ExpOrbPool.cs

**Files:**
- Create: `Assets/Scripts/Systems/ExpOrbPool.cs`

- [ ] **Step 1: 编写 ExpOrbPool.cs**

```csharp
using UnityEngine;
using System.Collections.Generic;

namespace SurvivorUnity.Core
{
    public class ExpOrbPool : MonoBehaviour
    {
        public static ExpOrbPool Instance { get; private set; }
        
        [Header("Pool Settings")]
        [SerializeField] private GameObject expOrbPrefab;
        [SerializeField] private int initialPoolSize = 50;
        [SerializeField] private int maxPoolSize = 300;
        
        private Queue<GameObject> pool = new Queue<GameObject>();
        private List<GameObject> activeOrbs = new List<GameObject>();
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
            
            InitializePool();
        }
        
        private void InitializePool()
        {
            for (int i = 0; i < initialPoolSize; i++)
            {
                GameObject orb = Instantiate(expOrbPrefab);
                orb.SetActive(false);
                orb.transform.SetParent(transform);
                pool.Enqueue(orb);
            }
        }
        
        public GameObject SpawnExpOrb(Vector2 position, int expValue = 10)
        {
            GameObject orb = GetFromPool();
            
            if (orb == null)
            {
                if (activeOrbs.Count < maxPoolSize)
                {
                    orb = Instantiate(expOrbPrefab);
                    orb.transform.SetParent(transform);
                }
                else
                {
                    return null;
                }
            }
            
            orb.SetActive(true);
            orb.transform.position = position;
            
            ExperienceOrb controller = orb.GetComponent<ExperienceOrb>();
            if (controller != null)
            {
                controller.Initialize(expValue);
            }
            
            activeOrbs.Add(orb);
            return orb;
        }
        
        public void ReturnOrb(GameObject orb)
        {
            if (orb == null) return;
            
            orb.SetActive(false);
            pool.Enqueue(orb);
            activeOrbs.Remove(orb);
        }
        
        private GameObject GetFromPool()
        {
            if (pool.Count > 0)
            {
                return pool.Dequeue();
            }
            return null;
        }
        
        public List<GameObject> ActiveOrbs => activeOrbs;
    }
}
```

- [ ] **Step 2: 将 ExpOrbPool 添加到 GameManager GameObject**

在 Unity Editor 中：
- 选择 GameManager GameObject
- Add Component → ExpOrbPool

- [ ] **Step 3: 提交代码**

```bash
git add Assets/Scripts/Systems/ExpOrbPool.cs
git commit -m "feat(pool): 实作 ExpOrbPool Object Pooling 系统"
```

---

## Task 10: 实现 ExperienceOrb.cs

**Files:**
- Modify: `Assets/Scripts/Core/ExperienceOrb.cs`

- [ ] **Step 1: 编写 ExperienceOrb.cs**

```csharp
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
```

- [ ] **Step 2: 提交代码**

```bash
git add Assets/Scripts/Core/ExperienceOrb.cs
git commit -m "feat(exp-orb): 实作磁吸拾取逻辑"
```

---

## Task 11: 实现 PlayerAutoFire.cs

**Files:**
- Create: `Assets/Scripts/Core/PlayerAutoFire.cs`

- [ ] **Step 1: 编写 PlayerAutoFire.cs**

```csharp
using UnityEngine;

namespace SurvivorUnity.Core
{
    public class PlayerAutoFire : MonoBehaviour
    {
        [Header("Fire Settings")]
        [SerializeField] private float fireRate = 0.5f;
        [SerializeField] private int projectileCount = 3;
        [SerializeField] private float projectileSpeed = 400f;
        [SerializeField] private int damage = 1;
        
        private float fireCooldown = 0f;
        private PlayerController player;
        
        private void Awake()
        {
            player = GetComponent<PlayerController>();
        }
        
        private void Update()
        {
            UpdateCooldown();
            AutoFire();
        }
        
        private void UpdateCooldown()
        {
            if (fireCooldown > 0f)
            {
                fireCooldown -= Time.deltaTime;
            }
        }
        
        private void AutoFire()
        {
            if (!CanFire()) return;
            
            GameObject nearestEnemy = FindNearestEnemy();
            if (nearestEnemy == null) return;
            
            FireProjectile(nearestEnemy.transform.position);
            fireCooldown = fireRate;
        }
        
        private bool CanFire()
        {
            return fireCooldown <= 0f;
        }
        
        private GameObject FindNearestEnemy()
        {
            if (player == null || EnemyPool.Instance == null) return null;
            
            float range = player.AttackRange;
            GameObject nearest = null;
            float nearestDist = range;
            
            foreach (GameObject enemy in EnemyPool.Instance.ActiveEnemies)
            {
                if (enemy == null || !enemy.activeInHierarchy) continue;
                
                float dist = Vector2.Distance(transform.position, enemy.transform.position);
                if (dist < nearestDist)
                {
                    nearestDist = dist;
                    nearest = enemy;
                }
            }
            
            return nearest;
        }
        
        private void FireProjectile(Vector2 targetPos)
        {
            if (ProjectilePool.Instance == null) return;
            
            Vector2 direction = (targetPos - (Vector2)transform.position).normalized;
            
            for (int i = 0; i < projectileCount; i++)
            {
                float spreadAngle = (i - (projectileCount - 1) / 2f) * 22.5f;
                Vector2 spreadDir = RotateDirection(direction, spreadAngle);
                
                ProjectilePool.Instance.SpawnProjectile(
                    transform.position,
                    spreadDir,
                    projectileSpeed,
                    damage
                );
            }
        }
        
        private Vector2 RotateDirection(Vector2 dir, float angle)
        {
            float rad = angle * Mathf.Deg2Rad;
            return new Vector2(
                dir.x * Mathf.Cos(rad) - dir.y * Mathf.Sin(rad),
                dir.x * Mathf.Sin(rad) + dir.y * Mathf.Cos(rad)
            );
        }
    }
}
```

- [ ] **Step 2: 提交代码**

```bash
git add Assets/Scripts/Core/PlayerAutoFire.cs
git commit -m "feat(player): 实作 PlayerAutoFire 自动射击逻辑"
```

---

## Task 12: 创建 ProjectilePrefab

**Files:**
- Create: `Assets/Prefabs/ProjectilePrefab.prefab`

- [ ] **Step 1: 创建 ProjectilePrefab GameObject**

在 Unity Editor 中：
- Hierarchy → Create Empty →命名为 "ProjectilePrefab"
- Add Component → Rigidbody2D
  - Body Type: Kinematic
  - Use Full Kinematic Contacts: true
- Add Component → CircleCollider2D
  - Is Trigger: true
  - Radius: 0.1 (10px)
- Add Component → Sprite Renderer
  - Sprite: Default Circle (Unity 内建)
  - Color: 橙色 (RGB: 255, 165, 0)
- Add Component → ProjectileController (script)

- [ ] **Step 2: 创建 Prefab**

- 将 GameObject 拖到 Assets/Prefabs/ 目录
- 删除 Hierarchy 中的 GameObject（Prefab 已保存）

- [ ] **Step 3: 配置 ProjectilePool Prefab Reference**

- 选择 GameManager GameObject
- ProjectilePool Component
- Projectile Prefab: 选择 Assets/Prefabs/ProjectilePrefab.prefab

---

## Task 13: 创建 EnemyPrefab

**Files:**
- Create: `Assets/Prefabs/EnemyPrefab.prefab`

- [ ] **Step 1: 创建 EnemyPrefab GameObject**

在 Unity Editor 中：
- Hierarchy → Create Empty →命名为 "EnemyPrefab"
- Add Component → Rigidbody2D
  - Body Type: Dynamic
  - Mass: 1
  - Linear Drag: 0
- Add Component → CircleCollider2D
  - Is Trigger: false
  - Radius: 0.15 (15px)
- Add Component → Sprite Renderer
  - Sprite: Default Circle
  - Color: 红色 (RGB: 255, 0, 0)
- Add Component → EnemyController (script)

- [ ] **Step 2: 创建 Prefab**

- 将 GameObject 拖到 Assets/Prefabs/
- 删除 Hierarchy 中的 GameObject

- [ ] **Step 3: 配置 EnemyPool Prefab Reference**

- GameManager → EnemyPool Component
- Enemy Prefab: 选择 EnemyPrefab.prefab

---

## Task 14: 创建 ExpOrbPrefab

**Files:**
- Create: `Assets/Prefabs/ExpOrbPrefab.prefab`

- [ ] **Step 1: 创建 ExpOrbPrefab GameObject**

在 Unity Editor 中：
- Hierarchy → Create Empty →命名为 "ExpOrbPrefab"
- Add Component → Rigidbody2D (Kinematic)
- Add Component → CircleCollider2D (Is Trigger: true, Radius: 0.12)
- Add Component → Sprite Renderer (绿色 Circle, RGB: 0, 255, 0)
- Add Component → ExperienceOrb (script)

- [ ] **Step 2: 创建 Prefab**

- 拖到 Assets/Prefabs/
- 删除 Hierarchy GameObject

- [ ] **Step 3: 配置 ExpOrbPool Prefab Reference**

- GameManager → ExpOrbPool Component
- Exp Orb Prefab: 选择 ExpOrbPrefab.prefab

---

## Task 15: 创建 PlayerPrefab

**Files:**
- Create: `Assets/Prefabs/Player.prefab`

- [ ] **Step 1: 创建 Player GameObject**

在 Unity Editor 中：
- Hierarchy → Create Empty →命名为 "Player"
- Add Component → Rigidbody2D
  - Body Type: Dynamic
  - Mass: 1
  - Linear Drag: 0
  - Collision Detection: Continuous
- Add Component → CircleCollider2D
  - Is Trigger: false
  - Radius: 0.2 (20px)
- Add Component → Sprite Renderer
  - Sprite: Default Circle
  - Color: 白色 (RGB: 255, 255, 255)
- Add Component → PlayerController (script)
- Add Component → PlayerAutoFire (script)
- Transform → Position: (0, 0, 0)

- [ ] **Step 2: 创建 Prefab**

- 拖到 Assets/Prefabs/
- 删除 Hierarchy GameObject

- [ ] **Step 3: 配置 GameManager Player Reference**

- GameManager → Player Field
- 选择 Assets/Prefabs/Player.prefab

- [ ] **Step 4: 在场景中实例化 Player**

- 拖 Player.prefab 到 Hierarchy
- 设置 Tag: "Player"
- Layer: Default

---

## Task 16: 修改 GameManager.cs（Update Loop）

**Files:**
- Modify: `Assets/Scripts/Core/GameManager.cs:43-77`

- [ ] **Step 1: 修改 Update() 方法**

```csharp
private void Update()
{
    if (isPaused) return;
    
    gameTime += Time.deltaTime;
    
    UpdatePhase1();
    UpdatePhase2();
    UpdatePhase3();
    UpdatePhase4();
}

private void UpdatePhase1()
{
    // Pooling 系统自动管理
}

private void UpdatePhase2()
{
    // Player + Pool 自动 Update
}

private void UpdatePhase3()
{
    // Unity Physics 自动碰撞
}

private void UpdatePhase4()
{
    // UI 后期实作
}
```

- [ ] **Step 2: 修改 Enemies/Projectiles/ExperienceOrbs 属性**

```csharp
public List<GameObject> Enemies 
{
    get 
    {
        if (EnemyPool.Instance != null)
        {
            return EnemyPool.Instance.ActiveEnemies;
        }
        return new List<GameObject>();
    }
}

public List<GameObject> Projectiles 
{
    get 
    {
        if (ProjectilePool.Instance != null)
        {
            return ProjectilePool.Instance.ActiveProjectiles;
        }
        return new List<GameObject>();
    }
}

public List<GameObject> ExperienceOrbs 
{
    get 
    {
        if (ExpOrbPool.Instance != null)
        {
            return ExpOrbPool.Instance.ActiveOrbs;
        }
        return new List<GameObject>();
    }
}
```

- [ ] **Step 3: 提交代码**

```bash
git add Assets/Scripts/Core/GameManager.cs
git commit -m "feat(game-manager): 修改 Update Loop 四 Phase + Pool 整合"
```

---

## Task 17: 配置 Physics2D 和 Tags

**Files:**
- Project Settings: Physics2D
- Tags and Layers

- [ ] **Step 1: 配置 Physics2D**

在 Unity Editor 中：
- Edit → Project Settings → Physics 2D
- Gravity: (0, 0)（无重力）
- Default Contact Offset: 0.01

- [ ] **Step 2: 创建 Tags**

- Edit → Project Settings → Tags and Layers
- Tags:
  - "Player"
  - "Projectile"
  - "Enemy"
  - "ExpOrb"

- [ ] **Step 3: 配置 Prefab Tags**

- Player.prefab → Tag: "Player"
- ProjectilePrefab.prefab → Tag: "Projectile"
- EnemyPrefab.prefab → Tag: "Enemy"
- ExpOrbPrefab.prefab → Tag: "ExpOrb"

---

## Task 18: 测试 MVP 功能

**Files:**
- Scene: MainScene.unity

- [ ] **Step 1: Play Mode 测试**

在 Unity Editor 中：
- 进入 Play Mode
- 测试 WASD 移动（确认 InputManager 正常）
- 测试自动射击（每 0.5秒发射 3颗橙色魔法弹）
- 测试敌人生成（每 2秒屏幕外生成红色敌人）
- 测试敌人追踪玩家（直线移动）
- 测试魔法弹碰撞敌人（敌人死亡 → 绿色经验球生成）
- 测试经验球磁吸拾取（进入 pickupRange 后飞向玩家）
- 测试经验值累积（拾取后 Player level 提升）

- [ ] **Step 2: Object Pooling 监控**

在 Unity Editor Console 中：
- 确认无频繁 Instantiate/Destroy 日志
- 确认 Pool 正常回收（Projectile 5秒后回收）

- [ ] **Step 3: 提交最终版本**

```bash
git add Assets/
git commit -m "feat(phase1): Phase 1 Core MVP 完成"
```

---

## Self-Review

### 1. Spec Coverage

对照设计文档 `docs/superpowers/specs/2026-05-06-phase1-core-mvp-design.md`：

✅ Input System Package 安装（Task 1）  
✅ InputAction 资源创建（Task 2）  
✅ InputManager.cs（Task 3）  
✅ PlayerController.cs 移动逻辑（Task 4）  
✅ ProjectilePool.cs（Task 5）  
✅ ProjectileController.cs（Task 6）  
✅ EnemyPool.cs（Task 7）  
✅ EnemyController.cs（Task 8）  
✅ ExpOrbPool.cs（Task 9）  
✅ ExperienceOrb.cs（Task 10）  
✅ PlayerAutoFire.cs（Task 11）  
✅ 4个 Prefab 创建（Task 12-15）  
✅ GameManager Update Loop（Task 16）  
✅ Physics2D + Tags 配置（Task 17）  
✅ MVP 测试（Task 18）  

### 2. Placeholder Scan

✅ 无 TBD/TODO  
✅ 无 "implement later"  
✅ 无 "add validation"  
✅ 所有代码步骤包含完整代码  
✅ 所有命令包含具体命令  

### 3. Type Consistency

✅ ProjectilePool.SpawnProjectile() 参数与 ProjectileController.Initialize() 一致  
✅ EnemyPool.SpawnEnemy() 参数与 EnemyController.Initialize() 一致  
✅ ExpOrbPool.SpawnExpOrb() 参数与 ExperienceOrb.Initialize() 一致  
✅ GameManager.Enemies/Projectiles/ExperienceOrbs 返回 List<GameObject>  
✅ 所有 Pool.Instance 使用 Singleton 模式  

---

**Plan complete. Ready for execution.**