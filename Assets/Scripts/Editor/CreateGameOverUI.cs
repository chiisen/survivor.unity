using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

namespace SurvivorUnity.Editor
{
    public static class CreateGameOverUI
    {
        [MenuItem("Survivor/Create Game Over UI")]
        public static void Create()
        {
            var canvasObj = new GameObject("GameOverCanvas");
            var canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 100;
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();
            Undo.RegisterCreatedObjectUndo(canvasObj, "Create GameOverCanvas");

            var panelObj = new GameObject("GameOverPanel");
            panelObj.transform.SetParent(canvasObj.transform, false);
            var panelImage = panelObj.AddComponent<Image>();
            panelImage.color = new Color(0f, 0f, 0f, 0.8f);
            panelObj.SetActive(false);
            Undo.RegisterCreatedObjectUndo(panelObj, "Create GameOverPanel");

            var titleObj = CreateText("TitleText", "GAME OVER", 72, Color.red, new Vector2(0.5f, 0.7f), new Vector2(800, 100));
            titleObj.transform.SetParent(panelObj.transform, false);

            var statsObj = CreateText("StatsText", "存活時間: 0 秒\n擊殺數: 0", 36, Color.white, new Vector2(0.5f, 0.45f), new Vector2(600, 150));
            statsObj.transform.SetParent(panelObj.transform, false);

            var restartObj = CreateText("RestartText", "按 R 鍵重新開始", 28, Color.yellow, new Vector2(0.5f, 0.25f), new Vector2(400, 80));
            restartObj.transform.SetParent(panelObj.transform, false);

            Selection.activeGameObject = canvasObj;
            Debug.Log("[CreateGameOverUI] Game Over UI created! Panel is disabled, enable it in GameOver logic.");
        }

        private static GameObject CreateText(string name, string text, int fontSize, Color color, Vector2 anchorPos, Vector2 sizeDelta)
        {
            var obj = new GameObject(name);
            var textComp = obj.AddComponent<Text>();
            textComp.text = text;
            textComp.fontSize = fontSize;
            textComp.color = color;
            textComp.alignment = TextAnchor.MiddleCenter;

            var rect = obj.GetComponent<RectTransform>();
            rect.anchorMin = anchorPos;
            rect.anchorMax = anchorPos;
            rect.anchoredPosition = Vector2.zero;
            rect.sizeDelta = sizeDelta;

            Undo.RegisterCreatedObjectUndo(obj, $"Create {name}");
            return obj;
        }
    }
}