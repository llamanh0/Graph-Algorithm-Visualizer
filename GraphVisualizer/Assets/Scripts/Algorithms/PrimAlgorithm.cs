using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Prim - Minimum Spanning Tree algoritmasi
/// Bir dugumden baslar, her adimda en ucuz kenari ekler
/// </summary>
public static class PrimAlgorithm
{
    #region Main Algorithm

    public static AlgorithmResult Run(Graph graph, int startNode = 0)
    {
        AlgorithmResult result = new AlgorithmResult();
        result.algorithmName = "Prim MST";
        result.steps = new List<AlgorithmStep>();
        result.resultEdges = new List<Edge>();
        result.totalCost = 0;

        int n = graph.nodes.Count;
        bool[] inMST = new bool[n];
        float[] minCost = new float[n];
        int[] parent = new int[n];

        InitializeArrays(n, minCost, parent, startNode);
        LogStart(result, startNode);

        // Ana dongu - her adimda bir dugum ekle
        for (int count = 0; count < n; count++)
        {
            int u = FindMinCostNode(n, inMST, minCost);

            if (u == -1)
            {
                result.steps.Add(AlgorithmStep.Log(
                    "UYARI: Erisilemeyen dugumler var (graf bagli degil)"));
                break;
            }

            AddNodeToMST(u, inMST, parent, minCost, result);
            UpdateNeighborCosts(graph, u, inMST, minCost, parent, result);
        }

        BuildResult(result, parent, minCost, n);

        return result;
    }

    #endregion

    #region Initialization

    private static void InitializeArrays(int n, float[] minCost, int[] parent, int startNode)
    {
        for (int i = 0; i < n; i++)
        {
            minCost[i] = float.MaxValue;
            parent[i] = -1;
        }
        minCost[startNode] = 0;
    }

    private static void LogStart(AlgorithmResult result, int startNode)
    {
        result.steps.Add(AlgorithmStep.Log(
            $"Prim MST Basladi - Baslangic dugum: {startNode}"));

        result.steps.Add(AlgorithmStep.HighlightNode(
            startNode, Color.green,
            $"Dugum {startNode} MST'ye eklendi (baslangic)"));
    }

    #endregion

    #region Node Selection

    private static int FindMinCostNode(int n, bool[] inMST, float[] minCost)
    {
        int minNode = -1;
        float minValue = float.MaxValue;

        for (int i = 0; i < n; i++)
        {
            if (!inMST[i] && minCost[i] < minValue)
            {
                minValue = minCost[i];
                minNode = i;
            }
        }

        return minNode;
    }

    private static void AddNodeToMST(int u, bool[] inMST, int[] parent,
        float[] minCost, AlgorithmResult result)
    {
        inMST[u] = true;

        if (parent[u] == -1)
        {
            // Baslangic dugumu
            result.steps.Add(AlgorithmStep.HighlightNode(
                u, Color.green,
                $"Baslangic dugumu {u} secildi"));
        }
        else
        {
            // Normal dugum ekleme
            result.steps.Add(AlgorithmStep.HighlightNode(
                u, Color.cyan,
                $"Dugum {u} secildi (maliyet={minCost[u]:F1}), MST'ye ekleniyor"));

            result.steps.Add(new AlgorithmStep
            {
                actionType = StepActionType.AddToMST,
                nodeId = parent[u],
                secondNodeId = u,
                value = minCost[u],
                color = Color.green,
                message = $"Kenar ({parent[u]}-{u}) MST'ye eklendi, maliyet={minCost[u]:F1}"
            });
        }
    }

    #endregion

    #region Cost Update

    private static void UpdateNeighborCosts(Graph graph, int u, bool[] inMST,
        float[] minCost, int[] parent, AlgorithmResult result)
    {
        List<Edge> edges = graph.GetEdgesFromNode(u);

        // Zaten islenmis kenarlari takip et (yonsuz grafta tekrar olmasin)
        HashSet<int> processedNeighbors = new HashSet<int>();

        foreach (Edge edge in edges)
        {
            int v = edge.toNodeId;

            // Bu komsuyu zaten islemissek atla
            if (processedNeighbors.Contains(v))
                continue;

            processedNeighbors.Add(v);

            result.steps.Add(AlgorithmStep.HighlightEdge(
                u, v, Color.yellow,
                $"Komsu dugum {v} inceleniyor (kenar agirlik={edge.weight:F1})"));

            if (inMST[v])
            {
                result.steps.Add(AlgorithmStep.HighlightEdge(
                    u, v, Color.gray,
                    $"Dugum {v} zaten MST'de, atla"));
                continue;
            }

            if (edge.weight < minCost[v])
            {
                UpdateCost(u, v, edge.weight, minCost, parent, result);
            }
            else
            {
                string currentCostStr = minCost[v] == float.MaxValue ? "sonsuz" : minCost[v].ToString("F1");
                result.steps.Add(AlgorithmStep.HighlightEdge(
                    u, v, Color.red,
                    $"Guncelleme yok: {edge.weight:F1} >= {currentCostStr}"));
            }
        }
    }

    private static void UpdateCost(int u, int v, float weight,
        float[] minCost, int[] parent, AlgorithmResult result)
    {
        float oldCost = minCost[v];
        minCost[v] = weight;
        parent[v] = u;

        string oldStr = oldCost == float.MaxValue ? "sonsuz" : oldCost.ToString("F1");

        result.steps.Add(AlgorithmStep.UpdateDist(
            v, weight, oldCost,
            $"Maliyet guncellendi: {v}: {oldStr} -> {weight:F1}"));

        result.steps.Add(AlgorithmStep.HighlightEdge(
            u, v, Color.green,
            $"Yeni en ucuz yol: ({u}-{v})"));
    }

    #endregion

    #region Result Building

    private static void BuildResult(AlgorithmResult result, int[] parent,
        float[] minCost, int n)
    {
        // MST kenarlarini olustur
        for (int i = 0; i < n; i++)
        {
            if (parent[i] != -1)
            {
                Edge mstEdge = new Edge(parent[i], i, minCost[i], false);
                result.resultEdges.Add(mstEdge);
                result.totalCost += minCost[i];
            }
        }

        string summary = $"Prim Tamamlandi!\n";
        summary += $"MST Toplam Maliyet: {result.totalCost:F1}\n";
        summary += $"MST Kenar Sayisi: {result.resultEdges.Count}\n";

        if (result.resultEdges.Count == n - 1)
        {
            summary += "Durum: Tam MST olusturuldu (graf bagli)";
        }
        else
        {
            summary += $"UYARI: Graf bagli degil! ({n - 1} yerine {result.resultEdges.Count} kenar)";
        }

        result.steps.Add(AlgorithmStep.Log(summary));
    }

    #endregion
}