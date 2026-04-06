using UnityEngine;
using TMPro;

/// <summary>
/// Edge'in gorsel temsili
/// Cizgi, agirlik etiketi ve ok basi icerir
/// </summary>
public class EdgeVisual : MonoBehaviour
{
    #region Unity References

    [Header("References")]
    public LineRenderer lineRenderer;
    public TextMeshPro weightText;
    public SpriteRenderer arrowHead;

    [Header("Settings")]
    public Color defaultColor = Color.white;
    public float defaultWidth = 0.05f;
    public float highlightWidth = 0.1f;

    [Header("Arrow Settings")]
    public float arrowSize = 0.3f;
    public float arrowDistance = 0.4f;
    public float nodeRadius = 0.5f;

    #endregion

    #region Private Fields

    private int fromId, toId;
    private float weight;
    private Transform fromNode, toNode;
    private bool isDirected;

    #endregion

    #region Initialization

    public void Initialize(int from, int to, float weight,
        Transform fromTransform, Transform toTransform, bool directed = false)
    {
        this.fromId = from;
        this.toId = to;
        this.weight = weight;
        this.fromNode = fromTransform;
        this.toNode = toTransform;
        this.isDirected = directed;

        SetupLineRenderer();
        SetupWeightLabel();
        SetupArrowHead(directed);

        UpdatePositions();
    }

    // LineRenderer'i ayarla
    private void SetupLineRenderer()
    {
        lineRenderer.positionCount = 2;
        lineRenderer.startWidth = defaultWidth;
        lineRenderer.endWidth = defaultWidth;
        lineRenderer.startColor = defaultColor;
        lineRenderer.endColor = defaultColor;
        lineRenderer.sortingOrder = -1;  // Nodelarin arkasinda
    }

    // Agirlik etiketini ayarla
    private void SetupWeightLabel()
    {
        weightText.text = weight.ToString("F1");
        weightText.fontSize = 3;
        weightText.alignment = TextAlignmentOptions.Center;
    }

    // Ok basini ayarla
    private void SetupArrowHead(bool directed)
    {
        if (arrowHead != null)
        {
            arrowHead.gameObject.SetActive(directed);

            if (directed)
            {
                // Ok basinin boyutunu ayarla
                arrowHead.transform.localScale = Vector3.one * arrowSize;
                arrowHead.color = defaultColor;
                arrowHead.sortingOrder = 0;  // Cizginin ustunde ama nodelarin altinda
            }
        }
    }

    #endregion

    #region Update

    private void Update()
    {
        UpdatePositions();
    }

    private void UpdatePositions()
    {
        if (fromNode == null || toNode == null) return;

        Vector3 start = fromNode.position;
        Vector3 end = toNode.position;
        Vector3 direction = (end - start).normalized;

        // Line ve Arrow icin ortak hesaplamalar
        Vector3 lineStart = start;
        Vector3 lineEnd = end;

        if (isDirected)
        {
            // Node yaricapi kadar iceride baslat
            lineStart = start + direction * nodeRadius;

            // Arrow'un boyutu + node yaricapi kadar once bitir
            float arrowLength = arrowSize * 0.5f; // Arrow'un uzunlugu
            lineEnd = end - direction * (nodeRadius + arrowLength);
        }
        else
        {
            // Yonsuz grafta sadece nodelardan biraz iceriden baslat/bitir
            lineStart = start + direction * nodeRadius;
            lineEnd = end - direction * nodeRadius;
        }

        // LINE'I CIZ
        lineRenderer.SetPosition(0, lineStart);
        lineRenderer.SetPosition(1, lineEnd);

        // AGIRLIK ETIKETINI ORTALA
        UpdateWeightPosition(start, end, direction);

        // ARROW BASINI LINE'IN TAM BITTIGINDE KOY
        if (isDirected && arrowHead != null && arrowHead.gameObject.activeSelf)
        {
            UpdateArrowPositionAligned(lineEnd, direction, direction);
        }
    }

    // Agirlik etiketini guncelle
    private void UpdateWeightPosition(Vector3 start, Vector3 end, Vector3 direction)
    {
        Vector3 mid = (start + end) / 2f;

        // Cizgiye dik vektor (yukari kaydirmak icin)
        Vector3 perpendicular = new Vector3(-direction.y, direction.x, 0);

        weightText.transform.position = mid + perpendicular * 0.3f;
        weightText.transform.rotation = Quaternion.identity;
    }

    // Arrow'u line ile hizala
    private void UpdateArrowPositionAligned(Vector3 lineEnd, Vector3 nodeCenter, Vector3 direction)
    {
        // Arrow'un GOVDESINI line'in bittigi yere koy
        // Arrow'un UCU node'un merkezine bakacak
        arrowHead.transform.position = lineEnd;

        // Z pozisyonunu sifirla
        Vector3 pos = arrowHead.transform.position;
        pos.z = -0.1f; // Biraz onde (line'in ustunde)
        arrowHead.transform.position = pos;

        // Rotasyonu ayarla - node merkezine baksın
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        arrowHead.transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    #endregion

    #region Visual Changes

    // Renk degistir
    public void SetColor(Color color)
    {
        lineRenderer.startColor = color;
        lineRenderer.endColor = color;
        weightText.color = color;

        if (arrowHead != null)
            arrowHead.color = color;
    }

    // Vurgulu goster
    public void SetHighlight(Color color)
    {
        SetColor(color);
        lineRenderer.startWidth = highlightWidth;
        lineRenderer.endWidth = highlightWidth;

        // Ok basini buyut
        if (arrowHead != null && arrowHead.gameObject.activeSelf)
        {
            arrowHead.transform.localScale = Vector3.one * arrowSize * 1.3f;
        }
    }

    // Varsayilan gorunume don
    public void ResetVisual()
    {
        SetColor(defaultColor);
        lineRenderer.startWidth = defaultWidth;
        lineRenderer.endWidth = defaultWidth;

        // Ok basini normal boyuta don
        if (arrowHead != null && arrowHead.gameObject.activeSelf)
        {
            arrowHead.transform.localScale = Vector3.one * arrowSize;
        }
    }

    #endregion

    #region Public Properties

    public int FromId => fromId;
    public int ToId => toId;

    // Bu edge belirtilen nodelar arasinda mi?
    public bool Matches(int from, int to)
    {
        // Yonlu grafta sadece tam eslesme
        if (isDirected)
            return (fromId == from && toId == to);

        // Yonsuz grafta her iki yon de olur
        return (fromId == from && toId == to) || (fromId == to && toId == from);
    }

    #endregion
}