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
    public bool hasCharacter;
    public bool hasEnemy;
    public bool hasEntity;
    public OverlayTileBehaviour previous;
    public EntityBehaviour entity;
    public Vector3Int gridLocation;
    public Vector2Int grid2DLocation { get { return new Vector2Int(gridLocation.x, gridLocation.y); } }

    public int level;

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

    public void ShowVoidTilePath()
    {
        gameObject.GetComponent<SpriteRenderer>().color = new Color(0.4f, 0.4f, 0.4f, 0.8f);
    }
    public void ShowEnemyMoveTile()
    {
        gameObject.GetComponent<SpriteRenderer>().color = new Color(0.5f, 0.5f, 0.5f, 1f);
    }

    public void ShowOverheatTile()
    {
        gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 0.55f, 0.10f, 1f);
    }
    public void ShowWarningTile()
    {
        gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 0.0f, 1f, 0.1f);
    }

    public void ShowAttackTile()
    {
        gameObject.GetComponent<SpriteRenderer>().color = new Color(1.0f, 0.0f, 0.0f, 1f);
    }
}
