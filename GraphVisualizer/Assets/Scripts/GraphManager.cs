using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Ana kontrol sinifi - tum sistemi yonetir
/// UI, graf olusturma ve algoritma calistirma islemleri burada
/// </summary>
public class GraphManager : MonoBehaviour
{
    #region Unity References

    [Header("Prefabs")]
    public GameObject nodePrefab;
    public GameObject edgePrefab;

    [Header("UI - Algorithm Controls")]
    public TMP_Dropdown algorithmDropdown;
    public Button runButton;
    public Button stepButton;
    public Button resetButton;
    public Button autoPlayButton;
    public Slider speedSlider;

    [Header("UI - Info Display")]
    public TextMeshProUGUI infoText;
    public TextMeshProUGUI stepInfoText;
    public TextMeshProUGUI stepCounterText;
    public ScrollRect logScrollRect;
    public TextMeshProUGUI logText;

    [Header("UI - Graph Building")]
    public Button addNodeButton;
    public Button addEdgeButton;
    public Button loadSampleButton;
    public TMP_InputField weightInput;
    public Toggle directedToggle;

    [Header("Settings")]
    public float stepDelay = 0.8f;

    #endregion

    #region Private Fields

    // Graf verileri
    private Graph graph;
    private Dictionary<int, NodeVisual> nodeVisuals = new Dictionary<int, NodeVisual>();
    private List<EdgeVisual> edgeVisuals = new List<EdgeVisual>();

    // Algoritma durumu
    private AlgorithmResult currentResult;
    private int currentStepIndex = 0;
    private bool isAutoPlaying = false;

    // Graf olusturma modu
    private enum BuildMode { None, AddNode, AddEdgeSelectFirst, AddEdgeSelectSecond }
    private BuildMode buildMode = BuildMode.None;
    private int selectedFirstNode = -1;

    #endregion

    #region Unity Lifecycle

    private void Start()
    {
        InitializeGraph();
        SetupUI();

        if (UIManager.Instance != null)
            UIManager.Instance.Initialize();

        LoadSampleGraph();
        LogMessage("Hazir! Bir algoritma secin ve Calistir'a basin.");
    }

    private void Update()
    {
        // Mouse tiklama ile graf olusturma
        if (Input.GetMouseButtonDown(0) && buildMode != BuildMode.None)
        {
            HandleBuildClick();
        }
    }

    #endregion

    #region Initialization

    // Graf'i baslat
    private void InitializeGraph()
    {
        graph = new Graph();
    }

    // UI butonlarini bagla
    private void SetupUI()
    {
        // Algoritma dropdown'u doldur
        algorithmDropdown.ClearOptions();
        algorithmDropdown.AddOptions(new List<string>
        {
            "Dijkstra",
            "Bellman-Ford",
            "Prim (MST)",
            "Kruskal (MST)"
        });

        // Buton eventlerini bagla
        runButton.onClick.AddListener(RunSelectedAlgorithm);
        stepButton.onClick.AddListener(PlayNextStep);
        resetButton.onClick.AddListener(ResetVisualization);
        autoPlayButton.onClick.AddListener(ToggleAutoPlay);
        addNodeButton.onClick.AddListener(() => buildMode = BuildMode.AddNode);
        addEdgeButton.onClick.AddListener(() => buildMode = BuildMode.AddEdgeSelectFirst);
        loadSampleButton.onClick.AddListener(LoadSampleGraph);

        // Hiz ayari
        speedSlider.onValueChanged.AddListener(val => stepDelay = 1.5f - val);
    }

    #endregion

    #region Graph Building

    // Mouse tiklama ile graf olustur
    private void HandleBuildClick()
    {
        Vector2 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        switch (buildMode)
        {
            case BuildMode.AddNode:
                HandleAddNode(worldPos);
                break;

            case BuildMode.AddEdgeSelectFirst:
                HandleSelectFirstNode(worldPos);
                break;

            case BuildMode.AddEdgeSelectSecond:
                HandleSelectSecondNode(worldPos);
                break;
        }
    }

    // Yeni node ekle
    private void HandleAddNode(Vector2 worldPos)
    {
        graph.AddNode(worldPos);
        CreateNodeVisual(graph.nodes[graph.nodes.Count - 1]);
        buildMode = BuildMode.None;
        LogMessage($"Dugum {graph.nodes.Count - 1} eklendi.");
    }

    // Edge icin ilk node'u sec
    private void HandleSelectFirstNode(Vector2 worldPos)
    {
        int clickedNode = GetNodeAtPosition(worldPos);
        if (clickedNode >= 0)
        {
            selectedFirstNode = clickedNode;
            nodeVisuals[clickedNode].SetColor(Color.yellow);
            buildMode = BuildMode.AddEdgeSelectSecond;
            LogMessage($"Kenar baslangici: dugum {clickedNode}. Bitis dugumunu secin.");
        }
    }

    // Edge icin ikinci node'u sec ve edge olustur
    private void HandleSelectSecondNode(Vector2 worldPos)
    {
        int clickedNode = GetNodeAtPosition(worldPos);
        if (clickedNode >= 0 && clickedNode != selectedFirstNode)
        {
            float weight = GetEdgeWeight();
            graph.AddEdge(selectedFirstNode, clickedNode, weight);
            CreateEdgeVisual(graph.edges[graph.edges.Count - 1]);
            nodeVisuals[selectedFirstNode].ResetColor();
            buildMode = BuildMode.None;
            LogMessage($"Kenar eklendi: ({selectedFirstNode} -> {clickedNode}), agirlik = {weight}");
        }
    }

    // Input'tan agirlik degerini al
    private float GetEdgeWeight()
    {
        float weight = 1f;
        if (weightInput != null && float.TryParse(weightInput.text, out float parsedWeight))
            weight = parsedWeight;
        return weight;
    }

    // Belirli pozisyondaki node'u bul
    private int GetNodeAtPosition(Vector2 worldPos)
    {
        float minDist = 0.5f;
        int closestNode = -1;

        foreach (var kvp in nodeVisuals)
        {
            float dist = Vector2.Distance(worldPos, kvp.Value.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                closestNode = kvp.Key;
            }
        }
        return closestNode;
    }

    // Ornek graf yukle
    public void LoadSampleGraph()
    {
        ClearGraph();

        graph = new Graph();
        graph.isDirected = directedToggle != null && directedToggle.isOn;
        //graph.isDirected = false;

        // 6 node ekle
        graph.AddNode(new Vector2(-4, 2));    // 0
        graph.AddNode(new Vector2(-1, 3));    // 1
        graph.AddNode(new Vector2(2, 2));     // 2
        graph.AddNode(new Vector2(-3, -1));   // 3
        graph.AddNode(new Vector2(0, -2));    // 4
        graph.AddNode(new Vector2(3, -1));    // 5

        // Edgeleri ekle
        graph.AddEdge(0, 1, 4);
        graph.AddEdge(0, 3, 2);
        graph.AddEdge(1, 2, 3);
        graph.AddEdge(1, 4, 5);
        graph.AddEdge(2, 5, 1);
        graph.AddEdge(3, 4, 7);
        graph.AddEdge(4, 5, 6);
        graph.AddEdge(1, 3, 1);
        graph.AddEdge(3, 5, 8);

        // Gorselleri olustur
        foreach (var node in graph.nodes)
            CreateNodeVisual(node);

        foreach (var edge in graph.edges)
            CreateEdgeVisual(edge);

        LogMessage("Ornek graf yuklendi.");
    }

    // Tum grafi temizle
    private void ClearGraph()
    {
        foreach (var nv in nodeVisuals.Values)
            if (nv != null) Destroy(nv.gameObject);
        nodeVisuals.Clear();

        foreach (var ev in edgeVisuals)
            if (ev != null) Destroy(ev.gameObject);
        edgeVisuals.Clear();
    }

    #endregion

    #region Visual Creation

    // Node gorseli olustur
    private void CreateNodeVisual(Node node)
    {
        GameObject go = Instantiate(nodePrefab,
            new Vector3(node.position.x, node.position.y, 0),
            Quaternion.identity, transform);

        NodeVisual nv = go.GetComponent<NodeVisual>();
        nv.Initialize(node.id, node.position);

        // Pozisyon degistiginde grafi guncelle
        nv.OnPositionChanged = (id, pos) =>
        {
            graph.nodes[id].position = pos;
        };

        nodeVisuals[node.id] = nv;
    }

    // Edge gorseli olustur
    private void CreateEdgeVisual(Edge edge)
    {
        if (!nodeVisuals.ContainsKey(edge.fromNodeId) ||
            !nodeVisuals.ContainsKey(edge.toNodeId))
            return;

        GameObject go = Instantiate(edgePrefab, Vector3.zero,
            Quaternion.identity, transform);

        EdgeVisual ev = go.GetComponent<EdgeVisual>();
        ev.Initialize(
            edge.fromNodeId, edge.toNodeId, edge.weight,
            nodeVisuals[edge.fromNodeId].transform,
            nodeVisuals[edge.toNodeId].transform,
            graph.isDirected
        );

        edgeVisuals.Add(ev);
    }

    #endregion

    #region Algorithm Execution

    // Secili algoritmay calistir
    public void RunSelectedAlgorithm()
    {
        ResetVisualization();

        int selectedIndex = algorithmDropdown.value;

        switch (selectedIndex)
        {
            case 0: // Dijkstra
                currentResult = DijkstraAlgorithm.Run(graph, 0);
                break;

            case 1: // Bellman-Ford
                currentResult = BellmanFordAlgorithm.Run(graph, 0);
                break;

            case 2: // Prim
                currentResult = PrimAlgorithm.Run(graph, 0);
                break;

            case 3: // Kruskal
                currentResult = KruskalAlgorithm.Run(graph);
                break;
        }

        currentStepIndex = 0;
        UpdateStepCounter();
        LogMessage($"{currentResult.algorithmName} hazir. " +
                   $"{currentResult.steps.Count} adim olusturuldu.");
        infoText.text = currentResult.algorithmName + " - Adim adim ilerleyin";
    }

    // Sonraki adimi oynat
    public void PlayNextStep()
    {
        if (currentResult == null || currentStepIndex >= currentResult.steps.Count)
        {
            LogMessage("Algoritma tamamlandi veya henuz calistirilmadi.");
            isAutoPlaying = false;
            return;
        }

        AlgorithmStep step = currentResult.steps[currentStepIndex];
        ExecuteStep(step);
        currentStepIndex++;
        UpdateStepCounter();
    }

    #endregion

    #region Step Execution

    // Bir adimi gerceklestir
    private void ExecuteStep(AlgorithmStep step)
    {
        stepInfoText.text = step.message;
        LogMessage($"[Adim {currentStepIndex + 1}] {step.message}");

        switch (step.actionType)
        {
            case StepActionType.HighlightNode:
                ExecuteHighlightNode(step);
                break;

            case StepActionType.HighlightEdge:
                ExecuteHighlightEdge(step);
                break;

            case StepActionType.UpdateDistance:
                ExecuteUpdateDistance(step);
                break;

            case StepActionType.MarkVisited:
                ExecuteMarkVisited(step);
                break;

            case StepActionType.AddToMST:
                ExecuteAddToMST(step);
                break;

            case StepActionType.UnionSets:
                ExecuteUnionSets(step);
                break;

            case StepActionType.LogMessage:
                break;
        }
    }

    // Node'u renklendir
    private void ExecuteHighlightNode(AlgorithmStep step)
    {
        if (nodeVisuals.ContainsKey(step.nodeId))
        {
            nodeVisuals[step.nodeId].SetColor(step.color);
            nodeVisuals[step.nodeId].PulseAnimation();
        }
    }

    // Edge'i renklendir
    private void ExecuteHighlightEdge(AlgorithmStep step)
    {
        var edgeVis = FindEdgeVisual(step.nodeId, step.secondNodeId);
        if (edgeVis != null)
            edgeVis.SetHighlight(step.color);
    }

    // Mesafe etiketini guncelle
    private void ExecuteUpdateDistance(AlgorithmStep step)
    {
        if (nodeVisuals.ContainsKey(step.nodeId))
        {
            string distStr = step.value == float.MaxValue ? "∞" : step.value.ToString("F1");
            nodeVisuals[step.nodeId].SetDistanceLabel($"d={distStr}");
            nodeVisuals[step.nodeId].SetColor(new Color(1f, 0.8f, 0.3f));
            nodeVisuals[step.nodeId].PulseAnimation();
        }
    }

    // Ziyaret edildi olarak isaretle
    private void ExecuteMarkVisited(AlgorithmStep step)
    {
        if (nodeVisuals.ContainsKey(step.nodeId))
            nodeVisuals[step.nodeId].SetColor(step.color);
    }

    // MST'ye ekle
    private void ExecuteAddToMST(AlgorithmStep step)
    {
        var mstEdge = FindEdgeVisual(step.nodeId, step.secondNodeId);
        if (mstEdge != null)
            mstEdge.SetHighlight(Color.green);

        if (nodeVisuals.ContainsKey(step.nodeId))
            nodeVisuals[step.nodeId].SetColor(Color.green);

        if (nodeVisuals.ContainsKey(step.secondNodeId))
            nodeVisuals[step.secondNodeId].SetColor(Color.green);
    }

    // Setleri birlestir (Kruskal icin)
    private void ExecuteUnionSets(AlgorithmStep step)
    {
        if (nodeVisuals.ContainsKey(step.nodeId))
            nodeVisuals[step.nodeId].SetColor(Color.blue);

        if (nodeVisuals.ContainsKey(step.secondNodeId))
            nodeVisuals[step.secondNodeId].SetColor(Color.blue);
    }

    // Edge gorselini bul
    private EdgeVisual FindEdgeVisual(int from, int to)
    {
        foreach (var ev in edgeVisuals)
        {
            if (ev.Matches(from, to))
                return ev;
        }
        return null;
    }

    #endregion

    #region Auto Play

    // Otomatik oynatmayi ac/kapat
    public void ToggleAutoPlay()
    {
        isAutoPlaying = !isAutoPlaying;

        if (isAutoPlaying)
        {
            autoPlayButton.GetComponentInChildren<TextMeshProUGUI>().text = "Stop";
            StartCoroutine(AutoPlayCoroutine());
        }
        else
        {
            autoPlayButton.GetComponentInChildren<TextMeshProUGUI>().text = "Auto Play";
        }
    }

    // Otomatik oynatma coroutine
    private IEnumerator AutoPlayCoroutine()
    {
        while (isAutoPlaying && currentResult != null &&
               currentStepIndex < currentResult.steps.Count)
        {
            PlayNextStep();
            yield return new WaitForSeconds(stepDelay);
        }

        isAutoPlaying = false;
        if (autoPlayButton != null)
            autoPlayButton.GetComponentInChildren<TextMeshProUGUI>().text = "Otomatik";
    }

    #endregion

    #region Visualization Control

    // Gorsellestirmeyi sifirla
    public void ResetVisualization()
    {
        isAutoPlaying = false;
        currentStepIndex = 0;
        currentResult = null;

        // Tum node'lari resetle
        foreach (var nv in nodeVisuals.Values)
        {
            nv.ResetColor();
            nv.SetDistanceLabel("");
        }

        // Tum edge'leri resetle
        foreach (var ev in edgeVisuals)
            ev.ResetVisual();

        stepInfoText.text = "Algoritma secin ve calistirin";
        UpdateStepCounter();
    }

    // Adim sayacini guncelle
    private void UpdateStepCounter()
    {
        if (currentResult != null)
        {
            stepCounterText.text = $"Adim: {currentStepIndex} / {currentResult.steps.Count}";
        }
        else
        {
            stepCounterText.text = "Adim: - / -";
        }
    }

    #endregion

    #region Logging

    // Log mesaji yazdir
    private void LogMessage(string msg)
    {
        if (logText != null)
        {
            logText.text += msg + "\n";
            Canvas.ForceUpdateCanvases();
            if (logScrollRect != null)
                logScrollRect.verticalNormalizedPosition = 0;
        }
        Debug.Log(msg);
    }

    #endregion
}