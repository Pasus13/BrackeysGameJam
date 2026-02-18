using UnityEngine;

[CreateAssetMenu(fileName = "New Tile Data", menuName = "Tile System/Tile Data")]
public class TileData : ScriptableObject
{
    [Header("Tile Properties")]
    public TileType tileType;
    public bool isLocked;
    public bool isWalkable = true;
    public GameObject prefab;

    [Header("Visual")]
    public Color highlightColor = new Color(1f, 0.8f, 0.3f, 1f);
}
