using UnityEngine;

/// <summary>
/// Algoritmanin her adimini gosteren sinif
/// Gorsel degisiklikleri ve mesajlari tutar
/// </summary>
[System.Serializable]
public class AlgorithmStep
{
    #region Fields

    public StepActionType actionType;
    public int nodeId = -1;
    public int secondNodeId = -1;
    public int edgeIndex = -1;
    public float value;
    public float previousValue;
    public string message;
    public Color color = Color.yellow;

    #endregion

    #region Factory Methods

    // Node'u renklendir
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

    // Edge'i renklendir
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

    // Mesafe guncelle
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

    // Sadece mesaj yaz
    public static AlgorithmStep Log(string message)
    {
        return new AlgorithmStep
        {
            actionType = StepActionType.LogMessage,
            message = message
        };
    }

    #endregion
}
