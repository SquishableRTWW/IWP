using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using TMPro;

public class MapManager : MonoBehaviour
{
    private static MapManager _instance;
    public static MapManager Instance { get { return _instance; } }

    public OverlayTileBehaviour overlayTilePrefab;
    public GameObject overlayContainer;

    public Dictionary<Vector2Int, OverlayTileBehaviour> map;
    public List<OverlayTileBehaviour> allTiles;
    public List<CharacterBehaviour> playerCharacters;

    [SerializeField] TextMeshProUGUI InfoText;

    // Start is called before the first frame update
    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    private void Start()
    {
        var tilemap = gameObject.GetComponentInChildren<Tilemap>();

        BoundsInt bounds = tilemap.cellBounds;
        map = new Dictionary<Vector2Int, OverlayTileBehaviour>();
        allTiles = new List<OverlayTileBehaviour>();
        int characterCount = 0;


        // Loop through all the tiles on the map
        for (int z = bounds.max.z; z > bounds.min.z; z--)
        {
            for (int y = bounds.min.y; y < bounds.max.y; y++)
            {
                for (int x = bounds.min.x; x < bounds.max.x; x++)
                {
                    var tileLocation = new Vector3Int(x, y, z);
                    var tileKey = new Vector2Int(x, y);

                    if (tilemap.HasTile(tileLocation) && !map.ContainsKey(tileKey))
                    {
                        var overlayTile = Instantiate(overlayTilePrefab, overlayContainer.transform);
                        var cellworldPosition = tilemap.GetCellCenterWorld(tileLocation);

                        overlayTile.transform.position = new Vector3(cellworldPosition.x, cellworldPosition.y, cellworldPosition.z + 1);
                        overlayTile.GetComponent<SpriteRenderer>().sortingOrder = tilemap.GetComponent<TilemapRenderer>().sortingOrder;
                        overlayTile.gridLocation = tileLocation;
                        map.Add(tileKey, overlayTile);
                        allTiles.Add(overlayTile);

                        //if (characterCount < playerCharacters.Count && playerCharacters[characterCount].grid2DLocation == overlayTile.grid2DLocation)
                        //{
                        //    playerCharacters[characterCount] = Instantiate(playerCharacters[characterCount]);
                        //    PositionCharacter(playerCharacters[characterCount], overlayTile);
                        //    //playerCharacters.Remove(playerCharacters[characterCount]);
                        //    characterCount++;
                        //}
                    }
                }
            }
        }
        for (int i = 0; i < allTiles.Count; i++)
        {
            if (characterCount < playerCharacters.Count && playerCharacters[characterCount].grid2DLocation == allTiles[i].grid2DLocation)
            {
                playerCharacters[characterCount] = Instantiate(playerCharacters[characterCount]);
                PositionCharacter(playerCharacters[characterCount], allTiles[i]);
                playerCharacters[characterCount].activeTile.hasCharacter = true;
                //playerCharacters.Remove(playerCharacters[characterCount]);
                characterCount++;
                i = 0;
            }
        }

        // Add enemies to the map
    }

    public List<OverlayTileBehaviour> GetNeighbourTiles(OverlayTileBehaviour currentOverlayTile, List<OverlayTileBehaviour> searchableTiles)
    {
        Dictionary<Vector2Int, OverlayTileBehaviour> tileToSearch = new Dictionary<Vector2Int, OverlayTileBehaviour>();

        if (searchableTiles.Count > 0)
        {
            foreach(var tile in searchableTiles)
            {
                tileToSearch.Add(tile.grid2DLocation, tile);
            }
        }
        else
        {
            tileToSearch = map;
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

    // Update is called once per frame
    void Update()
    {
        // Display Message to tell if all characters have moved
        InfoText.text = "All units have moved!";
        // Check if all characters have moved
        foreach (var item in playerCharacters)
        {
            if (item.finishedMove == false)
            {
                InfoText.text = ".......";
                break;
            }
        }
    }

    private void PositionCharacter(CharacterBehaviour character, OverlayTileBehaviour overlayTile)
    {
        character.transform.position = new Vector3(overlayTile.transform.position.x, overlayTile.transform.position.y + 0.0001f, overlayTile.transform.position.z);
        character.GetComponent<SpriteRenderer>().sortingOrder = overlayTile.GetComponent<SpriteRenderer>().sortingOrder + 1;
        character.activeTile = overlayTile;
    }

    public void turnEnded()
    {
        foreach (var item in playerCharacters)
        {
            item.finishedMove = false;
            item.isOverheated = false;
        }

        InfoText.text = "Turn Ended!";

        //Temp code for swapping turns

    }
}
