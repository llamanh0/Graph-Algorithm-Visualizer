using System.Collections.Generic;
using System.Numerics;

/// <summary>
/// A core class for managing graph data structures, 
/// holding relationships between <see cref="Node"/> and <see cref="Edge"/> objects.
/// </summary>
/// <remarks>
/// <para>Responsibilities:</para>
/// <list type="number">
///     <item><description>Manages adding and removing nodes within the graph.</description></item>
///     <item><description>Creates weighted edges (connections) between nodes.</description></item>
///     <item><description>Queries neighbors and edges associated with a specific node.</description></item>
///     <item><description>Generates an Adjacency Matrix representation for pathfinding algorithms.</description></item>
/// </list>
/// </remarks>
[System.Serializable]
public class Graph
{
    public List<Node> nodes = new List<Node>();
    public List<Edge> edges = new List<Edge>();
    public bool isDirected;

    public void AddNode(Vector2 position)
    {
        int newId = nodes.Count;
        nodes.Add(new Node(newId, position));
    }

    public void AddEdge(int from, int to, float weight)
    {
        // If edge already exist, do nothing.
        if (edges.Exists(e =>
            (e.fromNodeId == from && e.toNodeId == to) ||
            (!isDirected && e.fromNodeId == to && e.toNodeId == from)))
            return;

        edges.Add(new Edge(from, to, weight, isDirected));
    }

    public void RemoveNode(int id)
    {
        nodes.RemoveAll(n => n.id == id);
        edges.RemoveAll(e => e.fromNodeId == id || e.toNodeId == id);
    }

    public List<Edge> GetEdgesFromNode(int nodeId)
    {
        List<Edge> result = new List<Edge>();
        foreach (var edge in edges)
        {
            if (edge.fromNodeId == nodeId)
                result.Add(edge);
            if (!isDirected && edge.toNodeId == nodeId)
                result.Add(new Edge(edge.toNodeId, edge.fromNodeId, edge.weight));
        }
        return result;
    }

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

    /// <returns>
    /// _________ 2D Array _________
    /// <code>
    /// |     A, B, C ...  |
    /// |  A {0, ∞, ∞}     |
    /// |  B {∞, 0, ∞}     |
    /// |  C {∞, ∞, 0}     |
    /// |  .          .    |
    /// |  .            .  |
    /// |  .              .|
    /// </code>
    /// </returns>
    public float[,] GetAdjacencyMatrix()
    {
        int n = nodes.Count;
        float[,] matrix = new float[n, n];

        // At first, every distance is infinitive
        for (int i = 0; i < n; i++)
            for (int j = 0; j < n; j++)
                matrix[i, j] = float.MaxValue;

        // Distance of itself is zero
        for (int i = 0; i < n; i++)
            matrix[i, i] = 0;

        // Place the distances
        foreach (var edge in edges)
        {
            matrix[edge.fromNodeId, edge.toNodeId] = edge.weight;
            if (!isDirected)
                matrix[edge.toNodeId, edge.fromNodeId] = edge.weight;
        }

        return matrix;
    }
}