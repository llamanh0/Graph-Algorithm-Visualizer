using System.Collections.Generic;

/// <summary>
/// Algoritma sonuclarini tutan sinif
/// </summary>
public class AlgorithmResult
{
    #region Fields

    public string algorithmName;
    public List<AlgorithmStep> steps = new List<AlgorithmStep>();

    // Dijkstra ve Bellman-Ford icin
    public float[] distances;
    public int[] predecessors;

    // MST algoritmalari icin (Prim, Kruskal)
    public List<Edge> resultEdges;
    public float totalCost;

    // Bellman-Ford icin
    public bool hasNegativeCycle;

    #endregion
}