using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FloatingScorePopup : MonoBehaviour
{
    private Text textComponent;
    private Outline outline;
    private RectTransform rectTransform;

    public static void Create(Vector3 worldPos, string text, Color color)
    {
        GameHUD hud = FindFirstObjectByType<GameHUD>();
        if (hud != null && hud.HUDCanvas != null)
        {
            GameObject popupObj = new GameObject("ScorePopup", typeof(RectTransform));
            popupObj.transform.SetParent(hud.HUDCanvas.transform, false);

            FloatingScorePopup popup = popupObj.AddComponent<FloatingScorePopup>();
            popup.Init(worldPos, text, color);
        }
    }

    public void Init(Vector3 worldPos, string text, Color color)
    {
        rectTransform = GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(380, 60);

        Camera cam = Camera.main;
        if (cam != null)
        {
            Vector3 screenPos = cam.WorldToScreenPoint(worldPos);
            rectTransform.position = screenPos;
        }

        textComponent = gameObject.AddComponent<Text>();
        Font defaultFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        if (defaultFont == null) defaultFont = Resources.GetBuiltinResource<Font>("Arial.ttf");

        textComponent.font = defaultFont;
        textComponent.text = text;
        textComponent.fontSize = 22;
        textComponent.fontStyle = FontStyle.Bold;
        textComponent.alignment = TextAnchor.MiddleCenter;
        textComponent.horizontalOverflow = HorizontalWrapMode.Overflow;
        textComponent.verticalOverflow = VerticalWrapMode.Overflow;
        textComponent.color = color;

        outline = gameObject.AddComponent<Outline>();
        outline.effectColor = new Color(0, 0, 0, 0.85f);
        outline.effectDistance = new Vector2(1.5f, -1.5f);

        StartCoroutine(FloatAndFadeRoutine());
    }

    private IEnumerator FloatAndFadeRoutine()
    {
        float duration = 0.85f;
        float elapsed = 0f;
        Vector3 startPos = rectTransform.position;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / duration;

            // Float up and drift slightly
            rectTransform.position = startPos + new Vector3(0, t * 65f, 0);

            // Scale punch at start
            float scale = t < 0.2f ? Mathf.Lerp(0.6f, 1.2f, t / 0.2f) : Mathf.Lerp(1.2f, 1.0f, (t - 0.2f) / 0.8f);
            rectTransform.localScale = Vector3.one * scale;

            // Fade out near end
            if (t > 0.5f)
            {
                float alpha = Mathf.Lerp(1f, 0f, (t - 0.5f) / 0.5f);
                Color c = textComponent.color;
                c.a = alpha;
                textComponent.color = c;

                Color oc = outline.effectColor;
                oc.a = alpha * 0.85f;
                outline.effectColor = oc;
            }

            yield return null;
        }

        Destroy(gameObject);
    }
}
