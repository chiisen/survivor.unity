# Phase 1 Core MVP 设计文档

**日期：** 2026-05-06  
**项目：** survivor.unity  
**阶段：** Phase 1 - Core MVP  
**状态：** 设计完成，待实作

---

## 1. 项目概述

### 1.1 目标
建立 Unity 2D 倖存者游戏的最小可行产品（MVP），实现核心游戏循环：
- Player WASD 移动 + 自动射击
- Enemy 生成 + 追踪玩家 + 碰撞
- Projectile 飞行 + 碰撞敌人
- Experience Orb 磁吸拾取 + 升级

### 1.2 MVP 范围限制
- ✅ 简单形状美术（Unity Default Circle/Square）
- ✅ 简化敌人生成（固定间隔，完整 Wave System 等到 Phase 2）
- ✅ Object Pooling 系统（现阶段实作，符合 PRD 规范）
- ❌ Boss、Wave System、连杀系统、UI、特效（等到后续 Phase）

---

## 2. 技术选择

### 2.1 输入系统
**选择：新 Input System Package**  
**理由：**
- Console 日誌显示 Input Manager 即将废弃
- PRD 第 9.A.5 節推荐使用 Input System Package
- 更现代、可扩展、支持多平台

**实作方式：**
- 创建 `PlayerInputActions.inputactions` 资源
- Action Map: "Player"
- Action: "Move" (WASD)
- 使用 `InputManager.cs` 统一管理输入

---

### 2.2 美术资产
**选择：简单形状（后期补 Pixel Art）**  
**理由：**
- MVP 阶段优先验证逻辑
- 使用 Unity Default Circle/Square 快速开发
- Phase 4 再替换为 Pixel Art 素材

**Prefab 配置：**
- Player: 白色 Circle (半径 20px)
- Enemy: 红色 Circle (半径 15px)
- Projectile: 橙色 Circle (半径 10px)
- ExpOrb: 绿色 Circle (半径 12px)

---

### 2.3 物理碰撞
**选择：Unity 2D Physics**  
**理由：**
- PRD 第 9.B.4 節明确提到
- Unity 内建，自动处理碰撞检测
- Rigidbody2D + CircleCollider2D

**配置：**
- Gravity: (0, 0)（无重力）
- Player: Dynamic Rigidbody2D + CircleCollider2D
- Enemy: Dynamic Rigidbody2D + CircleCollider2D
- Projectile: Kinematic Rigidbody2D + CircleCollider2D (Is Trigger)
- ExpOrb: Kinematic Rigidbody2D + CircleCollider2D (Is Trigger)

---

### 2.4 敌人生成策略
**选择：简化版本（固定间隔生成）**  
**理由：**
- MVP 阶段优先验证基础循环
- 完整 Wave System（每 60秒一波 + Boss）等到 Phase 2

**实作参数：**
- `spawnInterval`: 2秒（固定间隔）
- `spawnDistance`: 10px（屏幕外生成）
- `maxPoolSize`: 100（限制敌人数量）
- `speed`: 50px/秒（固定速度）
- `hp`: 1（固定血量）

---

### 2.5 Object Pooling
**选择：现阶段实作**  
**理由：**
- PRD 第 9.C.2 節強调必须使用 Object Pooling
- 避免 GC 频繁 Instantiate/Destroy
- 符合 PRD 规范，后续无需重构

**Pool 配置：**
- ProjectilePool: initialSize=50, maxSize=200
- EnemyPool: initialSize=30, maxSize=100
- ExpOrbPool: initialSize=50, maxSize=300

---

## 3. 场景架构

### 3.1 Unity 场景层级结构

```
MainScene.unity
├── Grid (GameObject)
│   └── Ground Tilemap (后期实作)
│   └── Decoration Tilemap (后期实作)
│
├── Main Camera (Camera)
│   ├── Camera Component (Orthographic, Size 5)
│   └── Audio Listener
│
├── Directional Light (Light 2D)
│
├── GameManager (GameObject)
│   ├── GameManager.cs
│   ├── EnemyPool.cs
│   ├── ProjectilePool.cs
│   ├── ExpOrbPool.cs
│   └── InputManager.cs
│
├── Player (Prefab)
│   ├── PlayerController.cs
│   ├── PlayerAutoFire.cs
│   ├── Rigidbody2D (Dynamic)
│   ├── CircleCollider2D (Radius 20px)
│   ├── SpriteRenderer (白色 Circle)
│   └── Transform (Position: (0, 0, 0))
│
├── EnemyPrefab (Prefab)
│   ├── EnemyController.cs
│   ├── Rigidbody2D (Dynamic)
│   ├── CircleCollider2D (Radius 15px)
│   ├── SpriteRenderer (红色 Circle)
│   └── Transform (Scale: 0.5, 0.5, 1)
│
├── ProjectilePrefab (Prefab)
│   ├── ProjectileController.cs
│   ├── Rigidbody2D (Kinematic)
│   ├── CircleCollider2D (Is Trigger, Radius 10px)
│   ├── SpriteRenderer (橙色 Circle)
│   └── TrailRenderer (后期实作)
│
├── ExpOrbPrefab (Prefab)
│   ├── ExperienceOrb.cs
│   ├── Rigidbody2D (Kinematic)
│   ├── CircleCollider2D (Is Trigger, Radius 12px)
│   └── SpriteRenderer (绿色 Circle)
```

### 3.2 关键配置
- **Camera**: Orthographic 模式, Size 5（约 1920x1080 像素范围）
- **Physics2D**: Gravity = (0, 0)
- **Sorting Layers**: Default → Player → Enemies → Projectiles → ExpOrbs

---

## 4. 核心 C# 类设计

### 4.1 InputManager.cs（输入处理）

**职责：** 使用 Input System Package 处理 WASD 移动

**关键代码：**
```csharp
public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }
    
    private PlayerInputActions inputActions;
    
    public Vector2 GetMovementInput()
    {
        return inputActions.Player.Move.ReadValue<Vector2>();
    }
}
```

**InputAction 资源：**
- Action Map: "Player"
- Action: "Move"
- Binding: WASD (Keyboard)

---

### 4.2 PlayerController.cs（玩家移动）

**职责：** WASD 移动 + 状态管理（HP、Shield、Level）

**关键属性：**
- `maxHP`: 100
- `speed`: 200px/秒
- `attackRange`: 300px
- `pickupRange`: 80px
- `level`: 1 (升级系统)
- `shield`: 0 (护盾系统)

**关键方法：**
- `Update()`: 获取输入 + 更新方向
- `FixedUpdate()`: Rigidbody2D 移动
- `AddExp()`: 经验值累积 + 升级
- `TakeDamage()`: 受伤逻辑（优先扣护盾）

---

### 4.3 PlayerAutoFire.cs（自动射击）

**职责：** 每帧检查范围内最近敌人，自动发射魔法弹

**关键属性：**
- `fireRate`: 0.5秒
- `projectileCount`: 3（扇形散布 ±22.5°）
- `projectileSpeed`: 400px/秒
- `damage`: 1

**关键逻辑：**
- `AutoFire()`: 检查冷却 → 找最近敌人 → 发射
- `FindNearestEnemy()`: 遍历 GameManager.Enemies，找范围内最近
- `FireProjectile()`: 扇形发射 3颗魔法弹
- `RotateDirection()`: 旋转方向角度

---

### 4.4 EnemyController.cs（敌人追踪）

**职责：** 追踪玩家 + 受伤 + 死亡

**关键属性：**
- `speed`: 50px/秒（固定）
- `hp`: 1（固定）
- `damage`: 10

**关键逻辑：**
- `MoveTowardsPlayer()`: 直线追踪玩家
- `TakeDamage()`: 受伤 → 死亡
- `Die()`: 生成 ExpOrb → 回收到 Pool
- `OnTriggerEnter2D()`: 碰撞 Projectile → 受伤，碰撞 Player → 造成伤害

---

### 4.5 ProjectileController.cs（魔法弹飞行）

**职责：** 直线飞行 + 碰撞检测

**关键属性：**
- `direction`: Vector2（飞行方向）
- `speed`: 400px/秒
- `damage`: 1
- `lifetime`: 5秒（自动回收）

**关键逻辑：**
- `Initialize()`: 设置方向、速度、伤害
- `MoveProjectile()`: Rigidbody2D 移动
- `Update()`: 5秒后自动回收

---

### 4.6 ExperienceOrb.cs（磁吸拾取）

**职责：** 进入拾取范围后磁吸飞向玩家

**关键属性：**
- `expValue`: 10（固定）
- `pickupSpeed`: 300px/秒
- `isBeingPickedUp`: bool（是否进入拾取范围）

**关键逻辑：**
- `CheckPickupRange()`: 检查距离 Player.pickupRange
- `MoveTowardsPlayer()`: 磁吸飞行
- `OnTriggerEnter2D()`: 碰撞 Player → AddExp → 回收

---

## 5. Object Pooling 系统设计

### 5.1 ProjectilePool.cs（魔法弹池）

**职责：** 管理 Projectile Prefab 的生成/回收

**关键特性：**
- `initialPoolSize`: 50（预分配）
- `maxPoolSize`: 200（限制）
- `SpawnProjectile()`: 从 Pool 获取 + Initialize
- `ReturnProjectile()`: 回收 + Enqueue
- `ActiveProjectiles`: List（跟踪活跃对象）

**动态扩容逻辑：**
- Pool exhausted → 自动 Instantiate（限制 maxSize）
- maxSize 达到上限 → return null

---

### 5.2 EnemyPool.cs（敌人池）

**职责：** 管理敌人生成/回收 + 自动生成逻辑

**关键特性：**
- `initialPoolSize`: 30
- `maxPoolSize`: 100
- `spawnInterval`: 2秒（固定间隔）
- `spawnDistance`: 10px（屏幕外生成）
- `SpawnEnemy()`: 自动生成 + Initialize
- `ReturnEnemy()`: 回收 + GameManager.AddKill()

**生成逻辑：**
- `GetRandomSpawnPosition()`: 屏幕外随机位置（上下左右）
- `Initialize()`: 设置目标、速度、血量、伤害

---

### 5.3 ExpOrbPool.cs（经验球池）

**职责：** 管理经验球生成/回收

**关键特性：**
- `initialPoolSize`: 50
- `maxPoolSize`: 300
- `SpawnExpOrb()`: 从 Pool 获取 + Initialize
- `ReturnOrb()`: 回收 + Enqueue

---

## 6. GameManager Update Loop 四 Phase

### 6.1 设计原则

遵循 PRD 第 13 節 **AI Agent 开发规范** 的 Update Loop 四 Phase：

```
Phase 1: 清理与准备
Phase 2: 状态更新
Phase 3: 系统逻辑
Phase 4: UI 更新
```

### 6.2 实作方式

```csharp
private void Update()
{
    if (isPaused) return;
    
    gameTime += Time.deltaTime;
    
    UpdatePhase1(); // Pooling 系统自动管理
    UpdatePhase2(); // PlayerController + Pool 自动 Update
    UpdatePhase3(); // Unity Physics 自动碰撞
    UpdatePhase4(); // UI 后期实作
}
```

**关键特性：**
- Phase 1-3 由 Unity 自动处理（Pooling + Physics）
- Phase 4 等到后续实作 UI 系统
- 符合 PRD 规范，避免遗漏必要步骤

---

## 7. Prefab 和场景配置

### 7.1 Prefab 创建清单

1. **Player.prefab**
   - Components: PlayerController, PlayerAutoFire, Rigidbody2D, CircleCollider2D, SpriteRenderer
   - Rigidbody2D: Dynamic, Mass=1, Linear Drag=0
   - CircleCollider2D: Radius=20px, Is Trigger=false
   - SpriteRenderer: Sprite=Default Circle (白色)

2. **EnemyPrefab.prefab**
   - Components: EnemyController, Rigidbody2D, CircleCollider2D, SpriteRenderer
   - Rigidbody2D: Dynamic
   - CircleCollider2D: Radius=15px, Is Trigger=false
   - SpriteRenderer: Sprite=Default Circle (红色)

3. **ProjectilePrefab.prefab**
   - Components: ProjectileController, Rigidbody2D, CircleCollider2D, SpriteRenderer
   - Rigidbody2D: Kinematic
   - CircleCollider2D: Radius=10px, Is Trigger=true
   - SpriteRenderer: Sprite=Default Circle (橙色)

4. **ExpOrbPrefab.prefab**
   - Components: ExperienceOrb, Rigidbody2D, CircleCollider2D, SpriteRenderer
   - Rigidbody2D: Kinematic
   - CircleCollider2D: Radius=12px, Is Trigger=true
   - SpriteRenderer: Sprite=Default Circle (绿色)

---

### 7.2 Unity 项目设置

**Project Settings:**
- **Physics2D**: Gravity=(0, 0), Default Contact Offset=0.01
- **Time**: Fixed Timestep=0.02
- **Quality**: Medium, Anti Aliasing=2x

**Script Execution Order:**
- GameManager: -100
- InputManager: -90
- Pool Systems: -80
- PlayerController: 0
- UI Systems: 100（后期）

---

### 7.3 Package 安装

**必要 Package：**
- Universal RP: 14.0.x（内建）
- Input System: 1.7.x（需安装）
- TextMeshPro: 3.0.x（内建）

**安装方式：**
```bash
# 使用 Unity MCP Package Manager
ai-game-developer_package-add "Input System"
```

---

## 8. 测试验证

### 8.1 功能测试清单

**基础循环测试：**
- [ ] Player WASD 移动正常
- [ ] Player 自动射击正常（每 0.5秒发射 3颗）
- [ ] Projectile 扇形散布 ±22.5°
- [ ] Enemy 屏幕外生成正常（每 2秒）
- [ ] Enemy 追踪玩家正常
- [ ] Projectile 碰撞 Enemy → Enemy 死亡 → ExpOrb 生成
- [ ] ExpOrb 磁吸拾取正常（进入 pickupRange 后飞向玩家）
- [ ] Player 拾取 ExpOrb → 经验值累积
- [ ] Object Pooling 正常运作（无频繁 Instantiate/Destroy）

**边界测试：**
- [ ] Projectile 5秒后自动回收
- [ ] EnemyPool maxSize 达到上限 → 不再生成
- [ ] ProjectilePool maxSize 达到上限 → return null
- [ ] ExpOrbPool maxSize 达到上限 → return null

---

### 8.2 性能测试

**Object Pooling 监控：**
- [ ] ProjectilePool.activeCount < 200
- [ ] EnemyPool.activeCount < 100
- [ ] ExpOrbPool.activeCount < 300
- [ ] GC Alloc < 100KB/frame

---

## 9. 下一步计划

### 9.1 Phase 2: Combat & Progression
- Enemy Types（快速型、坦克型、远程型）
- Chain Kill System（连杀 buff）
- Talent System（升级选项）
- Wave System（波次生成）

### 9.2 Phase 3: Boss & Special Enemies
- Boss System（多阶段、狂暴模式）
- Elite Enemies（精英、分裂、爆炸、隐形）

### 9.3 Phase 4: UI & Effects
- UI System（HP/EXP 条、升级弹窗、游戏结束）
- Visual Effects（爆炸、挥剑特效）
- Audio System（音效 + BGM）

### 9.4 Phase 5: Advanced Systems
- Achievement System
- Leaderboard
- Difficulty Modes
- Tilemap System

---

## 10. 参考文档

- **PRD.md**: 项目完整需求文档（1322行）
- **PRD 第 9 節**: Unity 技术架构
- **PRD 第 13 節**: AI Agent 开发规范（Update Loop 四 Phase）
- **PRD 第 9.C.2 節**: Object Pooling 优化

---

**设计完成日期：** 2026-05-06  
**下一步：** Invoke writing-plans skill，创建实作计划