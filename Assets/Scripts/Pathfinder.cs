using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Pathfinder
{
    public List<OverlayTileBehaviour> FindPath(OverlayTileBehaviour start, OverlayTileBehaviour end, List<OverlayTileBehaviour> searchableTiles)
    {
        List<OverlayTileBehaviour> openList = new List<OverlayTileBehaviour>();
        List<OverlayTileBehaviour> closedList = new List<OverlayTileBehaviour>();

        openList.Add(start);

        while (openList.Count > 0)
        {
            OverlayTileBehaviour currentOverlayTile = openList.OrderBy(x => x.F).First();

            openList.Remove(currentOverlayTile);
            closedList.Add(currentOverlayTile);

            if (currentOverlayTile == end)
            {
                // Finalise the path
                return GetFinishedList(start, end);
            }

            var neighbourTiles = MapManager.Instance.GetNeighbourTiles(currentOverlayTile, searchableTiles);
            foreach (var neighbour in neighbourTiles)
            {
                if (neighbour.isBlocked || closedList.Contains(neighbour))
                {
                    continue;
                }

                neighbour.G = GetManhattenDistance(start, neighbour);
                neighbour.H = GetManhattenDistance(end, neighbour);
                neighbour.previous = currentOverlayTile;
                if (!openList.Contains(neighbour))
                {
                    openList.Add(neighbour);
                }
            }
        }

        return new List<OverlayTileBehaviour>();
    }

    private List<OverlayTileBehaviour> GetFinishedList(OverlayTileBehaviour start, OverlayTileBehaviour end)
    {
        List<OverlayTileBehaviour> finishedList = new List<OverlayTileBehaviour>();

        OverlayTileBehaviour currentTile = end;

        while (currentTile != start)
        {
            finishedList.Add(currentTile);
            currentTile = currentTile.previous;
        }
        finishedList.Reverse();
        return finishedList;
    }

    private int GetManhattenDistance(OverlayTileBehaviour start, OverlayTileBehaviour neighbour)
    {
        return Mathf.Abs(start.gridLocation.x - neighbour.gridLocation.x) + Mathf.Abs(start.gridLocation.y - neighbour.gridLocation.y);
    }

}
