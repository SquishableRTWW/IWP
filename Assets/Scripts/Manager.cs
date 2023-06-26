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
    public Canvas CPUIField;
    [SerializeField] Image CPUI;
    [SerializeField] float timeLimit;
    [SerializeField] float originalTime;
    [SerializeField] int CP;
    public bool isInCombat = true;
    public bool playerTurn = true;
    [SerializeField] TimerBar TimerSlider;

    private Pathfinder pathfinder;
    private MoveRangeFinder moveRangeFinder;
    public List<List<OverlayTileBehaviour>> enemyPath;
    public new CameraController camera;

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

        int CPUIOffset = 0;
        for (int i = 0; i < CP; i++)
        {
            Image CPImage = Instantiate(CPUI);
            CPImage.transform.SetParent(CPUIField.transform);
            CPImage.transform.position = new Vector3(CPUIField.transform.position.x + CPUIOffset, CPUIField.transform.position.y, 0);
            CPUIOffset -= 70;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (enemyPath.Count < MapManager.Instance.enemyList.Count)
        {
            enemyPath.Add(new List<OverlayTileBehaviour>());
        }

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
            StartMovingEnemies();

            //Give back the CP
            AddCPImage();
        }
    }

    private void StartMovingEnemies()
    {
        StartCoroutine(MoveEnemiesSequentially());
    }

    private IEnumerator MoveEnemiesSequentially()
    {
        int enemyToMove = 0;
        for (int i = 0; i < enemyPath.Count; i++)
        {
            EnemyBehaviour enemy = MapManager.Instance.enemyList[i];
            List<OverlayTileBehaviour> path = enemyPath[i];
            //StartCoroutine(camera.ZoomAtCharacter(enemy.transform.position));
            yield return new WaitForSeconds(1.0f);

            while (path.Count > 0)
            {
                OverlayTileBehaviour nextTile = path[0];
                MoveAlongEnemyPath(enemy, nextTile);

                yield return new WaitUntil(() => Vector2.Distance(enemy.transform.position, nextTile.transform.position) < 0.001f);
                i = enemyToMove - 1;

                PositionCharacter(enemy, nextTile);

                path.RemoveAt(0);

                if (path.Count == 0)
                {
                    enemyToMove++;
                    i = enemyToMove - 1;
                }
            }

            if (i == enemyPath.Count - 1)
            {
                playerTurn = true;
            }
        }
    }

    public void TempEnemyTurn()
    {
        infoText.text = "Enemy turn.";
        MapManager.Instance.turnEnded();
        mouseController.DeselectAction();

        // enemy movement code is in update.
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
            enemyPath[i] = (pathfinder.FindPathForEnemy(MapManager.Instance.enemyList[i].activeTile, nearestCharacterTile, MapManager.Instance.allTiles,
            MapManager.Instance.enemyList[i].movementRange));
        }
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

    public void DeleteCPImage(int CPImageCount)
    {
        for (int i = 0; i < CPImageCount; i++)
        {
            GameObject toDestroy = CPUIField.transform.GetChild(i).gameObject;
            Destroy(toDestroy);
        }
    }
    public void AddCPImage()
    {
        int CPUIOffset = 0;
        for (int i = 0; i < CP; i++)
        {
            Image CPImage = Instantiate(CPUI);
            CPImage.transform.SetParent(CPUIField.transform);
            CPImage.transform.position = new Vector3(CPUIField.transform.position.x + CPUIOffset - 200f, CPUIField.transform.position.y, 0);
            CPUIOffset += 70;
        }
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
}
