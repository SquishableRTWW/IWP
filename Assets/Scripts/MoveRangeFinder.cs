using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MoveRangeFinder
{
    public List<OverlayTileBehaviour> GetTilesInRange(OverlayTileBehaviour startingTile, int range)
    {
        var inRangeTiles = new List<OverlayTileBehaviour>();
        int rangeCount = 0;

        inRangeTiles.Add(startingTile);
        var tileForPreviouStep = new List<OverlayTileBehaviour>();
        tileForPreviouStep.Add(startingTile);

        while (rangeCount < range)
        {
            var surroundingTiles = new List<OverlayTileBehaviour>();
            foreach (var tile in tileForPreviouStep)
            {
                surroundingTiles.AddRange(MapManager.Instance.GetNeighbourTiles(tile, new List<OverlayTileBehaviour>()));
            }
            inRangeTiles.AddRange(surroundingTiles);
            tileForPreviouStep = surroundingTiles.Distinct().ToList();
            rangeCount++;
        }

        return inRangeTiles.Distinct().ToList();
    }
}
