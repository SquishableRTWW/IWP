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

    public List<OverlayTileBehaviour> FindLinearAttackPath(OverlayTileBehaviour characterTile, OverlayTileBehaviour end, int range, List<OverlayTileBehaviour> searchableTiles)
    {
        int x = (characterTile.grid2DLocation.x - end.grid2DLocation.x);
        int y = (characterTile.grid2DLocation.y - end.grid2DLocation.y);

        Vector2 direction = new Vector2(x, y).normalized;
        
        List<Vector2Int> vector2IntList = new List<Vector2Int>();
        List<OverlayTileBehaviour> list = new List<OverlayTileBehaviour>();
        Vector2Int realDirection = new Vector2Int((int)direction.x, (int)direction.y);

        for (int i = 1; i < range + 1; i++)
        {
            Vector2Int tilePosition = characterTile.grid2DLocation - realDirection * i;
            vector2IntList.Add(tilePosition);
        }

        for (int i = 0; i < searchableTiles.Count; i++)
        {
            if (searchableTiles[i].grid2DLocation == vector2IntList[0])
            {
                list.Add(searchableTiles[i]);
                vector2IntList.Remove(vector2IntList[0]);
            }
            if (vector2IntList.Count < 1)
            {
                break;
            }
        }
        return list;
    }

    public List<OverlayTileBehaviour> FindAOEAttackPath(OverlayTileBehaviour mousedTile, int AOERange, List<OverlayTileBehaviour> searchableTiles)
    {
        int rangeCount = 0;
        List<OverlayTileBehaviour> preFinalList = new List<OverlayTileBehaviour>();
        List<OverlayTileBehaviour> finalList = new List<OverlayTileBehaviour>();
        var tileForPreviouStep = new List<OverlayTileBehaviour>();
        tileForPreviouStep.Add(mousedTile);
        while (rangeCount < (AOERange - 1))
        {
            var surroundingTiles = new List<OverlayTileBehaviour>();
            foreach (var tile in tileForPreviouStep)
            {
                surroundingTiles.AddRange(GetNeighbourTilesPathFinder(tile, searchableTiles));
            }
            preFinalList.AddRange(surroundingTiles);
            tileForPreviouStep = surroundingTiles.Distinct().ToList();

            rangeCount++;
        }

        preFinalList.Add(mousedTile);
        foreach (var tile in preFinalList)
        {
            if (searchableTiles.Contains(tile))
            {
                finalList.Add(tile);
            }
        }
        return finalList;
    }
    public List<OverlayTileBehaviour> FindAcrossAttackPath(OverlayTileBehaviour mousedTile, int AOERange, List<OverlayTileBehaviour> searchableTiles)
    {
        int rangeCount = 0;
        List<OverlayTileBehaviour> preFinalList = new List<OverlayTileBehaviour>();
        List<OverlayTileBehaviour> finalList = new List<OverlayTileBehaviour>();
        var tileForPreviouStep = new List<OverlayTileBehaviour>();
        tileForPreviouStep.Add(mousedTile);
        while (rangeCount < (AOERange - 1))
        {
            var surroundingTiles = new List<OverlayTileBehaviour>();
            foreach (var tile in tileForPreviouStep)
            {
                surroundingTiles.AddRange(GetAdjacentTilesPathFinder(tile, searchableTiles));
            }
            preFinalList.AddRange(surroundingTiles);
            tileForPreviouStep = surroundingTiles.Distinct().ToList();

            rangeCount++;
        }

        preFinalList.Add(mousedTile);
        foreach (var tile in preFinalList)
        {
            if (searchableTiles.Contains(tile))
            {
                finalList.Add(tile);
            }
        }
        return finalList;
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

    public List<OverlayTileBehaviour> GetNeighbourTilesPathFinder(OverlayTileBehaviour currentOverlayTile, List<OverlayTileBehaviour> searchableTiles)
    {
        Dictionary<Vector2Int, OverlayTileBehaviour> tileToSearch = new Dictionary<Vector2Int, OverlayTileBehaviour>();

        if (searchableTiles.Count > 0)
        {
            foreach (var tile in searchableTiles)
            {
                tileToSearch.Add(tile.grid2DLocation, tile);
            }
        }
        else
        {
            return null;
        }

        List<OverlayTileBehaviour> neighbours = new List<OverlayTileBehaviour>();

        // Top neighbour
        Vector2Int locationToCheck = new Vector2Int(currentOverlayTile.gridLocation.x, currentOverlayTile.gridLocation.y + 1);
        if (tileToSearch.ContainsKey(locationToCheck))
        {
            if (Mathf.Abs(currentOverlayTile.gridLocation.z - tileToSearch[locationToCheck].gridLocation.z) <= 1)
            {
                neighbours.Add(tileToSearch[locationToCheck]);
            }
        }
        // Bottom neighbour
        locationToCheck = new Vector2Int(currentOverlayTile.gridLocation.x, currentOverlayTile.gridLocation.y - 1);
        if (tileToSearch.ContainsKey(locationToCheck))
        {
            if (Mathf.Abs(currentOverlayTile.gridLocation.z - tileToSearch[locationToCheck].gridLocation.z) <= 1)
            {
                neighbours.Add(tileToSearch[locationToCheck]);
            }
        }
        // Left neighbour
        locationToCheck = new Vector2Int(currentOverlayTile.gridLocation.x - 1, currentOverlayTile.gridLocation.y);
        if (tileToSearch.ContainsKey(locationToCheck))
        {
            if (Mathf.Abs(currentOverlayTile.gridLocation.z - tileToSearch[locationToCheck].gridLocation.z) <= 1)
            {
                neighbours.Add(tileToSearch[locationToCheck]);
            }
        }
        // Right neighbour
        locationToCheck = new Vector2Int(currentOverlayTile.gridLocation.x + 1, currentOverlayTile.gridLocation.y);
        if (tileToSearch.ContainsKey(locationToCheck))
        {
            if (Mathf.Abs(currentOverlayTile.gridLocation.z - tileToSearch[locationToCheck].gridLocation.z) <= 1)
            {
                neighbours.Add(tileToSearch[locationToCheck]);
            }
        }

        return neighbours;
    }

    public List<OverlayTileBehaviour> GetAdjacentTilesPathFinder(OverlayTileBehaviour currentOverlayTile, List<OverlayTileBehaviour> searchableTiles)
    {
        Dictionary<Vector2Int, OverlayTileBehaviour> tileToSearch = new Dictionary<Vector2Int, OverlayTileBehaviour>();

        if (searchableTiles.Count > 0)
        {
            foreach (var tile in searchableTiles)
            {
                tileToSearch.Add(tile.grid2DLocation, tile);
            }
        }
        else
        {
            return null;
        }

        List<OverlayTileBehaviour> neighbours = new List<OverlayTileBehaviour>();

        // Top neighbour
        Vector2Int locationToCheck = new Vector2Int(currentOverlayTile.gridLocation.x, currentOverlayTile.gridLocation.y + 1);
        //if (tileToSearch.ContainsKey(locationToCheck))
        //{
        //    if (Mathf.Abs(currentOverlayTile.gridLocation.z - tileToSearch[locationToCheck].gridLocation.z) <= 1)
        //    {
        //        neighbours.Add(tileToSearch[locationToCheck]);
        //    }
        //}
        //// Bottom neighbour
        //locationToCheck = new Vector2Int(currentOverlayTile.gridLocation.x, currentOverlayTile.gridLocation.y - 1);
        //if (tileToSearch.ContainsKey(locationToCheck))
        //{
        //    if (Mathf.Abs(currentOverlayTile.gridLocation.z - tileToSearch[locationToCheck].gridLocation.z) <= 1)
        //    {
        //        neighbours.Add(tileToSearch[locationToCheck]);
        //    }
        //}
        // Left neighbour
        locationToCheck = new Vector2Int(currentOverlayTile.gridLocation.x - 1, currentOverlayTile.gridLocation.y);
        if (tileToSearch.ContainsKey(locationToCheck))
        {
            if (Mathf.Abs(currentOverlayTile.gridLocation.z - tileToSearch[locationToCheck].gridLocation.z) <= 1)
            {
                neighbours.Add(tileToSearch[locationToCheck]);
            }
        }
        // Right neighbour
        locationToCheck = new Vector2Int(currentOverlayTile.gridLocation.x + 1, currentOverlayTile.gridLocation.y);
        if (tileToSearch.ContainsKey(locationToCheck))
        {
            if (Mathf.Abs(currentOverlayTile.gridLocation.z - tileToSearch[locationToCheck].gridLocation.z) <= 1)
            {
                neighbours.Add(tileToSearch[locationToCheck]);
            }
        }

        return neighbours;
    }
}
