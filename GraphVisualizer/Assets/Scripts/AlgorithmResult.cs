using System.Collections.Generic;

public class AlgorithmResult
{
    public string algorithmName;
    public List<AlgorithmStep> steps = new List<AlgorithmStep>();

    public float[] distances;           // Dijkstra, Bellman-Ford
    public int[] predecessors;          // For road backtracking
    public List<Edge> resultEdges;      // MST edges (Prim, Kruskal)
    public float totalCost;
    public bool hasNegativeCycle;       // Bellman-Ford
}