# Phase 1 Core MVP - Unity Editor 操作指南

**状态：** 代码已完成，需手动在 Unity Editor 中完成 Prefab + 配置 + 测试

---

## 已完成部分

✅ **C# 代码文件（已完成并提交）：**
- ProjectilePool.cs
- ProjectileController.cs
- EnemyPool.cs
- EnemyController.cs
- ExpOrbPool.cs
- ExperienceOrb.cs
- PlayerAutoFire.cs
- GameManager.cs（Update Loop 修改）

---

## 待完成部分（Unity Editor 手动操作）

### 1. 创建 Prefabs（Task 12-15）

#### 🎯 ProjectilePrefab

**步骤：**
1. Hierarchy → Create Empty →命名为 "ProjectilePrefab"
2. Add Component → Rigidbody2D
   - Body Type: **Kinematic**
   - Use Full Kinematic Contacts: **true**
3. Add Component → CircleCollider2D
   - Is Trigger: **true**
   - Radius: **0.1** (10px)
4. Add Component → Sprite Renderer
   - Sprite: Knob (Unity 内建 Circle)
   - Color: **橙色** (RGB: 255, 165, 0)
5. Add Component → ProjectileController (script)
6. 将 GameObject 拖到 **Assets/Prefabs/ProjectilePrefab.prefab**
7. 删除 Hierarchy 中的 GameObject

#### 👾 EnemyPrefab

**步骤：**
1. Hierarchy → Create Empty →命名为 "EnemyPrefab"
2. Add Component → Rigidbody2D
   - Body Type: **Dynamic**
   - Mass: **1**
   - Linear Drag: **0**
3. Add Component → CircleCollider2D
   - Is Trigger: **false**
   - Radius: **0.15** (15px)
4. Add Component → Sprite Renderer
   - Sprite: Knob
   - Color: **红色** (RGB: 255, 0, 0)
5. Add Component → EnemyController (script)
6. 拖到 **Assets/Prefabs/EnemyPrefab.prefab**
7. 删除 Hierarchy GameObject

#### 💎 ExpOrbPrefab

**步骤：**
1. Hierarchy → Create Empty →命名为 "ExpOrbPrefab"
2. Add Component → Rigidbody2D
   - Body Type: **Kinematic**
3. Add Component → CircleCollider2D
   - Is Trigger: **true**
   - Radius: **0.12** (12px)
4. Add Component → Sprite Renderer
   - Sprite: Knob
   - Color: **绿色** (RGB: 0, 255, 0)
5. Add Component → ExperienceOrb (script)
6. 拖到 **Assets/Prefabs/ExpOrbPrefab.prefab**
7. 删除 Hierarchy GameObject

#### 🛡️ PlayerPrefab

**步骤：**
1. Hierarchy → Create Empty →命名为 "Player"
2. Add Component → Rigidbody2D
   - Body Type: **Dynamic**
   - Mass: **1**
   - Linear Drag: **0**
   - Collision Detection: **Continuous**
3. Add Component → CircleCollider2D
   - Is Trigger: **false**
   - Radius: **0.2** (20px)
4. Add Component → Sprite Renderer
   - Sprite: Knob
   - Color: **白色** (RGB: 255, 255, 255)
5. Add Component → PlayerController (script)
6. Add Component → PlayerAutoFire (script)
7. Transform → Position: **(0, 0, 0)**
8. 拖到 **Assets/Prefabs/Player.prefab**
9. **保持 Hierarchy 中的 Player GameObject**（不要删除）
10. 设置 Tag: **"Player"**

---

### 2. 配置 Pool Prefab References

#### GameManager GameObject 配置

在 Unity Editor 中：
1. 选择 **GameManager GameObject**
2. Add Component → **ProjectilePool** (script)
   - Projectile Prefab: 选择 **ProjectilePrefab.prefab**
3. Add Component → **EnemyPool** (script)
   - Enemy Prefab: 选择 **EnemyPrefab.prefab**
4. Add Component → **ExpOrbPool** (script)
   - Exp Orb Prefab: 选择 **ExpOrbPrefab.prefab**
5. Player Field: 选择 **Hierarchy 中的 Player GameObject**

---

### 3. 配置 Physics2D + Tags（Task 17）

#### Physics2D 配置

在 Unity Editor 中：
1. Edit → Project Settings → **Physics 2D**
2. Gravity: **(0, 0)**（无重力）
3. Default Contact Offset: **0.01**

#### Tags 配置

在 Unity Editor 中：
1. Edit → Project Settings → **Tags and Layers**
2. 点击 **+** 添加 Tags：
   - **"Player"**
   - **"Projectile"**
   - **"Enemy"**
   - **"ExpOrb"**

#### Prefab Tags 配置

在 Unity Editor 中：
1. 选择 **Assets/Prefabs/Player.prefab**
   - Inspector → Tag: **"Player"**
2. 选择 **Assets/Prefabs/ProjectilePrefab.prefab**
   - Inspector → Tag: **"Projectile"**
3. 选择 **Assets/Prefabs/EnemyPrefab.prefab**
   - Inspector → Tag: **"Enemy"**
4. 选择 **Assets/Prefabs/ExpOrbPrefab.prefab**
   - Inspector → Tag: **"ExpOrb"**

---

### 4. 测试 MVP 功能（Task 18）

#### Play Mode 测试清单

在 Unity Editor 中进入 Play Mode，逐一测试：

✅ **Player 移动测试**
- WASD 移动 Player
- 确认移动方向正确

✅ **自动射击测试**
- Player 应每 0.5秒自动发射 3颗橙色魔法弹
- 魔法弹扇形散布 ±22.5°

✅ **敌人生成测试**
- 每 2秒屏幕外生成红色敌人
- 敌人直线追踪 Player

✅ **Projectile 碰撞测试**
- 魔法弹命中敌人 → 敌人死亡
- 敌人死亡后生成绿色经验球

✅ **经验球拾取测试**
- 经验球进入 Player pickupRange（80px）后飞向 Player
- Player 拾取后经验值累积

✅ **Object Pooling 测试**
- Console 中确认无频繁 Instantiate/Destroy 日志
- Projectile 5秒后自动回收

---

### 5. 最终提交

完成所有手动操作后，在 Unity Editor 中：
1. Ctrl+S 保存场景
2. Ctrl+S 保存项目

在 Git Bash 中：
```bash
git add Assets/Prefabs/ Assets/Scenes/
git commit -m "feat(phase1): Unity Prefab + 配置完成，Phase 1 MVP 完成"
```

---

## 关键参数参考

### Player 属性（PlayerController.cs）
- maxHP: **100**
- speed: **200px/秒**
- attackRange: **300px**
- pickupRange: **80px**
- fireRate: **0.5秒**

### Projectile 属性（PlayerAutoFire.cs）
- projectileCount: **3颗**
- projectileSpeed: **400px/秒**
- damage: **1**
- spreadAngle: **±22.5°**

### Enemy 属性（EnemyPool.cs）
- spawnInterval: **2秒**
- spawnDistance: **10px**
- speed: **50px/秒**
- hp: **1**
- damage: **10**

### ExpOrb 属性（ExperienceOrb.cs）
- expValue: **10**
- pickupSpeed: **300px/秒**

### Pool Sizes
- ProjectilePool: **50** (initial), **200** (max)
- EnemyPool: **30** (initial), **100** (max)
- ExpOrbPool: **50** (initial), **300** (max)

---

## 预期效果

完成所有操作后，你应该看到：
- 白色 Player 在场景中心
- WASD 可以移动 Player
- Player 自动发射橙色魔法弹（扇形）
- 红色敌人每 2秒从屏幕外生成
- 魔法弹击中敌人后，敌人消失，绿色经验球出现
- 经验球靠近 Player 后飞向 Player，消失

---

**完成日期：** 2026-05-06  
**下一步：** Phase 2 - Combat & Progression（Enemy Types, Chain Kill, Talent, Wave System）