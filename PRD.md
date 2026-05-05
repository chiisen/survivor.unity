# 📋 survivor.unity - 戰鬥 MVP 產品需求文件 (PRD)

## 1. 專案願景
打造一個 Unity 2D 驅動的類倖存者 (Survivor-like) 遊戲，復刻《小女巫倖存者》的核心割草體驗。玩家扮演盔甲戰士，使用劍與魔法彈消滅敵人。

**技術平台：**
- **引擎**：Unity 2022.3 LTS
- **渲染管線**：URP (Universal Render Pipeline)
- **視角**：2D 遊戲模式
- **美術風格**：Pixel Art (像素風格)
- **目標平台**：PC (Windows/Mac)

## 2. 核心戰鬥機制 (The MVP Loop)
*   **自動戰鬥**：玩家負責走位避開碰撞，盔甲戰士自動鎖定範圍內最近敵人並揮劍發射魔法彈。
*   **攻擊範圍**：基礎攻擊範圍 300px，僅對範圍內敵人發動攻擊。
*   **割草感**：高頻率的敵人生成與流暢的擊殺回饋，配合爆炸特效。
*   **成長循環**：擊殺 -> 掉落經驗球 -> 拾取 -> 升級 -> 三選一隨機技能強化。
*   **連殺獎勵**：一次擊殺多隻重疊怪物可獲得攻擊速度加成。

## 3. 實體規格 (Entity Specs)

### A. 盔甲戰士 (Player)
*   **外觀**：銀色盔甲頭盔 + 盔甲身體 + 金色劍柄護手 + 藍色長劍。
*   **移動**：WASD 或方向鍵四向控制，移動時角色面向移動方向。
*   **屬性**：
    - 血量 (HP)：100
    - 移速 (Speed)：200
    - 拾取半徑 (Pickup Range)：80
    - 攻擊範圍 (Attack Range)：300（基礎）+ 可升級
    - 射擊間隔 (Fire Rate)：0.5秒
    - 傷害 (Damage)：1
    - 子彈速度 (Projectile Speed)：400
    - 多重射擊 (Projectile Count)：3
*   **動作**：
    - 受擊時短暫閃爍紅光，1秒無敵時間。
    - 攻擊時揮劍動畫（劍從 -45° 揮至 +45°），0.15秒動畫時間。
    - 揮劍時劍尖白光閃爍 + 藍色軌跡光效。
*   **範圍顯示**：
    - 內圈（基礎）：藍色半透明圓圈
    - 外圈（升級）：綠色半透明圓圈（升級後顯示）
    - 中圈：綠色漸層（連接基礎與升級範圍）

### B. 怪物 (Enemies)
實作四種敵人類型，隨遊戲時間逐步出現：

#### 1. 普通型 (Normal)
*   **外觀**：紅色圓形怪物，白色眼睛 + 生氣表情。
*   **屬性**：血量 1、速度 50-70 px/秒、傷害 10、經驗值 10。
*   **出現時間**：遊戲開始。

#### 2. 快速型 (Fast)
*   **外觀**：綠色圓形怪物，體型較小，頭頂三角尖角。
*   **屬性**：血量 1、速度 90-110 px/秒、傷害 8、經驗值 12。
*   **出現時間**：30秒後權重增加。

#### 3. 坦克型 (Tank)
*   **外觀**：灰色大型怪物，紅色眼睛 + 大嘴，外層光環。
*   **屬性**：血量 3、速度 25-45 px/秒、傷害 20、經驗值 25。
*   **特殊**：頭頂顯示血量條。
*   **出現時間**：60秒後權重增加。

#### 4. 遠程型 (Ranged)
*   **外觀**：紫色圓形怪物，黃色眼睛 + 弧形嘴，頭頂圓形標記。
*   **屬性**：血量 2、速度 30-50 px/秒、傷害 10、經驗值 15。
*   **特殊**：每 2 秒向玩家發射紫色子彈（傷害 5）。
*   **出現時間**：45秒後開始出現。

#### 共同機制
*   **生成**：生成於螢幕外，直線追蹤玩家座標。
*   **連殺**：連鎖範圍 40px，被擊殺時範圍內其他怪物連帶消滅。
*   **權重系統**：根據遊戲時間動態調整各類型出現機率。

### C. 魔法彈 (Projectiles)
*   **外觀**：橙色圓形彈體 + 白色高光 + 拖尾光效。
*   **行為**：直線飛行，碰撞敵人後消失並銷毀敵人。
*   **多重射擊**：升級後可同時發射多顆（扇形散布 ±22.5°）。

### D. 經驗球 (Experience Orbs)
*   **外觀**：綠色圓形 + 脈動動畫 + 白色高光 + 外層光暈。
*   **行為**：進入拾取範圍後自動飛向玩家（磁吸效果）。
*   **屬性**：每顆價值 10 經驗值。

### E. 傷害數字 (Damage Numbers)
*   **外觀**：浮動數字，高傷害（≥5）顯示金色 + 外框描邊。
*   **動畫**：放大 → 向上漂浮 → 漸隱消失（0.8秒）。
*   **字體大小**：16px + 傷害值 × 2（上限 36px）。

### F. 連殺顯示 (Chain Kill Display)
*   **觸發條件**：一次擊殺 2隻以上怪物（含連鎖）。
*   **顯示文字**：
    - 2 kills: "DOUBLE KILL!"（金色）
    - 3 kills: "TRIPLE KILL!"（橙色）
    - 4 kills: "QUAD KILL!"（紅色）
    - 5 kills: "MEGA KILL!"（深紅）
    - 6-9 kills: "ULTRA KILL!"（紫色）
    - 10+ kills: "GODLIKE!"（深紫）
*   **動畫效果**：
    - 放大縮放（scale 1.2 ~ 2.5）
    - 向上漂浮 30px
    - 光暈效果（shadowBlur 20）
    - 漸隱消失（1.5秒）

## 4. 連殺系統 (Chain Kill System)
*   **觸發條件**：一次擊殺 2隻以上怪物（含連鎖）。
*   **獎勵效果**：
    - 攻擊速度 +30%（射擊間隔 × 0.7）
    - 持續時間：5秒
    - 限制：buff期間不再叠加新的攻擊速度增益
*   **UI 提示**：
    - 頂部顯示綠色通知框："⚡ 連殺！攻擊速度 +30%"
    - 附帶倒計時進度條（5秒倒數）
    - 消失時滑出動畫

## 5. 視覺特效 (Visual Effects)
### A. 爆炸特效 (Explosion)
*   **中心閃光**：白→黃色快速擴散光圈（30px → 擴展）。
*   **粒子爆散**：12顆紅/橙色粒子向外飛射（速度 80-140 px/秒）。
*   **核心粒子**：6顆較大紅色粒子（速度 30-50 px/秒）。
*   **持續時間**：0.5秒，粒子逐漸消失。

### B. 揮劍特效
*   **軌跡光效**：藍色半透明揮劍弧線。
*   **劍尖閃光**：攻擊時劍尖白光閃爍。
*   **劍身漸層**：白→灰→銀色金屬質感。

### C. 範圍圈特效
*   **基礎圈**：藍色 rgba(52, 152, 219, 0.3)，2px 線宽。
*   **升級圈**：綠色 rgba(46, 204, 113, 0.3)，2px 線宽。
*   **漸層圈**：綠色 rgba(46, 204, 113, 0.15)，1px 線宽（中間位置）。

### D. 背景裝飾 (Ground Decoration)
隨機生成地面裝飾物與環境粒子，豐富視覺效果。

#### 裝飾物類型
| 類型 | 數量 | 特性 |
|------|------|------|
| 石頭 | 25% | 灰色不規則形狀，靜止 |
| 草叢 | 35% | 綠色草葉，左右微幅搖擺 |
|灌木 | 20% | 深綠圓形組合，靜止 |
|裂痕 | 20% | 細線條裂縫，靜止 |

#### 環境粒子
*   **數量**：20顆漂浮粒子。
*   **顏色**：白色半透明（alpha 0.1~0.25）。
*   **大小**：1~3px。
*   **運動**：向右下漂移（vx 10~30, vy 5~15），循環重生。
*   **透明度**：裝飾物 alpha 0.3~0.5，粒子 alpha 0.1~0.25。

## 6. 音效系統 (Audio System)
使用 Web Audio API 實作合成音效與背景音樂。

### A. 音效列表 (Sound Effects)
| 音效名稱 | 觸發時機 | 音色 | 特性 |
|---------|---------|------|------|
| swing | 玩家揮劍攻擊 | square wave | 200Hz, 0.15s |
| hit | 子彈命中敵人 | sine wave | 400Hz, 0.1s |
| kill | 敵人死亡 | square wave | 800→400Hz, 0.3s (下降) |
| chainKill | 連殺觸發 | sine wave | 1000→1500Hz, 0.5s (上升) |
| levelUp | 玩家升級 | sine wave | 600→1800Hz, 0.8s (漸升) |
| damage | 玩家受傷 | square wave | 150Hz, 0.2s |
| pickup | 拾取經驗球 | sine wave | 500Hz, 0.1s |
| gameOver | 遊戲結束 | sine wave | 100Hz, 1.0s |

### B. 背景音樂 (BGM)
*   **類型**：三角波 oscillator + LFO 調變。
*   **頻率**：基頻 80Hz，LFO 0.5Hz 調變幅度 20Hz。
*   **音量**：0.1 × bgmVolume × masterVolume。
*   **控制**：遊戲開始時啟動，結束時停止。

### C. 音量控制
*   **主音量**：0.5（預設）。
*   **音效音量**：0.7（預設）。
*   **背景音量**：0.3（預設）。
*   **可動態調整**：透過 setMasterVolume/setSfxVolume/setBgmVolume。

## 7. 天賦系統 (Talent System)
升級時隨機提供 3 項強化選項（8 種天賦池）：

| 天賦名稱 | 效果 | 圖示 |
|---------|------|------|
| 生命強化 | 最大生命值 +20 | ❤️ |
| 疾風步 | 移動速度 +30 | 💨 |
| 磁力手套 | 拾取範圍 +30 | 🧲 |
| 鹰眼 | 攻擊範圍 +50 | 👁️ |
| 急速射擊 | 射擊間隔 -0.08秒 | ⚡ |
| 魔力增幅 | 傷害 +1 | ✨ |
| 子彈加速 | 子彈速度 +100 | 🚀 |
| 多重射擊 | 同時發射 +1 顆子彈 | 🎯 |

## 8. 波次系統 (Wave System)
每 60 秒一波，波次間有 5 秒休息時間，每 5 波出現 Boss。

### A. 波次機制
*   **波次周期**：60秒戰鬥 + 5秒休息。
*   **敵人數量**：每波基礎 10隻，隨波次增加（×1.3）。
*   **生成間隔**：基礎 1.5秒，隨波次減少（每波 -0.05秒，最小 0.3秒）。
*   **敵人血量**：每 3波增加 50%（hpMultiplier）。

### B. Boss戰機制
*   **觸發條件**：第 5、10、15...波（每 5波）。
*   **Boss屬性**：
    - 體型：半徑 35px（最大）
    - 血量：50HP
    - 移速：25 px/秒
    - 傷害：30
    - 經驗值：100
    - 射擊：每 1.5秒發射紫色子彈
*   **Boss外觀**：
    - 深紅色主體 + 金色皇冠
    - 紅色光環效果
    - 大型血量條（紅色顯示）
    - 不悅嘴型（倒弧線）
*   **生成時間**：波次進行 50%（30秒）時生成。
*   **Boss波敵人數**：減少 50%（集中戰鬥）。

### C. 波次UI
*   **波次公告**：
    - 新波次：「第 N 波開始！」（橙色）
    - Boss波：「BOSS 波！第 N 波」（紅色）
    - 波次結束：「波次結束！休息時間」（綠色）
    - 漸隱動畫：2秒後消失
*   **波次信息**：
    - 左下角顯示「波次: N」
    - Boss波標記「BOSS 波！」
    - 休息時間標記「休息時間」

## 9. 技術架構（Unity 2D 版本）

### A. Unity 核心技術
*   **引擎**：Unity 2022.3 LTS
*   **渲染管線**：URP (Universal Render Pipeline) - 適合 2D 遊戲，支援 2D Renderer Features
*   **語言**：C# (.NET Standard 2.1)
*   **視角**：2D 遊戲模式（使用 Unity 2D Sprite 系統）
*   **輸入系統**：新 Input System（或舊 Input Manager）

### B. Unity 系統與組件

#### 1. 渲染系統
*   **2D Sprite Renderer**：使用 Sprite Renderer 組件渲染所有 2D 物件
*   **Sprite Atlas**：打包 Pixel Art 美術資產，減少 Draw Calls
*   **URP 2D Renderer**：2D Lighting、2D Shadows
*   **Sorting Layers**：分層渲染（Background → Ground → Player → Enemies → Projectiles → Effects → UI）

#### 2. 粒子與特效系統
*   **Particle System**：Unity 內建粒子系統（爆炸特效、揮劍軌跡、環境粒子）
*   **Visual Effect Graph**：可選的高級特效系統（Boss 死亡特效、魔法彈軌跡）
*   **Line Renderer**：揮劍軌跡光效
*   **Trail Renderer**：魔法彈拖尾效果

#### 3. 音效系統
*   **Audio Source**：每個音效物件（揮劍、命中、擊殺、升級）
*   **Audio Mixer**：控制主音量、音效音量、背景音量
*   **Audio Listener**：跟隨玩家位置（2D 音效）

#### 4. 物理與碰撞
*   **2D Physics**：使用 Unity 2D Physics 系統
*   **Collider 2D**：
    - Circle Collider 2D（玩家、敵人、經驗球）
    - Box Collider 2D（魔法彈，可選）
*   **Rigidbody 2D**：動態物件（敵人、魔法彈）
*   **Collision Detection**：Discrete 或 Continuous（魔法彈高速移動）

#### 5. 輸入系統
*   **Input System Package**（推薦）：
    - WASD 移動（Keyboard）
    - Q 技能（Keyboard）
    - ESC/P 暫停（Keyboard）
*   **Input Manager**（舊版）：直接使用 Input.GetKeyDown/GetKey

#### 6. 資源管理
*   **ScriptableObject**：
    - EnemyData（敵人屬性配置）
    - TalentData（天賦配置）
    - WaveData（波次配置）
    - AchievementData（成就配置）
*   **Prefab**：
    - Player Prefab
    - Enemy Prefabs（9種敵人）
    - Projectile Prefab
    - Experience Orb Prefab
    - Effect Prefabs（爆炸、分裂、護盾破碎）

### C. Unity 優化技術

#### 1. 空間分割優化
*   **替代方案 A**：Unity 2D Physics 內建碰撞檢測（Physics2D.OverlapCircle）
*   **替代方案 B**：自製 Spatial Grid（C# 版本，保持 JavaScript版本的優化邏輯）
*   **Grid Size**：100px 格子大小（與 JavaScript 版本相同）

#### 2. 物件池化
*   **Object Pooling**：
    - Projectile Pool（魔法彈池）
    - Explosion Pool（爆炸特效池）
    - Experience Orb Pool（經驗球池）
*   **實作方式**：
    - 使用 Unity 2021+ 內建 ObjectPool API
    - 或自製 PoolManager（List<T> + Active/Inactive 管理）

#### 3. Sprite Atlas
*   **打包策略**：
    - Player Sprites Atlas
    - Enemy Sprites Atlas
    - Effect Sprites Atlas
    - UI Sprites Atlas
*   **減少 Draw Calls**：Sprite Atlas 可批次渲染相同 Atlas 的 Sprite

#### 4. GC 優化
*   **避免 Instantiate/Destroy 頻繁呼叫**：使用物件池化
*   **避免 string 拼接**：使用 StringBuilder（傷害數字、連殺顯示）
*   **避免 foreach**：使用 for loop（迭代敵人、投射物陣列）

### D. Unity 跨平台考量

#### 1. PC (Windows/Mac)
*   **分辨率**：支援多種分辨率（1920x1080、2560x1440）
*   **窗口模式**：Windowed / Fullscreen
*   **輸入**：Keyboard + Mouse

#### 2. 未來跨平台（預留）
*   **Mobile**：需調整 UI 尺寸、觸控輸入
*   **WebGL**：需優化資源大小、GC 壓力
*   **Console**：需調整輸入映射

### 9.1 Unity Package 配置

#### 必要 Package
| Package | 版本 |用途 |
|---------|------|------|
| **Universal RP** | 14.0.x | URP 渲染管線（Unity 2022.3 LTS 内建） |
| **Input System** | 1.7.x | 新輸入系統（WASD、Q键、ESC/P） |
| **2D Sprite** | 1.0.x | Sprite Atlas、Sprite Editor |
| **TextMeshPro** | 3.0.x | 高品質文字渲染（UI 文字） |
| **Unity Test Framework** | 1.1.x |單元測試、Play Mode 測試 |

#### 可選 Package
| Package | 版本 |用途 |
|---------|------|------|
| **Visual Effect Graph** | 14.0.x | 高級特效（Boss 死亡特效） |
| **Addressables** | 1.21.x | 资源管理（大地圖動態加載） |
| **Cinemachine** | 2.9.x | 相機控制（2D 相機跟隨） |

### 9.2 Unity 專案設定

#### Project Settings 配置
- **Physics 2D**：
  - Gravity: (0, 0)（無重力，2D 遊戲）
  - Default Contact Offset: 0.01
- **Time**：
  - Fixed Timestep: 0.02（50 Hz，物理更新）
  - Maximum Allowed Timestep: 0.1
- **Quality**：
  - Quality Level: Medium（PC 平台）
  - Anti Aliasing: 2x
  - Texture Quality: Full Res
- **Script Execution Order**：
  - GameManager: -100（最先執行）
  - SpatialGrid: -50（Grid 更新）
  - PlayerMovement: 0（默认）
  - UIManager: 100（最後執行）

#### URP Asset 配置
- **2D Renderer Data**：
  - Post Processing: Enable（視野遮罩）
  - HDR: Disable（2D遊戲不需要 HDR）
- **2D Lighting**：
  - 2D Lights: Enable（可選）
  - Light Layers: Default

---

### AI Agent 開發規範（Unity版本）

如果選擇使用引擎，仍可能遇到代碼流程錯誤（如變數未定義），建議：

1.遵守 PRD.md 的「AI Agent開發規範」（Update Loop 四個 Phase）
2. 實作 GameLogger（Console 日誌）
3. 實作 DebugOverlay（可視化調試工具）

**引擎不會完全解決代碼規範問題，仍需要遵守規範。**

### AI Agent 開發規範（建議執行）

以下規範基於實際開發過程中遇到的問題，建議 AI Agent 遵守以避免類似錯誤。

#### A. Update Loop 四個 Phase（建議執行）

**問題根因**: Update Loop 缺少明確的執行順序，導致必要步驟遺漏。

建議將 `game.update(dt)` 分為四個 Phase，每個 Phase 有明確的職責：

```
Phase 1: 清理與準備
  - 清空 SpatialGrid
  - 插入所有活著的實體到 Grid

Phase 2: 狀態更新
  - 更新玩家狀態（player.update()）
  - 更新敵人狀態（enemy.update()）
  - 更新投射物狀態（projectile.update()）

Phase 3: 系統邏輯
  - 自動射擊（autoFire()）
  - 碰撞檢測（checkCollisions()）
  - 清理死亡實體

Phase 4: UI 更新
  - 更新 UI 狀態
  - 更新特效
```

**執行順序**: Phase 1 → Phase 2 → Phase 3 → Phase 4，不可跳過或逆序。

#### B. 必要步驟 Checklist（建議執行）

**問題根因**: 缺少必要步驟的檢查機制，導致核心更新被遺漏。

建議 AI Agent 在完成 Update Loop 時，逐一檢核以下步驟：

**Phase 1 Checklist:**
- [ ] enemyGrid.clear() 已執行
- [ ] 所有敵人已插入 enemyGrid.insert(enemy)
- [ ] enemyGrid.getTotalEntities() === enemies.length

**Phase 2 Checklist:**
- [ ] player.update(dt, keys, canvasWidth, canvasHeight) 已執行
- [ ] 所有敵人 update() 已執行
- [ ] 所有投射物 update() 已執行

**Phase 3 Checklist:**
- [ ] autoFire() 已執行（依賴 player.fireCooldown 已更新）
- [ ] checkCollisions() 已執行（依賴 Grid 已填充）
- [ ] 死亡實體已從陣列中移除

**Phase 4 Checklist:**
- [ ] UI 狀態已更新
- [ ] 特效已更新

#### C. Debug 工具要求（建議執行）

**問題根因**: 缺少 Debug 工具快速定位問題，導致問題發生時無法快速發現。

建議實作以下 Debug 工具：

1. **GameLogger（Console 日誌）**
   - 記錄每個 Phase 的執行狀態
   - 記錄關鍵變數（Grid 體數、fireCooldown 等）
   - 錯誤時 Console 顯示警告

2. **DebugOverlay（可視化調試工具）**
   - 按 Ctrl+D 鍵開啟
   - 顯示 Grid 狀態（格子數、實體數）
   - 顯示 Player 狀態（fireCooldown、canFire）
   - 顯示 Pipeline 狀態（四個 Phase 是否執行）
   - 自動檢測異常並顯示 ⚠ 警告

**異常檢測範例:**
```
⚠ Grid空 → Phase 1 未執行
⚠ 冷卻未更新 → Phase 2 未執行
⚠ 子彈未移動 → Projectile update() 未執行
```

#### D. 代碼流程規範（建議執行）

**問題根因**: 代碼流程混亂，變數在使用前未定義。

建議遵守以下規範：

1. **變數定義順序**
   - 所有變數必須在使用前定義
   - 建議使用 ESLint 或 Biome 檢查 `no-use-before-define`

2. **函數拆分原則**
   - update() 方法職責過重時，拆分為多個子方法
   - 每個子方法只做一件事（單一職責）

3. **SpatialGrid 更新**
   - Grid 必須在碰撞檢測前更新
   - 每幀開始時 clear() + insert() 所有實體

#### E. 錯誤案例與根因分析（警示）

以下錯誤來自實際開發過程，建議 AI Agent 避免相同錯誤：

---

**錯誤案例 #1: chainKills 未定義**

**問題**: 主角只攻擊一次就停止，子彈打不到怪物

**根因**: `game.js:448` 引用了在第 456 行才定義的 `chainKills`變數

**影響**: ReferenceError 打斷遊戲循環，後續攻擊失效

**錯誤代碼:**
```javascript
// 第 448 行 - 引用 chainKills 但尚未定義
const chainKillExpBonus = this.getChainKillExpBonus(chainKills);  // BUG!

// 第 456 行才定義
let chainKills = 1;
```

**修復方案**: 移動 `let chainKills = 1;` 到第 448 行之前

**防堵措施**: 使用 ESLint `no-use-before-define` 檢查，變數必須在使用前定義

---

**錯誤案例 #2: enemyGrid 未更新**

**問題**: 子彈射出後無法命中敵人（敵人血量不下降）

**根因**: `enemyGrid` 未在每幀更新，導致 `getNearby()` 返回空陣列，碰撞檢測失效

**影響**: 所有子彈無法命中敵人，遊戲無法進行

**錯誤代碼:**
```javascript
// game.js 中缺少 enemyGrid 更新
update(dt) {
    // ... 狀態更新
    
    this.autoFire();  // 自動射擊
    
    // ⚠缺少 enemyGrid.clear() 和 enemyGrid.insert()
    
    const nearbyEnemies = this.enemyGrid.getNearby(...);  //返回空陣列！
}
```

**修復方案**: 每幀開始時 `enemyGrid.clear()` + `enemyGrid.insert(enemy)` 所有敵人

**防堵措施**: Phase 1 必須包含 Grid 更新，DebugOverlay 檢測 Grid 空時顯示 ⚠ 警告

---

**錯誤案例 #3: player.update() 未調用**

**問題**: 主角射擊一次後就停止射擊

**根因**: `player.update(dt, keys, canvasWidth, canvasHeight)` 未被調用，導致 `fireCooldown` 不減少，`canFire()` 永遠返回 false

**影響**: 主角只能射擊一次，之後無法繼續攻擊

**錯誤代碼:**
```javascript
update(dt) {
    this.gameTime += dt;
    
    // ⚠ 缺少 player.update()！
    
    this.autoFire();  // autoFire() 檢查 canFire()，但 fireCooldown 未更新
    
    // player.update() 在這裡才調用（錯誤位置）
    this.player.update(dt, this.keys, this.canvas.width, this.canvas.height);
}
```

**修復方案**: 在 Phase 2（狀態更新）時調用 `player.update()`，且必須在 `autoFire()` 之前

**防堵措施**: Phase 2 必須包含 player.update()，DebugOverlay 檢測 fireCooldown 未更新時顯示 ⚠ 警告

---

**錯誤案例共同根因:**

1. **Update Loop 缺少明確的執行順序**
2. **缺少必要步驟的檢查機制**
3. **缺少 Debug 工具快速定位問題**

**建議 AI Agent:**

- 遵守 Update Loop 四個 Phase
- 使用 Checklist 檢核必要步驟
-實作 GameLogger 和 DebugOverlay
- 變數必須在使用前定義
- Grid 必須在碰撞檢測前更新

#### F. 測試檢核表（建議執行）

每個功能完成後，建議測試以下項目：

**Update Loop 測試:**
- [ ] Phase 1: Grid 已清空並填充所有實體
- [ ] Phase 2: 所有實體狀態已更新（player.update、enemy.update）
- [ ] Phase 3: 系統邏輯已執行（autoFire、checkCollisions）
- [ ] Phase 4: UI 已更新

**Debug 工具測試:**
- [ ] GameLogger: Console 日誌正常輸出
- [ ] DebugOverlay: 按 Ctrl+D 鍵開啟
- [ ] Grid 狀態可視化（格子數、實體數）
- [ ] Player 狀態可視化（fireCooldown、canFire）
- [ ] 錯誤警告自動顯示

**功能測試:**
- [ ] 主角能持續自動射擊
- [ ] 子彈能正常命中敵人
- [ ] 敵人死亡後能正常消失
- [ ] 經驗值能正常拾取
- [ ] 升級彈窗能正常顯示

## 10. UI 系統規格
### A. 狀態列 (Stats Bar)
*   **血量條**：200px 寬，紅色漸層 fill，顯示 "當前/最大"。
*   **經驗條**：200px 寬，橙色漸層 fill，顯示 "當前/升級需求"。
*   **等級顯示**：金色字體 "Lv. N"。
*   **計時器**：右上角，白色 "MM:SS" 格式。

### B. 升級彈窗
*   **標題**："升級！選擇一項強化"。
*   **選項卡片**：藍色漸層背景，hover 時上浮 + 金色邊框。
*   **卡片內容**：圖示 + 名稱 + 描述。

### C. Buff 通知
*   **位置**：頂部中央偏下（距狀態列 40px）。
*   **外觀**：綠色漸層背景 + 白色邊框 + 陰影。
*   **內容**：圖示 + 文字 + 倒計時條。
*   **動畫**：滑入（0.3秒）→ 滑出（0.3秒）。

### D. 遊戲結束畫面
*   **本次成績**：
    - 等級（新紀錄標記 🏆）
    - 擊殺數
    - Boss擊殺數
    - 最高波次（新紀錄標記 🏆）
    - 存活時間（新紀錄標記 🏆）
*   **歷史紀錄**：
    - 最高等級
    - 最長存活時間
    - 總擊殺數
    - 最高波次
    - Boss擊殺總數
    - 總遊戲次數
*   **重新開始按鈕**：綠色漸層，hover 放大 + 陰影。

### E. 暫停畫面
*   **觸發方式**：ESC 或 P 鍵。
*   **外觀**：深灰漸層背景 + 藍色邊框。
*   **顯示內容**：「遊戲暫停」標題 + 操作提示。
*   **背景半透明**：rgba(0, 0, 0, 0.7) 覆蓋遊戲畫面。
*   **限制**：升級彈窗開啟時無法暫停。

## 11. 存檔系統 (Storage System)
使用 localStorage 儲存玩家遊戲紀錄，持久化保存歷史最佳成績。

### A. 儲存項目
| 項目 | 描述 | 類型 |
|------|------|------|
| highestLevel | 最高達成等級 | 整數 |
| longestTime | 最長存活時間（秒） | 整數 |
| totalKills | 總擊殺數（累積所有遊戲） | 整數 |
| highestWave | 最高波次 | 整數 |
| totalGames | 總遊戲次數 | 整數 |
| bossesKilled | Boss擊殺總數 | 整數 |

### B. 更新時機
*   **遊戲結束時**：自動更新並儲存統計資料。
*   **新紀錄檢測**：比對歷史紀錄，破紀錄項目標記 🏆 符號。
*   **累積統計**：總擊殺數、Boss擊殺數、遊戲次數持續累加。

### C. 技術實作
*   **StorageManager 類別**：管理 localStorage 存取邏輯。
*   **JSON 序列化**：統計資料以 JSON 格式儲存（key: `survivor_js_stats`）。
*   **錯誤處理**：localStorage 失效時靜默失敗，console.warn 提示，不影響遊戲運行。
*   **格式化輸出**：`formatTime()` 轉換秒數為「N分M秒」格式。

## 12. 已開發功能清單 (Completed Features)

### 核心遊戲功能
- [x] 基礎 Canvas 畫布與玩家移動控制（WASD / 方向鍵）。
- [x] 盔甲戰士外觀渲染（頭盔、盔甲、劍）。
- [x] 攻擊範圍限制與雙圈顯示（藍色基礎 + 綠色升級）。
- [x] 自動索敵射擊邏輯（範圍內最近敵人）。
- [x] 揮劍動畫與特效（揮動軌跡 + 劍尖閃光）。
- [x] 怪物波次生成器（難度隨時間遞增）。
- [x] 敵人類型多樣化（9種類型）。
- [x] 遠程型敵人射擊系統（紫色追蹤子彈）。
- [x] 坦克型敵人血量條顯示。
- [x] 爆炸特效（粒子爆散 + 中心閃光）。
- [x] 背景裝飾（石頭/草叢/灌木/裂痕 + 環境粒子）。
- [x] 傷害數字顯示（浮動數字 + 放大漸隱動畫）。
- [x] 經驗值拾取與升級 UI 彈窗。
- [x] 天賦系統（14種強化選項：暴击率、暴击伤害、吸血、护盾、经验加成、护甲等）。
- [x] 連殺系統與攻擊速度 buff + 經驗加成（最高+150%）。
- [x] 連殺顯示系統（DOUBLE/TRIPLE/QUAD/MEGA/ULTRA/GODLIKE 大字動畫）。
- [x] 波次系統（每60秒一波 + 5秒休息時間 + Boss戰機制）。
- [x] Boss敵人（皇冠 + 光環 + 大血量條 + 多階段狂暴模式 + 多方向子彈 +召喚精英小怪）。
- [x] 存檔功能（localStorage 储存最高紀錄）。
- [x] 音效系統（揮劍/命中/擊殺/連殺/升級/受傷/拾取/結束）。
- [x] 背景音樂（三角波 oscillator + LFO 調變）。
- [x] 暫停功能（ESC/P 鍵暫停遊戲）。
- [x] Buff 通知 UI（倒計時 + 滑入滑出動畫）。
- [x] 遊戲結束畫面與重新開始功能。

### 新增功能（詳細）
- [x] **視野遮罩系統**：玩家周圍清晰可見，視野外深色模糊（戰爭迷霧效果）。
- [x] **護盾系統**：藍色護盾 UI（HP 條上方）+ 護盾吸收傷害 + 護盾破碎特效 + 休息時間自動回復。
- [x] **技能狀態顯示**：左上方顯示已升級技能 + 右上角顯示技能冷卻（Q鍵终极技能）。
- [x] **成就系統**：19個成就（首殺/百殺/千殺/存活時間/Boss擊殺/波次/等級/遊戲次數/地狱模式）。
- [x] **排行榜 TOP 10**：本地排行榜顯示前10名最高成绩（預設關閉，點擊展開）。
- [x] **難度系統**：普通/困難/地狱模式（影響敵人生成速度、血量、傷害）。
- [x] **遊戲開始畫面**：標題、操作說明、難度選擇、開始按鈕、設定選單。
- [x] **遊戲內音量設定**：暫停畫面可調整主音量/音效/背景音量。
- [x] **Q键终极技能**：按Q键釋放全屏攻擊（傷害=玩家攻擊力×10，冷卻30秒）。
- [x] **暴击系统**：暴击子弹显示红色 + 金色光环 + 暴击傷害倍率。
- [x] **連殺經驗加成**：連殺數越高獲得額外經驗加成（最高+150%）。

### 敵人類型擴展
- [x] **精英敵人**：金色光環、藍色護盾（需先破盾才能傷害本體）、護盾破碎特效。
- [x] **分裂敵人**：死亡時分裂成2個小型敵人、觸發周圍80px內分裂敵人鏈式分裂、分裂特效。
- [x] **爆炸敵人**：死亡時對範圍內玩家造成爆炸傷害（範圍60px）。
- [x] **隱形敵人**：半透明狀態（alpha 0.3）、受擊後短暫現形閃爍（1秒）。
- [x] **敵人類型權重系統**：根據遊戲時間動態調整出現機率。

### Boss 多階段系統
- [x] **Boss 出場特效**：紅色光環擴散、警告公告、震動效果。
- [x] **Boss 放大**：radius 160 + UI 血量條标注（屏幕上方显示 BOSS 血量）。
- [x] **Boss 多階段**：血量低時進入狂暴模式（速度加快、射擊加快、召喚小怪）。
- [x] **Boss 狂暴技能**：第二階段射出4方向子彈，第三階段射出8方向子彈並召喚精英小怪。
- [x] **Boss 死亡特效**：多重粒子爆散、光環擴散、閃電效果。

### Tileset 系統
- [x] **TileManager 系統**：從素材圖集裁切 tiles，支援地板拼接與環境物件（待圖集裁切完成後使用）。
- [x] **TilesetCleaner 工具**：手動框選素材區域，生成乾淨圖集（避開設計稿文字說明）。
- [x] **TILESET_GUIDE.md**：Canvas drawImage() 裁切原理、素材定位表、與 DecorationManager 分工說明。
- [x] **TILESET_FIX_GUIDE.md**：設計稿文字說明問題的4種解決方案對比。

### 重構完成（Phase 1-3）
- [x] **Phase 1**：Update Loop 四個 Phase（清理 → 狀態 → 系統 → UI）+ GameLogger + DebugOverlay。
- [x] **Phase 2**：
    - Player 拆分（PlayerCore + PlayerCombat + PlayerRenderer）。
    - Enemy 拆分（EnemyCore + EnemyBehaviors + BossPhaseManager + EnemyRenderer）。
- [x] **Phase 3**：
    - GameValidator.js（硬斷言檢查）。
    - DebugOverlay FPS/内存/實體統計（P/E/Exp/EP/DN）。
    - 自動警告系統（FPS過低、Memory過高、Grid空、冷卻未更新、敵人過多）。
    - 日誌等級切換（Ctrl+Shift+L 循環切換 ERROR → INFO → DEBUG）。

### 效能優化
- [x] **ObjectPool 全面優化**：
    - 預分配大小調整：ProjectilePool 50→200，其他池按需調整。
    - 對象狀態標記（_active）避免 indexOf 性能损耗。
    - 自動扩容：池用完后自动扩容 50%（限制 maxSize）。
    - 統計监控：peakActiveCount、hitRate、efficiency、autoExpansions。
    - 清理优化：cleanInactive() 定期清理无效对象。
    - 自动调整：autoAdjust() 根据峰值使用量自动扩容。
    - 调试热键：Ctrl+Shift+P 查看 ObjectPool 统计。
- [x] **空間網格分割（SpatialGrid）**：格子大小 100px，優化碰撞檢測從 O(n×m) 降至 O(n×k)。
- [x] **距離平方比較**：新增 distanceSquared() 函數避免 Math.sqrt 運算。
- [x] **網格緩存**：每幀清空重建，僅檢測鄰近格子內物件。

### Bug 修復
- [x] **修復 chainKills 變數未定義錯誤**：移動變數定義至使用前（Commit: 6350085）。
- [x] **修復敵人無法被子彈命中**：每幀更新前將敵人插入 SpatialGrid（Commit: f5926c7）。
- [x] **修復主角射擊一次就停止**：在 update() 中調用 player.update() 更新 fireCooldown（Commit: 8b79a6e）。
- [x] **HP 條同步問題**：护盾吸收傷害時 UI 即時更新，避免玩家突然死亡無預警。

## 13. TileManager 系統規格（Unity版本）

### A. 系統概述
TileManager 用於管理 Unity Tilemap 系統，支援地板拼接與環境物件渲染。與 DecorationManager 分工：TileManager 畫底層地板（Tilemap），DecorationManager 畫頂層動態裝飾（Particle System）。

### B. Unity Tilemap 系統

#### 1. Tilemap 組件結構
```
Tilemap GameObject
├── Grid                          # Grid 組件（管理所有 Tilemap）
│   ├── Cell Size: (1, 1)        # 格子大小（Unity Units）
│   └── Cell Layout: Rectangle   # 格子布局
├── Ground Tilemap                # 地板 Tilemap
│   ├── Tilemap Renderer          # 渲染組件
│   │   ├── Mode: Chunk          # 批次渲染模式（優化性能）
│   │   ├── Sort Order: Bottom    # 渲染順序（底層）
│   │   └── Material: Default Tilemap Material
│   ├── Tilemap Collider 2D       # 碰撞检测（可選，障礙物）
│   └   └   └ Is Trigger: false  # 固體障礙物（水、石頭）
│   └── Tilemap                   # Tilemap 組件（儲存 Tiles）
├── Decoration Tilemap            # 裝飾 Tilemap（樹木、石頭）
│   ├── Tilemap Renderer          # 渲染組件
│   │   ├── Mode: Chunk
│   │   ├── Sort Order: Top       # 渲染順序（頂層）
│   └ Tilemap                     # Tilemap 組件
```

#### 2. Tile Asset 系統

**Tile Asset 配置：**
```
Assets/Tiles/
├── GroundTiles.asset             # 地板 Tiles（ScriptableObject）
│   ├── grass Tile                # 草地 Tile
│   ├── grass2 Tile               # 草地變體
│   ├── dirt Tile                 # 泥土 Tile
│   ├── stone Tile                # 石頭 Tile（障礙物）
│   └ water Tile                  # 水面 Tile（障礙物）
├── DecorationTiles.asset         # 裝飾 Tiles
│   ├── tree Tile                 # 樹木 Tile
│   ├── bush Tile                 # 灌木 Tile
│   ├── flower Tile               # 花朵 Tile
│   └ rock Tile                   # 石頭 Tile（障礙物）
└── RuleTiles.asset               # Rule Tile（自動拼接）
    ├── grass Rule Tile           # 草地自动拼接（邊緣處理）
    └ dirt Rule Tile              # 泥土自动拼接
```

#### 3. Sprite Atlas配置

**GroundAtlas.spriteatlas：**
```
Assets/SpriteAtlases/
├── GroundAtlas.spriteatlas       # 地板 Sprite Atlas
│   ├── Objects for Packing:
│   │   ├── Sprites/Ground/       # 地板 Sprites
│   │   ├── grass.png             # 草地 Sprite（32x32）
│   │   ├── grass2.png
│   │   ├── dirt.png
│   │   ├── stone.png
│   │   ├── water.png
│   ├── Packing Settings:
│   │   ├── Block Size: 1024x1024
│   │   ├── Padding: 2px
│   └── Settings for Platform:
│   │   ├── Default: Enable
│   │   ├── Format: RGBA Compressed
├── DecorationAtlas.spriteatlas   # 裝飾 Sprite Atlas
│   ├── Objects for Packing:
│   │   ├── Sprites/Decoration/   # 裝飾 Sprites
│   │   ├── tree.png              # 樹木 Sprite（64x96）
│   │   ├── bush.png
│   │   ├── flower.png
│   │   ├── rock.png
```

### C. Tilemap C# 使用範例

#### 1. TileManager.cs（C# 版本）
```csharp
using UnityEngine.Tilemaps;

public class TileManager : MonoBehaviour
{
    [SerializeField] private Tilemap groundTilemap;
    [SerializeField] private Tilemap decorationTilemap;
    [SerializeField] private TileBase grassTile;
    [SerializeField] private TileBase dirtTile;
    [SerializeField] private TileBase treeTile;
    
    public void GenerateMap(int width, int height)
    {
        // 隨機生成地板
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                TileBase tile = Random.value > 0.3f ? grassTile : dirtTile;
                groundTilemap.SetTile(new Vector3Int(x, y, 0), tile);
            }
        }
        
        // 隨機添加裝飾
        AddDecorations(width, height, 100);
    }
    
    private void AddDecorations(int width, int height, int count)
    {
        for (int i = 0; i < count; i++)
        {
            int x = Random.Range(0, width);
            int y = Random.Range(0, height);
            decorationTilemap.SetTile(new Vector3Int(x, y, 0), treeTile);
        }
    }
}
```

#### 2. Rule Tile 自動拼接（Unity 内建）

**Rule Tile 配置：**
- 使用 Unity Rule Tile 系統自動處理地板邊緣拼接
- 例如：草地遇到泥土地自動生成邊緣 Tile
- 不需要手動處理每個格子的拼接邏緣

### D. TileManager 與 DecorationManager 分工

| 功能 | TileManager（Unity Tilemap） | DecorationManager（Unity Particle） |
|------|-------------|-------------------|
| 地板拼接 | ✅ Tilemap 系統 | ❌ |
| 靜態物件 | ✅ Tilemap（樹木、石頭） | ❌ |
| 動態裝飾 | ❌ | ✅ Particle System（搖擺的草、發光水晶） |
| 粒子效果 | ❌ | ✅ Particle System（螢火虫、落叶） |
| 地圖生成 | ✅ C# Tilemap.SetTile() | ❌ |
| 碰撞检测 | ✅ Tilemap Collider 2D | ❌ |

### E. Unity Tilemap優化

#### 1. Tilemap Renderer Mode
- **Chunk Mode**：批次渲染，减少 Draw Calls（推薦）
- **Individual Mode**：每個 Tile 独立渲染（不推薦，性能差）

#### 2. Tilemap Collider 2D
- **静态障礙物**：水、石頭使用 Tilemap Collider 2D
- **动态碰撞**：玩家、敵人使用 Rigidbody 2D + Circle Collider 2D

#### 3. 地圖捲動（相機跟隨）
- 使用 Cinemachine 2D相機跟隨玩家
- 或手動控制 Camera.transform.position跟隨玩家

### F. 下一步優化
1. **Rule Tile 自動拼接**：使用 Unity Rule Tile 系統自動處理地板邊緣
2. **碰撞檢測**：部分 Tile（水、石頭）使用 Tilemap Collider 2D
3. **多層渲染**：Grid → Ground Tilemap → Decoration Tilemap → Player → Enemies → Effects
4. **動態載入**：大地图使用 Unity Addressables 系統動態載入

## 14. TilesetCleaner 工具規格

### A. 工具用途
設計稿通常包含 **素材 + 設計說明文字 + 尺寸標註**，文字說明會導致自動裁切錯亂。TilesetCleaner 可手動框選純素材區域，生成乾淨的 tileset.png。

### B. 啟動方式
```bash
npm run dev
# 瀏覽器打開 http://localhost:3000/tilesetCleaner.html
```

### C. 操作流程
1. **調整素材尺寸**（預設 32px，可改為 16/64/128）
2. **選擇尺寸處理模式**：
   - 🔒 **保留原始尺寸**（推薦）：保持每個素材的原始大小
   - 📐 **縮放為統一尺寸**：強制縮放為 32x32
3. **點擊「➕ 新增素材位置」**
4. **在設計稿上框選純素材區域**（避開文字說明）
5. **手動調整 X Y W H 輸入框**（精確控制裁切範圍）
6. **框選錯誤時**：
   - 點擊已框選區域（紅色高亮）
   - 點擊「❌ 删除选中」或「↩️ 撤销」
7. **選擇布局模式**：
   - 🎯 **最優布局**（推薦）：自動計算最小空白格數
   - ⬜ **強制正方形**：適合需要正方形圖集的場合
   - ⚙️ **自定義尺寸**：手動設定列數/行數
8. **點擊「👀 预览图集」**
9. **檢查預覽是否正確**
10. **確認無誤後點擊「⬇️ 確認下載」**

### D. 功能特色

| 功能 | 說明 |
|------|------|
| **手動框選** | 避開設計稿文字說明，只選取純素材 |
| **手動調整** | X Y W H 四個輸入框，精確控制裁切範圍 |
| **浮點誤差修正** | 自動計算圖片缩放比例，避免坐標偏移 |
| **預覽功能** | 先預覽生成的圖集，確認無誤後再下載 |
| **多種布局** | 最優布局、強制正方形、水平/垂直排列、自定義尺寸 |
| **格子間距** | 可關閉生成完美正方形，或啟用 2px 間距 |
| **尺寸處理** | 保留原始尺寸或縮放為統一尺寸 |

### E. 設計稿文字說明問題的解決方案

| 方案 | 適用對象 | 優點 | 缺點 |
|------|---------|------|------|
| **方案1：手動框選工具** | 進階使用者 | 可視化操作、自動生成乾淨圖集 | 需要操作工具 |
| **方案2：圖片編輯軟體** | 新手 | 最簡單、最可靠 | 需要圖片編輯技能 |
| **方案3：修改定義** | 快速修復 | 不需修改原始圖片 | 需手動測量每個素材位置 |
| **方案4：自動化腳本** | 專業使用者 | 全自動處理、批量處理 | 需圖像處理知識 |

**推薦方案**：
- 新手 → 方案 2（圖片編輯軟體）
- 進階 → 方案 1（手動框選工具）
- 專業 → 方案 4（自動化腳本）

## 15. 專業切圖工具推薦

### A. 工具對比表

| 工具 | 用途 | 優點 | 缺點 | 適用場景 |
|------|------|------|------|---------|
| **TexturePacker** | Sprite Sheet/Tileset 自動排列 | 專為遊戲設計、自動優化空白、多格式导出、去除透明邊框 | 付费版才有高级功能 | 專業遊戲開發、批量處理 |
| **Tiled Map Editor** | Tilemap 地圖編輯 | 免費開源、專業地圖編輯、多層支持、多格式导出 | 不會自動生成 tileset | 已有 tileset、需設計地圖 |
| **Aseprite** | 像素藝術 + Sprite Sheet | 專為像素風格、動畫編輯、便宜（$20） | 主要用於像素風格 | 像素風格遊戲、動畫編輯 |
| **SpriteForge** | 線上 Sprite Sheet 工具 | 免費線上、簡單易用、拖放操作 | 功能較簡單 | 快速處理、不想安裝軟體 |

### B. 推薦選擇

| 求 | 推薦工具 |
|------|---------|
| **專業遊戲開發** | TexturePacker（自動優化、多格式导出） |
| **已有 tileset，需設計地圖** | Tiled Map Editor（免費、專業地圖編輯） |
| **像素風格遊戲** | Aseprite（像素編輯、動畫、 sprite sheet） |
| **快速簡單處理** | SpriteForge（線上免費） |
| **手動裁切設計稿** | TilesetCleaner（避開文字說明） |

### C. 最佳實踐：組合使用流程
```
設計稿（含文字說明）
↓
TilesetCleaner（手動框選、生成乾淨圖集）
↓
TexturePacker（自動優化排列、去除透明邊框）
↓
Tiled Map Editor（設計地圖、設定碰撞）
↓
遊戲引擎渲染
```

### D. TexturePacker 設定建議
```
Algorithm: MaxRects（最小空白）
Padding: 2px（避免素材相連）
Trim: Enable（去除透明邊框）
Extrude: 1px（避免渲染缝隙）
Format: JSON-Array（通用格式）
```

### E. 官網連結
- **TexturePacker**: https://www.codeandweb.com/texturepacker
- **Tiled Map Editor**: https://www.mapeditor.org/
- **Aseprite**: https://www.aseprite.org/
- **SpriteForge**: https://spriteforge.com/

## 16. 視野遮罩系統

### A. 系統概述
玩家周圍清晰可見，視野外深色模糊，營造戰爭迷霧效果。

### B. 實作方式
- **遮罩範圍**：玩家位置為中心，半徑可調整
- **渲染順序**：所有遊戲物件 → 視野遮罩（最後繪製）
- **模糊效果**：視野外區域降低透明度 + 深色覆蓋

### C. 技術實作
```javascript
// visibilityMask.js
class VisibilityMask {
    constructor(canvas) {
        this.canvas = canvas;
        this.radius = 200;  // 可視半徑
    }
    
    draw(ctx, playerX, playerY) {
        ctx.save();
        
        // 繪製遮罩（視野外深色）
        ctx.fillStyle = 'rgba(0, 0, 0, 0.6)';
        ctx.fillRect(0, 0, this.canvas.width, this.canvas.height);
        
        // 清除玩家周圍區域（可視範圍）
        ctx.globalCompositeOperation = 'destination-out';
        ctx.beginPath();
        ctx.arc(playerX, playerY, this.radius, 0, Math.PI * 2);
        ctx.fill();
        
        ctx.restore();
    }
}
```

## 17. 成就系統

### A. 成就列表（19個）

| 成就名稱 | 觸發條件 | 描述 |
|---------|---------|------|
| 首殺 | 擊殺 1隻敵人 | 第一次擊殺敵人 |
| 百殺 | 擊殺 100隻敵人 | 擊殺100隻敵人 |
| 千殺 | 擊殺 1000隻敵人 | 擊殺1000隻敵人 |
| 存活時間 5分 | 存活 5分鐘 | 存活5分鐘 |
| 存活時間 10分 | 存活 10分鐘 | 存活10分鐘 |
| 存活時間 20分 | 存活 20分鐘 | 存活20分鐘 |
| Boss擊殺 1 | 擊殺 1隻Boss | 第一次擊殺Boss |
| Boss擊殺 5 | 擊殺 5隻Boss | 擊殺5隻Boss |
| Boss擊殺 10 | 擊殺 10隻Boss | 擊殺10隻Boss |
| 波次 5 | 完成第5波 | 完成第5波 |
| 波次 10 | 完成第10波 | 完成第10波 |
| 波次 20 | 完成第20波 | 完成第20波 |
| 等級 5 |達到等級5 |達到等級5 |
| 等級 10 |達到等級10 |達到等級10 |
| 等級 20 |達到等級20 |達到等級20 |
| 游戲次数 10 | 游玩 10次 | 游玩10次 |
| 游戲次数 50 | 游玩 50次 | 游玩50次 |
| 地狱模式存活 5分 | 地狱模式存活 5分鐘 |地狱模式存活5分鐘 |
| 地狱模式波次 10 | 地狱模式完成第10波 |地狱模式完成第10波 |

### B. 成就顯示
- **位置**：遊戲結束畫面
- **新成就標記**：金色標籤 + ⭐ 符號
- **通知動畫**：達成成就時彈出通知（2秒後消失）

### C. 技術實作
```javascript
// achievement.js
class Achievement {
    constructor(storage) {
        this.storage = storage;
        this.achievements = this.initAchievements();
    }
    
    check(type, value) {
        const achievement = this.achievements.find(a => 
            a.type === type && value >= a.threshold
        );
        if (achievement && !achievement.unlocked) {
            achievement.unlocked = true;
            this.showNotification(achievement);
            this.storage.saveAchievements(this.achievements);
        }
    }
}
```

## 18. 排行榜系統

### A. 排行榜功能
- **TOP 10 排行榜**：顯示前10名最高成绩
- **預設關閉**：点击按钮展開
- **排序依據**：存活時間 + 等級 + 擊殺數

### B. 排行榜顯示
| 排名 | 等級 | 存活時間 | 擊殺數 | Boss擊殺 |
|------|------|---------|-------|---------|
| 1 | Lv.20 | 20分30秒 | 500 | 5 |
| 2 | Lv.15 | 15分20秒 | 300 | 3 |
| ... | ... | ... | ... | ... |

### C. 技術實作
```javascript
// storage.js
class StorageManager {
    getLeaderboard() {
        const records = this.getAllRecords();
        return records
            .sort((a, b) => b.time - a.time || b.level - a.level)
            .slice(0, 10);
    }
}
```

## 19. 階段性難度系統

### A. 难度模式

| 难度 | 敵人生成速度 | 敵人血量倍率 | 敵人傷害倍率 | 描述 |
|------|-------------|-------------|-------------|------|
| **普通** | 1.0x | 1.0x | 1.0x | 預設難度 |
| **困難** | 1.5x | 1.5x | 1.5x | 敵人更快更強 |
| **地狱** | 2.0x | 2.0x | 2.0x | 极限挑戰 |

### B. 难度選擇
- **位置**：遊戲開始畫面
- **按鈕**：三個難度選項（普通/困難/地狱）
- **標籤**：地狱模式標記 🔥 符號

### C. 技術實作
```javascript
// game.js
class Game {
    constructor(canvas, difficulty = 'normal') {
        this.difficulty = difficulty;
        this.setDifficultyParams();
    }
    
    setDifficultyParams() {
        switch (this.difficulty) {
            case 'hard':
                this.spawnRateMultiplier = 1.5;
                this.hpMultiplier = 1.5;
                this.damageMultiplier = 1.5;
                break;
            case 'hell':
                this.spawnRateMultiplier = 2.0;
                this.hpMultiplier = 2.0;
                this.damageMultiplier = 2.0;
                break;
            default:
                this.spawnRateMultiplier = 1.0;
                this.hpMultiplier = 1.0;
                this.damageMultiplier = 1.0;
        }
    }
}
```

## 20. 敵人類型擴展

### A. 精英敵人 (Elite)
*   **外觀**：金色光環 + 藍色護盾（需先破盾才能傷害本體）。
*   **屬性**：血量 5、速度 60px/秒、傷害 15、經驗值 50。
*   **護盾**：藍色護盾（HP 20），護盾破碎特效（藍色碎片爆散）。
*   **出現時間**：90秒後開始出現。

### B. 分裂敵人 (Split)
*   **外觀**：綠色圓形怪物 + 分裂標記。
*   **屬性**：血量 2、速度 70px/秒、傷害 10、經驗值 20。
*   **特殊**：死亡時分裂成2個小型敵人（半徑 10px），觸發周圍80px內分裂敵人鏈式分裂。
*   **分裂特效**：綠色光環擴散 + 粒子爆散。
*   **出現時間**：120秒後開始出現。

### C. 爆炸敵人 (Explosive)
*   **外觀**：橙色圓形怪物 + 爆炸標記。
*   **屬性**：血量 1、速度 50px/秒、傷害 5（爆炸傷害 30）、經驗值 15。
*   **特殊**：死亡時對範圍內玩家造成爆炸傷害（範圍 60px）。
*   **出現時間**：150秒後開始出現。

### D.隱形敵人 (Invisible)
*   **外觀**：半透明灰色怪物，受擊後短暫現形閃爍。
*   **屬性**：血量 2、速度 80px/秒、傷害 12、經驗值 25。
*   **特殊**：初始半透明（alpha 0.3），受擊後現形（alpha 1.0）1秒。
*   **出現時間**：180秒後開始出現。

### E. 敵人類型權重系統
根據遊戲時間動態調整各類型出現機率（詳見 waveManager.js）。

## 21. 护盾系統規格

### A. 系統概述
玩家擁有藍色護盾，可吸收傷害。護盾歸零後才能傷害本體 HP。

### B. 护盾属性
- **初始护盾**：0（需透過天賦「护盾强化」獲得）
- **护盾上限**：50（可透過天賦提升）
- **护盾回復**：休息時間自動回復至满值

### C. 护盾 UI
- **位置**：HP 條上方
- **顏色**：藍色半透明
- **顯示**：「护盾: 20/50」
- **护盾歸零**：半透明顯示（提示護盾已失效）

### D. 护盾變化
- **护盾吸收傷害**：UI 即時更新
- **护盾恢復**：休息時間自動回復，UI 即時更新

### E. 技術實作
```javascript
// player.js
class Player {
    constructor(x, y) {
        this.shield = 0;
        this.maxShield = 50;
    }
    
    takeDamage(damage) {
        if (this.shield > 0) {
            const shieldDamage = Math.min(damage, this.shield);
            this.shield -= shieldDamage;
            damage -= shieldDamage;
        }
        this.hp -= damage;
    }
    
    recoverShield() {
        if (this.shield < this.maxShield) {
            this.shield = this.maxShield;
        }
    }
}
```

## 22. 技能狀態顯示系統

### A. 技能狀態 UI
- **位置**：畫面左上方
- **顯示內容**：已升級技能（未升級不顯示）
- **格式**：圖示 + 技能名稱 + 數值

### B. 技能冷卻 UI
- **位置**：右上角
- **顯示內容**：Q键终极技能狀態
- **格式**：「Q技能: 就緒」或「Q技能: 冷卻 15秒」
- **冷卻時間**：30秒

### C. 技術實作
```javascript
// ui.js
class UI {
    drawSkills(ctx, player) {
        const skills = player.getUpgradedSkills();
        ctx.font = '16px Arial';
        ctx.fillStyle = '#fff';
        ctx.fillText('已升級技能:', 10, 30);
        
        skills.forEach((skill, i) => {
            ctx.fillText(`${skill.icon} ${skill.name}: ${skill.value}`, 10, 50 + i * 20);
        });
    }
    
    drawSkillCooldown(ctx, player) {
        const cooldown = player.skillCooldown;
        const status = cooldown > 0 ? `冷卻 ${cooldown.toFixed(1)}秒` : '就緒';
        ctx.fillText(`Q技能: ${status}`, this.canvas.width - 120, 30);
    }
}
```

## 23. 重構後的物件架構

### A. Player 拆分（組合模式）

#### 1. PlayerCore.js
**職責**：位置、移動、碰撞
- **屬性**：x, y, radius, speed, hp, shield
- **方法**：update(), move(), takeDamage()

#### 2. PlayerCombat.js
**職責**：射擊、技能、傷害計算
- **屬性**：fireRate, fireCooldown, damage, critChance, skillCooldown
- **方法**：canFire(), shoot(), useSkill()

#### 3. PlayerRenderer.js
**職責**：繪製盔甲戰士
- **方法**：draw(), drawHelmet(), drawBody(), drawLegs(), drawArms(), drawSword()

#### 4. Player.js（組合類別）
```javascript
class Player {
    constructor(x, y) {
        this.core = new PlayerCore(x, y);
        this.combat = new PlayerCombat(this.core);
        this.renderer = new PlayerRenderer();
    }
    
    // 使用 Getter/Setter 暴露所有原有屬性（保持接口兼容）
    get x() { return this.core.x; }
    get y() { return this.core.y; }
    get hp() { return this.core.hp; }
    get fireCooldown() { return this.combat.fireCooldown; }
    
    update(dt, keys, canvasWidth, canvasHeight) {
        this.core.update(dt, keys, canvasWidth, canvasHeight);
        this.combat.update(dt);
    }
    
    draw(ctx) {
        this.renderer.draw(ctx, this.core, this.combat);
    }
}
```

### B. Enemy 拆分（組合模式）

#### 1. EnemyCore.js
**職責**：位置、移動、碰撞、傷害計算
- **屬性**：x, y, radius, speed, hp, damage, type
- **方法**：update(), move(), takeDamage()

#### 2. EnemyBehaviors.js
**職責**：射擊、分裂、隱形行為
- **屬性**：shootCooldown, canShoot, canSplit, isInvisible
- **方法**：update(), shoot(), split(), reveal()

#### 3. BossPhaseManager.js
**職責**：Boss 多階段管理（狂暴模式、召喚小怪）
- **屬性**：phase, phaseThresholds, isEnraged
- **方法**：update(), enterNextPhase(), summonMinions()

#### 4. EnemyRenderer.js
**職責**：繪製敵人外觀（240行繪製邏輯，支援 9種敵人類型）
- **方法**：draw(), drawNormal(), drawFast(), drawTank(), drawRanged(), drawBoss()

#### 5. Enemy.js（組合類別）
```javascript
class Enemy {
    constructor(type, x, y) {
        this.core = new EnemyCore(type, x, y);
        this.behaviors = new EnemyBehaviors(type, this.core);
        this.renderer = new EnemyRenderer(type);
        
        if (type.isBoss) {
            this.phaseManager = new BossPhaseManager(this.core);
        }
    }
    
    update(dt, playerX, playerY) {
        this.core.update(dt, playerX, playerY);
        
        if (this.phaseManager) {
            this.phaseManager.update(dt, this.core.hp);
        }
        
        return this.behaviors.update(dt, playerX, playerY);
    }
    
    draw(ctx) {
        this.renderer.draw(ctx, this.core, this.behaviors);
    }
}
```

## 24. 调试機制完善

### A. DebugOverlay 功能

| 功能 | 熱鍵 | 描述 |
|------|------|------|
| **開啟調試** | Ctrl+D |顯示 Grid、fireCooldown、警告 |
| **FPS顯示** | Ctrl+D |顯示 FPS、Memory、實體數量 |
| **硬斷言** | Ctrl+Shift+V |啟用 GameValidator 檢查 |

#### DebugOverlay顯示內容
- **Grid 狀態**：格子數、實體數、⚠ Grid空警告
- **Player 狀態**：fireCooldown、canFire、⚠ 冷卻未更新警告
- **FPS/Memory**：FPS數值、Memory使用量、⚠ FPS過低警告
- **實體統計**：P（玩家）、E（敵人）、Exp（經驗球）、EP（投射物）、DN（傷害數字）

### B. GameLogger 日誌等級切換

| 等級 | 熱鍵 | 描述 |
|------|------|------|
| **ERROR** | Ctrl+Shift+L | 只顯示錯誤 |
| **INFO** | Ctrl+Shift+L |顯示重要資訊 + 錯誤 |
| **DEBUG** | Ctrl+Shift+L |顯示所有日誌（包含每個Phase） |

#### 日誌內容
- **Phase 日誌**：每個 Phase 的執行狀態
- **關鍵變數**：Grid 實體數、fireCooldown、projectiles數量
- **錯誤警告**：Grid空、冷卻未更新、敵人過多

### C. GameValidator 硬斷言

| 檢查項目 | 描述 | 錯誤訊息 |
|---------|------|---------|
| **validatePhase1** | Grid 實體數 = 敵人數 | Phase 1失敗：Grid實體數 ≠ 敵人數 |
| **validatePhase2** | fireCooldown 已減少 | Phase 2失敗：fireCooldown未減少 |
| **validatePhase3** | getNearby返回非空陣列 | Phase 3失敗：碰撞檢測失效 |

### D. ObjectPool 統計

| 功能 | 熱鍵 | 描述 |
|------|------|------|
| **統計顯示** | Ctrl+Shift+P |顯示 ObjectPool 使用率、峰值、命中率 |

#### ObjectPool 統計內容
- **peakActiveCount**：峰值使用量
- **hitRate**：命中率（reuse /total requests）
- **efficiency**：效率（active / poolSize）
- **autoExpansions**：自動扩容次數

## 25. Unity 檔案結構

### A. 專案根目錄
```
survivor.unity/
├── Assets/                      # Unity 資源目錄
│   ├── Scripts/                 # C# 脚本
│   ├── Sprites/                 # Pixel Art 美術資產
│   ├── Prefabs/                 # Prefab 資源
│   ├── ScriptableObjects/       # ScriptableObject 資源
│   ├── Audio/                   # 音效資源
│   ├── Scenes/                  # Unity 場景
│   ├── Materials/               # 材質資源（URP 2D Materials）
│   ├── ParticleSystems/         # 粒子特效 Prefab
│   ├── Fonts/                   # 字體資源
│   ├── UI/                      # UI 資源（Canvas、Sprite）
│   └── Resources/               # Resources.Load 資源（可選）
├── Packages/                    # Unity Package 配置
├── ProjectSettings/             # Unity 專案設定
├── PRD.md                       # 產品需求文件（本文件）
├── README.md                    # 專案說明
├── CHANGELOG.md                 # 更新日誌
└── .agent_task_state.md         # 任務狀態快照
```

### B. Scripts 脚本結構（對應 JavaScript 版本）
```
Assets/Scripts/
├── Core/                        # 核心遊戲邏輯
│   ├── GameManager.cs           # 遊戲主邏輯（對應 game.js）
│   ├── PlayerController.cs      # 玩家控制器（對應 player.js）
│   ├── EnemyController.cs       # 敵人控制器（對應 enemy.js）
│   ├── ProjectileController.cs  # 魔法彈控制器（對應 projectile.js）
│   ├── ExperienceOrb.cs         # 驗球（對應 experience.js）
│   └── WaveManager.cs           # 波次管理（對應 waveManager.js）
├── Components/                  # Unity 組件（拆分邏輯）
│   ├── PlayerMovement.cs        # 玩家移動（對應 playerCore.js）
│   ├── PlayerCombat.cs          # 玩家戰鬥（對應 playerCombat.js）
│   ├── PlayerRenderer.cs        # 玩家渲染（對應 playerRenderer.js）
│   ├── EnemyMovement.cs         # 敵人移動（對應 enemyCore.js）
│   ├── EnemyBehavior.cs         # 敵人行为（對應 enemyBehaviors.js）
│   ├── EnemyRenderer.cs         # 敵人渲染（對應 enemyRenderer.js）
│   └── BossPhaseManager.cs      # Boss 阶段管理（對應 bossPhaseManager.js）
├── Effects/                     # 特效系統
│   ├── ExplosionEffect.cs       # 爆炸特效（對應 explosion.js）
│   ├── DamageNumber.cs          # 傷害數字（對應 damageNumber.js）
│   ├── ChainKillDisplay.cs      # 連殺顯示（對應 chainKillDisplay.js）
│   ├── BossSpawnEffect.cs       # Boss 出場特效（對應 bossSpawnEffect.js）
│   ├── BossDeathEffect.cs       # Boss 死亡特效（對應 bossDeathEffect.js）
│   ├── ShieldBreakEffect.cs     # 護盾破碎特效（對應 shieldBreakEffect.js）
│   └── SplitEffect.cs           # 分裂特效（對應 splitEffect.js）
├── Systems/                     # 系統管理
│   ├── SpatialGrid.cs           # 空間網格分割（對應 spatialGrid.js）
│   ├── ObjectPool.cs            # 物件池化（對應 objectPool.js）
│   ├── AudioManager.cs          # 音效管理（對應 audio.js）
│   ├── DecorationManager.cs     # 背景裝飾（對應 decoration.js）
│   ├── StorageManager.cs        # 存檔管理（對應 storage.js）
│   ├── TalentManager.cs         # 天賦系統（對應 talent.js）
│   ├── AchievementManager.cs    # 成就系統（對應 achievement.js）
│   ├── TileManager.cs           # Tileset 管理（對應 tileManager.js）
│   └── VisibilityMask.cs        # 視野遮罩（對應 visibilityMask.js）
├── UI/                          # UI 系統
│   ├── UIManager.cs             # UI 管理（對應 ui.js）
│   ├── HUD.cs                   # 狀態列 UI（血量、經驗、等級）
│   ├── UpgradePanel.cs          # 升級彈窗
│   ├── GameOverPanel.cs         # 遊戲結束畫面
│   ├── PausePanel.cs            # 暫停畫面
│   ├── StartPanel.cs            # 遊戲開始畫面
│   ├── BuffNotification.cs      # Buff 通知 UI
│   ├── SkillStatusUI.cs         # 技能狀態顯示
│   ├── LeaderboardUI.cs         # 排行榜 UI
│   └── AchievementUI.cs         # 成就 UI
├── Debug/                       # 調試工具
│   ├── DebugOverlay.cs          # 可視化調試工具（對應 debugOverlay.js）
│   ├── GameLogger.cs            # Console 日誌（對應 gameLogger.js）
│   └── GameValidator.cs         # 硬斷言檢查（對應 gameValidator.js）
├── Data/                        # 資料結構
│   ├── EnemyData.cs             # 敵人資料（ScriptableObject）
│   ├── TalentData.cs            # 天賦資料（ScriptableObject）
│   ├── WaveData.cs              # 波次資料（ScriptableObject）
│   ├── AchievementData.cs       # 成就資料（ScriptableObject）
│   └── GameStats.cs             # 遊戲統計（存檔資料）
└── Utils/                       # 工具函數
    ├── MathUtils.cs             # 數學工具（distanceSquared）
    ├── ExtensionMethods.cs      # C# Extension Methods
    └── Constants.cs             # 常量定義
```

### C. Prefabs 资源結構
```
Assets/Prefabs/
├── Player.prefab                # 玱家 Prefab
├── Enemies/                     # 敵人 Prefabs
│   ├── NormalEnemy.prefab       # 普通型
│   ├── FastEnemy.prefab         # 快速型
│   ├── TankEnemy.prefab         # 坦克型
│   ├── RangedEnemy.prefab       # 遠程型
│   ├── EliteEnemy.prefab        # 精英敵人
│   ├── SplitEnemy.prefab        # 分裂敵人
│   ├── ExplosiveEnemy.prefab    # 爆炸敵人
│   ├── InvisibleEnemy.prefab    #隱形敵人
│   └── BossEnemy.prefab         # Boss
├── Projectile.prefab            # 魔法彈 Prefab
├── ExperienceOrb.prefab         # 驗球 Prefab
├── Effects/                     # 特效 Prefabs
│   ├── ExplosionEffect.prefab   # 爆炸特效
│   ├── SwingTrail.prefab        # 揮劍軌跡
│   ├── BossSpawnEffect.prefab   # Boss 出場特效
│   ├── BossDeathEffect.prefab   # Boss 死亡特效
│   ├── ShieldBreakEffect.prefab # 護盾破碎特效
│   └── SplitEffect.prefab       # 分裂特效
└── UI/                          # UI Prefabs
    ├── HUD.prefab               # 狀態列
    ├── UpgradePanel.prefab      # 升級彈窗
    ├── GameOverPanel.prefab     # 遊戲結束畫面
    └── DamageNumber.prefab      # 傷害數字
```

### D. ScriptableObjects 资源結構
```
Assets/ScriptableObjects/
├── Enemies/                     # 敵人資料配置
│   ├── NormalEnemyData.asset
│   ├── FastEnemyData.asset
│   ├── TankEnemyData.asset
│   ├── RangedEnemyData.asset
│   ├── EliteEnemyData.asset
│   ├── SplitEnemyData.asset
│   ├── ExplosiveEnemyData.asset
│   ├── InvisibleEnemyData.asset
│   └── BossEnemyData.asset
├── Talents/                     # 天賦配置
│   ├── HealthBoost.asset        # 生命強化
│   ├── SpeedBoost.asset         # 疾風步
│   ├── PickupRangeBoost.asset   # 磁力手套
│   ├── AttackRangeBoost.asset   #鹰眼
│   ├── FireRateBoost.asset      # 急速射擊
│   ├── DamageBoost.asset        # 魔力增幅
│   ├── ProjectileSpeedBoost.asset # 子彈加速
│   ├── MultiShot.asset          # 多重射擊
│   ├── CritChance.asset         # 暴击率
│   ├── CritDamage.asset         # 暴击傷害
│   ├── Vampire.asset            # 吸血
│   ├── Shield.asset             # 护盾
│   ├── ExpBonus.asset           # 经驗加成
│   └── Armor.asset              # 护甲
├── Waves/                       # 波次配置
│   ├── Wave1.asset              # 第1波
│   ├── Wave5.asset              # 第5波（Boss 波）
│   ├── Wave10.asset             # 第10波
│   └── WaveDataTemplate.asset   # 波次模板
├── Achievements/                # 成就配置
│   ├── FirstKill.asset          # 首殺
│   ├── HundredKills.asset       # 百殺
│   ├── ThousandKills.asset      # 千殺
│   └── ...（19個成就）
└── Audio/                       # 音效配置
    ├── SFXSettings.asset        # 音效設定（主音量、音效音量）
    └── BGMSettings.asset        # 背景音樂設定
```

## 26. Unity Asset 系統

### A. Prefab 系統

#### 1. Prefab 用途
Prefab 是 Unity 的預製物件模板，可重用並保持一致性。

**用途對照表：**

| JavaScript 版本 | Unity 版本 | 說明 |
|----------------|-----------|------|
| new Player(x, y) | Player.prefab Instantiate | 玱家物件 |
| new Enemy(type, x, y) | Enemy Prefabs Instantiate | 敵人物件（9種 Prefab） |
| new Projectile(x, y) | Projectile.prefab Instantiate | 魔法彈（物件池化） |
| new Explosion(x, y) | ExplosionEffect.prefab Instantiate | 爆炸特效（物件池化） |

#### 2. Prefab 設計原則

- **單一職責**：每個 Prefab 只做一件事
- **組件化**：使用多個 MonoBehaviour組件拆分邏輯
- **可配置**：使用 ScriptableObject配置屬性（敵人、天賦、波次）
- **物件池化**：Projectile、Explosion、ExperienceOrb 使用 ObjectPool

#### 3. Prefab 組件結構範例

**Player.prefab 組件：**
```
Player GameObject
├── Sprite Renderer              # 顯示盔甲戰士 Sprite
├── Rigidbody 2D                 # 2D 物理（Dynamic）
├── Circle Collider 2D           # 碰撞检测
├── PlayerController             # 主控制器（MonoBehaviour）
├── PlayerMovement               # 移動邏輯（MonoBehaviour）
├── PlayerCombat                 # 戰鬥邏輯（MonoBehaviour）
├── PlayerRenderer               # 渲染邏輯（MonoBehaviour）
├── Audio Source                 # 音效（揮劍、受傷）
└── Trail Renderer               # 揮劍軌跡（可選）
```

**Enemy.prefab 組件範例：**
```
Enemy GameObject
├── Sprite Renderer              # 顯示敵人 Sprite
├── Rigidbody 2D                 # 2D 物理（Dynamic）
├── Circle Collider 2D           # 碰撞检测
├── EnemyController              # 主控制器（MonoBehaviour）
├── EnemyMovement                # 移動邏輯（MonoBehaviour）
├── EnemyBehavior                # 行为邏輯（MonoBehaviour）
├── EnemyRenderer                # 渲染邏輯（MonoBehaviour）
├── Audio Source                 # 音效（死亡）
└── Boss Phase Manager           # Boss專用組件（條件添加）
```

### B. ScriptableObject 系統

#### 1. ScriptableObject 用途
ScriptableObject 是 Unity 的資料容器，用於配置遊戲數值。

**用途對照表：**

| JavaScript 版本 | Unity 版本 | 說明 |
|----------------|-----------|------|
| Enemy 属性物件 | EnemyData.asset | 敵人屬性配置（血量、速度、傷害） |
| Talent 属性物件 | TalentData.asset | 天賦配置（效果、圖示） |
| Wave 配置物件 | WaveData.asset | 波次配置（敵人數、生成間隔） |
| Achievement 配置 | AchievementData.asset | 成就配置（條件、描述） |

#### 2. ScriptableObject 範例

**EnemyData.cs（C#類別）：**
```csharp
[CreateAssetMenu(fileName = "EnemyData", menuName = "Survivor/Enemy Data")]
public class EnemyData : ScriptableObject
{
    public string enemyName;
    public int hp;
    public float speed;
    public int damage;
    public int expValue;
    public Sprite sprite;
    public EnemyType type;
    public float spawnWeight;
    public float spawnTime;      # 何時開始出現（秒）
}
```

**TalentData.cs（C#類別）：**
```csharp
[CreateAssetMenu(fileName = "TalentData", menuName = "Survivor/Talent Data")]
public class TalentData : ScriptableObject
{
    public string talentName;
    public TalentType type;
    public int value;            # 效果數值
    public Sprite icon;
    public string description;
}
```

#### 3. ScriptableObject 使用方式

**在 MonoBehaviour 中引用：**
```csharp
public class EnemyController : MonoBehaviour
{
    [SerializeField] private EnemyData enemyData;    # Inspector 中選擇 EnemyData.asset
    
    private void Start()
    {
        hp = enemyData.hp;
        speed = enemyData.speed;
        spriteRenderer.sprite = enemyData.sprite;
    }
}
```

**動態加載（Resources.Load）：**
```csharp
EnemyData bossData = Resources.Load<EnemyData>("ScriptableObjects/Enemies/BossEnemyData");
```

## 27. Unity 組件架構（MonoBehaviour）

### A. MonoBehaviour 設計原則

#### 1. 單一職責原則
每個 MonoBehaviour 組件只負責一件事（移動、戰鬥、渲染），保持代碼清晰。

#### 2. Update Loop 四個 Phase（對應 JavaScript 版本）

**JavaScript 版本 Update Loop：**
```javascript
Phase 1: 清理與準備（Grid.clear + Grid.insert）
Phase 2: 狀態更新（player.update、enemy.update）
Phase 3: 系統邏輯（autoFire、checkCollisions）
Phase 4: UI 更新（UI.update）
```

**Unity 版本 Update Loop：**
```csharp
void Update()
{
    // Phase 1: 清理與準備（使用 Unity Physics 或自製 SpatialGrid）
    spatialGrid.Clear();
    spatialGrid.InsertAllEnemies(enemies);
    
    // Phase 2: 狀態更新（所有 MonoBehaviour Update自動執行）
    // - PlayerMovement.Update() 自動執行
    // - EnemyMovement.Update() 自動執行
    // - PlayerCombat.Update() 自動執行
    
    // Phase 3: 系統邏輯（GameManager 控制）
    AutoFire();               # 玱家自動射擊
    CheckCollisions();        # 碰撞检测（使用 Unity Physics 或 SpatialGrid）
    CleanupDeadEntities();    # 清理死亡實體
    
    // Phase 4: UI 更新（UIManager Update自動執行）
    // - UIManager.Update() 自動執行
}
```

#### 3. Unity Update vs JavaScript Update

| JavaScript 版本 | Unity 版本 | 說明 |
|----------------|-----------|------|
| game.update(dt) | GameManager.Update() | 主遊戲迴圈 |
| player.update(dt, keys) | PlayerMovement.Update() | Unity 自動調用，不需要手動調用 |
| enemy.update(dt, playerX, playerY) | EnemyMovement.Update() | Unity 自動調用 |
| projectile.update(dt) | ProjectileMovement.Update() | Unity 自動調用 |
| requestAnimationFrame(gameLoop) | Unity Engine Update Loop | Unity 内建遊戲迴圈 |

**Unity Update 自動調用順序：**
- Unity Engine 自動調用所有 MonoBehaviour 的 Update()
- 順序由 Execution Order設定（Project Settings → Script Execution Order）
- 建議設定：GameManager（-100） → Systems（-50） → Components（0） → UI（100）

### B. MonoBehaviour 範例

#### 1. PlayerMovement.cs（玩家移動）
```csharp
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed = 200f;
    private Rigidbody2D rb;
    private Vector2 movement;
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    
    private void Update()
    {
        // 輸入處理（新 Input System）
        movement.x = Input.GetAxisRaw("Horizontal");    # WASD / 方向鍵
        movement.y = Input.GetAxisRaw("Vertical");
        
        // 朝向移動方向（面向方向）
        if (movement != Vector2.zero)
        {
            transform.rotation = Quaternion.LookRotation(Vector3.forward, movement);
        }
    }
    
    private void FixedUpdate()
    {
        // 物理移動（Rigidbody2D）
        rb.MovePosition(rb.position + movement * speed * Time.fixedDeltaTime);
    }
}
```

#### 2. PlayerCombat.cs（玩家戰鬥）
```csharp
public class PlayerCombat : MonoBehaviour
{
    [SerializeField] private float fireRate = 0.5f;
    [SerializeField] private float attackRange = 300f;
    [SerializeField] private GameObject projectilePrefab;
    
    private float fireCooldown = 0f;
    private ObjectPool projectilePool;
    
    private void Update()
    {
        fireCooldown -= Time.deltaTime;
        
        if (CanFire())
        {
            AutoFire();
        }
    }
    
    private bool CanFire()
    {
        return fireCooldown <= 0f;
    }
    
    private void AutoFire()
    {
        GameObject nearestEnemy = FindNearestEnemy();
        if (nearestEnemy != null)
        {
            FireProjectile(nearestEnemy.transform.position);
            fireCooldown = fireRate;
        }
    }
    
    private void FireProjectile(Vector2 targetPosition)
    {
        GameObject projectile = projectilePool.Get();
        projectile.transform.position = transform.position;
        projectile.GetComponent<ProjectileMovement>().SetTarget(targetPosition);
    }
    
    private GameObject FindNearestEnemy()
    {
        // 使用 SpatialGrid 或 Unity Physics
        GameObject[] nearbyEnemies = spatialGrid.GetNearby(transform.position, attackRange);
        return nearbyEnemies.OrderBy(e => Vector2.Distance(transform.position, e.transform.position)).FirstOrDefault();
    }
}
```

#### 3. EnemyMovement.cs（敵人移動）
```csharp
public class EnemyMovement : MonoBehaviour
{
    [SerializeField] private float speed = 50f;
    [SerializeField] private EnemyData enemyData;
    
    private Rigidbody2D rb;
    private Transform playerTransform;
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        speed = enemyData.speed;
    }
    
    private void Start()
    {
        playerTransform = GameManager.Instance.Player.transform;
    }
    
    private void FixedUpdate()
    {
        // 直線追蹤玩家
        Vector2 direction = (playerTransform.position - transform.position).normalized;
        rb.MovePosition(rb.position + direction * speed * Time.fixedDeltaTime);
    }
}
```

### C. Unity 組件通信方式

#### 1. 直接引用（Inspector）
```csharp
public class GameManager : MonoBehaviour
{
    [SerializeField] private PlayerController player;    # Inspector 中引用 Player GameObject
    [SerializeField] private UIManager uiManager;
}
```

#### 2. GetComponent（同 GameObject）
```csharp
public class PlayerController : MonoBehaviour
{
    private PlayerMovement movement;
    private PlayerCombat combat;
    
    private void Awake()
    {
        movement = GetComponent<PlayerMovement>();
        combat = GetComponent<PlayerCombat>();
    }
}
```

#### 3. FindObjectOfType（全局查找）
```csharp
GameManager gameManager = FindObjectOfType<GameManager>();
```

#### 4. Singleton Pattern（GameManager）
```csharp
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
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
    }
}
```

## 28. Unity 粒子與特效系統

### A. Particle System（Unity 內建）

#### 1. Particle System 組件結構

**ExplosionEffect.prefab 組件：**
```
ExplosionEffect GameObject
├── Particle System               # 主粒子系統
│   ├── Duration: 0.5s
│   ├── Looping: false
│   ├── Start Lifetime: 0.5s
│   ├── Start Speed: 80-140
│   ├── Start Size: 2-5
│   ├── Emission: Rate 12, Burst 12
│   ├── Shape: Circle, Radius 30
│   ├── Color: Red → Orange → Yellow (Gradient)
│   └── Renderer: Default Particle Material
├── Sub Emitters                  # 子粒子發射器
│   ├── Core Particles (6顆大粒子，速度 30-50)
│   └── Flash Light (白→黃光圈擴散)
└── Auto Destroy                  # 0.5秒後自動销毁
```

#### 2. Particle System 參數對照

| JavaScript 版本 | Unity Particle System | 說明 |
|----------------|----------------------|------|
| 12顆粒子爆散 | Emission: Burst 12 | 爆發數量 |
| 速度 80-140 px/秒 | Start Speed: 80-140 | 粒子速度 |
| 紅/橙色粒子 | Color Gradient: Red → Orange | 粒子顏色 |
| 0.5秒消失 | Duration: 0.5s | 特效持續時間 |
| 中心閃光 | Sub Emitter: Flash Light | 子粒子發射器 |

#### 3. Particle System C# 控制

```csharp
public class ExplosionEffect : MonoBehaviour
{
    private ParticleSystem particleSystem;
    
    private void Awake()
    {
        particleSystem = GetComponent<ParticleSystem>();
    }
    
    public void Play(Vector2 position)
    {
        transform.position = position;
        particleSystem.Play();
        
        // 0.5秒後自動销毁
        Destroy(gameObject, 0.5f);
    }
}
```

### B. Trail Renderer（揮劍軌跡）

#### 1. Trail Renderer 組件

**SwingTrail.prefab 組件：**
```
SwingTrail GameObject
├── Trail Renderer                # 揮劍軌跡
│   ├── Time: 0.15s              # 軌跡持續時間
│   ├── Min Vertex Distance: 0.1
│   ├── Width: Curve (0 → 2 → 0) # 軌跡宽度曲線
│   ├── Color: Gradient (Blue → Transparent) # 藍色半透明
│   └── Material: Default Trail Material
└── Line Renderer                 # 可選的靜態軌跡線
```

#### 2. Trail Renderer C# 控制

```csharp
public class SwingTrail : MonoBehaviour
{
    private TrailRenderer trailRenderer;
    private bool isSwinging = false;
    
    private void Awake()
    {
        trailRenderer = GetComponent<TrailRenderer>();
        trailRenderer.enabled = false;    # 預設關閉
    }
    
    public void StartSwing()
    {
        isSwinging = true;
        trailRenderer.enabled = true;
        
        // 劍尖位置更新（跟隨劍的 Transform）
        transform.position = swordTipTransform.position;
    }
    
    public void EndSwing()
    {
        isSwinging = false;
        trailRenderer.enabled = false;
    }
}
```

### C. Visual Effect Graph（高级特效）

#### 1. Visual Effect Graph 用途
適合複雜特效（Boss 死亡特效、多重粒子爆散），比 Particle System 更强大。

**BossDeathEffect.vfx：**
```
Visual Effect Graph
├── Spawn System
│   ├── Spawn Rate: 0（手動觸發）
│   └── Spawn Burst: 50顆粒子
├── Particle System
│   ├── Lifetime: 1s
│   ├── Speed: Random(100-200)
│   ├── Size: Random(5-10)
│   ├── Color: Gradient (Red → Orange → Transparent)
│   └── Gravity: -50（向下落）
├── Output
│   ├── Shader: Unlit/Particle
│   └── Blend Mode: Additive
```

#### 2. Visual Effect Graph C# 控制

```csharp
public class BossDeathEffect : MonoBehaviour
{
    private VisualEffect visualEffect;
    
    private void Awake()
    {
        visualEffect = GetComponent<VisualEffect>();
    }
    
    public void Play(Vector2 position)
    {
        transform.position = position;
        visualEffect.SendEvent("OnPlay");    # 觸發 Spawn Burst
        
        // 1秒後自動销毁
        Destroy(gameObject, 1f);
    }
}
```

## 29. Unity 音效系統

### A. Audio Source（音效播放）

#### 1. Audio Source 組件

**PlayerAudio.prefab 組件：**
```
PlayerAudio GameObject
├── Audio Source                  # 音效播放組件
│   ├── Play On Awake: false
│   ├── Loop: false
│   ├── Volume: 0.7              # 音效音量（預設 0.7）
│   ├── Spatial Blend: 0         # 2D 音效（非 3D）
│   ├── Audio Clips: (陣列)
│   │   ├── swingClip            # 揮劍音效
│   │   ├── hitClip              # 命中音效
│   │   ├── damageClip           # 受傷音效
│   │   └── levelUpClip          # 升級音效
│   └── Output: Audio Mixer Group（主音效组）
```

#### 2. Audio Source C# 控制

```csharp
public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioClip swingClip;
    [SerializeField] private AudioClip hitClip;
    [SerializeField] private AudioClip killClip;
    [SerializeField] private AudioClip chainKillClip;
    [SerializeField] private AudioClip levelUpClip;
    [SerializeField] private AudioClip damageClip;
    [SerializeField] private AudioClip pickupClip;
    [SerializeField] private AudioClip gameOverClip;
    
    [SerializeField] private AudioSource audioSource;
    
    public void PlaySwing()
    {
        audioSource.PlayOneShot(swingClip, 0.7f);
    }
    
    public void PlayHit()
    {
        audioSource.PlayOneShot(hitClip, 0.7f);
    }
    
    public void PlayKill()
    {
        audioSource.PlayOneShot(killClip, 0.7f);
    }
    
    public void PlayChainKill()
    {
        audioSource.PlayOneShot(chainKillClip, 0.7f);
    }
    
    public void PlayLevelUp()
    {
        audioSource.PlayOneShot(levelUpClip, 0.7f);
    }
    
    public void PlayDamage()
    {
        audioSource.PlayOneShot(damageClip, 0.7f);
    }
    
    public void PlayPickup()
    {
        audioSource.PlayOneShot(pickupClip, 0.7f);
    }
    
    public void PlayGameOver()
    {
        audioSource.PlayOneShot(gameOverClip, 0.7f);
    }
}
```

### B. Audio Mixer（音量控制）

#### 1. Audio Mixer 組件

**AudioMixer.mixer：**
```
Audio Mixer
├── Master Group                  # 主音量
│   ├── Volume: 0.5 dB           # 主音量（預設 0.5）
│   ├── SFX Group                 # 音效组
│   │   ├── Volume: 0.7 dB       # 音效音量（預設 0.7）
│   │   ├── Attenuation: -10 dB ~ 10 dB
│   │   └── Audio Sources: PlayerAudio, EnemyAudio, EffectAudio
│   ├── BGM Group                 # 背景音樂组
│   │   ├── Volume: 0.3 dB       # 背景音量（預設 0.3）
│   │   ├── Attenuation: -10 dB ~ 10 dB
│   │   └── Audio Sources: BGMAudio
│   └── UI Group                  # UI 音效组
│   │   ├── Volume: 0.5 dB
│   │   └── Audio Sources: UIAudio
```

#### 2. Audio Mixer C# 控制

```csharp
public class AudioSettings : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;
    
    public void SetMasterVolume(float volume)
    {
        audioMixer.SetFloat("MasterVolume", Mathf.Log10(volume) * 20);
    }
    
    public void SetSFXVolume(float volume)
    {
        audioMixer.SetFloat("SFXVolume", Mathf.Log10(volume) * 20);
    }
    
    public void SetBGMVolume(float volume)
    {
        audioMixer.SetFloat("BGMVolume", Mathf.Log10(volume) * 20);
    }
}
```

### C. Background Music（背景音樂）

#### 1. BGM Audio Source 組件

**BGMAudio GameObject：**
```
BGMAudio GameObject
├── Audio Source                  # 背景音樂播放
│   ├── Play On Awake: true      # 遊戲開始時自動播放
│   ├── Loop: true               # 循環播放
│   ├── Volume: 0.3              # 背景音量（預設 0.3）
│   ├── Spatial Blend: 0         # 2D 音效
│   ├── Audio Clip: bgmClip      # 背景音樂音效檔
│   └ Output: Audio Mixer BGM Group
```

#### 2. BGM C# 控制

```csharp
public class BGMManager : MonoBehaviour
{
    [SerializeField] private AudioSource bgmSource;
    
    public void PlayBGM()
    {
        bgmSource.Play();
    }
    
    public void StopBGM()
    {
        bgmSource.Stop();
    }
    
    public void SetVolume(float volume)
    {
        bgmSource.volume = volume;
    }
}
```

## 30. Unity 輸入系統

### A. New Input System（推薦）

#### 1. Input Action Asset

**PlayerControls.inputactions：**
```
Input Actions
├── Action Map: Player
│   ├── Move (WASD / 方向鍵)
│   │   ├── Binding: Keyboard W, A, S, D
│   │   ├── Binding: Keyboard Up, Down, Left, Right Arrow
│   │   └── Composite: 2D Vector
│   ├── Skill (Q键)
│   │   ├── Binding: Keyboard Q
│   ├── Pause (ESC / P键)
│   │   ├── Binding: Keyboard Escape
│   │   ├── Binding: Keyboard P
│   └── Resume (任意键)
│   │   ├── Binding: Keyboard Any Key
```

#### 2. Input System C# 使用

```csharp
public class PlayerInput : MonoBehaviour
{
    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private InputActionReference skillAction;
    [SerializeField] private InputActionReference pauseAction;
    
    private Vector2 movementInput;
    
    private void Awake()
    {
        moveAction.action.Enable();
        skillAction.action.Enable();
        pauseAction.action.Enable();
        
        skillAction.action.performed += OnSkill;
        pauseAction.action.performed += OnPause;
    }
    
    private void Update()
    {
        movementInput = moveAction.action.ReadValue<Vector2>();
    }
    
    private void OnSkill(InputAction.CallbackContext context)
    {
        // Q技能觸發
        GameManager.Instance.Player.UseSkill();
    }
    
    private void OnPause(InputAction.CallbackContext context)
    {
        // ESC/P暫停觸發
        GameManager.Instance.TogglePause();
    }
}
```

### B. Legacy Input Manager（舊版）

#### 1. Input Manager 直接使用

```csharp
public class PlayerMovement : MonoBehaviour
{
    private void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");    # WASD / 方向鍵
        float vertical = Input.GetAxisRaw("Vertical");
        
        Vector2 movement = new Vector2(horizontal, vertical);
        
        if (Input.GetKeyDown(KeyCode.Q))
        {
            UseSkill();
        }
        
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
        {
            TogglePause();
        }
    }
}
```

## 31. Unity UI 系統（Canvas）

### A. Canvas 系統

#### 1. UI Canvas 組件

**HUDCanvas GameObject：**
```
HUDCanvas GameObject
├── Canvas                        # UI Canvas 組件
│   ├── Render Mode: Screen Space - Overlay
│   ├── UI Scale Mode: Scale With Screen Size
│   ├── Reference Resolution: 1920x1080
│   └── Match: 0.5 (Width & Height)
├── Canvas Scaler                # UI 缩放
├── Graphic Raycaster            # UI 交互
├── HUD Panel                    # 狀態列
│   ├── HP Bar                   # 血量條（Slider）
│   ├── Exp Bar                  # 等級條（Slider）
│   ├── Level Text               # 等級文字 "Lv. N"
│   └── Timer Text               # 計時器 "MM:SS"
├── Buff Notification            # Buff 通知
│   ├── Icon Image               # 圖示
│   ├── Text Text                # 文字
│   ├── Countdown Bar            # 倒計時進度條
│   └── Animation                # 滑入滑出動畫
└── Skill Status UI              # 技能狀態顯示
    ├── Skills List              # 已升級技能列表
    └── Q Skill Cooldown         # Q技能冷卻顯示
```

#### 2. UI C# 控制

```csharp
public class HUD : MonoBehaviour
{
    [SerializeField] private Slider hpBar;
    [SerializeField] private Slider expBar;
    [SerializeField] private Text levelText;
    [SerializeField] private Text timerText;
    
    public void UpdateHP(int currentHP, int maxHP)
    {
        hpBar.value = currentHP / maxHP;
    }
    
    public void UpdateExp(int currentExp, int expToLevel)
    {
        expBar.value = currentExp / expToLevel;
    }
    
    public void UpdateLevel(int level)
    {
        levelText.text = $"Lv. {level}";
    }
    
    public void UpdateTimer(float time)
    {
        int minutes = (int)(time / 60);
        int seconds = (int)(time % 60);
        timerText.text = $"{minutes:00}:{seconds:00}";
    }
}
```

### B. UI Prefabs

#### 1. UpgradePanel.prefab（升級彈窗）

```
UpgradePanel GameObject
├── Canvas                        #彈窗 Canvas
│   ├── Render Mode: Screen Space - Overlay
├── Panel                         # 背景面板
│   ├── Title Text                # "升級！選擇一項強化"
│   ├── Option Cards (3個)
│   │   ├── Card 1                # 選項卡片 1
│   │   ├── Card 2                # 選項卡片 2
│   │   ├── Card 3                # 選項卡片 3
│   │   └── Card Components:
│   │       ├── Icon Image        #圖示
│   │       ├── Name Text         # 天賦名稱
│   │       ├── Description Text  # 效果描述
│   │       └── Button            # 選擇按鈕
```

#### 2. GameOverPanel.prefab（遊戲結束畫面）

```
GameOverPanel GameObject
├── Canvas                        # 結束畫面 Canvas
├── Panel                         # 背景面板
│   ├── Title Text                # "遊戲結束"
│   ├── Stats Panel               # 本次成績
│   │   ├── Level Text            # 等級（新紀錄標記🏆）
│   │   ├── Kills Text            # 擊殺數
│   │   ├── BossKills Text        # Boss擊殺數
│   │   ├── Wave Text             # 最高波次（新紀錄標記🏆）
│   │   └── Time Text             # 存活時間（新紀錄標記🏆）
│   ├── Leaderboard Panel         # 排行榜 TOP 10（預設關閉）
│   │   ├── Toggle Button         # 展開/收起按鈕
│   │   └── Leaderboard List      # TOP 10列表
│   └── Restart Button            # 重新開始按鈕
```

## 32. Unity 存檔系統

### A. PlayerPrefs（簡單存檔）

#### 1. PlayerPrefs 用途
適合簡單存檔（最高等級、最長時間、總擊殺數等整数/浮點數）。

```csharp
public class StorageManager : MonoBehaviour
{
    public void SaveStats(GameStats stats)
    {
        PlayerPrefs.SetInt("HighestLevel", stats.highestLevel);
        PlayerPrefs.SetInt("LongestTime", stats.longestTime);
        PlayerPrefs.SetInt("TotalKills", stats.totalKills);
        PlayerPrefs.SetInt("HighestWave", stats.highestWave);
        PlayerPrefs.SetInt("TotalGames", stats.totalGames);
        PlayerPrefs.SetInt("BossesKilled", stats.bossesKilled);
        PlayerPrefs.Save();
    }
    
    public GameStats LoadStats()
    {
        GameStats stats = new GameStats
        {
            highestLevel = PlayerPrefs.GetInt("HighestLevel", 0),
            longestTime = PlayerPrefs.GetInt("LongestTime", 0),
            totalKills = PlayerPrefs.GetInt("TotalKills", 0),
            highestWave = PlayerPrefs.GetInt("HighestWave", 0),
            totalGames = PlayerPrefs.GetInt("TotalGames", 0),
            bossesKilled = PlayerPrefs.GetInt("BossesKilled", 0)
        };
        return stats;
    }
}
```

### B. JSON Serialization（複雜存檔）

#### 1. JSON 存檔用途
適合複雜存檔（排行榜 TOP 10、成就列表、歷史紀錄）。

```csharp
[System.Serializable]
public class GameStats
{
    public int highestLevel;
    public int longestTime;
    public int totalKills;
    public int highestWave;
    public int totalGames;
    public int bossesKilled;
    public List<AchievementRecord> achievements;
    public List<LeaderboardRecord> leaderboard;
}

[System.Serializable]
public class LeaderboardRecord
{
    public int level;
    public int time;
    public int kills;
    public int bossesKilled;
    public DateTime date;
}

public class StorageManager : MonoBehaviour
{
    private string savePath;
    
    private void Awake()
    {
        savePath = Path.Combine(Application.persistentDataPath, "survivor_stats.json");
    }
    
    public void SaveStats(GameStats stats)
    {
        string json = JsonUtility.ToJson(stats, true);
        File.WriteAllText(savePath, json);
    }
    
    public GameStats LoadStats()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            return JsonUtility.FromJson<GameStats>(json);
        }
        return new GameStats();
    }
}
```

## 33. Unity 調試工具

### A. DebugOverlay（可視化調試）

#### 1. DebugOverlay.cs（C# 版本）

```csharp
public class DebugOverlay : MonoBehaviour
{
    [SerializeField] private bool showDebug = false;
    [SerializeField] private KeyCode toggleKey = KeyCode.D;    # Ctrl+D
    
    private void Update()
    {
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(toggleKey))
        {
            showDebug = !showDebug;
        }
    }
    
    private void OnGUI()
    {
        if (!showDebug) return;
        
        // FPS顯示
        GUILayout.Label($"FPS: {(int)(1f / Time.deltaTime)}");
        
        // Memory顯示
        GUILayout.Label($"Memory: {System.GC.GetTotalMemory(false) / 1024} KB");
        
        //實體統計
        GUILayout.Label($"Player: {(GameManager.Instance.Player != null)}");
        GUILayout.Label($"Enemies: {GameManager.Instance.Enemies.Count}");
        GUILayout.Label($"Projectiles: {GameManager.Instance.Projectiles.Count}");
        GUILayout.Label($"Exp Orbs: {GameManager.Instance.ExperienceOrbs.Count}");
        
        // Grid 狀態
        GUILayout.Label($"Grid Entities: {GameManager.Instance.SpatialGrid.TotalEntities}");
        
        // fireCooldown 狀態
        GUILayout.Label($"fireCooldown: {GameManager.Instance.PlayerCombat.FireCooldown}");
        
        // 警告顯示
        if (GameManager.Instance.Enemies.Count > 100)
        {
            GUI.color = Color.red;
            GUILayout.Label("⚠ 敵人過多");
        }
        
        if (GameManager.Instance.SpatialGrid.TotalEntities == 0)
        {
            GUI.color = Color.red;
            GUILayout.Label("⚠ Grid空");
        }
    }
}
```

### B. GameLogger（Console 日誌）

#### 1. GameLogger.cs（C# 版本）

```csharp
public enum LogLevel
{
    ERROR,
    INFO,
    DEBUG
}

public static class GameLogger
{
    public static LogLevel currentLevel = LogLevel.INFO;
    
    public static void LogError(string message)
    {
        Debug.LogError($"[ERROR] {message}");
    }
    
    public static void LogInfo(string message)
    {
        if (currentLevel >= LogLevel.INFO)
        {
            Debug.Log($"[INFO] {message}");
        }
    }
    
    public static void LogDebug(string message)
    {
        if (currentLevel >= LogLevel.DEBUG)
        {
            Debug.Log($"[DEBUG] {message}");
        }
    }
    
    public static void ToggleLogLevel()
    {
        currentLevel = (LogLevel)(((int)currentLevel + 1) % 3);
        Debug.Log($"Log Level: {currentLevel}");
    }
}
```

### C. GameValidator（硬斷言檢查）

#### 1. GameValidator.cs（C# 版本）

```csharp
public static class GameValidator
{
    public static void ValidatePhase1(SpatialGrid grid, List<GameObject> enemies)
    {
        int gridEntities = grid.TotalEntities;
        int enemyCount = enemies.Count;
        
        if (gridEntities != enemyCount)
        {
            GameLogger.LogError($"Phase 1失敗：Grid實體數({gridEntities}) ≠ 敵人數({enemyCount})");
        }
    }
    
    public static void ValidatePhase2(float previousCooldown, float currentCooldown)
    {
        if (currentCooldown >= previousCooldown)
        {
            GameLogger.LogError($"Phase 2失敗：fireCooldown未減少({previousCooldown} → {currentCooldown})");
        }
    }
    
    public static void ValidatePhase3(List<GameObject> nearbyEnemies)
    {
        if (nearbyEnemies.Count == 0)
        {
            GameLogger.LogError("Phase 3失敗：碰撞檢測失效（getNearby返回空陣列）");
        }
    }
}
```

## 34. Unity 效能優化

### A. Object Pooling（物件池化）

#### 1. ObjectPool.cs（C# 版本）

```csharp
public class ObjectPool : MonoBehaviour
{
    [SerializeField] private GameObject prefab;
    [SerializeField] private int initialSize = 50;
    [SerializeField] private int maxSize = 200;
    
    private Queue<GameObject> pool = new Queue<GameObject>();
    private List<GameObject> activeObjects = new List<GameObject>();
    
    private void Awake()
    {
        for (int i = 0; i < initialSize; i++)
        {
            GameObject obj = Instantiate(prefab);
            obj.SetActive(false);
            pool.Enqueue(obj);
        }
    }
    
    public GameObject Get()
    {
        GameObject obj;
        
        if (pool.Count > 0)
        {
            obj = pool.Dequeue();
        }
        else if (activeObjects.Count < maxSize)
        {
            obj = Instantiate(prefab);
        }
        else
        {
            GameLogger.LogError("ObjectPool已達最大容量");
            return null;
        }
        
        obj.SetActive(true);
        activeObjects.Add(obj);
        return obj;
    }
    
    public void Return(GameObject obj)
    {
        obj.SetActive(false);
        activeObjects.Remove(obj);
        pool.Enqueue(obj);
    }
    
    public void CleanupInactive()
    {
        for (int i = activeObjects.Count - 1; i >= 0; i--)
        {
            if (!activeObjects[i].activeInHierarchy)
            {
                GameObject obj = activeObjects[i];
                activeObjects.RemoveAt(i);
                pool.Enqueue(obj);
            }
        }
    }
}
```

### B. SpatialGrid（空間分割）

#### 1. SpatialGrid.cs（C# 版本）

```csharp
public class SpatialGrid
{
    private float cellSize = 100f;
    private Dictionary<Vector2Int, List<GameObject>> grid = new Dictionary<Vector2Int, List<GameObject>>();
    
    public void Clear()
    {
        grid.Clear();
    }
    
    public void Insert(GameObject obj)
    {
        Vector2Int cell = GetCell(obj.transform.position);
        
        if (!grid.ContainsKey(cell))
        {
            grid[cell] = new List<GameObject>();
        }
        
        grid[cell].Add(obj);
    }
    
    public List<GameObject> GetNearby(Vector2 position, float range)
    {
        List<GameObject> nearbyObjects = new List<GameObject>();
        
        Vector2Int centerCell = GetCell(position);
        int cellRange = (int)(range / cellSize) + 1;
        
        for (int x = -cellRange; x <= cellRange; x++)
        {
            for (int y = -cellRange; y <= cellRange; y++)
            {
                Vector2Int cell = new Vector2Int(centerCell.x + x, centerCell.y + y);
                
                if (grid.ContainsKey(cell))
                {
                    nearbyObjects.AddRange(grid[cell]);
                }
            }
        }
        
        return nearbyObjects;
    }
    
    private Vector2Int GetCell(Vector2 position)
    {
        return new Vector2Int(
            Mathf.FloorToInt(position.x / cellSize),
            Mathf.FloorToInt(position.y / cellSize)
        );
    }
    
    public int TotalEntities
    {
        get { return grid.Values.Sum(list => list.Count); }
    }
}
```

### C. Sprite Atlas（美術打包）

#### 1. Sprite Atlas 配置

```
Assets/SpriteAtlases/
├── PlayerAtlas.spriteatlas       # 玱家 Sprite Atlas
│   ├── Objects for Packing:
│   │   ├── Sprites/Player/
│   │   ├── Sprites/Sword/
│   │   └── Sprites/Armor/
│   ├── Packing Settings:
│   │   ├── Block Size: 1024x1024
│   │   ├── Padding: 2px
│   │   └── Tight Packing: true
│   └ Settings for Platform:
│   │   ├── Default: Enable
│   │   ├── Max Texture Size: 1024
│   │   ├── Format: RGBA Compressed
├── EnemyAtlas.spriteatlas        # 敵人 Sprite Atlas
│   ├── Objects for Packing:
│   │   ├── Sprites/Enemies/
│   │   ├── Sprites/Boss/
├── EffectAtlas.spriteatlas       # 特效 Sprite Atlas
│   ├── Objects for Packing:
│   │   ├── Sprites/Effects/
│   │   ├── Sprites/Particles/
└── UIAtlas.spriteatlas           # UI Sprite Atlas
│   ├── Objects for Packing:
│   │   ├── Sprites/UI/
│   │   ├── Sprites/Icons/
```

#### 2. Sprite Atlas C# 使用

```csharp
// Sprite Atlas 自動批次渲染相同 Atlas 的 Sprite
// 不需要手動代碼，Unity 自動處理
// 只需確保 Sprite Renderer 使用 Sprite Atlas 中的 Sprite

Sprite playerSprite = Resources.Load<Sprite>("Sprites/Player/player_idle");
spriteRenderer.sprite = playerSprite;    # Unity 自動批次渲染
```
