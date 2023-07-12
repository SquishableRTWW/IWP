using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using TMPro;
using System.Linq;

public class MapManager : MonoBehaviour
{
    private static MapManager _instance;
    public static MapManager Instance { get { return _instance; } }

    public OverlayTileBehaviour overlayTilePrefab;
    public GameObject overlayContainer;

    public Dictionary<Vector2Int, OverlayTileBehaviour> map;
    public List<OverlayTileBehaviour> allTiles;

    public List<CharacterBehaviour> playerCharacters;
    public List<EnemyBehaviour> enemyList;
    public List<EnemyBehaviour> enemies;
    // List of levels
    public List<GameObject> tier1Maps;
    public List<GameObject> tier2Maps;

    public int level;
    public int levelTier;
    private Tilemap pre_tilemap;
    private Tilemap tilemap;

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
        ReloadMap();
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
        //InfoText.text = ".......";
        // Check if all characters have moved
        foreach (var item in playerCharacters)
        {
            if (item.finishedMove == false && Manager.Instance.playerTurn)
            {
                InfoText.text = "YOUR TURN";
                break;
            }
        }

        for (int i = 0; i < playerCharacters.Count; i++)
        {
            if (!playerCharacters[i].finishedMove)
            {
                break;
            }
            else
            {
                if (i == playerCharacters.Count - 1)
                {
                    InfoText.text = "ALL UNITS MOVED!";
                }
            }
        }

        if (Manager.Instance.GetCP() <= 0 && Manager.Instance.playerTurn == true)
        {
            InfoText.text = "NO MORE CP";
        }

        if (Manager.Instance.playerTurn == false)
        {
            InfoText.text = "ENEMIES's TURN";
        }
    }

    private void PositionCharacter(CharacterBehaviour character, OverlayTileBehaviour overlayTile)
    {
        character.transform.position = new Vector3(overlayTile.transform.position.x, overlayTile.transform.position.y + 0.0001f, overlayTile.transform.position.z);
        character.GetComponent<SpriteRenderer>().sortingOrder = overlayTile.GetComponent<SpriteRenderer>().sortingOrder + 1;
        character.activeTile = overlayTile;
    }
    private void PositionCharacter(EnemyBehaviour enemy, OverlayTileBehaviour overlayTile)
    {
        enemy.transform.position = new Vector3(overlayTile.transform.position.x, overlayTile.transform.position.y + 0.0001f, overlayTile.transform.position.z);
        enemy.GetComponent<SpriteRenderer>().sortingOrder = overlayTile.GetComponent<SpriteRenderer>().sortingOrder + 1;
        enemy.activeTile = overlayTile;
    }

    public void turnEnded()
    {
        Manager.Instance.playerTurn = false;
        foreach (var item in playerCharacters)
        {
            item.finishedMove = false;
            item.isOverheated = false;
        }

        InfoText.text = "Turn Ended!";

        //Temp code for swapping turns

    }

    public void ReloadMap()
    {
        // Randomly generate a map based on tier
        int randomLevel = Random.Range(0, 2);
        switch (levelTier)
        {
            case 1:
                pre_tilemap = Instantiate(tier1Maps[randomLevel].GetComponent<Tilemap>());
                pre_tilemap.transform.SetParent(this.gameObject.transform);
                tilemap = gameObject.GetComponentInChildren<Tilemap>();
                break;
            case 2:
                pre_tilemap = Instantiate(tier2Maps[randomLevel].GetComponent<Tilemap>());
                pre_tilemap.transform.SetParent(this.gameObject.transform);
                tilemap = gameObject.GetComponentInChildren<Tilemap>();
                break;
            default:
                pre_tilemap = Instantiate(tier1Maps[randomLevel].GetComponent<Tilemap>());
                pre_tilemap.transform.SetParent(this.gameObject.transform);
                tilemap = gameObject.GetComponentInChildren<Tilemap>();
                break;
        }


        BoundsInt bounds = tilemap.cellBounds;
        map = new Dictionary<Vector2Int, OverlayTileBehaviour>();
        allTiles = new List<OverlayTileBehaviour>();
        level = 1;

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

        // Add characters to map:
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

        // Randomly adding enemies to the map depending on the level:
        for (int i = 0; i < 1 + level; i++)
        {
            int enemyTypeToSpawn = Random.Range(0, enemies.Count);
            int enemyX = 0, enemyY = 0;


            switch (enemyTypeToSpawn)
            {
                case 0:
                    bool Checking = true;
                    bool noTileError = true;
                    enemyList.Add(enemies[0]);

                    while (Checking)
                    {
                        enemyX = Random.Range(6, 7);
                        enemyY = Random.Range(-7, 7);
                        int childCount = 0;

                        // Check x and y coords coincides with an overlay tile
                        foreach (Transform child in overlayContainer.transform)
                        {
                            if (child.gameObject.GetComponent<OverlayTileBehaviour>().grid2DLocation == new Vector2Int(enemyX, enemyY))
                            {
                                break;
                            }
                            childCount++;
                            if (childCount + 1 == allTiles.Count)
                            {
                                noTileError = false;
                            }
                            else
                            {
                                noTileError = true;
                            }
                        }

                        // Check no clash with player's characters
                        for (int j = 0; j < playerCharacters.Count; j++)
                        {
                            if (playerCharacters[j].grid2DLocation == new Vector2Int(enemyX, enemyY))
                            {
                                noTileError = false;
                                Debug.Log("Spawn clash; resetting");
                            }
                            else if (j + 1 == enemyList.Count)
                            {
                                noTileError = true;
                                break;
                            }
                        }

                        // Check no clash with other enemies
                        for (int j = 0; j < enemyList.Count; j++)
                        {
                            if (enemyList[j].grid2DLocation == new Vector2Int(enemyX, enemyY))
                            {
                                noTileError = false;
                                Debug.Log("Spawn clash; resetting");
                                break;
                            }
                            else if (j + 1 == enemyList.Count)
                            {
                                noTileError = true;
                            }
                        }

                        // If no error then ok
                        if (noTileError == true)
                        {
                            Checking = false;
                        }
                    }

                    Vector3Int enemyLocation = new Vector3Int(enemyX, enemyY, 2);
                    enemyList[i].gridLocation = enemyLocation;
                    //enemyList[i] = Instantiate(enemyList[i]);
                    //PositionCharacter(enemyList[i], allTiles[i]);
                    //enemyList[i].activeTile.hasCharacter = true;
                    for (int k = 0; k < allTiles.Count; k++)
                    {
                        if (i < enemyList.Count && enemyList[i].grid2DLocation == allTiles[k].grid2DLocation)
                        {
                            enemyList[i] = Instantiate(enemyList[i]);
                            PositionCharacter(enemyList[i], allTiles[k]);
                            enemyList[i].activeTile.hasEnemy = true;
                            //playerCharacters.Remove(playerCharacters[characterCount]);
                            //i++;
                            //k = 0;
                        }
                    }
                    break;

                default:
                    break;
            }
        }
    }
}
