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

    public List<CharacterBehaviour> characterList;
    public List<CharacterBehaviour> playerCharacters;
    public List<EnemyBehaviour> enemyList;
    public List<EntityBehaviour> entitiesInGame;
    public List<EnemyBehaviour> enemies;
    public List<EnemyBehaviour> possibleEnemies;
    public List<Vector3Int> characterPositions;
    // List of levels
    public List<GameObject> tier1Maps;
    public List<GameObject> tier2Maps;
    public List<GameObject> tier3Maps;

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
        int characterCount = 0;

        ReloadMap();
        // Add characters to map:
        int j = 0;
        for (int i = 0; i < allTiles.Count; i++)
        {
            if (characterCount < playerCharacters.Count && characterPositions[j] == allTiles[i].gridLocation)
            {
                playerCharacters[characterCount] = Instantiate(playerCharacters[characterCount]);
                playerCharacters[characterCount].ogPosition = allTiles[i].gridLocation;
                playerCharacters[characterCount].gridLocation = allTiles[i].gridLocation;
                PositionCharacter(playerCharacters[characterCount], allTiles[i]);
                playerCharacters[characterCount].activeTile.hasCharacter = true;
                characterCount++;
                i = 0;
                j++;
            }
        }
        
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
            item.currentHeat = 0;
        }

        InfoText.text = "Turn Ended!";

        //Temp code for swapping turns

    }

    public void ReloadMap()
    {
        // Destroy all non-necessary entities:
        foreach (var tile in allTiles)
        {
            if (tile.entity != null)
            {
                Destroy(tile.entity.gameObject);
            }
        }

        Manager.Instance.DestroyTiles();
        // Randomly generate a map based on tier
        int randomLevel = Random.Range(0, 3);
        switch (levelTier)
        {
            case 1:
                pre_tilemap = Instantiate(tier1Maps[randomLevel].GetComponent<Tilemap>());
                pre_tilemap.transform.SetParent(this.gameObject.transform);
                tilemap = pre_tilemap;
                break;
            case 2:
                pre_tilemap = Instantiate(tier2Maps[randomLevel].GetComponent<Tilemap>());
                pre_tilemap.transform.SetParent(this.gameObject.transform);
                tilemap = pre_tilemap;
                break;
            case 3:
                pre_tilemap = Instantiate(tier3Maps[randomLevel].GetComponent<Tilemap>());
                pre_tilemap.transform.SetParent(this.gameObject.transform);
                tilemap = pre_tilemap;
                break;
            default:
                pre_tilemap = Instantiate(tier1Maps[randomLevel].GetComponent<Tilemap>());
                pre_tilemap.transform.SetParent(this.gameObject.transform);
                tilemap = pre_tilemap;
                break;
        }


        BoundsInt bounds = tilemap.cellBounds;
        map = new Dictionary<Vector2Int, OverlayTileBehaviour>();
        allTiles = new List<OverlayTileBehaviour>();

        // Loop through all the tiles on the map
        for (int z = bounds.min.z; z < bounds.max.z; z++)
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
                        overlayTile.level = level;

                        if (tileLocation.z == 0)
                        {
                            overlayTile.isBlocked = true;
                        }
                        else
                        {
                            overlayTile.isBlocked = false;
                        }
                        map.Add(tileKey, overlayTile);
                        allTiles.Add(overlayTile);
                    }
                }
            }
        }

        // Randomly adding enemies to the map depending on the level:
        for (int i = 0; i < 1 + level + (levelTier - 1); i++)
        {
            int enemyTypeToSpawn = Random.Range(0, possibleEnemies.Count);
            int enemyX = 0, enemyY = 0;
            bool Checking = true;
            bool noTileError = true;

            switch (enemyTypeToSpawn)
            {
                case 0:
                    enemyList.Add(possibleEnemies[0]);
                    break;
                case 1:
                    enemyList.Add(possibleEnemies[1]);
                    break;
                case 2:
                    enemyList.Add(possibleEnemies[2]);
                    break;
                default:
                    break;
            }

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

            for (int k = 0; k < allTiles.Count; k++)
            {
                if (i < enemyList.Count && enemyList[i].grid2DLocation == allTiles[k].grid2DLocation)
                {
                    enemyList[i] = Instantiate(enemyList[i]);
                    PositionCharacter(enemyList[i], allTiles[k]);
                    enemyList[i].activeTile.hasEnemy = true;

                }
            }

        }

        // Randomly add a few Rocks to the map:
        int randomRocks = Random.Range(3, 10);
        for (int i = 0; i < randomRocks; i++)
        {
            Vector2Int randomLocation = new Vector2Int(Random.Range(-bounds.x, bounds.x), Random.Range(-bounds.y, bounds.y));
            foreach (var tile in allTiles)
            {
                if (randomLocation == tile.grid2DLocation && !tile.hasCharacter && !tile.hasEnemy && !tile.isBlocked)
                {
                    var rock = Instantiate(entitiesInGame[0]);
                    rock.PositionEntity(rock, tile);
                    tile.entity = rock;
                    break;
                }
            }
        }
        // Randomly add item crates to map
        int randomCrates = Random.Range(1, 2);
        for (int i = 0; i < randomCrates; i++)
        {
            Vector2Int randomLocation = new Vector2Int(Random.Range(-bounds.x, bounds.x), Random.Range(-bounds.y, bounds.y));
            foreach (var tile in allTiles)
            {
                if (randomLocation == tile.grid2DLocation && !tile.hasCharacter && !tile.hasEnemy && !tile.isBlocked && tile.entity == null)
                {
                    var crate = Instantiate(entitiesInGame[1]);
                    crate.PositionEntity(crate, tile);
                    tile.entity = crate;
                    break;
                }
            }
        }
        // Randomly add barrels to map
        int randomBarrels = Random.Range(2, 3);
        for (int i = 0; i < randomBarrels; i++)
        {
            Vector2Int randomLocation = new Vector2Int(Random.Range(-bounds.x, bounds.x), Random.Range(-bounds.y, bounds.y));
            foreach (var tile in allTiles)
            {
                if (randomLocation == tile.grid2DLocation && !tile.hasCharacter && !tile.hasEnemy && !tile.isBlocked && tile.entity == null)
                {
                    var crate = Instantiate(entitiesInGame[2]);
                    crate.PositionEntity(crate, tile);
                    tile.entity = crate;
                    break;
                }
            }
        }
    }

    public void SetOGPosition()
    {
        int characterCount = 0;
        int j = 0;
        for (int i = 0; i < allTiles.Count; i++)
        {
            if (characterCount < playerCharacters.Count && characterPositions[j] == allTiles[i].gridLocation)
            {
                playerCharacters[characterCount].ogPosition = allTiles[i].gridLocation;
                PositionCharacter(playerCharacters[characterCount], allTiles[i]);
                playerCharacters[characterCount].activeTile.hasCharacter = true;
                characterCount++;
                i = 0;
                j++;
            }
        }
    }

    public List<OverlayTileBehaviour> Get8DirectionTiles(OverlayTileBehaviour selectedTile, int radius)
    {
        List<OverlayTileBehaviour> surroundingTiles = new List<OverlayTileBehaviour>();
        Vector2Int selectedGridLocation = selectedTile.grid2DLocation;

        for (int x = selectedGridLocation.x - radius; x <= selectedGridLocation.x + radius; x++)
        {
            for (int y = selectedGridLocation.y - radius; y <= selectedGridLocation.y + radius; y++)
            {
                Vector2Int tileGridLocation = new Vector2Int(x, y);
                if (Vector2Int.Distance(selectedGridLocation, tileGridLocation) <= radius)
                {
                    // Assuming you have a method to get the OverlayTileBehaviour at a given grid location
                    OverlayTileBehaviour tile = GetOverlayTileAt(tileGridLocation);
                    if (tile != null)
                    {
                        surroundingTiles.Add(tile);
                    }
                }
            }
        }

        return surroundingTiles;
    }

    public OverlayTileBehaviour GetOverlayTileAt(Vector2Int gridLocation)
    {
        foreach (var tile in allTiles)
        {
            if (tile.grid2DLocation == gridLocation)
            {
                return tile;
            }
        }
        return null;
    }
    public EnemyBehaviour GetEnemyAt(OverlayTileBehaviour tile)
    {
        foreach (var enemy in enemyList)
        {
            if (enemy.grid2DLocation == tile.grid2DLocation)
            {
                return enemy;
            }
        }
        return null;
    }

    public void HideAllTiles()
    {
        foreach (var tile in allTiles)
        {
            tile.HideTile();
        }
    }
}
