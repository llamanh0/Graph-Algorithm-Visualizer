using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Bellman-Ford - En kisa yol algoritmasi
/// Negatif agirlikli kenarlarda calisir, negatif dongu tespiti yapar
/// </summary>
public static class BellmanFordAlgorithm
{
    #region Main Algorithm

    public static AlgorithmResult Run(Graph graph, int sourceId)
    {
        AlgorithmResult result = new AlgorithmResult();
        result.algorithmName = "Bellman-Ford";
        result.steps = new List<AlgorithmStep>();

        int n = graph.nodes.Count;
        float[] dist = new float[n];
        int[] prev = new int[n];

        InitializeDistances(n, dist, prev, sourceId);
        LogStart(result, sourceId);

        // Ana dongu - tum kenarlari V-1 kez relax et
        for (int iteration = 0; iteration < n - 1; iteration++)
        {
            result.steps.Add(AlgorithmStep.Log(
                $"Iterasyon {iteration + 1}/{n - 1} basladi"));

            bool anyUpdate = RelaxAllEdges(graph, dist, prev, result);

            if (!anyUpdate)
            {
                result.steps.Add(AlgorithmStep.Log(
                    "Hicbir guncelleme yapilmadi, erken cikis"));
                break;
            }
        }

        // Negatif dongu kontrolu
        bool hasNegCycle = CheckNegativeCycle(graph, dist, result);

        BuildFinalResult(result, dist, prev, hasNegCycle, n);

        return result;
    }

    #endregion

    #region Initialization

    private static void InitializeDistances(int n, float[] dist, int[] prev, int sourceId)
    {
        for (int i = 0; i < n; i++)
        {
            dist[i] = float.MaxValue;
            prev[i] = -1;
        }
        dist[sourceId] = 0;
    }

    private static void LogStart(AlgorithmResult result, int sourceId)
    {
        result.steps.Add(AlgorithmStep.Log(
            $"Bellman-Ford Basladi - Kaynak: {sourceId}"));
        result.steps.Add(AlgorithmStep.HighlightNode(
            sourceId, Color.green, $"Kaynak dugum {sourceId}, mesafe = 0"));
    }

    #endregion

    #region Edge Relaxation

    private static bool RelaxAllEdges(Graph graph, float[] dist, int[] prev, AlgorithmResult result)
    {
        bool updated = false;

        // Yonlu grafta: her kenari oldugu gibi isle
        // Yonsuz grafta: her kenari iki yon icin isle ama gorsellesitirmeyi bir kez yap
        foreach (Edge edge in graph.edges)
        {
            int u = edge.fromNodeId;
            int v = edge.toNodeId;
            float w = edge.weight;

            // Ilk yon: u -> v
            if (TryRelaxEdge(u, v, w, dist, prev, result, true))
                updated = true;

            // Yonsuz grafta ters yon: v -> u
            if (!graph.isDirected)
            {
                if (TryRelaxEdge(v, u, w, dist, prev, result, false))
                    updated = true;
            }
        }

        return updated;
    }

    private static bool TryRelaxEdge(int u, int v, float weight,
        float[] dist, int[] prev, AlgorithmResult result, bool visualize)
    {
        // Kaynak node'a henuz erisilmediyse atla
        if (dist[u] == float.MaxValue)
            return false;

        float newDist = dist[u] + weight;

        // Iyilestirme yoksa atla
        if (newDist >= dist[v])
        {
            // Sadece ana yonde gorsellesir (gereksiz tekrar olmasin)
            if (visualize)
            {
                result.steps.Add(AlgorithmStep.HighlightEdge(
                    u, v, Color.gray,
                    $"({u}->{v}) iyilestirme yok: {newDist:F1} >= {dist[v]:F1}"));
            }
            return false;
        }

        // Iyilestirme var, uygula
        PerformRelaxation(u, v, newDist, weight, dist, prev, result, visualize);
        return true;
    }

    private static void PerformRelaxation(int u, int v, float newDist, float weight,
        float[] dist, int[] prev, AlgorithmResult result, bool visualize)
    {
        float oldDist = dist[v];
        dist[v] = newDist;
        prev[v] = u;

        if (visualize)
        {
            string oldStr = oldDist == float.MaxValue ? "sonsuz" : oldDist.ToString("F1");

            result.steps.Add(AlgorithmStep.HighlightEdge(
                u, v, Color.yellow,
                $"Kenar ({u}->{v}) inceleniyor, w={weight:F1}"));

            result.steps.Add(AlgorithmStep.UpdateDist(
                v, newDist, oldDist,
                $"RELAX: Dugum {v}: {oldStr} -> {newDist:F1} (uzerinden {u})"));

            result.steps.Add(AlgorithmStep.HighlightEdge(
                u, v, Color.green,
                $"Kenar ({u}->{v}) relax edildi"));
        }
    }

    #endregion

    #region Negative Cycle Detection

    private static bool CheckNegativeCycle(Graph graph, float[] dist, AlgorithmResult result)
    {
        result.steps.Add(AlgorithmStep.Log(
            "Negatif dongu kontrolu yapiliyor..."));

        foreach (Edge edge in graph.edges)
        {
            if (IsNegativeCycleEdge(edge.fromNodeId, edge.toNodeId, edge.weight, dist, result))
                return true;

            if (!graph.isDirected)
            {
                if (IsNegativeCycleEdge(edge.toNodeId, edge.fromNodeId, edge.weight, dist, result))
                    return true;
            }
        }

        result.steps.Add(AlgorithmStep.Log("Negatif dongu bulunamadi"));
        return false;
    }

    private static bool IsNegativeCycleEdge(int u, int v, float weight,
        float[] dist, AlgorithmResult result)
    {
        if (dist[u] == float.MaxValue) return false;

        float newDist = dist[u] + weight;

        if (newDist < dist[v])
        {
            result.steps.Add(AlgorithmStep.HighlightEdge(
                u, v, new Color(1f, 0f, 0f),
                $"NEGATIF DONGU BULUNDU! Kenar ({u}->{v})"));
            return true;
        }

        return false;
    }

    #endregion

    #region Result Building

    private static void BuildFinalResult(AlgorithmResult result, float[] dist,
        int[] prev, bool hasNegCycle, int n)
    {
        result.distances = dist;
        result.predecessors = prev;
        result.hasNegativeCycle = hasNegCycle;

        if (hasNegCycle)
        {
            result.steps.Add(AlgorithmStep.Log(
                "UYARI: Grafta negatif dongu var! Mesafeler gecersiz."));
        }
        else
        {
            result.steps.Add(AlgorithmStep.Log("Bellman-Ford Tamamlandi!"));
        }
    }

    #endregion
}