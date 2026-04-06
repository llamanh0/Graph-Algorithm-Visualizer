/// <summary>
/// Iki node arasindaki baglanti (kenar)
/// Agirlik ve yon bilgisi tutar
/// </summary>
[System.Serializable]
public class Edge
{
    #region Fields

    public int fromNodeId;      // Baslangic node
    public int toNodeId;        // Bitis node
    public float weight;        // Agirligi (maliyeti)
    public bool isDirected;     // Yonlu mu?

    #endregion

    #region Constructor

    public Edge(int from, int to, float weight, bool isDirected = false)
    {
        this.fromNodeId = from;
        this.toNodeId = to;
        this.weight = weight;
        this.isDirected = isDirected;
    }

    #endregion
}