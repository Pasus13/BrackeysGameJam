using UnityEngine;

public enum TileEffectResult
{
    Continue,
    Win,
    Fail,
    Teleport,
    Jump
}

public static class TileEffectResolver
{
    public static TileEffectResult Resolve(TileData tileData, TileGrid tileGrid, ref Vector2Int currentDirection, Vector2Int currentPosition)
    {
        TileType tileType = tileData.tileType;

        switch (tileType)
        {
            case TileType.Normal:
                return TileEffectResult.Continue;

            case TileType.GoalTile:
                return TileEffectResult.Win;

            case TileType.Block:
                return TileEffectResult.Fail;

            case TileType.Empty:
                return TileEffectResult.Fail;

            case TileType.StartingTile:
                return TileEffectResult.Continue;

            case TileType.Locked:
                Debug.LogWarning($"[TileEffectResolver] TileType 'Locked' not yet implemented - treating as Normal");
                return TileEffectResult.Continue;

            case TileType.Portal:
                Debug.LogWarning($"[TileEffectResolver] TileType 'Portal' not yet implemented - treating as Normal");
                return TileEffectResult.Continue;

            case TileType.Teleport:
                return TileEffectResult.Teleport;

            case TileType.Rotate90Left:
                currentDirection = RotateDirectionLeft(currentDirection);
                return TileEffectResult.Continue;

            case TileType.Rotate90Right:
                currentDirection = RotateDirectionRight(currentDirection);
                return TileEffectResult.Continue;

            case TileType.Rotate180:
                currentDirection = RotateDirection180(currentDirection);
                return TileEffectResult.Continue;

            case TileType.JumpForward:
                return HandleJumpForward(tileData, tileGrid, currentDirection, currentPosition);

            case TileType.JumpVertical:
                Debug.LogWarning($"[TileEffectResolver] TileType 'JumpVertical' not yet implemented - treating as Normal");
                return TileEffectResult.Continue;

            case TileType.SpeedUp:
                Debug.LogWarning($"[TileEffectResolver] TileType 'SpeedUp' not yet implemented - treating as Normal");
                return TileEffectResult.Continue;

            case TileType.Trigger:
                Debug.LogWarning($"[TileEffectResolver] TileType 'Trigger' not yet implemented - treating as Normal");
                return TileEffectResult.Continue;

            case TileType.Door:
                Debug.LogWarning($"[TileEffectResolver] TileType 'Door' not yet implemented - treating as Normal");
                return TileEffectResult.Continue;

            default:
                Debug.LogWarning($"[TileEffectResolver] TileType '{tileType}' not recognized - treating as Normal");
                return TileEffectResult.Continue;
        }
    }

    private static Vector2Int RotateDirectionLeft(Vector2Int direction)
    {
        return new Vector2Int(-direction.y, direction.x);
    }

    private static Vector2Int RotateDirectionRight(Vector2Int direction)
    {
        return new Vector2Int(direction.y, -direction.x);
    }

    private static Vector2Int RotateDirection180(Vector2Int direction)
    {
        return new Vector2Int(-direction.x, -direction.y);
    }

    public static bool FindTeleportPair(
    TileGrid tileGrid,
    int teleportID,
    Vector2Int currentPosition,
    out Vector2Int pairPosition,
    out Vector2Int exitDirection)

    {
        pairPosition = Vector2Int.zero;
        exitDirection = Vector2Int.right;

        if (teleportID == 0)
        {
            Debug.LogWarning("[TileEffectResolver] Teleport ID is 0, no pairing possible");
            return false;
        }

        var allTiles = tileGrid.GetAllTiles();

        foreach (var kvp in allTiles)
        {
            Vector2Int pos = kvp.Key;

            if (pos == currentPosition)
                continue;

            TileData tileData = tileGrid.GetTileData(pos);
            if (tileData != null)
            {
                if (tileData.tileType == TileType.Teleport && tileData.teleportID == teleportID)
                {
                    pairPosition = pos;
                    exitDirection = tileData.exitDirection;
                    return true;
                }
            }
        }

        Debug.LogWarning($"[TileEffectResolver] No teleport pair found for ID {teleportID}");
        return false;
    }

    private static TileEffectResult HandleJumpForward(TileData jumpTileData, TileGrid tileGrid, Vector2Int currentDirection, Vector2Int currentPosition)
    {
        int jumpDistance = jumpTileData.jumpDistance;

        Vector2Int intermediatePosition = currentPosition + currentDirection;
        Vector2Int landingPosition = currentPosition + (currentDirection * (jumpDistance + 1));

        TileData intermediateTileData = tileGrid.GetTileData(intermediatePosition);
        if (intermediateTileData != null && intermediateTileData.isNotJumpable)
        {
            Debug.LogWarning($"[TileEffectResolver] Cannot jump - intermediate tile at {intermediatePosition} is not jumpable!");
            return TileEffectResult.Fail;
        }

        TileData landingTileData = tileGrid.GetTileData(landingPosition);
        if (landingTileData == null)
        {
            Debug.LogWarning($"[TileEffectResolver] Cannot jump - no landing tile at {landingPosition}!");
            return TileEffectResult.Fail;
        }

        if (!landingTileData.isWalkable)
        {
            Debug.LogWarning($"[TileEffectResolver] Cannot jump - landing tile at {landingPosition} is not walkable!");
            return TileEffectResult.Fail;
        }

        return TileEffectResult.Jump;
    }
}
