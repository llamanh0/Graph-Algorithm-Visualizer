using UnityEngine;
using TMPro;
using System;

/// <summary>
/// Node'un gorsel temsili
/// Daire, etiket ve mesafe bilgisi icerir
/// Suruklenebilir
/// </summary>
public class NodeVisual : MonoBehaviour
{
    #region Unity References

    [Header("References")]
    public SpriteRenderer spriteRenderer;
    public TextMeshPro labelText;          // Node ID'si
    public TextMeshPro distanceText;       // Mesafe bilgisi

    [Header("Settings")]
    public Color defaultColor = Color.white;
    public Color highlightColor = Color.yellow;
    public Color visitedColor = new Color(0.5f, 0.8f, 0.5f);

    #endregion

    #region Private Fields

    private int nodeId;
    private bool isDragging = false;

    #endregion

    #region Events

    // Pozisyon degistiginde tetiklenen event
    public System.Action<int, Vector2> OnPositionChanged;

    #endregion

    #region Initialization

    public void Initialize(int id, Vector2 position)
    {
        nodeId = id;
        transform.position = new Vector3(position.x, position.y, 0);
        labelText.text = id.ToString();
        distanceText.text = "";
        ResetColor();
    }

    #endregion

    #region Visual Changes

    // Renk degistir
    public void SetColor(Color color)
    {
        spriteRenderer.color = color;
    }

    // Varsayilan renge don
    public void ResetColor()
    {
        spriteRenderer.color = defaultColor;
    }

    // Mesafe etiketini ayarla
    public void SetDistanceLabel(string text)
    {
        distanceText.text = text;
    }

    #endregion

    #region Animation

    // Buyuyup kuculme animasyonu
    public void PulseAnimation()
    {
        StartCoroutine(PulseCoroutine());
    }

    private System.Collections.IEnumerator PulseCoroutine()
    {
        Vector3 originalScale = transform.localScale;
        Vector3 targetScale = originalScale * 1.3f;
        float duration = 0.2f;

        // Buyut
        float elapsed = 0;
        while (elapsed < duration)
        {
            transform.localScale = Vector3.Lerp(originalScale, targetScale, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Kucult
        elapsed = 0;
        while (elapsed < duration)
        {
            transform.localScale = Vector3.Lerp(targetScale, originalScale, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localScale = originalScale;
    }

    #endregion

    #region Drag and Drop

    // Mouse'a basinca
    private void OnMouseDown()
    {
        isDragging = true;
    }

    // Mouse'u suruklerken
    private void OnMouseDrag()
    {
        if (isDragging)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;
            transform.position = mousePos;
            OnPositionChanged?.Invoke(nodeId, mousePos);
        }
    }

    // Mouse'u birakirken
    private void OnMouseUp()
    {
        isDragging = false;
    }

    #endregion

    #region Public Methods

    public int GetNodeId() => nodeId;

    #endregion
}