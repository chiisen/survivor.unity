# Survivor Unity Editor 工具集

## 📋 工具概览

本工具集提供Unity Editor菜单命令，用于快速配置Survivor游戏的核心系统。

**最终保留的3个必需工具**：

| 工具 | 菜单路径 | 功能 | 详细文档 |
|------|---------|------|---------|
| **CreateScriptableObjects** | Survivor → Create ScriptableObjects | 创建Enemy配置文件 | [README](README_CreateScriptableObjects.md) |
| **ScaleEnemyPrefab** | Survivor → Scale All Enemy Prefabs 4x | 放大敌人Prefab 4倍 | [README](README_ScaleEnemyPrefab.md) |
| **SetupPlayerShooting** | Survivor → Setup Player Shooting | 配置Player射击系统 | [README](README_SetupPlayerShooting.md) |

---

## 🚀 一键配置流程

```
按顺序执行菜单命令：

1️⃣ Survivor → Create ScriptableObjects
   → 创建Assets/Config/Enemy/*.asset配置文件

2️⃣ Survivor → Scale All Enemy Prefabs 4x
   → 放大EnemyPrefab scale: (1,1,1) → (4,4,1)

3️⃣ Survivor → Setup Player Shooting
   → 设置Player projectilePrefab引用

4️⃣ Ctrl+P (Play Mode)
   → 测试敌人生成和射击功能
```

---

## 🔧 工具详解

### 1. CreateScriptableObjects

**用途**: 创建ScriptableObject配置文件

**创建文件**:
- NormalEnemyConfig.asset
- FastEnemyConfig.asset
- TankEnemyConfig.asset
- RangedEnemyConfig.asset
- EnemySpawnSettings.asset

**何时使用**:
- ❌ Assets/Config/Enemy/目录不存在或为空时
- ✅ 初次设置项目
- ✅ 配置文件损坏需要重建

**详细说明**: [README_CreateScriptableObjects.md](README_CreateScriptableObjects.md)

---

### 2. ScaleEnemyPrefab

**用途**: 批量放大敌人Prefab

**调整内容**:
- Transform Scale: (1,1,1) → (4,4,1)
- CircleCollider2D Radius: 0.08 → 0.32

**何时使用**:
- ❌ EnemyPrefab scale = (1,1,1)（太小）
- ✅ 敌人在游戏中不可见或太小
- ✅ EnemyPrefab刚创建时

**详细说明**: [README_ScaleEnemyPrefab.md](README_ScaleEnemyPrefab.md)

---

### 3. SetupPlayerShooting

**用途**: 配置Player自动射击系统

**配置内容**:
- projectilePrefab引用设置
- autoFire = true（开启自动射击）
- autoFireInterval = 0.5秒

**何时使用**:
- ❌ PlayerController.projectilePrefab = null
- ✅ Player不会自动射击
- ✅ 射击配置丢失

**详细说明**: [README_SetupPlayerShooting.md](README_SetupPlayerShooting.md)

---

## 📊 已完成的配置（无需工具）

以下配置已在项目中完成，无需再次执行：

| 配置项 | 状态 | 说明 |
|--------|------|------|
| **EnemySpawner** | ✅ 已配置 | SimpleEnemySpawner组件已添加，Prefab已引用 |
| **EnemyPrefab Sprite** | ✅ 已设置 | Sprite和Color已配置 |
| **Player Movement** | ✅ 已配置 | PlayerController已设置，移动正常 |
| **Player Scale** | ✅ 已设置 | Scale = (3,3,1)，大小适中 |
| **PuzzleFloor** | ✅ 已生成 | 50个tiles已创建10x5网格 |

---

## 🎮 完整配置状态检查

### 必需配置项检查清单

```
✅ EnemySpawner GameObject存在
✅ SimpleEnemySpawner.normalEnemyPrefab = EnemyPrefab
✅ Player GameObject Tag = "Player"
✅ PlayerController.projectilePrefab = ProjectilePrefab ← 需运行SetupPlayerShooting
✅ EnemyPrefab scale = (4,4,1) ← 需运行ScaleEnemyPrefab
✅ Assets/Config/Enemy/*.asset存在 ← 需运行CreateScriptableObjects
```

### 如果配置缺失

```
检查步骤：
1. Console查看错误日志
2. Inspector检查字段是否为null
3. 运行对应菜单命令
4. Ctrl+S保存场景
5. Ctrl+P测试
```

---

## 🐛 常见问题

### Q1: EnemyPrefab太小看不见

```
原因: scale = (1,1,1)
解决: Survivor → Scale All Enemy Prefabs 4x
结果: scale → (4,4,1)，敌人放大4倍
```

### Q2: Player不射击

```
原因: projectilePrefab = null
解决: Survivor → Setup Player Shooting
结果: projectilePrefab引用设置，autoFire开启
```

### Q3: EnemyPoolManager报错

```
原因: 配置文件缺失
解决: Survivor → Create ScriptableObjects
结果: 创建Assets/Config/Enemy/*.asset
```

---

## 📚 项目结构

```
Assets/
├── Scripts/Editor/           ← Editor工具集（本目录）
│   ├── CreateScriptableObjects.cs    ← 创建配置文件
│   ├── ScaleEnemyPrefab.cs           ← 放大Prefab
│   ├── SetupPlayerShooting.cs        ← 配置射击
│   ├── README.md                     ← 总览文档（本文件）
│   ├── README_CreateScriptableObjects.md
│   ├── README_ScaleEnemyPrefab.md
│   └── README_SetupPlayerShooting.md
│
├── Config/Enemy/            ← 配置文件（需运行CreateScriptableObjects）
│   ├── NormalEnemyConfig.asset
│   ├── FastEnemyConfig.asset
│   ├── TankEnemyConfig.asset
│   ├── RangedEnemyConfig.asset
│   └── EnemySpawnSettings.asset
│
├── Prefabs/                 ← Prefab资产
│   ├── EnemyPrefab.prefab     ← scale需放大到(4,4,1)
│   ├── FastEnemyPrefab.prefab
│   ├── TankEnemyPrefab.prefab
│   ├── RangedEnemyPrefab.prefab
│   ├── Player.prefab          ← projectilePrefab需设置
│   └── ProjectilePrefab.prefab
│
└── Scenes/MainScene.unity   ← 主场景
    ├── Player (Tag: Player)
    │   └ PlayerController
    │      └ projectilePrefab: null ← 需设置
    │
    └── EnemySpawner
        └ SimpleEnemySpawner
           └ normalEnemyPrefab: EnemyPrefab ✅
```

---

## 💡 最佳实践

### 配置顺序

```
推荐顺序：
1. CreateScriptableObjects → 创建配置文件（最优先）
2. ScaleEnemyPrefab → 放大敌人
3. SetupPlayerShooting → 配置射击
```

### 验证配置

```
进入Play Mode后观察：
✅ Console显示生成日志
✅ Console显示射击日志
✅ Game View可见敌人（大小适中）
✅ Game View可见子弹轨迹
```

### 避免重复执行

```
判断是否需要执行：
- CreateScriptableObjects: Assets/Config/Enemy/目录为空时才执行
- ScaleEnemyPrefab: EnemyPrefab scale < 4时才执行
- SetupPlayerShooting: projectilePrefab = null时才执行
```

---

## 🎯 预期效果

### 敌人系统
- ✅ 每2秒生成一个敌人
- ✅ 敌人大小适中（Scale 4x，占视野25%）
- ✅ 最多50个敌人同时存在
- ✅ 敌人向Player移动

### 射击系统
- ✅ 每0.5秒自动射击
- ✅ 子弹向右飞行（速度400）
- ✅ Console显示射击日志

---

**工具维护**: Chiisen Liao  
**创建时间**: 2026-05-10  
**适用版本**: Unity 2022.3+  
**最终工具**: 3个必需工具（已删除暂时性工具）