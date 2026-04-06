using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Dijkstra - En kisa yol bulma algoritmasi
/// Negatif agirlik olmayan graflar icin
/// </summary>
public static class DijkstraAlgorithm
{
    #region Main Algorithm

    public static AlgorithmResult Run(Graph graph, int sourceId)
    {
        AlgorithmResult result = new AlgorithmResult();
        result.algorithmName = "Dijkstra";
        result.steps = new List<AlgorithmStep>();

        int n = graph.nodes.Count;

        float[] dist = new float[n];
        int[] prev = new int[n];
        bool[] visited = new bool[n];

        InitializeArrays(n, dist, prev, visited, sourceId);
        LogInitialState(result, sourceId);

        // Ana dongu
        for (int iteration = 0; iteration < n; iteration++)
        {
            int u = FindMinDistanceNode(n, dist, visited);

            if (u == -1) break;

            visited[u] = true;
            result.steps.Add(AlgorithmStep.HighlightNode(
                u, Color.cyan,
                $"Dugum {u} secildi (mesafe = {dist[u]:F1}), ziyaret edildi olarak isaretlendi"));

            ProcessNeighbors(graph, u, dist, prev, visited, result);
            MarkNodeAsCompleted(u, result);
        }

        FinalizeResult(result, dist, prev, n);

        return result;
    }

    #endregion

    #region Helper Methods

    private static void InitializeArrays(int n, float[] dist, int[] prev, bool[] visited, int sourceId)
    {
        for (int i = 0; i < n; i++)
        {
            dist[i] = float.MaxValue;
            prev[i] = -1;
            visited[i] = false;
        }
        dist[sourceId] = 0;
    }

    private static void LogInitialState(AlgorithmResult result, int sourceId)
    {
        result.steps.Add(AlgorithmStep.Log(
            $"Baslangic: Kaynak dugum = {sourceId}, tum mesafeler = sonsuz"));
        result.steps.Add(AlgorithmStep.HighlightNode(
            sourceId, Color.green, $"Kaynak dugum {sourceId} secildi, mesafe = 0"));
    }

    private static int FindMinDistanceNode(int n, float[] dist, bool[] visited)
    {
        int minNode = -1;
        float minDist = float.MaxValue;

        for (int i = 0; i < n; i++)
        {
            if (!visited[i] && dist[i] < minDist)
            {
                minDist = dist[i];
                minNode = i;
            }
        }

        return minNode;
    }

    private static void ProcessNeighbors(Graph graph, int u, float[] dist, int[] prev,
        bool[] visited, AlgorithmResult result)
    {
        List<Edge> neighbors = graph.GetEdgesFromNode(u);

        foreach (var edge in neighbors)
        {
            int v = edge.toNodeId;

            // HER KENARI ONCE GORSELLESTIR
            result.steps.Add(AlgorithmStep.HighlightEdge(
                u, v, Color.yellow,
                $"Kenar ({u} -> {v}) inceleniyor, agirlik = {edge.weight}"));

            // Eger zaten ziyaret edildiyse
            if (visited[v])
            {
                result.steps.Add(AlgorithmStep.HighlightEdge(
                    u, v, Color.gray,
                    $"Dugum {v} zaten ziyaret edildi (mesafe kesinlesmis), bu kenar atlanacak"));
                continue;
            }

            float newDist = dist[u] + edge.weight;

            if (newDist < dist[v])
            {
                RelaxEdge(u, v, newDist, dist, prev, result);
            }
            else
            {
                result.steps.Add(AlgorithmStep.HighlightEdge(
                    u, v, Color.red,
                    $"Relaxation gerekli degil: {newDist:F1} >= {dist[v]:F1}"));
            }
        }
    }

    private static void RelaxEdge(int u, int v, float newDist, float[] dist, int[] prev,
        AlgorithmResult result)
    {
        float oldDist = dist[v];
        dist[v] = newDist;
        prev[v] = u;

        string oldDistStr = oldDist == float.MaxValue ? "sonsuz" : oldDist.ToString("F1");
        result.steps.Add(AlgorithmStep.UpdateDist(
            v, newDist, oldDist,
            $"Mesafe guncellendi: dugum {v}: {oldDistStr} -> {newDist:F1} (uzerinden: {u})"));

        result.steps.Add(AlgorithmStep.HighlightEdge(
            u, v, Color.green,
            $"Kenar ({u} -> {v}) relaxation basarili!"));
    }

    private static void MarkNodeAsCompleted(int u, AlgorithmResult result)
    {
        result.steps.Add(new AlgorithmStep
        {
            actionType = StepActionType.MarkVisited,
            nodeId = u,
            color = Color.gray,
            message = $"Dugum {u} tamamlandi (mesafesi kesinlesti)"
        });
    }

    private static void FinalizeResult(AlgorithmResult result, float[] dist, int[] prev, int n)
    {
        result.distances = dist;
        result.predecessors = prev;

        string summary = "Dijkstra Tamamlandi!\nMesafeler: ";
        for (int i = 0; i < n; i++)
        {
            string d = dist[i] == float.MaxValue ? "sonsuz" : dist[i].ToString("F1");
            summary += $"\n  Dugum {i}: {d}";
        }
        result.steps.Add(AlgorithmStep.Log(summary));
    }

    #endregion

    #region Path Reconstruction

    public static List<int> GetPath(int[] predecessors, int target)
    {
        List<int> path = new List<int>();
        int current = target;

        while (current != -1)
        {
            path.Insert(0, current);
            current = predecessors[current];
        }

        return path;
    }

    #endregion
}