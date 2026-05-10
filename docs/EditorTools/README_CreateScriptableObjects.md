# CreateScriptableObjects 工具说明

## 📋 功能概述

自动创建Enemy配置的ScriptableObject资产文件，用于管理不同类型敌人的属性和行为。

## 🎯 使用场景

- **初次设置项目**：创建基础的Enemy配置文件
- **重置配置**：当配置文件损坏或丢失时，重新生成
- **快速配置**：避免手动创建多个配置文件的繁琐过程

## 🚀 使用方法

### 菜单路径
```
Unity Editor → 菜单栏 → Survivor → Create ScriptableObjects
```

### 执行结果

自动创建以下配置文件：
- `Assets/Config/Enemy/NormalEnemyConfig.asset` - 普通敌人配置
- `Assets/Config/Enemy/FastEnemyConfig.asset` - 快速敌人配置
- `Assets/Config/Enemy/TankEnemyConfig.asset` - 重型敌人配置
- `Assets/Config/Enemy/RangedEnemyConfig.asset` - 远程敌人配置
- `Assets/Config/Enemy/EnemySpawnSettings.asset` - 敌人生成设置

### Console输出

```
✅ ScriptableObjects created successfully!
```

## 📊 配置参数说明

### NormalEnemyConfig（普通敌人）
- **移动速度**: 2 units/秒
- **最大生命**: 10
- **经验值**: 5
- **生成时间**: 0秒开始
- **颜色**: 红色
- **权重曲线**: 0秒=1.0 → 60秒=0.5 → 300秒=0.2

### FastEnemyConfig（快速敌人）
- **移动速度**: 4 units/秒
- **最大生命**: 5
- **经验值**: 3
- **生成时间**: 30秒开始
- **颜色**: 黄色
- **权重曲线**: 0秒=0.0 → 30秒=0.3 → 60秒=0.6 → 300秒=0.4

### TankEnemyConfig（重型敌人）
- **移动速度**: 1 units/秒
- **最大生命**: 30
- **经验值**: 15
- **生成时间**: 60秒开始
- **颜色**: 紫色
- **权重曲线**: 0秒=0.0 → 60秒=0.2 → 120秒=0.5 → 300秒=0.6

### RangedEnemyConfig（远程敌人）
- **移动速度**: 1.5 units/秒
- **最大生命**: 8
- **经验值**: 8
- **攻击范围**: 5 units
- **攻击间隔**: 2秒
- **生成时间**: 90秒开始
- **颜色**: 青色
- **权重曲线**: 0秒=0.0 → 90秒=0.1 → 180秒=0.3 → 300秒=0.5

## ⚙️ 配置文件作用

### EnemyTypeConfig.asset
- 定义单个敌人类型的所有属性
- 包含移动速度、生命值、经验值等基础参数
- 包含生成权重曲线（控制不同时间的生成概率）

### EnemySpawnSettings.asset
- 整合所有敌人类型配置
- 定义全局生成参数（间隔、最大数量等）
- 作为EnemyPoolManager的配置引用

## 🔧 手动修改

创建后可以在Inspector中手动调整参数：

```
步骤：
1. Project面板 → Assets/Config/Enemy/
2. 选择配置文件（如NormalEnemyConfig）
3. Inspector中修改参数
4. Ctrl+S保存
```

## ⚠️ 注意事项

- **避免重复创建**：已有配置文件时再次执行会覆盖原文件
- **引用检查**：创建后需在GameManager或EnemyPoolManager中引用
- **备份配置**：修改前建议备份原配置文件

## 📚 相关文档

- [SetupEnemySpawner](README_SetupEnemySpawner.md) - 配置敌人生成系统
- [ScaleEnemyPrefab](README_ScaleEnemyPrefab.md) - 放大敌人预制件

## 🎮 效果验证

创建配置后：
1. 运行 `Survivor → Setup Enemy Spawner`
2. 进入Play Mode (Ctrl+P)
3. 观察不同类型敌人的生成和行为

---

**创建时间**: 2026-05-10  
**适用版本**: Unity 2022.3+  
**维护状态**: ✅ 活跃维护