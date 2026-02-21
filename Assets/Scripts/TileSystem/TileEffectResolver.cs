using UnityEngine;

public enum TileEffectResult
{
    Continue,
    Win,
    Fail
}

public static class TileEffectResolver
{
    public static TileEffectResult Resolve(ref TileEffectContext context)
    {
        switch (context.tileData.tileType)
        {
            case TileType.Normal:
                Debug.Log($"[TileEffectResolver] Normal tile at {context.position} - Continue");
                return TileEffectResult.Continue;

            case TileType.GoalTile:
                Debug.Log($"[TileEffectResolver] Goal tile at {context.position} - Win!");
                return TileEffectResult.Win;

            case TileType.Block:
                Debug.Log($"[TileEffectResolver] Block tile at {context.position} - Fail");
                return TileEffectResult.Fail;

            case TileType.Empty:
                Debug.Log($"[TileEffectResolver] Empty tile at {context.position} - Fail");
                return TileEffectResult.Fail;

            case TileType.StartingTile:
                Debug.Log($"[TileEffectResolver] Starting tile at {context.position} - Continue");
                return TileEffectResult.Continue;

            case TileType.Locked:
                Debug.LogWarning($"[TileEffectResolver] TileType 'Locked' not yet implemented - treating as Normal");
                return TileEffectResult.Continue;

            case TileType.Portal:
                Debug.LogWarning($"[TileEffectResolver] TileType 'Portal' not yet implemented - treating as Normal");
                return TileEffectResult.Continue;

            case TileType.Teleport:
                {
                    Debug.Log($"[TileEffectResolver] Teleport triggered");

                    if (FindTeleportPair(
                        context.tileGrid,
                        context.tileData.teleportID,
                        context.position,
                        out Vector2Int pairPosition,
                        out Vector2Int exitDirection))
                    {
                        context.position = pairPosition;
                        context.direction = exitDirection;

                        context.visualEffect = TileEffectVisual.Teleport;

                        return TileEffectResult.Continue;
                    }

                    return TileEffectResult.Fail;
                }
            case TileType.Rotate90Left:
                Debug.Log($"[TileEffectResolver] Rotate90Left tile at {context.position} - Rotating left (anticlockwise)");
                context.direction = RotateDirectionLeft(context.direction);
                return TileEffectResult.Continue;

            case TileType.Rotate90Right:
                Debug.Log($"[TileEffectResolver] Rotate90Right tile at {context.position} - Rotating right (clockwise)");
                context.direction = RotateDirectionRight(context.direction);
                return TileEffectResult.Continue;

            case TileType.Rotate180:
                Debug.Log($"[TileEffectResolver] Rotate180 tile at {context.position} - Rotating 180°");
                context.direction = RotateDirection180(context.direction);
                return TileEffectResult.Continue;

            case TileType.JumpForward:
                return HandleJumpForward(ref context);
                    

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
                Debug.LogWarning($"[TileEffectResolver] TileType '{context.tileData.tileType}' not recognized - treating as Normal");
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

        // Buscar todos los tiles en el grid
        var allTiles = tileGrid.GetAllTiles();

        foreach (var kvp in allTiles)
        {
            Vector2Int pos = kvp.Key;
            GameObject tileObj = kvp.Value;

            // Ignorar el tile actual
            if (pos == currentPosition)
                continue;

            // Verificar si es un teleport con el mismo ID
            TileBase tileBase = tileObj.GetComponent<TileBase>();
            if (tileBase != null && tileBase.tileData != null)
            {
                if (tileBase.tileData.tileType == TileType.Teleport && tileBase.tileData.teleportID == teleportID)
                {
                    pairPosition = pos;
                    exitDirection = tileBase.tileData.exitDirection;
                    Debug.Log($"[TileEffectResolver] Found teleport pair at {pairPosition} with exit direction {exitDirection}");
                    return true;
                }
            }
        }

        Debug.LogWarning($"[TileEffectResolver] No teleport pair found for ID {teleportID}");
        return false;
    }

    private static TileEffectResult HandleJumpForward(
    ref TileEffectContext context)
    {
        Debug.Log("[TileEffectResolver] JumpForward triggered");

        Vector2Int landing =
            context.position +
            context.direction *
            (context.tileData.jumpDistance + 1);

        GameObject landingTile =
            context.tileGrid.GetTile(landing);

        if (landingTile == null)
            return TileEffectResult.Fail;

        TileBase tileBase =
            landingTile.GetComponent<TileBase>();

        if (!tileBase.tileData.isWalkable)
            return TileEffectResult.Fail;
        context.position = landing;
        context.visualEffect = TileEffectVisual.Jump;

        return TileEffectResult.Continue;
    }
}