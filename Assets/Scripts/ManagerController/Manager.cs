using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.SceneManagement;

public class Manager : MonoBehaviour
{
    private static Manager _instance;
    public static Manager Instance { get { return _instance; } }


    public MouseController mouseController;
    [SerializeField] TextMeshProUGUI timerText;
    [SerializeField] TextMeshProUGUI infoText;
    [SerializeField] TextMeshProUGUI CPText;
    public Canvas CPUIField;
    public Canvas combatCanvas;
    public Canvas prepCanvas;
    public Canvas eventCanvas;

    [SerializeField] Image CPUI;
    [SerializeField] float timeLimit;
    [SerializeField] float originalTime;
    public int maxfuelPool;
    public int fuelPool;
    public int CP;
    public int maxCP;

    // Booleans for phases of the game; Might be referenced outside of Manager like
    // in the prep phase manager.
    public bool isInCombat = true;
    public bool isInEvent = false;
    public bool playerTurn = true;

    [SerializeField] TimerBar TimerSlider;

    private Pathfinder pathfinder;
    private MoveRangeFinder moveRangeFinder;
    public List<List<OverlayTileBehaviour>> enemyPath;
    public new CameraController camera;
    public GameObject gridMap;
    public GameObject tileContainer;

    // List of weapons and equipment player owns:
    public List<GameObject> playerItemList;

    private void Awake()
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
    // Start is called before the first frame update
    void Start()
    {
        pathfinder = new Pathfinder();
        moveRangeFinder = new MoveRangeFinder();
        enemyPath = new List<List<OverlayTileBehaviour>>();
        timeLimit = originalTime;
        TimerSlider.SetMaxTime(timeLimit);
        CP = 2;
        maxCP = 2;
        maxfuelPool = 15;
        fuelPool = maxfuelPool;

        int CPUIOffset = 70;
        for (int i = 0; i < CP; i++)
        {
            Image CPImage = Instantiate(CPUI);
            CPImage.transform.SetParent(CPUIField.transform);
            CPImage.transform.position = new Vector3(CPUIField.transform.position.x + 75f + (i * CPUIOffset), CPUIField.transform.position.y, 0);
        }

        isInCombat = false;
    }

    // Update is called once per frame
    void Update()
    {
        // Check to see if there are any enemies left
        if (MapManager.Instance.enemyList.Count <= 0)
        {
            // If there are no enemies left, end the level
            if (MapManager.Instance.level % 3 != 0)
            {
                MapManager.Instance.level++;
            }
            else
            {
                MapManager.Instance.levelTier++;
                MapManager.Instance.level = 1;
            }

            MapManager.Instance.ReloadMap();
            timeLimit = originalTime;
            mouseController.inRangeTiles.Clear();

            // And move on to Event Phase
            combatCanvas.gameObject.SetActive(false);
            prepCanvas.gameObject.SetActive(false);
            eventCanvas.gameObject.SetActive(true);
            Instance.isInCombat = false;
            isInEvent = true;
            EventManager.Instance.RandomiseEvent();
        }

        // If statement to update the enemy paths when new enemies are made or destroyed
        if (enemyPath.Count < MapManager.Instance.enemyList.Count)
        {
            enemyPath.Add(new List<OverlayTileBehaviour>());
        }

        // Game logic for when level is being played
        if (isInCombat)
        {
            // Off all other canvas, On combat canvas
            combatCanvas.gameObject.SetActive(true);
            prepCanvas.gameObject.SetActive(false);
            eventCanvas.gameObject.SetActive(false);
            // Game logic
            if (playerTurn)
            {
                timeLimit -= Time.deltaTime;
                foreach (var enemy in MapManager.Instance.enemyList)
                {
                    enemy.hasAttacked = false;
                    enemy.shouldAttack = false;
                }
            }
            timerText.text = string.Format("{0:0.00}", timeLimit);
            CPText.text = "CP Left: " + CP.ToString();
            TimerSlider.SetTime(timeLimit);
        }

        // If level time limit is over?
        if (timeLimit <= 0)
        {
            playerTurn = false;
            TempEnemyTurn();
            timeLimit = originalTime;
            //Give back the CP
            AddCPImage();
            foreach (var character in MapManager.Instance.playerCharacters)
            {
                character.currentFuel++;
            }
        }

        // NOTE FOR ME:
        // HOW THE ENEMY AI SEQUENCES WORKS RIGHT NOW:
        // 1. TempEnemyTurn() loops through all enemies, what they should target, if they should attack and not move/move etc
        // 2. StartMovingEnemies() coroutine to one by one, move each enemy/attack with each enemy until all enemies have been done
        // 3. Turn ends at the end of MoveEnemiesSequentially(). TempEnemyTurn is the precursor. 

        if (!playerTurn)
        {
            StartMovingEnemies();
        }
    }

    private void StartMovingEnemies()
    {
        StartCoroutine(MoveEnemiesSequentially());
    }

    private IEnumerator MoveEnemiesSequentially()
    {
        for (int i = 0; i < MapManager.Instance.enemyList.Count; i++)
        {
            if (MapManager.Instance.enemyList[i] != null)
            {
                EnemyBehaviour enemy = MapManager.Instance.enemyList[i];
                List<OverlayTileBehaviour> path = enemyPath[i];

                //StartCoroutine(camera.ZoomAtCharacter(enemy.transform.position));
                yield return new WaitForSeconds(1.0f);

                if (!MapManager.Instance.enemyList[i].shouldAttack)
                {
                    while (path.Count > 0 && MapManager.Instance.enemyList[i].shouldAttack == false)
                    {
                        OverlayTileBehaviour nextTile = path[0];
                        MoveAlongEnemyPath(enemy, nextTile);
                        yield return new WaitUntil(() => Vector2.Distance(enemy.transform.position, nextTile.transform.position) <= 0.0f);

                        PositionCharacter(enemy, nextTile);
                        if (MapManager.Instance.enemyList[i].InAttackRange(MapManager.Instance.enemyList[i].targetTile) == true)
                        {
                            MapManager.Instance.enemyList[i].shouldAttack = true;
                        }

                        //Debug.Log("E: " + i + "Count:" + path.Count);
                        path.RemoveAt(0);
                        if (MapManager.Instance.enemyList[i].shouldAttack == true)
                        {
                            path.Clear();
                        }
                    }
                }

                if (MapManager.Instance.enemyList[i].shouldAttack)
                {
                    foreach (CharacterBehaviour character in MapManager.Instance.playerCharacters)
                    {
                        if (character.grid2DLocation == MapManager.Instance.enemyList[i].targetTile.grid2DLocation && !MapManager.Instance.enemyList[i].hasAttacked)
                        {
                            StartCoroutine(MapManager.Instance.enemyList[i].DoAttackAnimation());
                            DoDamageToCharacter(character, MapManager.Instance.enemyList[i].enemyScriptable.damage);
                            StartCoroutine(character.ShowDamage((MapManager.Instance.enemyList[i].enemyScriptable.damage - character.defence).ToString()));
                            MapManager.Instance.enemyList[i].hasAttacked = true;
                            MapManager.Instance.enemyList[i].shouldAttack = false;
                        }
                    }
                }

                if (i == enemyPath.Count - 1)
                {
                    playerTurn = true;
                }
            }
        }
    }

    public void TempEnemyTurn()
    {
        infoText.text = "Enemy turn.";
        MapManager.Instance.turnEnded();
        mouseController.DeselectAction();

        // MOVEMENT AI logic
        for (int i = 0; i < MapManager.Instance.enemyList.Count; i++)
        {
            // Check for nearest character to target for movement
            OverlayTileBehaviour nearestCharacterTile = null;
            float nearestDistance = float.MaxValue;
            foreach (var tile in MapManager.Instance.allTiles)
            {
                if (tile.hasCharacter)
                {
                    float distance = Vector3.Distance(tile.transform.position, MapManager.Instance.enemyList[i].transform.position);

                    if (distance < nearestDistance)
                    {
                        nearestDistance = distance;
                        nearestCharacterTile = tile;
                        MapManager.Instance.enemyList[i].targetTile = tile;
                    }
                }
            }
            if (nearestCharacterTile == null)
            {
                // A check can be inserted here to see if all the characters are dead
                // If no characters left, lose.
                isInCombat = false;
                SceneManager.LoadScene("MainMenu");
            }

            // Find the path towards the decided target if not in range
            if (MapManager.Instance.enemyList[i].InAttackRange(nearestCharacterTile) == false)
            {
                enemyPath[i] = (pathfinder.FindPathForEnemy(MapManager.Instance.enemyList[i].activeTile, nearestCharacterTile, MapManager.Instance.allTiles,
                MapManager.Instance.enemyList[i].movementRange));
                // Change sprite direction
                if (nearestCharacterTile.transform.position.x <= MapManager.Instance.enemyList[i].transform.position.x)
                {
                    MapManager.Instance.enemyList[i].GetComponent<SpriteRenderer>().sprite = MapManager.Instance.enemyList[i].reverseSprite;
                    MapManager.Instance.enemyList[i].directionIndicator = 0;
                }
                else
                {
                    MapManager.Instance.enemyList[i].GetComponent<SpriteRenderer>().sprite = MapManager.Instance.enemyList[i].normalSprite;
                    MapManager.Instance.enemyList[i].directionIndicator = 1;
                }
            }
            // Else declare that the enemy should attack
            else
            {
                MapManager.Instance.enemyList[i].shouldAttack = true;
            }
        }
        AddCPImage();
        CP = maxCP;
        timeLimit = originalTime;
    }


    public void EndTurn()
    {
        playerTurn = false;
        TempEnemyTurn();
        //Give back the CP
        foreach (var character in MapManager.Instance.playerCharacters)
        {
            character.currentFuel++;
        }
    }

    public void StartCombat()
    {
        isInCombat = true;
        playerTurn = true;
        CP = maxCP;
        Debug.Log("Level start");
    }

    public void ChangeCP(int amount)
    {
        CP -= amount;
    }
    public int GetCP()
    {
        return CP;
    }
    public void SetTimeLimit(float newLimit)
    {
        timeLimit = newLimit;
    }

    public float GetTimeLimit()
    {
        return timeLimit;
    }

    public void DeleteCPImage(int CPImageCount)
    {
        int imageCountReal = CPUIField.transform.childCount;
        for (int i = 0; i < CPImageCount; i++)
        {
            int highestChildIndex = imageCountReal - 1;
            GameObject toDestroy = CPUIField.transform.GetChild(highestChildIndex).gameObject;
            Destroy(toDestroy);
            imageCountReal--;
        }
    }
    public void AddCPImage()
    {
        int amount = maxCP - CP;
        int CPUIOffset = 70;
        for (int i = 0; i < amount; i++)
        {
            Image CPImage = Instantiate(CPUI);
            CPImage.transform.SetParent(CPUIField.transform);
            CPImage.transform.position = new Vector3(CPUIField.transform.position.x + 75f + (i * CPUIOffset), CPUIField.transform.position.y, 0);
        }
    }
    public void AddAmountedCPImage(int number)
    {
        int amount = number;
        int CPUIOffset = 70;
        for (int i = 0; i < amount; i++)
        {
            Image CPImage = Instantiate(CPUI);
            CPImage.transform.SetParent(CPUIField.transform);
            CPImage.transform.position = new Vector3(CPUIField.transform.position.x + 75f + (i * CPUIOffset), CPUIField.transform.position.y, 0);
        }
    }

    public void DoDamageToCharacter(CharacterBehaviour character, int damage)
    {
        Debug.Log(damage);
        character.HP -= damage - character.defence;
        Debug.Log(damage - character.defence);
        character.healthBar.SetHealth(character.HP);
        camera.CameraShake();
    }

    // Code for helping enemies to move
    private void MoveAlongEnemyPath(EnemyBehaviour character, OverlayTileBehaviour targetTile)
    {
        character.activeTile.hasEnemy = false;

        float speed = 1.5f;
        var step = speed * Time.deltaTime;
        var zIndex = targetTile.transform.position.z;

        character.transform.position = Vector2.MoveTowards(character.transform.position, targetTile.transform.position, step);
        character.transform.position = new Vector3(character.transform.position.x, character.transform.position.y, zIndex);

        //if (Vector2.Distance(character.transform.position, enemyPath[0].transform.position) < 0.001f)
        //{
        //    PositionCharacter(character, enemyPath[0]);
        //    enemyPath.RemoveAt(0);
        //}

        //if (enemyPath.Count == 0)
        //{
        //    return;
        //}
    }
    private void PositionCharacter(EnemyBehaviour character, OverlayTileBehaviour overlayTile)
    {
        character.transform.position = new Vector3(overlayTile.transform.position.x, overlayTile.transform.position.y + 0.0001f, overlayTile.transform.position.z);
        character.GetComponent<SpriteRenderer>().sortingOrder = overlayTile.GetComponent<SpriteRenderer>().sortingOrder + 1;
        character.activeTile = overlayTile;
        character.activeTile.hasEnemy = true;
        character.gridLocation = (character.activeTile.gridLocation);
    }

    public Transform GetLowestChild(Transform parent)
    {
        Transform lowestChild = null;
        float lowestY = float.MaxValue;

        for (int i = 0; i < parent.childCount; i++)
        {
            Transform child = parent.GetChild(i);
            if (child.position.y < lowestY)
            {
                lowestY = child.position.y;
                lowestChild = child;
            }
        }

        return lowestChild;
    }

    public void DestroyTiles()
    {
        foreach (Transform map in gridMap.transform)
        {
            Destroy(map.gameObject);
        }
        foreach (Transform tileChild in tileContainer.transform)
        {
            Destroy(tileChild.gameObject);
        }
    }

    // Methods for giving the equipment buffs
    public void GiveEquipmentBuff(CharacterBehaviour character, EquipmentBehaviour equipment)
    {
        switch (equipment.equipmentScriptable.equipmentName)
        {
            case "Pop Shells":
                character.attackIncrease += 1;
                break;
            case "Steel Plating":
                character.defence += 1;
                break;
            case "Fuel Tank":
                character.maxFuel += 15;
                character.currentFuel += 15;
                break;
            default:
                Debug.Log("Equipment buff error");
                break;
        }
    }
    public void RemoveEquipmentBuff(CharacterBehaviour character, EquipmentBehaviour equipment)
    {
        switch (equipment.equipmentScriptable.equipmentName)
        {
            case "Pop Shells":
                character.attackIncrease -= 1;
                break;
            case "Steel Plating":
                character.defence -= 1;
                break;
            case "Fuel Tank":
                character.maxFuel -= 15;
                character.currentFuel -= 15;
                break;
            default:
                Debug.Log("Equipment buff error");
                break;
        }
    }
}
