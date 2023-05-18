using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterBehaviour : MonoBehaviour
{
    public int maxFuel;
    public int currentFuel;
    public int overheatAmount;
    public int HP;
    public bool finishedMove;

    public string characterName;

    public Vector3Int gridLocation;
    public Vector2Int grid2DLocation { get { return new Vector2Int(gridLocation.x, gridLocation.y); } }

    public OverlayTileBehaviour activeTile;
    // Start is called before the first frame update
    void Start()
    {
        currentFuel = maxFuel;
        finishedMove = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
