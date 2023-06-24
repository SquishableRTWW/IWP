using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class Manager : MonoBehaviour
{
    private static Manager _instance;
    public static Manager Instance { get { return _instance; } }


    public MouseController mouseController;
    [SerializeField] TextMeshProUGUI timerText;
    [SerializeField] TextMeshProUGUI infoText;
    [SerializeField] TextMeshProUGUI CPText;
    [SerializeField] float timeLimit;
    [SerializeField] float originalTime;
    [SerializeField] int CP;
    public bool isInCombat = true;
    public bool playerTurn = true;
    [SerializeField] TimerBar TimerSlider;

    private Pathfinder pathfinder;
    private MoveRangeFinder moveRangeFinder;
    public List<OverlayTileBehaviour> enemyPath;

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
        timeLimit = originalTime;
        TimerSlider.SetMaxTime(timeLimit);
        CP = 2;
    }

    // Update is called once per frame
    void Update()
    {
        if (isInCombat)
        {
            if (playerTurn)
            {
                timeLimit -= Time.deltaTime;
            }
            timerText.text = string.Format("{0:0.00}", timeLimit);
            CPText.text = "CP Left: " + CP.ToString();
            TimerSlider.SetTime(timeLimit);
        }
        if (timeLimit <= 0)
        {
            // Insert function call to start enemy AI here;
            playerTurn = false;
            TempEnemyTurn();
            timeLimit = originalTime;

        }

        if (!playerTurn)
        {
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
                        }
                    }
                }
                if (nearestCharacterTile == null)
                {
                    // A check can be inserted here to see if all the characters are dead
                }

                // Find the path towards the decided target
                enemyPath = pathfinder.FindPathForEnemy(MapManager.Instance.enemyList[i].activeTile, nearestCharacterTile, MapManager.Instance.allTiles,
                MapManager.Instance.enemyList[i].movementRange);

                // Initiate the movement
                if (enemyPath.Count > 1)
                {
                    MoveAlongEnemyPath(MapManager.Instance.enemyList[i]);
                }

            }

            if (enemyPath.Count == 0)
            {
                playerTurn = true;
            }
        }
    }

    public void TempEnemyTurn()
    {
        infoText.text = "Enemy turn. Wait 3 S";
        MapManager.Instance.turnEnded();
        mouseController.DeselectAction();

        // enemy movement code is in update.
        CP = 2;
        timeLimit = originalTime;
    }
    public void EndTurn()
    {
        playerTurn = false;
        TempEnemyTurn();
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

    // Code for helping enemies to move
    private void MoveAlongEnemyPath(EnemyBehaviour character)
    {
        character.activeTile.hasEnemy = false;

        int speed = 3;
        var step = speed * Time.deltaTime;
        var zIndex = enemyPath[0].transform.position.z;
        character.transform.position = Vector2.MoveTowards(character.transform.position, enemyPath[0].transform.position, step);
        character.transform.position = new Vector3(character.transform.position.x, character.transform.position.y, zIndex);

        if (Vector2.Distance(character.transform.position, enemyPath[0].transform.position) < 0.001f)
        {
            PositionCharacter(character, enemyPath[0]);
            enemyPath.RemoveAt(0);
        }

        if (enemyPath.Count == 0)
        {
            return;
        }
    }
    private void PositionCharacter(EnemyBehaviour character, OverlayTileBehaviour overlayTile)
    {
        character.transform.position = new Vector3(overlayTile.transform.position.x, overlayTile.transform.position.y + 0.0001f, overlayTile.transform.position.z);
        character.GetComponent<SpriteRenderer>().sortingOrder = overlayTile.GetComponent<SpriteRenderer>().sortingOrder + 1;
        character.activeTile = overlayTile;
        character.activeTile.hasEnemy = true;
        character.gridLocation = (character.activeTile.gridLocation);
    }
}
