using UnityEngine;

/// <summary>
/// Graf uzerindeki bir nokta (dugum)
/// </summary>
[System.Serializable]
public class Node
{
    #region Fields

    public int id;
    public Vector2 position;    // 2D pozisyon
    public string label;        // Ekranda gorunecek etiket

    #endregion

    #region Constructor

    public Node(int id, Vector2 position)
    {
        this.id = id;
        this.position = position;
        this.label = id.ToString();
    }

    #endregion
}