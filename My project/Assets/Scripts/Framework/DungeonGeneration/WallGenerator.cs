using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class WallGenerator
{
    public static void CreateWalls(HashSet<Vector2Int> floorPos, TilemapVisualizer tilemapVisualizer)
    {
        var basicPos = FindWallsInDirections(floorPos, Direction2D.cardinalDirectionList);
        foreach (var positon in basicPos)
        {
            tilemapVisualizer.PaintSingleBasicWall(positon);
        }
    }

    private static HashSet<Vector2Int> FindWallsInDirections(HashSet<Vector2Int> floorPos, List<Vector2Int> directionList)
    {
        HashSet<Vector2Int> wallPositions = new HashSet<Vector2Int>();
        foreach (var position in floorPos) 
        { 
            foreach (var direction in directionList) 
            { 
                var neighbourPosition = position + direction;
                // ÅÐ¶ÏÊÇ·ñÎªÇ½Ìå
                if (floorPos.Contains(neighbourPosition) == false)
                {
                    wallPositions.Add(neighbourPosition);
                }
            }
        }
        return wallPositions;
    }
}
