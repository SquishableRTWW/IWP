using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MouseController : MonoBehaviour
{
    public float speed;
    // List of tiles for character's movement path
    private List<OverlayTileBehaviour> path = new List<OverlayTileBehaviour>();
    // List of tiles for character's in-range tiles
    public List<OverlayTileBehaviour> inRangeTiles = new List<OverlayTileBehaviour>();
    // List of tiles that are part of the attack path
    private List<OverlayTileBehaviour> attackTiles = new List<OverlayTileBehaviour>();

    private Pathfinder pathfinder;
    private MoveRangeFinder moveRangeFinder;

    public GameObject characterPrefab;
    public CharacterBehaviour character;

    // GUI Elements
    [SerializeField] private Button moveButton;
    [SerializeField] private Button cancelButton;

    [SerializeField] private Canvas characterSheet;

    [SerializeField] private Button attack1Button;
    [SerializeField] private Button attack2Button;
    public Button turnEndButton;
    [SerializeField] CameraController sceneCameraController;
    public HealthBar characterSheetHealthbar;
    public FuelBar characterSheetFuelbar;

    private bool isMoving = false;
    public int WeaponSelected = 0;
    public bool movementSelected = false;
    public bool attackSelected = false;
    public LayerMask layerMask;
    public bool attackJustSelected;

    private void Start()
    {
        pathfinder = new Pathfinder();
        moveRangeFinder = new MoveRangeFinder();
    }

    // Update is called once per frame
    void Update()
    {
        var focusedTiledHit = GetFocusedOnTile();


        if (focusedTiledHit.HasValue)
        {
            OverlayTileBehaviour overlayTile = focusedTiledHit.Value.collider.gameObject.GetComponent<OverlayTileBehaviour>();
            transform.position = overlayTile.transform.position;
            gameObject.GetComponent<SpriteRenderer>().sortingOrder = overlayTile.GetComponent<SpriteRenderer>().sortingOrder;

            // Logic to pathfind and show tiles should the character be able to move
            if (character != null && inRangeTiles.Contains(overlayTile) && !isMoving && !character.isOverheated)
            {
                if (movementSelected == true)
                {
                    path = pathfinder.FindPath(character.activeTile, overlayTile, inRangeTiles);

                    // Change sprite direction
                    if (overlayTile.transform.position.x >= character.transform.position.x)
                    {
                        character.GetComponent<SpriteRenderer>().sprite = character.normalSprite;
                        character.directionIndicator = 1;
                    }
                    else
                    {
                        character.GetComponent<SpriteRenderer>().sprite = character.reverseSprite;
                        character.directionIndicator = 0;
                        Debug.Log("Reversed ssprite");
                    }

                    GetInRangeTiles();

                    for (int i = 0; i < path.Count; i++)
                    {
                        if (path.Count >= (character.overheatAmount - character.currentHeat))
                        {
                            path[i].ShowOverheatTile();
                        }
                        else
                        {
                            path[i].ShowTileInPath();
                        }
                    }
                }
            }
            // Logic for constant updating of attack range, area of effect and location
            if (character != null)
            {
                if (attackSelected == true)
                {
                    GetAttackRangeTiles();
                    switch (character.weaponsEquipped[WeaponSelected].GetComponent<WeaponBehaviour>().GetShotType())
                    {
                        case "Linear":
                            attackTiles = pathfinder.FindLinearAttackPath(character.activeTile, overlayTile, character.weaponsEquipped[WeaponSelected].GetComponent<WeaponBehaviour>().GetWeaponRange(), inRangeTiles);
                            break;
                        case "Laser1":
                            attackTiles = pathfinder.FindSingleLazerAttackPath(character.activeTile, overlayTile, character.weaponsEquipped[WeaponSelected].GetComponent<WeaponBehaviour>().GetWeaponRange(), inRangeTiles);
                            break;
                        case "Lobbing":
                            attackTiles = pathfinder.FindAOEAttackPath(overlayTile, (int)character.weaponsEquipped[WeaponSelected].GetComponent<WeaponBehaviour>().GetAttackPattern().x, inRangeTiles);
                            break;
                        case "Across":
                            attackTiles = pathfinder.FindAcrossAttackPath(character.activeTile, overlayTile, (int)character.weaponsEquipped[WeaponSelected].GetComponent<WeaponBehaviour>().GetAttackPattern().x,
                                character.weaponsEquipped[WeaponSelected].GetComponent<WeaponBehaviour>().GetWeaponRange(), inRangeTiles);
                            break;
                        case "Single":
                            attackTiles = pathfinder.FindSingleAttackPath(character.activeTile, overlayTile, (int)character.weaponsEquipped[WeaponSelected].GetComponent<WeaponBehaviour>().GetAttackPattern().x,
                                character.weaponsEquipped[WeaponSelected].GetComponent<WeaponBehaviour>().GetWeaponRange(), inRangeTiles);
                            break;
                        default:
                            attackTiles = pathfinder.FindLinearAttackPath(character.activeTile, overlayTile, character.weaponsEquipped[WeaponSelected].GetComponent<WeaponBehaviour>().GetWeaponRange(), inRangeTiles);
                            break;
                    }
                    

                    for (int i = 0; i < attackTiles.Count; i++)
                    {
                        attackTiles[i].ShowAttackTile();
                    }
                }
            }

            if (Input.GetMouseButtonDown(0) && Manager.Instance.playerTurn)
            {

                if (path.Count > 0)
                {
                    isMoving = true;
                }
                else if (attackTiles.Count > 0 && attackSelected == true)
                {
                    // Change sprite direction
                    if (overlayTile.transform.position.x >= character.transform.position.x)
                    {
                        character.GetComponent<SpriteRenderer>().sprite = character.normalSprite;
                        character.directionIndicator = 1;
                    }
                    else
                    {
                        character.GetComponent<SpriteRenderer>().sprite = character.reverseSprite;
                        character.directionIndicator = 0;
                        Debug.Log("Reversed ssprite");
                    }

                    bool hasEnemy = false;
                    bool hasCharacter = false;
                    foreach (OverlayTileBehaviour tile in attackTiles)
                    {
                        if (tile.hasEnemy || tile.entity != null)
                        {
                            hasEnemy = true;
                        }
                        if (tile.hasCharacter)
                        {
                            hasCharacter = true;
                        }
                    }
                    if (character.weaponsEquipped[WeaponSelected].GetComponent<WeaponBehaviour>().GetWeaponName() == "Barreler" && !overlayTile.hasCharacter && !overlayTile.hasEnemy)
                    {
                        SpawnEntity(MapManager.Instance.entitiesInGame[2], overlayTile);
                        Manager.Instance.ChangeCP(character.weaponsEquipped[WeaponSelected].GetComponent<WeaponBehaviour>().GetCPCost());
                        Manager.Instance.DeleteCPImage(character.weaponsEquipped[WeaponSelected].GetComponent<WeaponBehaviour>().GetCPCost());
                    }
                    else if (hasEnemy)
                    {
                        // Play attack animation
                        character.shootingEffect = character.weaponsEquipped[WeaponSelected].GetComponent<WeaponBehaviour>().GetAnimation();
                        StartCoroutine(character.DoAttackAnimation(character.shootingEffect));
                        DoDamage();
                        Manager.Instance.ChangeCP(character.weaponsEquipped[WeaponSelected].GetComponent<WeaponBehaviour>().GetCPCost());
                        Manager.Instance.DeleteCPImage(character.weaponsEquipped[WeaponSelected].GetComponent<WeaponBehaviour>().GetCPCost());
                    }
                    else if (hasCharacter)
                    {
                        // Play attack animation
                        character.shootingEffect = character.weaponsEquipped[WeaponSelected].GetComponent<WeaponBehaviour>().GetAnimation();
                        StartCoroutine(character.DoAttackAnimation(character.shootingEffect));
                        DoHeal();
                        Manager.Instance.ChangeCP(character.weaponsEquipped[WeaponSelected].GetComponent<WeaponBehaviour>().GetCPCost());
                        Manager.Instance.DeleteCPImage(character.weaponsEquipped[WeaponSelected].GetComponent<WeaponBehaviour>().GetCPCost());
                    }
                    else
                    {
                        DeselectAction();
                    }
                }

                // Check to see if clicked on a character, enemy, entity etc:
                Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector2 mousePos2d = new Vector2(mousePos.x, mousePos.y);

                RaycastHit2D[] hits = Physics2D.RaycastAll(mousePos2d, Vector2.zero);
                if (hits.Count() == 1 && !movementSelected && character == null)
                {
                    DeselectAction();
                }
                foreach (RaycastHit2D hit in hits)
                {
                    GameObject objectHit = hit.collider.gameObject;

                    // Check if the collider is a trigger
                    if (hit.collider.isTrigger && !movementSelected && Manager.Instance.isInCombat)
                    {
                       
                        if (objectHit.CompareTag("Character") && !attackSelected)
                        {
                            // Set selected character as the clicked one
                            DeselectAction();
                            character = objectHit.GetComponent<CharacterBehaviour>();
                            // Zoom into character
                            //StartCoroutine(sceneCameraController.ZoomAtCharacter(character.transform.position));
                            // Logic for showing UI stuffs
                            if (character.isOverheated == false)
                            {
                                moveButton.gameObject.SetActive(true);
                            }
                            if (character.isOverheated == false)
                            {
                                if (character.weaponsEquipped[0] != null && Manager.Instance.CP >= character.weaponsEquipped[0].GetComponent<WeaponBehaviour>().GetCPCost())
                                {
                                    attack1Button.gameObject.SetActive(true);
                                    attack1Button.gameObject.GetComponent<Image>().sprite = character.weaponsEquipped[0].GetComponent<WeaponBehaviour>().GetAttackSprite();
                                }
                                if (character.weaponsEquipped.Count > 1 && character.weaponsEquipped[1] != null && Manager.Instance.CP >= character.weaponsEquipped[1].GetComponent<WeaponBehaviour>().GetCPCost())
                                {
                                    attack2Button.gameObject.SetActive(true);
                                    attack2Button.gameObject.GetComponent<Image>().sprite = character.weaponsEquipped[1].GetComponent<WeaponBehaviour>().GetAttackSprite();
                                }
                                else
                                {
                                    attack2Button.gameObject.SetActive(false);
                                }
                            }
                            // Update Character sheet UI
                            characterSheet.gameObject.SetActive(true);
                            characterSheet.gameObject.transform.Find("CharacterSheet_FuelBar").gameObject.SetActive(true);
                            characterSheet.gameObject.transform.Find("CharacterSheet_HPBar").gameObject.SetActive(true);
                            Manager.Instance.sheetHPBar.SetBarLimit(character.maxHP); Manager.Instance.sheetFuelBar.SetBarLimit(character.maxFuel);
                            characterSheet.gameObject.transform.Find("SpriteImage").GetComponent<Image>().sprite = character.GetComponent<SpriteRenderer>().sprite;
                            characterSheet.gameObject.GetComponentInChildren<TextMeshProUGUI>().text = character.characterName + "\n\nHP " + "          " + character.HP + "/" + character.maxHP
                                + "\n\nFUEL " + "      " + character.currentFuel + "/" + character.maxFuel;
                            characterSheetHealthbar.SetHealth(character.HP);
                            characterSheetFuelbar.SetMaxFuel(character.maxFuel);
                            characterSheetFuelbar.SetFuel(character.currentFuel);

                            // Change tool tip message for each weapon
                            for (int w = 0; w < character.weaponsEquipped.Count; w++)
                            {
                                if (character.weaponsEquipped[w] != null)
                                {
                                    Button AB;
                                    if (w == 0)
                                    {
                                        AB = attack1Button;
                                    }
                                    else if (w == 1)
                                    {
                                        AB = attack2Button;
                                    }
                                    else
                                    {
                                        AB = attack1Button;
                                    }
                                    switch (character.weaponsEquipped[w].GetComponent<WeaponBehaviour>().GetWeaponName())
                                    {
                                        case "Bolter":
                                            AB.GetComponent<ToolTip>().message = "A short range weapon that fires in a row of 3\n" + "DMG: 2\n" + "CP: 1";
                                            break;
                                        case "Grenade Launcher":
                                            AB.GetComponent<ToolTip>().message = "A long range weapon that bombards a large area\n" + "DMG: 4\n" + "CP: 2";
                                            break;
                                        case "Smoothbore":
                                            AB.GetComponent<ToolTip>().message = "A long range weapon that shoots in a straight line. Hits only first target.\n" + "DMG: 3\n" + "CP: 1";
                                            break;
                                        case "Laser Cannon":
                                            AB.GetComponent<ToolTip>().message = "A medium range weapon that straight down a line, hitting all targets.\n" + "DMG: 4\n" + "CP: 2";
                                            break;
                                        case "Aider":
                                            AB.GetComponent<ToolTip>().message = "A medium range weapon that heals target.\n" + "HEAL: 2\n" + "CP: 1";
                                            break;
                                        case "Barreler":
                                            AB.GetComponent<ToolTip>().message = "A short range weapon that spawns a barrel at location.\n" + "\n" + "CP: 2";
                                            break;
                                        case "Refueller":
                                            AB.GetComponent<ToolTip>().message = "A short range weapon that gives fuel to target ally.\n" + "REFUEL: 3\n" + "CP: 1";
                                            break;
                                        default:
                                            AB.GetComponent<ToolTip>().message = "Weapon error";
                                            break;
                                    }
                                }
                            }
                        }
                        else if (objectHit.CompareTag("Enemy"))
                        {
                            var enemy = objectHit.GetComponent<EnemyBehaviour>();
                            // Set selected character as the clicked one
                            DeselectCharacter();

                            // Update info sheet UI
                            characterSheet.gameObject.SetActive(true);
                            characterSheet.gameObject.transform.Find("CharacterSheet_FuelBar").gameObject.SetActive(false);
                            characterSheet.gameObject.transform.Find("CharacterSheet_HPBar").gameObject.SetActive(true);
                            Manager.Instance.sheetHPBar.SetBarLimit(enemy.maxHP);
                            characterSheet.gameObject.transform.Find("SpriteImage").GetComponent<Image>().sprite = enemy.GetComponent<SpriteRenderer>().sprite;
                            characterSheet.gameObject.GetComponentInChildren<TextMeshProUGUI>().text = enemy.characterName + ", Enemy";
                            characterSheetHealthbar.SetHealth(enemy.HP);

                            // Show enemy's attack range and movement range
                            if (enemy.enemyScriptable.movementRange > enemy.enemyScriptable.attackRange)
                            {
                                List<OverlayTileBehaviour> movementRangeTiles = moveRangeFinder.GetTilesInRange(enemy.activeTile, enemy.enemyScriptable.movementRange);
                                foreach (var tile in movementRangeTiles)
                                {
                                    tile.ShowEnemyMoveTile();
                                }
                                List<OverlayTileBehaviour> tilesInRange = moveRangeFinder.GetTilesInAttackRange(enemy.activeTile, enemy.enemyScriptable.attackRange);
                                foreach (var tile in tilesInRange)
                                {
                                    tile.ShowAttackTile();
                                }
                            }
                            else
                            {
                                List<OverlayTileBehaviour> tilesInRange = moveRangeFinder.GetTilesInAttackRange(enemy.activeTile, enemy.enemyScriptable.attackRange);
                                foreach (var tile in tilesInRange)
                                {
                                    tile.ShowAttackTile();
                                }
                                List<OverlayTileBehaviour> movementRangeTiles = moveRangeFinder.GetTilesInRange(enemy.activeTile, enemy.enemyScriptable.movementRange);
                                foreach (var tile in movementRangeTiles)
                                {
                                    tile.ShowEnemyMoveTile();
                                }
                            }

                            if (attackJustSelected)
                            {
                                DeselectAction();
                                attackJustSelected = false;
                            }
                        }
                        else if (objectHit.CompareTag("Entity"))
                        {
                            var rock = objectHit.GetComponent<EntityBehaviour>();
                            // Set selected character as the clicked one
                            DeselectAction();

                            characterSheet.gameObject.SetActive(true);
                            characterSheet.gameObject.transform.Find("CharacterSheet_FuelBar").gameObject.SetActive(false);
                            characterSheet.gameObject.transform.Find("CharacterSheet_HPBar").gameObject.SetActive(false);
                            characterSheet.gameObject.transform.Find("SpriteImage").GetComponent<Image>().sprite = rock.GetComponent<SpriteRenderer>().sprite;
                            characterSheet.gameObject.GetComponentInChildren<TextMeshProUGUI>().text = rock.entityScriptable.entityName + ": " + rock.entityScriptable.description;

                            if (rock.entityScriptable.type == "Barrel")
                            {
                                List<OverlayTileBehaviour> tilesInRange = MapManager.Instance.Get8DirectionTiles(rock.activeTile, 3);
                                foreach (var tile in tilesInRange)
                                {
                                    tile.ShowAttackTile();
                                }
                            }

                            if (attackJustSelected)
                            {
                                DeselectAction();
                                attackJustSelected = false;
                            }
                        }
                    }
                    else
                    {
                        // Do nothing
                    }
                }
            }
        }

        if (path.Count > 0 && isMoving)
        {
            SoundManager.Instance.PlaySound(SoundManager.Sound.CharacterMove2);
            turnEndButton.gameObject.SetActive(false);
            MoveAlongPath();
        }
    }

    private void GetInRangeTiles()
    {
        foreach (var tile in inRangeTiles)
        {
            tile.HideTile();
        }

        inRangeTiles = moveRangeFinder.GetTilesInRange(character.activeTile, character.currentFuel);

        foreach (var tile in inRangeTiles)
        {
            if (tile.isBlocked)
            {
                tile.ShowVoidTilePath();
            }
            //else if (inRangeTiles.IndexOf(tile) < (Mathf.Pow((character.overheatAmount - character.currentHeat), 2)))
            //{
            //    tile.ShowTile();
            //}
            else
            {
                tile.ShowTile();
            }
        }
    }
    private void GetAttackRangeTiles()
    {
        foreach (var tile in inRangeTiles)
        {
            tile.HideTile();
        }

        inRangeTiles = moveRangeFinder.GetTilesInAttackRange(character.activeTile, character.weaponsEquipped[WeaponSelected].GetComponent<WeaponBehaviour>().GetWeaponRange());

        foreach (var tile in inRangeTiles)
        {
            tile.ShowTile();
        }
    }

    // Logic for moving
    private void MoveAlongPath()
    {
        List<int> overheatValue = new List<int>();
        character.activeTile.hasCharacter = false;
        if (path.Count > character.overheatAmount && overheatValue.Count == 0)
        {
            character.finishedMove = true;
            character.isOverheated = true;
            overheatValue.Add(path.Count);
        }
        var step = speed * Time.deltaTime;
        var zIndex = path[0].transform.position.z;
        character.transform.position = Vector2.MoveTowards(character.transform.position, path[0].transform.position, step);
        character.transform.position = new Vector3(character.transform.position.x, character.transform.position.y, zIndex);

        if (Vector2.Distance(character.transform.position, path[0].transform.position) < 0.001f && character.currentFuel > 0)
        {
            PositionCharacter(path[0]);

            // Check for entities to activate
            if (path[0].entity != null)
            {
                if (path[0].entity.entityScriptable.type == "Crate")
                {
                    path[0].entity.ActivateEntity();
                }
            }

            path.RemoveAt(0);
            character.currentFuel -= 1;
            character.currentHeat++;
        }
        if (character.currentHeat >= character.overheatAmount)
        {
            character.isOverheated = true;
        }

        if (path.Count == 0)
        {
            SoundManager.Instance.StopSound();
            character.finishedMove = true;
            foreach (var tile in inRangeTiles)
            {
                tile.HideTile();
            }
            isMoving = false;
            DeselectAction();
            character = null;
            turnEndButton.gameObject.SetActive(true);
        }
    }

    // Logic for doing damage
    public void DoDamage()
    {
        SoundManager.Instance.PlaySound(character.weaponsEquipped[WeaponSelected].GetComponent<WeaponBehaviour>().weaponScriptable.soundEnum);
        List<OverlayTileBehaviour> tilesWithCharacters = new List<OverlayTileBehaviour>();
        List<OverlayTileBehaviour> tilesWithEntities = new List<OverlayTileBehaviour>();
        int affectedCount = 0;
        for (int i = 0; i < attackTiles.Count; i++)
        {
            if (attackTiles[i].hasEnemy)
            {
                tilesWithCharacters.Add(attackTiles[i]);
            }
            if (attackTiles[i].entity != null)
            {
                tilesWithEntities.Add(attackTiles[i]);
            }
        }
        // Handle enemy Damage
        for (int i = 0; i < tilesWithCharacters.Count; i++)
        {
            if (MapManager.Instance.enemyList[affectedCount] == null)
            {
                i = -1;
                continue;
            }
            if (MapManager.Instance.enemyList[affectedCount].grid2DLocation == tilesWithCharacters[i].grid2DLocation)
            {
                int damageDealt = character.weaponsEquipped[WeaponSelected].GetComponent<WeaponBehaviour>().GetWeaponDamage() + character.attackIncrease;
                MapManager.Instance.enemyList[affectedCount].HP -= damageDealt;
                MapManager.Instance.enemyList[affectedCount].healthBar.SetHealth(MapManager.Instance.enemyList[affectedCount].HP);
                StartCoroutine(MapManager.Instance.enemyList[affectedCount].ShowDamage(damageDealt.ToString()));
                affectedCount = 0;
                tilesWithCharacters.Remove(tilesWithCharacters[i]);
                //sceneCameraController.CameraShake();
                i = -1;
            }
            else
            {
                affectedCount++;
                i = -1;
            }
        }
        // Handle Entity activation
        for (int i = 0; i < tilesWithEntities.Count; i++)
        {
            if (tilesWithEntities[i].entity.entityScriptable.type == "Barrel")
            {
                SoundManager.Instance.PlaySound(SoundManager.Sound.DieSFX);
                tilesWithEntities[i].entity.ActivateEntity();
            }
        }
        
        DeselectAction();
        MapManager.Instance.HideAllTiles();
        attackJustSelected = true;
    }

    public void DoHeal()
    {
        SoundManager.Instance.PlaySound(character.weaponsEquipped[WeaponSelected].GetComponent<WeaponBehaviour>().weaponScriptable.soundEnum);
        List<OverlayTileBehaviour> tilesWithCharacters = new List<OverlayTileBehaviour>();
        int affectedCount = 0;
        for (int i = 0; i < attackTiles.Count; i++)
        {
            if (attackTiles[i].hasCharacter)
            {
                tilesWithCharacters.Add(attackTiles[i]);
            }
        }
        for (int i = 0; i < tilesWithCharacters.Count; i++)
        {
            if (MapManager.Instance.playerCharacters[affectedCount] == null)
            {
                i = -1;
                continue;
            }
            if (MapManager.Instance.playerCharacters[affectedCount].grid2DLocation == tilesWithCharacters[i].grid2DLocation)
            {
                int damageDealt = character.weaponsEquipped[WeaponSelected].GetComponent<WeaponBehaviour>().GetWeaponDamage();
                if (character.weaponsEquipped[WeaponSelected].GetComponent<WeaponBehaviour>().GetWeaponName() == "Aider")
                {
                    if (MapManager.Instance.playerCharacters[affectedCount].HP + damageDealt > MapManager.Instance.playerCharacters[affectedCount].maxHP)
                    {
                        MapManager.Instance.playerCharacters[affectedCount].HP = MapManager.Instance.playerCharacters[affectedCount].maxHP;
                    }
                    else
                    {
                        MapManager.Instance.playerCharacters[affectedCount].HP += damageDealt;
                    }
                    MapManager.Instance.playerCharacters[affectedCount].healthBar.SetHealth(MapManager.Instance.playerCharacters[affectedCount].HP);
                    StartCoroutine(MapManager.Instance.playerCharacters[affectedCount].ShowHeal(damageDealt.ToString()));
                    affectedCount = 0;
                    tilesWithCharacters.Remove(tilesWithCharacters[i]);
                    i = -1;
                }
                else if (character.weaponsEquipped[WeaponSelected].GetComponent<WeaponBehaviour>().GetWeaponName() == "Refueller")
                {
                    if (MapManager.Instance.playerCharacters[affectedCount].currentFuel + damageDealt > MapManager.Instance.playerCharacters[affectedCount].maxFuel)
                    {
                        MapManager.Instance.playerCharacters[affectedCount].currentFuel = MapManager.Instance.playerCharacters[affectedCount].maxFuel;
                    }
                    else
                    {
                        MapManager.Instance.playerCharacters[affectedCount].currentFuel += damageDealt;
                    }
                    StartCoroutine(MapManager.Instance.playerCharacters[affectedCount].ShowRefuel(damageDealt.ToString()));
                    affectedCount = 0;
                    tilesWithCharacters.Remove(tilesWithCharacters[i]);
                    i = -1;
                }
            }
            else
            {
                affectedCount++;
                i = -1;
            }
        }
        DeselectAction();
        attackJustSelected = true;
    }


    private void PositionCharacter(OverlayTileBehaviour overlayTile)
    {
        character.transform.position = new Vector3(overlayTile.transform.position.x, overlayTile.transform.position.y + 0.0001f, overlayTile.transform.position.z);
        character.GetComponent<SpriteRenderer>().sortingOrder = overlayTile.GetComponent<SpriteRenderer>().sortingOrder + 1;
        character.activeTile = overlayTile;
        character.activeTile.hasCharacter = true;
        character.gridLocation = (character.activeTile.gridLocation);
    }

    public RaycastHit2D? GetFocusedOnTile()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mousePos2d = new Vector2(mousePos.x, mousePos.y);

        RaycastHit2D[] hits = Physics2D.RaycastAll(mousePos2d, Vector2.zero, 999f, layerMask);

        if (hits.Length > 0)
        {
            return hits.Where(hit => hit.collider.CompareTag("OverlayTile")).OrderByDescending(i => i.collider.transform.position.z).First();
        }

        return null;
    }

    //Method to on the movement selection
    public void SelectMovement()
    {
        SoundManager.Instance.PlaySound(SoundManager.Sound.DragDropSFX);
        // For tutorial purposes
        if (StateNameController.isInTutorial && Manager.Instance.tutorialNumber == 8)
        {
            SoundManager.Instance.PlaySound(SoundManager.Sound.TutorialClick);
            Manager.Instance.tutorialNumber++;
            TTTextBoxBehaviour firstGameObject = FindObjectOfType<TTTextBoxBehaviour>();
            if (firstGameObject != null)
            {
                Destroy(firstGameObject.gameObject);
            }
            Manager.Instance.SpawnNextTutorial(Manager.Instance.tutorialNumber);
        }

        movementSelected = true;
        cancelButton.gameObject.SetActive(true);
        attack1Button.gameObject.SetActive(false);
        attack2Button.gameObject.SetActive(false);
        GetInRangeTiles();
    }
    // Method to cancel selected action (Function is names movement but this accounts for everything, attacking etc.)
    public void DeselectAction()
    {
        // Reset the booleans
        movementSelected = false;
        if (attackSelected)
        {
            attackSelected = false;
        }
        // Make buttons disappear
        moveButton.gameObject.SetActive(false);
        cancelButton.gameObject.SetActive(false);
        attack1Button.GetComponent<Image>().color = new Color(attack1Button.GetComponent<Image>().color.r, attack1Button.GetComponent<Image>().color.g
        , attack1Button.GetComponent<Image>().color.b, 0.5f);
        attack2Button.GetComponent<Image>().color = new Color(attack1Button.GetComponent<Image>().color.r, attack1Button.GetComponent<Image>().color.g
        , attack1Button.GetComponent<Image>().color.b, 0.5f);
        attack1Button.gameObject.SetActive(false);
        attack2Button.gameObject.SetActive(false);

        // Hide all the tiles (If not they will be stuck)
        foreach (var item in inRangeTiles)
        {
            item.HideTile();
        }
        foreach (var tile in MapManager.Instance.allTiles)
        {
            tile.HideTile();
        }

        // Clear the A* path of any remnants
        path.Clear();

        //Hide the character sheet
        characterSheet.gameObject.SetActive(false);
    }
    public void DeselectCharacter()
    {
        // Reset the booleans
        movementSelected = false;
        // Make buttons disappear
        moveButton.gameObject.SetActive(false);
        cancelButton.gameObject.SetActive(false);
        attack1Button.GetComponent<Image>().color = new Color(attack1Button.GetComponent<Image>().color.r, attack1Button.GetComponent<Image>().color.g
        , attack1Button.GetComponent<Image>().color.b, 0.5f);
        attack2Button.GetComponent<Image>().color = new Color(attack1Button.GetComponent<Image>().color.r, attack1Button.GetComponent<Image>().color.g
        , attack1Button.GetComponent<Image>().color.b, 0.5f);
        attack1Button.gameObject.SetActive(false);
        attack2Button.gameObject.SetActive(false);

        // Hide all the tiles (If not they will be stuck)
        foreach (var item in inRangeTiles)
        {
            item.HideTile();
        }
        foreach (var tile in MapManager.Instance.allTiles)
        {
            tile.HideTile();
        }

        // Clear the A* path of any remnants
        path.Clear();

        //Hide the character sheet
        characterSheet.gameObject.SetActive(false);
    }
    // Methods to on the attack selection:
    public void SelectAttack(int weaponNumber)
    {
        WeaponSelected = weaponNumber;
        if (character != null && Manager.Instance.GetCP() >= character.weaponsEquipped[WeaponSelected].GetComponent<WeaponBehaviour>().GetCPCost())
        {
            SoundManager.Instance.PlaySound(SoundManager.Sound.DragDropSFX);
            attackSelected = true;
            moveButton.gameObject.SetActive(false);
            cancelButton.gameObject.SetActive(true);

            // Ensure movement is no longer selected
            movementSelected = false;

            // Lighten up the button that was clicked
            switch (weaponNumber)
            {
                case 0:
                    attack1Button.GetComponent<Image>().color = new Color(attack1Button.GetComponent<Image>().color.r, attack1Button.GetComponent<Image>().color.g
                        , attack1Button.GetComponent<Image>().color.b, 1);
                    attack2Button.GetComponent<Image>().color = new Color(attack1Button.GetComponent<Image>().color.r, attack1Button.GetComponent<Image>().color.g
                        , attack1Button.GetComponent<Image>().color.b, 0.5f);
                    break;
                case 1:
                    attack2Button.GetComponent<Image>().color = new Color(attack1Button.GetComponent<Image>().color.r, attack1Button.GetComponent<Image>().color.g
                        , attack1Button.GetComponent<Image>().color.b, 1);
                    attack1Button.GetComponent<Image>().color = new Color(attack1Button.GetComponent<Image>().color.r, attack1Button.GetComponent<Image>().color.g
                        , attack1Button.GetComponent<Image>().color.b, 0.5f);
                    break;
                default:
                    break;
            }
        }
    }

    public void SpawnEntity(EntityBehaviour Entity, OverlayTileBehaviour focusedTile)
    {
        SoundManager.Instance.PlaySound(SoundManager.Sound.DragDropSFX);
        var barrel = Instantiate(Entity);
        barrel.PositionEntity(barrel, focusedTile);
        DeselectAction();
    }
}
