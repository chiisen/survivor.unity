# SetupPlayerShooting 工具说明

## 📋 功能概述

配置Player的自动射击系统，包括设置子弹Prefab引用、开启自动射击、调整射击频率和子弹速度。

## 🎯 使用场景

- **Player不会射击**: 需要启用自动射击功能
- **射击配置丢失**: projectilePrefab引用未设置
- **调整射击参数**: 自定义射击频率和子弹速度

## 🚀 使用方法

### 配置射击系统
```
Unity Editor → 菜单栏 → Survivor → Setup Player Shooting
```

### Console输出

```
✅ [SetupPlayerShooting] ✅ Player shooting configured successfully!
✅ - Projectile Prefab: ProjectilePrefab
✅ - Auto Fire: true
✅ - Fire Interval: 0.5s
✅ - Damage: 1
✅ [SetupPlayerShooting] 🎮 Ready! Enter Play Mode to test auto-shooting.
```

## 📊 配置内容

### PlayerController组件参数

| 参数 | 默认值 | 说明 |
|------|--------|------|
| **projectilePrefab** | ProjectilePrefab | 子弹Prefab引用 |
| **autoFire** | true | 自动射击开关 |
| **autoFireInterval** | 0.5秒 | 射击间隔时间 |
| **projectileSpeed** | 400 | 子弹移动速度 |
| **damage** | 1 | 子弹伤害值 |

## 🎮 射击机制

### 自动射击逻辑

```
每0.5秒：
1. 检查autoFire是否为true
2. 检查projectilePrefab是否已设置
3. 从Player位置生成子弹
4. 子弹向右发射（Vector2.right）
5. 子弹速度 = projectileSpeed (400)
```

### 子弹生成位置

```
Player.transform.position

示例：
Player在(0, 0)
→ 子弹从(0, 0)生成
→ 子弹向右移动，速度400 units/秒
```

## 🔧 手动调整参数

配置完成后可手动修改：

```
步骤：
1. Hierarchy → Player
2. Inspector → PlayerController
3. 修改射击参数：
   - Auto Fire: true/false（开关自动射击）
   - Auto Fire Interval: 0.3-1.0秒（射击频率）
   - Projectile Speed: 200-600（子弹速度）
4. Ctrl+S保存场景
```

### 参数建议值

```
射击频率：
- 极快: 0.2秒（每秒5发）
- 快速: 0.3秒（每秒3.3发）
- 正常: 0.5秒（每秒2发） ← 默认推荐
- 慢速: 1.0秒（每秒1发）

子弹速度：
- 慢速: 200-300（子弹可见轨迹）
- 正常: 400 ← 默认推荐
- 快速: 600-800（瞬间击中）
```

## 🎯 测试验证

### Play Mode测试
```
Unity Editor → Survivor → Test Player Shooting (Play Mode)

必须在Play Mode下运行！

输出：
✅ Shooting configuration verified
✅ Projectile Prefab: ProjectilePrefab
✅ Auto Fire: true
✅ Fire Interval: 0.5s
✅ Active Projectiles: N（当前活跃子弹数量）
```

### 验证检查清单

```
✅ Player GameObject的Tag为"Player"
✅ projectilePrefab引用已设置（不是None）
✅ autoFire为true
✅ 进入Play Mode后Console显示射击日志
✅ Game View中能看到子弹发射
✅ 子弹向右移动
✅ 子弹速度适中（不会太快或太慢）
```

## ⚠️ 注意事项

- **Player标签**: 必须确保Player GameObject的Tag为"Player"
- **Prefab引用**: ProjectilePrefab必须存在于Assets/Prefabs/
- **子弹速度**: 过快会看不清轨迹，过慢会打不到敌人
- **射击频率**: 过快会消耗性能，过慢会降低攻击力

## 🐛 常见问题

### 问题1: Console显示 "projectilePrefab is null!"

```
原因: ProjectilePrefab引用未设置
解决: 重新运行 Survivor → Setup Player Shooting
```

### 问题2: 看不到子弹

```
检查：
1. ProjectilePrefab的Sprite是否设置？
2. ProjectilePrefab的scale是否太小？
3. projectileSpeed是否太快？（轨迹不可见）
4. Camera视野是否包含子弹轨迹？
```

### 问题3: 子弹不移动

```
检查：
1. ProjectilePrefab是否有Rigidbody2D？
2. ProjectilePrefab的Rigidbody2D是否simulated？
3. projectileSpeed是否>0？
```

## 🔄 关闭自动射击

如果需要手动射击：

```
步骤：
1. Hierarchy → Player
2. Inspector → PlayerController
3. Auto Fire → false
4. Ctrl+S保存
```

手动射击需要添加射击输入代码：

```csharp
// 在PlayerController.Update中添加：
if (Input.GetButtonDown("Fire1"))
{
    FireProjectile();
}
```

## 📚 相关文档

- [SetupEnemySpawner](README_SetupEnemySpawner.md) - 配置敌人生成系统
- [ScaleEnemyPrefab](README_ScaleEnemyPrefab.md) - 放大敌人Prefab

## 🎮 射击效果预览

```
正常配置（默认）：
- 每0.5秒射击一次
- 每秒2发子弹
- 子弹速度400（适中可见）
- 子弹向右飞行

进入Play Mode：
✅ Player每0.5秒自动射击
✅ Console显示 "[PlayerController] Fired projectile at (x, y)"
✅ Game View显示子弹轨迹
```

---

**创建时间**: 2026-05-10  
**适用版本**: Unity 2022.3+  
**维护状态**: ✅ 活跃维护