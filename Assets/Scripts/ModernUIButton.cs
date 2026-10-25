using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class ModernUIButton : MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler,
    IPointerDownHandler, IPointerUpHandler,
    IPointerClickHandler
{
    public enum ButtonType
    {
        Primary,
        Secondary,
        Danger
    }

    [Header("Configuration")]
    [SerializeField] private ButtonType type = ButtonType.Secondary;
    [SerializeField] private bool enablePulse = false;
    [SerializeField] private bool interactable = true;

    [Header("Visual References")]
    [SerializeField] private Image buttonImage;
    [SerializeField] private Text buttonText;

    public Action OnClick;

    private Sprite normalSprite;
    private Sprite hoverSprite;
    private Sprite pressedSprite;

    private Color normalTextColor;
    private Color hoverTextColor;
    private Color pressedTextColor;
    private Color disabledTextColor = new Color(0.5f, 0.6f, 0.7f, 0.5f);

    private Vector3 baseScale = Vector3.one;
    private Coroutine scaleCoroutine;
    private bool isHovered = false;
    private bool isPressed = false;
    private float pulseTimer = 0f;

    public bool Interactable
    {
        get => interactable;
        set
        {
            interactable = value;
            UpdateVisualState();
        }
    }

    public void Setup(ButtonType btnType, string text, Action clickAction, bool pulse = false)
    {
        type = btnType;
        OnClick = clickAction;
        enablePulse = pulse;

        if (buttonImage == null) buttonImage = GetComponent<Image>();
        if (buttonText == null) buttonText = GetComponentInChildren<Text>();

        if (buttonText != null && !string.IsNullOrEmpty(text))
        {
            buttonText.text = text;
        }

        AssignSpritesAndColors();
        UpdateVisualState();
    }

    void Awake()
    {
        if (buttonImage == null) buttonImage = GetComponent<Image>();
        if (buttonText == null) buttonText = GetComponentInChildren<Text>();
        baseScale = transform.localScale;
    }

    void Start()
    {
        AssignSpritesAndColors();
        UpdateVisualState();
    }

    void AssignSpritesAndColors()
    {
        if (type == ButtonType.Primary)
        {
            normalSprite = UIProceduralSprites.GetPrimaryNormal();
            hoverSprite = UIProceduralSprites.GetPrimaryHover();
            pressedSprite = UIProceduralSprites.GetPrimaryPressed();

            normalTextColor = Color.white;
            hoverTextColor = new Color(0.9f, 1f, 1f, 1f);
            pressedTextColor = new Color(0.75f, 0.9f, 1f, 1f);
        }
        else
        {
            normalSprite = UIProceduralSprites.GetSecondaryNormal();
            hoverSprite = UIProceduralSprites.GetSecondaryHover();
            pressedSprite = UIProceduralSprites.GetSecondaryPressed();

            normalTextColor = new Color(0.95f, 0.98f, 1f, 1f);
            hoverTextColor = new Color(0.40f, 0.95f, 1f, 1f);
            pressedTextColor = new Color(0.70f, 0.88f, 1f, 0.9f);
        }
    }

    void Update()
    {
        if (enablePulse && interactable && !isHovered && !isPressed)
        {
            pulseTimer += Time.unscaledDeltaTime * 2.8f;
            float pulseScale = 1f + Mathf.Sin(pulseTimer) * 0.022f;
            transform.localScale = baseScale * pulseScale;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!interactable) return;
        isHovered = true;
        UpdateVisualState();

        if (UISoundManager.Instance != null)
        {
            UISoundManager.Instance.PlayHover();
        }

        AnimateScale(1.05f, 0.15f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!interactable) return;
        isHovered = false;
        isPressed = false;
        UpdateVisualState();
        AnimateScale(1.0f, 0.15f);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!interactable) return;
        isPressed = true;
        UpdateVisualState();
        AnimateScale(0.96f, 0.08f);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!interactable) return;
        isPressed = false;
        UpdateVisualState();
        AnimateScale(isHovered ? 1.05f : 1.0f, 0.12f);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!interactable) return;

        if (UISoundManager.Instance != null)
        {
            UISoundManager.Instance.PlayClick();
        }

        OnClick?.Invoke();
    }

    void UpdateVisualState()
    {
        if (buttonImage == null) return;

        if (!interactable)
        {
            buttonImage.sprite = normalSprite;
            buttonImage.color = new Color(0.6f, 0.65f, 0.75f, 0.45f);
            if (buttonText != null) buttonText.color = disabledTextColor;
            return;
        }

        buttonImage.color = Color.white;

        if (isPressed)
        {
            buttonImage.sprite = pressedSprite;
            if (buttonText != null) buttonText.color = pressedTextColor;
        }
        else if (isHovered)
        {
            buttonImage.sprite = hoverSprite;
            if (buttonText != null) buttonText.color = hoverTextColor;
        }
        else
        {
            buttonImage.sprite = normalSprite;
            if (buttonText != null) buttonText.color = normalTextColor;
        }
    }

    void AnimateScale(float targetMultiplier, float duration)
    {
        if (scaleCoroutine != null)
        {
            StopCoroutine(scaleCoroutine);
        }
        scaleCoroutine = StartCoroutine(ScaleRoutine(baseScale * targetMultiplier, duration));
    }

    IEnumerator ScaleRoutine(Vector3 target, float duration)
    {
        Vector3 start = transform.localScale;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            // Smooth ease out
            t = Mathf.Sin(t * Mathf.PI * 0.5f);
            transform.localScale = Vector3.Lerp(start, target, t);
            yield return null;
        }

        transform.localScale = target;
    }
}
