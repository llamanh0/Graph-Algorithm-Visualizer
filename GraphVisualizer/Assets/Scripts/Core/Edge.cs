/// <summary>
/// <para>Edge of between two nodes</para>
/// <para>It has;</para>
/// <list type="bullet">
///     <item>
///      Mandatory:
///         <list type="number">
///             <item>(int) before and after nodeIDs: <see cref="fromNodeId"/>, <see cref="toNodeId"/></item>
///             <item>(float) <see cref="weight"/></item>
///         </list>
///     </item>
///     <item>
///     Non-mandatory:
///         <list type="number">
///             <item>(bool) <see cref="isDirected"/>, default value is <c>false</c></item>
///         </list>
///     </item>
/// </list>
/// </summary>
[System.Serializable]
public class Edge
{
    public int fromNodeId;
    public int toNodeId;
    public float weight;
    public bool isDirected;

    public Edge(int from, int to, float weight, bool isDirected = false)
    {
        this.fromNodeId = from;
        this.toNodeId = to;
        this.weight = weight;
        this.isDirected = isDirected;
    }
}