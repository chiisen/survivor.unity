# Changelog

本專案的所有重要更改都會記錄在此文件中。

格式基於 [Keep a Changelog](https://keepachangelog.com/zh-TW/1.0.0/)，
並且本專案遵循 [Semantic Versioning](https://semver.org/lang/zh-TW/) 版本號規則。

---

## [Unreleased]

### 新增
- 待測試：Play Mode 測試（敵人生成、主角射擊）

---

## [1.0.0] - 2026-05-10

### 新增
- ✅ SimpleEnemySpawner 敵人生成器（Assets/Scripts/Core/SimpleEnemySpawner.cs）
  - 每2秒生成一個敵人
  - 最多50個敵人
  - 在主角周圍8 units處生成
- ✅ PlayerController 自動射擊功能（Assets/Scripts/Core/PlayerController.cs）
  - 每0.5秒自動發射子彈
  - 子彈速度400 units/s
- ✅ ScaleEnemyPrefab Editor工具（Assets/Scripts/Editor/ScaleEnemyPrefab.cs）
  - 批量放大敵人Prefab到4x
  - 支援4種敵人類型（Normal/Fast/Tank/Ranged）
- ✅ SetupPlayerShooting Editor工具（Assets/Scripts/Editor/SetupPlayerShooting.cs）
  - 配置主角自動射擊功能
  - 設置ProjectilePrefab引用
- ✅ docs/EditorTools 文檔目錄
  - README.md（工具集總覽）
  - README_CreateScriptableObjects.md
  - README_ScaleEnemyPrefab.md
  - README_SetupPlayerShooting.md

### 變更
- ✅ Enemy Prefab放大4倍（Assets/Prefabs/*.prefab）
  - EnemyPrefab.scale = (4, 4, 1)
  - FastEnemyPrefab.scale = (4, 4, 1)
  - TankEnemyPrefab.scale = (4, 4, 1)
  - RangedEnemyPrefab.scale = (4, 4, 1)
- ✅ EnemyController 修復追踪Player邏輯（Assets/Scripts/Core/EnemyController.cs）
  - 使用GameObject.FindGameObjectWithTag("Player")
  - 正確取得Player Transform
- ✅ README.md 添加專案目錄結構說明
  - 根目錄結構（13個主要目錄）
  - Assets/ 目錄詳解（Unity資源）
  - docs/ 目錄詳解（文檔目錄）
  - Unity .meta 文件系統說明

### 刪除
- ✅ 删除根目錄無用Prefab檔案
  - EnemySpawner.prefab（誤操作生成的临时文件）
  - EnemySpawnerGO.prefab（誤操作生成的临时文件）
  - FloorTile.prefab（誤操作生成的临时文件）
- ✅ 删除暫時性Editor工具（Assets/Scripts/Editor/）
  - CreateEnemyConfigs.cs（已手動配置）
  - FixGameManager.cs（已修復）
  - ManualSetupPlayer.cs（已手動配置）
  - PlayerScaler.cs（已用ScaleEnemyPrefab替代）
  - SetupEnemyPrefab.cs（已手動配置）
  - SetupPlayerSprite.cs（已手動配置）
  - TestPlayerInput.cs（已測試完成）

### 修復
- ✅ Player移動控制修復（Assets/Scripts/Core/PlayerController.cs）
  - gravityScale = 0（不受重力）
  - freezeRotation = true（不旋轉）
  - speed = 5（適中速度）
- ✅ EnemyController追踪Player修復
  - 修正FindObjectOfType錯誤
  - 使用FindGameObjectWithTag正確方法

---

## [0.1.0] - 2026-05-06

### 新增
- ✅ PuzzleFloor 地板系統（Assets/Scenes/MainScene.unity）
  - 50個tiles（10x5網格）
  - floor_tileset.png素材
- ✅ Player主角控制（Assets/Scripts/Core/PlayerController.cs）
  - WASD/方向鍵移動
  - SpriteRenderer設置
- ✅ Enemy敵人Prefab（Assets/Prefabs/EnemyPrefab.prefab）
  - EnemyController追踪邏輯
  - CircleCollider2D碰撞器
- ✅ Projectile子彈Prefab（Assets/Prefabs/ProjectilePrefab.prefab）
  - 子彈移動邏輯
  - 碰撞检测

---

## 版本說明

- **[Unreleased]**: 正在開發中的功能
- **[1.0.0]**: 核心遊戲功能完成版本
- **[0.1.0]**: 基礎框架搭建版本

---

## 版本號規則

遵循 [Semantic Versioning](https://semver.org/lang/zh-TW/):

- **主版本號（MAJOR）**: 不兼容的API修改
- **次版本號（MINOR）**: 向下兼容的功能新增
- **修訂號（PATCH）**: 向下兼容的問題修復

範例：
- `1.0.0` → `1.1.0`: 新增SimpleEnemySpawner
- `1.1.0` → `1.1.1`: 修復EnemyController追踪錯誤
- `1.1.1` → `2.0.0`: 重構遊戲架構（不兼容）

---

## 更新類型說明

### 新增
- 新功能、新文件、新工具

### 變更
- 现有功能的修改、優化

### 刪除
- 删除文件、功能、工具

### 修復
- Bug修復、錯誤修正

### 安全
- 安全漏洞修復（本專案暫無）