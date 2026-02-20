using UnityEngine;

[CreateAssetMenu(fileName = "New Tile Data", menuName = "Tile System/Tile Data")]
public class TileData : ScriptableObject
{
    [Header("Tile Properties")]
    public TileType tileType;
    public bool isLocked;
    public bool isWalkable = true;
    public bool isNotJumpable = false;
    public GameObject prefab;

    [Header("Visual")]
    public Material material;
    public Color highlightColor = new Color(1f, 0.8f, 0.3f, 1f);
    
    [Header("Jump Settings")]
    [Tooltip("How many tiles to jump over when landing (0 = no jump, 1 = jump 1 skip 1)")]
    public int jumpDistance = 1;
    
    [Header("Teleport Settings")]
    [Tooltip("ID to pair teleport tiles. Tiles with same ID are connected (0 = not a teleport)")]
    public int teleportID = 0;
    [Tooltip("Direction the character exits when teleporting to this tile (relative to tile rotation)")]
    public Vector2Int exitDirection = Vector2Int.right;
    
    public bool IsSelectable()
    {
        return tileType != TileType.StartingTile && 
               tileType != TileType.GoalTile && 
               tileType != TileType.Locked;
    }
    
    public bool IsMovable()
    {
        return !isLocked && 
               tileType != TileType.StartingTile && 
               tileType != TileType.GoalTile;
    }
}
