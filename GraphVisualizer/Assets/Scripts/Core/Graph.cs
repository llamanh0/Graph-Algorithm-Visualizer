using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Graf veri yapisini yoneten ana sinif
/// Node ve Edge iliskilerini tutar
/// </summary>
[System.Serializable]
public class Graph
{
    #region Fields

    public List<Node> nodes = new List<Node>();
    public List<Edge> edges = new List<Edge>();
    public bool isDirected;  // Yonlu graf mi?

    #endregion

    #region Node Operations

    // Grafa yeni node ekle
    public void AddNode(Vector2 position)
    {
        int newId = nodes.Count;
        nodes.Add(new Node(newId, position));
    }

    // Node'u graftan sil
    public void RemoveNode(int id)
    {
        nodes.RemoveAll(n => n.id == id);
        edges.RemoveAll(e => e.fromNodeId == id || e.toNodeId == id);
    }

    #endregion

    #region Edge Operations

    // Grafa yeni edge ekle
    public void AddEdge(int from, int to, float weight)
    {
        // Ayni edge zaten varsa ekleme
        if (edges.Exists(e =>
            (e.fromNodeId == from && e.toNodeId == to) ||
            (!isDirected && e.fromNodeId == to && e.toNodeId == from)))
            return;

        edges.Add(new Edge(from, to, weight, isDirected));
    }

    // Belirli bir node'dan cikan edgeleri al
    public List<Edge> GetEdgesFromNode(int nodeId)
    {
        List<Edge> result = new List<Edge>();

        foreach (var edge in edges)
        {
            if (edge.fromNodeId == nodeId)
                result.Add(edge);

            // Yonsuz grafta ters yonden de ekle
            if (!isDirected && edge.toNodeId == nodeId)
                result.Add(new Edge(edge.toNodeId, edge.fromNodeId, edge.weight));
        }

        return result;
    }

    #endregion

    #region Neighbor Operations

    // Bir node'un komsularini al
    public List<int> GetNeighbors(int nodeId)
    {
        List<int> neighbors = new List<int>();

        foreach (var edge in edges)
        {
            if (edge.fromNodeId == nodeId)
                neighbors.Add(edge.toNodeId);

            if (!isDirected && edge.toNodeId == nodeId)
                neighbors.Add(edge.fromNodeId);
        }

        return neighbors;
    }

    #endregion

    #region Matrix Operations

    /// <summary>
    /// Adjacency Matrix olustur
    /// [i,j] = i'den j'ye olan mesafe
    /// </summary>
    public float[,] GetAdjacencyMatrix()
    {
        int n = nodes.Count;
        float[,] matrix = new float[n, n];

        // Once her seyi sonsuz yap
        for (int i = 0; i < n; i++)
            for (int j = 0; j < n; j++)
                matrix[i, j] = float.MaxValue;

        // Kendi kendine mesafe 0
        for (int i = 0; i < n; i++)
            matrix[i, i] = 0;

        // Edge mesafelerini yerleştir
        foreach (var edge in edges)
        {
            matrix[edge.fromNodeId, edge.toNodeId] = edge.weight;

            if (!isDirected)
                matrix[edge.toNodeId, edge.fromNodeId] = edge.weight;
        }

        return matrix;
    }

    #endregion
}
