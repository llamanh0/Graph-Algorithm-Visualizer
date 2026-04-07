using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

/// <summary>
/// UI elementlerini stillendirir ve animasyonlari yonetir
/// Tum UI stillendirme islemleri burada
/// </summary>
public class UIManager : MonoBehaviour
{
    #region Singleton

    public static UIManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    #endregion

    #region References

    [Header("Main Panels")]
    public GameObject controlPanel;
    public GameObject infoPanel;
    public GameObject buildPanel;

    [Header("Buttons")]
    public Button runButton;
    public Button stepButton;
    public Button resetButton;
    public Button autoPlayButton;
    public Button loadSampleButton;
    public Button addNodeButton;
    public Button addEdgeButton;

    [Header("Input Elements")]
    public TMP_Dropdown algorithmDropdown;
    public Slider speedSlider;
    public TMP_InputField weightInput;
    public Toggle directedToggle;

    [Header("Text Elements")]
    public TextMeshProUGUI infoText;
    public TextMeshProUGUI stepInfoText;
    public TextMeshProUGUI stepCounterText;
    public TextMeshProUGUI logText;

    [Header("Background Elements")]
    public Image controlPanelBg;
    public Image infoPanelBg;
    public Image buildPanelBg;
    public Image logPanelBg;

    #endregion

    #region Initialization

    public void Initialize()
    {
        StyleAllButtons();
        StyleAllPanels();
        StyleDropdown();
        StyleSlider();
        StyleInputField();
        StyleToggle();
        StyleTexts();

        AnimatePanelsOnStart();
    }

    #endregion

    #region Button Styling

    private void StyleAllButtons()
    {
        StyleButton(runButton, UIColors.Success, "CALISTIR", 22);
        StyleButton(stepButton, UIColors.Primary, "SONRAKI ADIM", 18);
        StyleButton(resetButton, UIColors.Danger, "SIFIRLA", 18);
        StyleButton(autoPlayButton, UIColors.Warning, "OTOMATIK", 18);
        StyleButton(loadSampleButton, UIColors.Secondary, "ORNEK YUKLE", 16);
        StyleButton(addNodeButton, UIColors.Elevated, "DUGUM EKLE", 16);
        StyleButton(addEdgeButton, UIColors.Elevated, "KENAR EKLE", 16);
    }

    private void StyleButton(Button btn, Color32 bgColor, string text, int fontSize)
    {
        if (btn == null) return;

        // Ana Image component
        Image btnImage = btn.GetComponent<Image>();
        if (btnImage != null)
        {
            btnImage.color = bgColor;
            btnImage.type = Image.Type.Sliced;
        }

        // Text ayarlari
        TextMeshProUGUI btnText = btn.GetComponentInChildren<TextMeshProUGUI>();
        if (btnText != null)
        {
            btnText.text = text;
            btnText.fontSize = fontSize;
            btnText.fontStyle = FontStyles.Bold;
            btnText.color = UIColors.TextPrimary;
            btnText.alignment = TextAlignmentOptions.Center;
        }

        // Hover efekti
        ColorBlock colors = btn.colors;
        colors.normalColor = Color.white;
        colors.highlightedColor = new Color(1.1f, 1.1f, 1.1f, 1f);
        colors.pressedColor = new Color(0.9f, 0.9f, 0.9f, 1f);
        colors.selectedColor = Color.white;
        colors.disabledColor = new Color(0.6f, 0.6f, 0.6f, 1f);
        colors.colorMultiplier = 1f;
        colors.fadeDuration = 0.15f;
        btn.colors = colors;

        // Padding ekle (RectTransform)
        RectTransform rt = btn.GetComponent<RectTransform>();
        if (rt != null)
        {
            rt.sizeDelta = new Vector2(rt.sizeDelta.x, 45); // Minimum boy
        }
    }

    #endregion

    #region Panel Styling

    private void StyleAllPanels()
    {
        StylePanel(controlPanelBg, UIColors.Panel);
        StylePanel(infoPanelBg, UIColors.Panel);
        StylePanel(buildPanelBg, UIColors.Panel);
        StylePanel(logPanelBg, UIColors.Elevated);
    }

    private void StylePanel(Image panelImage, Color32 color)
    {
        if (panelImage == null) return;

        panelImage.color = color;
        panelImage.type = Image.Type.Sliced;

        // Shadow efekti icin Outline ekle
        Outline outline = panelImage.gameObject.GetComponent<Outline>();
        if (outline == null)
            outline = panelImage.gameObject.AddComponent<Outline>();

        outline.effectColor = new Color32(0, 0, 0, 100);
        outline.effectDistance = new Vector2(3, -3);
    }

    #endregion

    #region Dropdown Styling

    private void StyleDropdown()
    {
        if (algorithmDropdown == null) return;

        // Ana arkaplan
        Image dropdownBg = algorithmDropdown.GetComponent<Image>();
        if (dropdownBg != null)
        {
            dropdownBg.color = UIColors.Elevated;
        }

        // Label text
        TMP_Text label = algorithmDropdown.captionText;
        if (label != null)
        {
            label.fontSize = 18;
            label.color = UIColors.TextPrimary;
            label.fontStyle = FontStyles.Bold;
        }

        // Item text
        TMP_Text itemText = algorithmDropdown.itemText;
        if (itemText != null)
        {
            itemText.fontSize = 16;
            itemText.color = UIColors.TextPrimary;
        }

        // Dropdown template
        GameObject template = algorithmDropdown.template.gameObject;
        Image templateBg = template.GetComponent<Image>();
        if (templateBg != null)
        {
            templateBg.color = UIColors.Panel;
        }

        // Item arkaplan rengi
        Toggle itemToggle = algorithmDropdown.template.GetComponentInChildren<Toggle>();
        if (itemToggle != null)
        {
            Image toggleBg = itemToggle.GetComponent<Image>();
            if (toggleBg != null)
            {
                ColorBlock cb = itemToggle.colors;
                cb.normalColor = UIColors.Elevated;
                cb.highlightedColor = UIColors.Primary;
                cb.pressedColor = UIColors.Secondary;
                cb.selectedColor = UIColors.Primary;
                cb.colorMultiplier = 1f;
                itemToggle.colors = cb;
            }
        }
    }

    #endregion

    #region Slider Styling

    private void StyleSlider()
    {
        if (speedSlider == null) return;

        // Arkaplan (Background)
        Image background = speedSlider.transform.Find("Background")?.GetComponent<Image>();
        if (background != null)
        {
            background.color = UIColors.Elevated;
        }

        // Fill Area (Dolan kisim)
        Image fillImage = speedSlider.fillRect?.GetComponent<Image>();
        if (fillImage != null)
        {
            fillImage.color = UIColors.Primary;
        }

        // Handle (Tutacak)
        Image handleImage = speedSlider.handleRect?.GetComponent<Image>();
        if (handleImage != null)
        {
            handleImage.color = UIColors.TextPrimary;
        }

        // Slider renk degisimleri
        ColorBlock colors = speedSlider.colors;
        colors.normalColor = Color.white;
        colors.highlightedColor = new Color(1.1f, 1.1f, 1.1f);
        colors.pressedColor = new Color(0.9f, 0.9f, 0.9f);
        colors.selectedColor = Color.white;
        colors.disabledColor = new Color(0.5f, 0.5f, 0.5f);
        colors.colorMultiplier = 1f;
        speedSlider.colors = colors;
    }

    #endregion

    #region InputField Styling

    private void StyleInputField()
    {
        if (weightInput == null) return;

        // Arkaplan
        Image inputBg = weightInput.GetComponent<Image>();
        if (inputBg != null)
        {
            inputBg.color = UIColors.Elevated;
        }

        // Text
        TMP_Text inputText = weightInput.textComponent;
        if (inputText != null)
        {
            inputText.fontSize = 18;
            inputText.color = UIColors.TextPrimary;
        }

        // Placeholder
        TextMeshProUGUI placeholder = weightInput.placeholder as TextMeshProUGUI;
        if (placeholder != null)
        {
            placeholder.text = "Agirlik...";
            placeholder.fontSize = 16;
            placeholder.color = UIColors.TextMuted;
            placeholder.fontStyle = FontStyles.Italic;
        }

        // Focus efekti icin Outline
        Outline outline = weightInput.gameObject.GetComponent<Outline>();
        if (outline == null)
            outline = weightInput.gameObject.AddComponent<Outline>();

        outline.effectColor = UIColors.Primary;
        outline.effectDistance = new Vector2(2, -2);
        outline.enabled = false; // Baslangicta kapali

        // Focus eventleri
        weightInput.onSelect.AddListener((string val) => {
            if (outline != null) outline.enabled = true;
        });

        weightInput.onDeselect.AddListener((string val) => {
            if (outline != null) outline.enabled = false;
        });
    }

    #endregion

    #region Toggle Styling

    private void StyleToggle()
    {
        if (directedToggle == null) return;

        // Background
        Image toggleBg = directedToggle.targetGraphic as Image;
        if (toggleBg != null)
        {
            ColorBlock colors = directedToggle.colors;
            colors.normalColor = UIColors.Elevated;
            colors.highlightedColor = UIColors.Secondary;
            colors.pressedColor = UIColors.Primary;
            colors.selectedColor = UIColors.Primary;
            colors.disabledColor = new Color(0.5f, 0.5f, 0.5f);
            colors.colorMultiplier = 1f;
            directedToggle.colors = colors;
        }

        // Checkmark
        Image checkmark = directedToggle.graphic as Image;
        if (checkmark != null)
        {
            checkmark.color = UIColors.TextPrimary;
        }

        // Label
        TextMeshProUGUI label = directedToggle.GetComponentInChildren<TextMeshProUGUI>();
        if (label != null)
        {
            label.text = "Yonlu Graf";
            label.fontSize = 16;
            label.color = UIColors.TextPrimary;
        }
    }

    #endregion

    #region Text Styling

    private void StyleTexts()
    {
        StyleText(infoText, UIColors.TextPrimary, 20, FontStyles.Bold);
        StyleText(stepInfoText, UIColors.TextSecondary, 16, FontStyles.Normal);
        StyleText(stepCounterText, UIColors.TextMuted, 14, FontStyles.Italic);
        StyleText(logText, UIColors.TextSecondary, 13, FontStyles.Normal);
    }

    private void StyleText(TextMeshProUGUI text, Color32 color, int size, FontStyles style)
    {
        if (text == null) return;

        text.color = color;
        text.fontSize = size;
        text.fontStyle = style;
    }

    #endregion

    #region Animations

    private void AnimatePanelsOnStart()
    {
        if (controlPanel != null)
            StartCoroutine(SlideInPanel(controlPanel.GetComponent<RectTransform>(), 0.3f, 0f));

        if (infoPanel != null)
            StartCoroutine(SlideInPanel(infoPanel.GetComponent<RectTransform>(), 0.3f, 0.1f));

        if (buildPanel != null)
            StartCoroutine(SlideInPanel(buildPanel.GetComponent<RectTransform>(), 0.3f, 0.2f));
    }

    private IEnumerator SlideInPanel(RectTransform panel, float duration, float delay)
    {
        if (panel == null) yield break;

        yield return new WaitForSeconds(delay);

        Vector2 originalPos = panel.anchoredPosition;
        Vector2 startPos = originalPos + new Vector2(-300, 0);
        panel.anchoredPosition = startPos;

        CanvasGroup canvasGroup = panel.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = panel.gameObject.AddComponent<CanvasGroup>();

        canvasGroup.alpha = 0;

        float elapsed = 0;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            float smoothT = Mathf.SmoothStep(0, 1, t);

            panel.anchoredPosition = Vector2.Lerp(startPos, originalPos, smoothT);
            canvasGroup.alpha = smoothT;

            yield return null;
        }

        panel.anchoredPosition = originalPos;
        canvasGroup.alpha = 1;
    }

    public void PulseButton(Button btn)
    {
        if (btn == null) return;
        StartCoroutine(PulseButtonCoroutine(btn.transform));
    }

    private IEnumerator PulseButtonCoroutine(Transform btnTransform)
    {
        Vector3 originalScale = btnTransform.localScale;
        Vector3 targetScale = originalScale * 1.1f;
        float duration = 0.15f;

        // Buyut
        float elapsed = 0;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            btnTransform.localScale = Vector3.Lerp(originalScale, targetScale, elapsed / duration);
            yield return null;
        }

        // Kucult
        elapsed = 0;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            btnTransform.localScale = Vector3.Lerp(targetScale, originalScale, elapsed / duration);
            yield return null;
        }

        btnTransform.localScale = originalScale;
    }

    #endregion

    #region Public Methods

    public void ShowNotification(string message, Color color)
    {
        StartCoroutine(ShowNotificationCoroutine(message, color));
    }

    private IEnumerator ShowNotificationCoroutine(string message, Color color)
    {
        // Simdilik log'a yazdir, ileride Toast sistemi eklenebilir
        Debug.Log($"[NOTIFICATION] {message}");
        yield return null;
    }

    #endregion
}