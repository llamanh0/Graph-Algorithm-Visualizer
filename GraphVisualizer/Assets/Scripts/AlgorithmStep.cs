using UnityEngine;

/// <summary>
/// Represents a single discrete step within a graph algorithm for visualization and logging.
/// </summary>
/// <remarks>
/// <para>Key Features:</para>
/// <list type="bullet">
///     <item><description>Stores the <see cref="StepActionType"/> to trigger visual changes (colors, labels).</description></item>
///     <item><description>Holds metadata like <see cref="nodeId"/> and <see cref="value"/> for UI updates.</description></item>
///     <item><description>Provides <c>static factory methods</c> for clean and readable step creation.</description></item>
/// </list>
/// </remarks>
[System.Serializable]
public class AlgorithmStep
{
    public StepActionType actionType;
    public int nodeId = -1;
    public int secondNodeId = -1;
    public int edgeIndex = -1;
    public float value;
    public float previousValue;
    public string message;
    public Color color = Color.yellow;

    public static AlgorithmStep HighlightNode(int nodeId, Color color, string msg = "")
    {
        return new AlgorithmStep
        {
            actionType = StepActionType.HighlightNode,
            nodeId = nodeId,
            color = color,
            message = msg
        };
    }

    public static AlgorithmStep HighlightEdge(int from, int to, Color color, string msg = "")
    {
        return new AlgorithmStep
        {
            actionType = StepActionType.HighlightEdge,
            nodeId = from,
            secondNodeId = to,
            color = color,
            message = msg
        };
    }

    public static AlgorithmStep UpdateDist(int nodeId, float newDist, float oldDist, string msg = "")
    {
        return new AlgorithmStep
        {
            actionType = StepActionType.UpdateDistance,
            nodeId = nodeId,
            value = newDist,
            previousValue = oldDist,
            message = msg
        };
    }

    public static AlgorithmStep Log(string message)
    {
        return new AlgorithmStep
        {
            actionType = StepActionType.LogMessage,
            message = message
        };
    }
}