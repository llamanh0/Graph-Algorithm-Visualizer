/// <summary>
/// Algoritma adim turleri
/// Her adim tipi farkli bir gorsel degisiklik yapar
/// </summary>
public enum StepActionType
{
    HighlightNode,          // Node'u renklendir
    HighlightEdge,          // Edge'i renklendir
    UpdateDistance,         // Mesafe etiketini guncelle
    MarkVisited,            // Ziyaret edildi olarak isaretle
    MarkAsPartOfResult,     // Sonucun parcasi olarak isaretle
    CompareEdge,            // Edgeleri karsilastir
    RelaxEdge,              // Edge relaxation
    UnionSets,              // Kruskal - setleri birlestir
    AddToMST,               // MST'ye ekle
    LogMessage              // Sadece mesaj
}