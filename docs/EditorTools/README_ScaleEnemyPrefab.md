# ScaleEnemyPrefab 工具说明

## 📋 功能概述

批量调整敌人预制件（Prefab）的大小，将scale放大4倍并相应调整碰撞器半径。

## 🎯 使用场景

- **敌人太小**：游戏中敌人不可见或太小
- **比例调整**：需要统一调整所有敌人Prefab的大小
- **碰撞范围**：调整碰撞器以匹配新的scale

## 🚀 使用方法

### 单个Prefab放大
```
Unity Editor → 菜单栏 → Survivor → Scale Enemy Prefab 4x
```

仅放大 `EnemyPrefab.prefab`

### 批量放大所有敌人Prefab
```
Unity Editor → 菜单栏 → Survivor → Scale All Enemy Prefabs 4x
```

批量放大以下Prefab：
- `EnemyPrefab.prefab`
- `FastEnemyPrefab.prefab`
- `TankEnemyPrefab.prefab`
- `RangedEnemyPrefab.prefab`

### Console输出

```
✅ [ScaleEnemyPrefab] EnemyPrefab scaled to (4, 4, 1)
✅ [ScaleEnemyPrefab] Collider radius: 0.32
✅ [ScaleEnemyPrefab] 🎮 All enemy prefabs scaled successfully!
```

## 📊 调整内容

### Transform Scale
- **原始值**: (1, 1, 1)
- **调整后**: (4, 4, 1)
- **效果**: 敌人视觉大小放大4倍

### CircleCollider2D Radius
- **原始值**: 0.08
- **调整后**: 0.32 (0.08 × 4)
- **效果**: 碰撞范围匹配新的大小

## 📐 大小对比

### Player vs Enemy

| 对象 | Scale | 显示大小 | 比例 |
|------|-------|---------|------|
| **Player** | (3, 3, 1) | 约1.92×1.92 units | 基准 |
| **Enemy** | (4, 4, 1) | 约2.56×2.56 units | Player的1.33倍 |

### Camera视野
- Camera orthographic size: 5
- 视野范围: 约10×10 units
- Enemy占视野: 约25%（适中可见）

## 🔧 手动调整参数

如果需要自定义大小：

```
步骤：
1. Project面板 → Assets/Prefabs/EnemyPrefab.prefab
2. Inspector中修改Transform → Scale
3. 调整CircleCollider2D → Radius
4. Ctrl+S保存

建议比例：
Scale = N
Collider Radius = 0.08 × N
```

## ⚠️ 注意事项

- **批量调整**：建议使用批量命令统一调整所有敌人Prefab
- **比例一致**：确保所有敌人Prefab大小比例一致
- **碰撞器匹配**：手动调整scale时需同步调整collider radius
- **已生成敌人**：不会影响已经在场景中的敌人实例

## 🔄 逆向操作

如果需要恢复原始大小：

```
手动步骤：
1. Project → Prefab → EnemyPrefab
2. Inspector → Transform → Scale → (1, 1, 1)
3. CircleCollider2D → Radius → 0.08
4. Ctrl+S保存
```

## 📚 相关文档

- [SetupEnemySpawner](README_SetupEnemySpawner.md) - 配置敌人生成系统
- [CreateScriptableObjects](README_CreateScriptableObjects.md) - 创建敌人配置

## 🎮 效果验证

调整后：
1. 进入Play Mode (Ctrl+P)
2. 观察生成的敌人大小
3. 检查碰撞是否合理（敌人接近Player时触发）

### 可见性检查

```
✅ 敌人应明显可见（占视野约25%）
✅ 比Player略大（约1.33倍）
✅ 不会太小导致难以察觉
✅ 不会太大导致遮挡Player
```

---

**创建时间**: 2026-05-10  
**适用版本**: Unity 2022.3+  
**维护状态**: ✅ 活跃维护