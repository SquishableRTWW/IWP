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
    public Canvas CPUIField;
    public Canvas combatCanvas;
    public Canvas prepCanvas;
    public Canvas eventCanvas;
    public Button endTurnButton;
    public HealthBar sheetHPBar;
    public FuelBar sheetFuelBar;

    public GameObject warningGUI;
    public GameObject tutorialBox;
    public List<string> ttTextList;
    public int tutorialNumber;
    public int maxTutorial;
    public bool tempTutorialPause;

    int CPUIOffset;

    [SerializeField] Image CPUI;
    public Image CPBarIcon;
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
        maxfuelPool = 20;
        fuelPool = maxfuelPool;
        CPUIOffset = 55;

        // Tutorial Set-Up
        tutorialNumber = 0;
        maxTutorial = 16;
        ttTextList = new List<string>();
        if (StateNameController.isInTutorial)
        {
            // Start tutorial
            SpawnNextTutorial(tutorialNumber);
        }


        isInCombat = false;
    }

    // Update is called once per frame
    void Update()
    {
        // Check to see if player lost all mechs
        if (MapManager.Instance.playerCharacters.Count <= 0)
        {
            // End the game and send them to the losing screen
            isInCombat = false;
            StateNameController.gameResult = "You Lose.";
            SoundManager.Instance.ChangeMusic(SoundManager.Sound.CalmBGM);
            SceneManager.LoadScene("WinLose");

        }

        // Check to see if there are any enemies left
        if (MapManager.Instance.enemyList.Count <= 0)
        {
            // If there are no enemies left, end the level
            SoundManager.Instance.ChangeMusic(SoundManager.Sound.CalmBGM);
            if (MapManager.Instance.level % 4 != 0)
            {
                MapManager.Instance.level++;
            }
            else
            {
                MapManager.Instance.levelTier++;
                MapManager.Instance.level = 1;
            }

            // Adding more difficult enemy on tier 2
            if (MapManager.Instance.level >= 2)
            {
                MapManager.Instance.possibleEnemies.Add(MapManager.Instance.enemies[2]);
            }
            // Winning the game if its tier 4 level 1
            if (MapManager.Instance.levelTier == 4)
            {
                // End the game and send them to the winning screen
                isInCombat = false;
                StateNameController.gameResult = "You Win!";
                SoundManager.Instance.ChangeMusic(SoundManager.Sound.CalmBGM);
                SceneManager.LoadScene("WinLose");
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
                if (character.currentFuel < character.maxFuel)
                {
                    character.currentFuel++;
                }
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
        if (playerTurn)
        {
            endTurnButton.gameObject.SetActive(true);
        }

        if (CP <= 0)
        {
            CPBarIcon.color = new Color(1f, 0f, 0f, 0.7f);
        }
        else
        {
            CPBarIcon.color = new Color(1f, 1f, 1f, 1f);
        }

        // For CP Fade in and fade out
        if (mouseController.attackSelected)
        {
            int imageCountReal = CPUIField.transform.childCount;
            if (mouseController.character.weaponsEquipped[mouseController.WeaponSelected].GetComponent<WeaponBehaviour>().GetCPCost() <= 1)
            {
                int highestChildIndex = imageCountReal - 1;
                CPUIField.transform.GetChild(highestChildIndex).gameObject.GetComponent<CPPipBehaviour>().onFade = true;
            }
            else
            {
                for (int i = mouseController.character.weaponsEquipped[mouseController.WeaponSelected].GetComponent<WeaponBehaviour>().GetCPCost() - 1; i >= 0; i--)
                {
                    CPUIField.transform.GetChild(i).gameObject.GetComponent<CPPipBehaviour>().onFade = true;
                }
            }
        }
        else
        {
            foreach (Transform child in CPUIField.transform)
            {
                child.gameObject.GetComponent<CPPipBehaviour>().onFade = false;
                child.gameObject.GetComponent<Image>().color = new Color(1, 1, 1, 1);
            }
        }

        // Tutorial check
        if (tutorialNumber >= maxTutorial || MapManager.Instance.level > 1)
        {
            StateNameController.isInTutorial = false;
        }
        if (StateNameController.isInTutorial && isInCombat)
        {
            if (tutorialNumber < 11)
            {
                endTurnButton.gameObject.SetActive(false);
            }
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
                        SoundManager.Instance.PlaySound(SoundManager.Sound.CharacterMove2);
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
            if (character.currentFuel < character.maxFuel)
            {
                character.currentFuel++;
            }
        }
        endTurnButton.gameObject.SetActive(false);
    }

    public void StartCombat()
    {
        SoundManager.Instance.PlaySound(SoundManager.Sound.ButtonClick);
        foreach (var character in MapManager.Instance.playerCharacters)
        {
            if (character.currentFuel <= 0)
            {
                warningGUI.SetActive(true);
                return;
            }
            foreach (var weapon in character.weaponsEquipped)
            {
                if (weapon == null)
                {
                    warningGUI.SetActive(true);
                    return;
                }
            }
        }

        isInCombat = true;
        playerTurn = true;
        CP = maxCP;
        AddAmountedCPImage(maxCP);
        //Debug.Log("Level start");
        SoundManager.Instance.ChangeMusic(SoundManager.Sound.CombatBGM);
    }
    public void ConfirmStart()
    {
        isInCombat = true;
        playerTurn = true;
        CP = maxCP;
        AddAmountedCPImage(maxCP);
        //Debug.Log("Level confirm start");
        SoundManager.Instance.ChangeMusic(SoundManager.Sound.CombatBGM);
    }
    public void CancelStart()
    {
        warningGUI.SetActive(false);
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
        for (int i = 0; i < amount; i++)
        {
            Image CPImage = Instantiate(CPUI);
            CPImage.transform.SetParent(CPUIField.transform);
            CPImage.transform.position = new Vector3(CPUIField.transform.position.x + 80f + (i * CPUIOffset), CPUIField.transform.position.y, 0);
        }
    }
    public void AddAmountedCPImage(int number)
    {
        int amount = number;
        for (int i = 0; i < amount; i++)
        {
            Image CPImage = Instantiate(CPUI);
            CPImage.transform.SetParent(CPUIField.transform);
            CPImage.transform.position = new Vector3(CPUIField.transform.position.x + 80f + (i * CPUIOffset), CPUIField.transform.position.y, 0);
        }
    }

    public void DoDamageToCharacter(CharacterBehaviour character, int damage)
    {
        character.HP -= damage - character.defence;
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

        if (enemyPath.Count == 0)
        {
            SoundManager.Instance.StopSound();
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
                PrepPhaseManager.Instance.characterFuelbar.SetBarLimit(character.maxFuel);
                PrepPhaseManager.Instance.characterFuelbar.SetFuel(character.currentFuel);
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
                PrepPhaseManager.Instance.characterFuelbar.SetBarLimit(character.maxFuel);
                PrepPhaseManager.Instance.characterFuelbar.SetFuel(character.currentFuel);
                break;
            default:
                Debug.Log("Equipment buff error");
                break;
        }
    }

    public void SpawnNextTutorial(int number)
    {
        GameObject nextTutorial = Instantiate(tutorialBox);
        switch(number)
        {
            case 0:
                nextTutorial.transform.SetParent(prepCanvas.transform);
                nextTutorial.GetComponent<RectTransform>().anchoredPosition = new Vector3(-465f, 0f, 0f);
                nextTutorial.GetComponentInChildren<TextMeshProUGUI>().text = "My name is Jeff. Let Jeff show you how to prepare your team before each mission. What you see here is the preparation menu.";
                break;
            case 1:
                nextTutorial.transform.SetParent(prepCanvas.transform);
                nextTutorial.transform.position = PrepPhaseManager.Instance.changeButtons[0].transform.position;
                nextTutorial.GetComponentInChildren<TextMeshProUGUI>().text = "Left click on your mech(s) here to select and customize them.";
                break;
            case 2:
                nextTutorial.transform.SetParent(prepCanvas.transform);
                nextTutorial.transform.position = PrepPhaseManager.Instance.weaponSlots[0].transform.position;
                nextTutorial.GetComponentInChildren<TextMeshProUGUI>().text = "You will be given FUEL each mission shown in the bar on the top left. You must assign fuel among your mechs before each mission";
                break;
            case 3:
                nextTutorial.transform.SetParent(prepCanvas.transform);
                nextTutorial.transform.position = PrepPhaseManager.Instance.fuelButtons[0].transform.position;
                nextTutorial.GetComponentInChildren<TextMeshProUGUI>().text = "Mechs NEED FUEL to move on the battlefield. Use these 4 small grey buttons to assign it to them. REMEMBER AH.";
                break;
            case 4:
                nextTutorial.transform.SetParent(prepCanvas.transform);
                nextTutorial.transform.position = PrepPhaseManager.Instance.weaponSlots[0].transform.position;
                nextTutorial.GetComponentInChildren<TextMeshProUGUI>().text = "Hover over your mech's weapons and equipment to see their details. As you collect items in your inventory, you can swap out your mech's gear via drag and drop.";
                break;
            case 5:
                nextTutorial.transform.SetParent(prepCanvas.transform);
                nextTutorial.GetComponent<RectTransform>().anchoredPosition = new Vector3(0f, 0f, 0f);
                nextTutorial.GetComponentInChildren<TextMeshProUGUI>().text = "You can press the 'Show-Level' button to scout out how the level will look like.";
                break;
            case 6:
                nextTutorial.transform.SetParent(prepCanvas.transform);
                nextTutorial.GetComponent<RectTransform>().anchoredPosition = new Vector3(0f, 0f, 0f);
                nextTutorial.GetComponentInChildren<TextMeshProUGUI>().text = "Take your time to explore the preparation menu. Once you're done prepping your team with fuel, press START to begin the level";
                break;
            case 7:
                nextTutorial.transform.SetParent(combatCanvas.transform);
                nextTutorial.GetComponent<RectTransform>().anchoredPosition = new Vector3(-465f, -500f, 0f);
                nextTutorial.GetComponentInChildren<TextMeshProUGUI>().text = "You can L-Click objects to view them. Left click your mech on the battlefield.";
                break;
            case 8:
                nextTutorial.transform.SetParent(combatCanvas.transform);
                nextTutorial.GetComponent<RectTransform>().anchoredPosition = new Vector3(-465f, -500f, 0f);
                nextTutorial.GetComponentInChildren<TextMeshProUGUI>().text = "On the left are move and attack buttons. Click MOVE to see where your character can move.";
                break;
            case 9:
                nextTutorial.transform.SetParent(combatCanvas.transform);
                nextTutorial.GetComponent<RectTransform>().anchoredPosition = new Vector3(-465f, -500f, 0f);
                nextTutorial.GetComponentInChildren<TextMeshProUGUI>().text = "GREEN = ok, ORANGE = mech is OVERHEATED. L-CLICK to move onto any white tile. Fuel is consumed equal to movement amount.";
                break;
            case 10:
                nextTutorial.transform.SetParent(combatCanvas.transform);
                nextTutorial.GetComponent<RectTransform>().anchoredPosition = new Vector3(-465f, -500f, 0f);
                nextTutorial.GetComponentInChildren<TextMeshProUGUI>().text = "OVERHEATED mechs have an orange hue. They can't move/attack for the rest of the turn. 1 Fuel point is regained each turn.";
                break;
            case 11:
                nextTutorial.transform.SetParent(combatCanvas.transform);
                nextTutorial.GetComponent<RectTransform>().anchoredPosition = new Vector3(-465f, -500f, 0f);
                nextTutorial.GetComponentInChildren<TextMeshProUGUI>().text = "If your mech is OVERHEATED, you will not see the option to attack/move. Regardless, END the turn first and let the enemy move";
                break;
            case 12:
                nextTutorial.transform.SetParent(combatCanvas.transform);
                nextTutorial.GetComponent<RectTransform>().anchoredPosition = new Vector3(-465f, -500f, 0f);
                nextTutorial.GetComponentInChildren<TextMeshProUGUI>().text = "When its your turn, click on your mech again and HOVER over your ATTACK button to read about the attack";
                break;
            case 13:
                nextTutorial.transform.SetParent(combatCanvas.transform);
                nextTutorial.GetComponent<RectTransform>().anchoredPosition = new Vector3(-465f, -500f, 0f);
                nextTutorial.GetComponentInChildren<TextMeshProUGUI>().text = "Select the ATTACK to see its range. If an enemy is in range, click on them to attack them. If not just cancel it. You'll gettem later.";
                break;
            case 14:
                nextTutorial.transform.SetParent(combatCanvas.transform);
                nextTutorial.GetComponent<RectTransform>().anchoredPosition = new Vector3(-465f, -500f, 0f);
                nextTutorial.GetComponentInChildren<TextMeshProUGUI>().text = "Attacks cost COMMAND POINTS (CP), shown on the TOP of your screen in yellow bars, notice the bars that flash are the ones that you will consume. You start with 2 and they replenish each turn.";
                break;
            case 15:
                nextTutorial.transform.SetParent(combatCanvas.transform);
                nextTutorial.GetComponent<RectTransform>().anchoredPosition = new Vector3(-465f, -500f, 0f);
                nextTutorial.GetComponentInChildren<TextMeshProUGUI>().text = "You are on your own now. Do what it takes to destroy all the enemies. Good luck!";
                break;
            default:
                Debug.Log("Tutorial Ended.");
                break;
        }
    }
}
