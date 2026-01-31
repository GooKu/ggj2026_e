# Antigravity 協作守則 (AGENT.md)

本文件定義了 Antigravity 與用戶 (KuGoo) 協作時的最高準則。**即使 IDE 重啟，我也必須在每次開始工作前先閱讀此文件。**

## 1. 語言規範
- **預設語言**：所有計畫 (implementation_plan.md)、導覽 (walkthrough.md)、任務清單 (task.md) 以及與用戶的溝通，必須**主動且預設使用繁體中文**。

## 2. 工程流程 (Strict Plan-First Implementation)
- **任何邏輯或代碼變更**，必須遵守以下流程：
    1. **研究 (PLANNING)**：閱讀代碼，理解需求。
    2. **提案 (PLANNING)**：撰寫 `implementation_plan.md` (中文)。
    3. **審核 (PLANNING)**：使用 `notify_user` 並將 `BlockedOnUser` 設為 `true`，靜候用戶確認。
    4. **執行 (EXECUTION)**：用戶核准後，方可開始修改代碼。
    5. **驗證與導覽 (VERIFICATION)**：完成後提供 `walkthrough.md` 並通知用戶。

## 3. 代碼風格與專案細節 (GGJ 2026)
- **UI 系統**：使用 `UnityEngine.UI`。
- **組件式開發**：確保事件連結邏輯清晰，盡量在 `GameManager` 或 `UIManager` 中自動初始化連結。
- **對話系統**：使用 Prefab 實例化，並在生成點 (SpawnPoints) 下生成。

## 4. 錯誤記憶
- 嚴禁「不先給計畫就直接動手」。
- 嚴禁「使用英文回覆或撰寫文檔」。