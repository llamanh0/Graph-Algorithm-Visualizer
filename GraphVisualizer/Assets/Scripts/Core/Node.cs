using System.Numerics;

/// <summary>
/// <para>Node :)</para>
/// <para>It has;</para>
/// <list type="bullet">
///     <item>(int) <see cref="id"/></item>
///     <item>(Vector2) <see cref="position"/></item>
///     <item>(string) <see cref="label"/> => basically <term>string of <see cref="id"/></term></item>
/// </list>
/// </summary>
[System.Serializable]
public class Node
{
    public int id;
    public Vector2 position;
    public string label;

    public Node(int id, Vector2 position)
    {
        this.id = id;
        this.position = position;
        this.label = id.ToString();
    }
}