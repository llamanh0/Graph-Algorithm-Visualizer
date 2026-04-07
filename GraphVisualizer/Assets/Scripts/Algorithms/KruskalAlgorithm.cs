using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Kruskal - Minimum Spanning Tree algoritmasi
/// Kenarlari siralar ve Union-Find ile dongu kontrolu yapar
/// </summary>
public static class KruskalAlgorithm
{
    #region Main Algorithm

    public static AlgorithmResult Run(Graph graph)
    {
        AlgorithmResult result = new AlgorithmResult();
        result.algorithmName = "Kruskal MST";
        result.steps = new List<AlgorithmStep>();
        result.resultEdges = new List<Edge>();
        result.totalCost = 0;

        int n = graph.nodes.Count;
        UnionFind uf = new UnionFind(n);

        List<Edge> sortedEdges = GetSortedUniqueEdges(graph, result);

        LogStart(result, sortedEdges.Count);

        ProcessEdges(sortedEdges, uf, result, n);

        FinalizeMST(result, n);

        return result;
    }

    #endregion

    #region Edge Sorting

    private static List<Edge> GetSortedUniqueEdges(Graph graph, AlgorithmResult result)
    {
        HashSet<string> processedEdges = new HashSet<string>();
        List<Edge> uniqueEdges = new List<Edge>();

        foreach (Edge e in graph.edges)
        {
            // Yonsuz grafta ayni kenari bir kez isle
            string edgeKey = GetEdgeKey(e.fromNodeId, e.toNodeId, graph.isDirected);

            if (!processedEdges.Contains(edgeKey))
            {
                uniqueEdges.Add(e);
                processedEdges.Add(edgeKey);
            }
        }

        // Agirliga gore sirala
        uniqueEdges = uniqueEdges.OrderBy(e => e.weight).ToList();

        result.steps.Add(AlgorithmStep.Log(
            $"{uniqueEdges.Count} kenar agirliga gore siralandilar"));

        return uniqueEdges;
    }

    private static string GetEdgeKey(int from, int to, bool isDirected)
    {
        if (isDirected)
        {
            return $"{from}-{to}";
        }
        else
        {
            // Yonsuz grafta kucuk id'yi once yaz (tekrar olmasin)
            int a = Mathf.Min(from, to);
            int b = Mathf.Max(from, to);
            return $"{a}-{b}";
        }
    }

    #endregion

    #region Edge Processing

    private static void ProcessEdges(List<Edge> edges, UnionFind uf, AlgorithmResult result, int n)
    {
        int edgeCount = 0;
        int maxEdges = n - 1; // MST icin gerekli kenar sayisi

        foreach (Edge edge in edges)
        {
            // MST tamamlandiysa dur
            if (edgeCount >= maxEdges)
            {
                result.steps.Add(AlgorithmStep.Log(
                    $"MST tamamlandi ({maxEdges} kenar), kalan kenarlar atlanacak"));
                break;
            }

            int u = edge.fromNodeId;
            int v = edge.toNodeId;

            result.steps.Add(AlgorithmStep.HighlightEdge(
                u, v, Color.yellow,
                $"Kenar ({u}-{v}) inceleniyor, agirlik={edge.weight:F1}"));

            int setU = uf.Find(u);
            int setV = uf.Find(v);

            if (setU == setV)
            {
                // Dongu olusturur, ekleme
                result.steps.Add(AlgorithmStep.HighlightEdge(
                    u, v, Color.red,
                    $"ATLA: Dongu olusturur (ayni set: {setU})"));
            }
            else
            {
                // MST'ye ekle
                AddToMST(edge, uf, result);
                edgeCount++;
            }
        }

        result.steps.Add(AlgorithmStep.Log(
            $"MST'ye toplam {edgeCount} kenar eklendi"));
    }

    private static void AddToMST(Edge edge, UnionFind uf, AlgorithmResult result)
    {
        int u = edge.fromNodeId;
        int v = edge.toNodeId;

        uf.Union(u, v);
        result.resultEdges.Add(edge);
        result.totalCost += edge.weight;

        result.steps.Add(AlgorithmStep.HighlightEdge(
            u, v, Color.green,
            $"KABUL: MST'ye eklendi! (agirlik={edge.weight:F1})"));

        result.steps.Add(new AlgorithmStep
        {
            actionType = StepActionType.AddToMST,
            nodeId = u,
            secondNodeId = v,
            color = Color.green,
            message = $"Set birlestirme: {u} <-> {v}"
        });
    }

    #endregion

    #region Finalization

    private static void LogStart(AlgorithmResult result, int edgeCount)
    {
        result.steps.Add(AlgorithmStep.Log(
            $"Kruskal MST Basladi - {edgeCount} kenar kontrol edilecek"));
    }

    private static void FinalizeMST(AlgorithmResult result, int nodeCount)
    {
        string summary = $"Kruskal Tamamlandi!\n";
        summary += $"MST Toplam Maliyet: {result.totalCost:F1}\n";
        summary += $"MST Kenar Sayisi: {result.resultEdges.Count}\n";

        if (result.resultEdges.Count == nodeCount - 1)
        {
            summary += "Durum: Tam MST olusturuldu (graf bagli)";
        }
        else
        {
            summary += $"UYARI: Graf bagli degil! ({nodeCount - 1} yerine {result.resultEdges.Count} kenar)";
        }

        result.steps.Add(AlgorithmStep.Log(summary));
    }

    #endregion

    #region Union-Find Data Structure

    private class UnionFind
    {
        private int[] parent;
        private int[] rank;

        public UnionFind(int size)
        {
            parent = new int[size];
            rank = new int[size];

            for (int i = 0; i < size; i++)
            {
                parent[i] = i;
                rank[i] = 0;
            }
        }

        public int Find(int x)
        {
            if (parent[x] != x)
                parent[x] = Find(parent[x]); // Path compression

            return parent[x];
        }

        public void Union(int x, int y)
        {
            int rootX = Find(x);
            int rootY = Find(y);

            if (rootX == rootY) return;

            // Union by rank
            if (rank[rootX] < rank[rootY])
            {
                parent[rootX] = rootY;
            }
            else if (rank[rootX] > rank[rootY])
            {
                parent[rootY] = rootX;
            }
            else
            {
                parent[rootY] = rootX;
                rank[rootX]++;
            }
        }
    }

    #endregion
}