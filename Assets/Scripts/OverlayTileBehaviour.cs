using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverlayTileBehaviour : MonoBehaviour
{
    // Variables for A* pathfinding
    public int G;
    public int H;
    public int F { get { return G + H; } }

    public bool isBlocked;
    public OverlayTileBehaviour previous;
    public Vector3Int gridLocation;
    public Vector2Int grid2DLocation { get { return new Vector2Int(gridLocation.x, gridLocation.y); } }

    public void ShowTile()
    {
        gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
    }

    public void HideTile()
    {
        gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);
    }

    public void ShowTileInPath()
    {
        gameObject.GetComponent<SpriteRenderer>().color = new Color(0, 1, 0, 1);
    }
}
