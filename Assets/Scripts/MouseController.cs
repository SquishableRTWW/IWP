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
    private List<OverlayTileBehaviour> inRangeTiles = new List<OverlayTileBehaviour>();
    // List of tiles that are part of the attack path
    private List<OverlayTileBehaviour> attackTiles = new List<OverlayTileBehaviour>();

    private Pathfinder pathfinder;
    private MoveRangeFinder moveRangeFinder;

    public GameObject characterPrefab;
    [SerializeField] private CharacterBehaviour character;

    // GUI Elements
    [SerializeField] private Button moveButton;
    [SerializeField] private Button cancelButton;
    [SerializeField] private Canvas characterSheet;
    [SerializeField] private Button attack1Button;
    [SerializeField] private Button attack2Button;
    [SerializeField] CameraController sceneCameraController;

    private bool isMoving = false;
    private int WeaponSelected = 0;
    public bool movementSelected = false;
    public bool attackSelected = false;

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
            if (character != null && inRangeTiles.Contains(overlayTile) && !isMoving && !character.finishedMove)
            {
                if (movementSelected == true)
                {
                    path = pathfinder.FindPath(character.activeTile, overlayTile, inRangeTiles);

                    GetInRangeTiles();

                    for (int i = 0; i < path.Count; i++)
                    {
                        path[i].ShowTileInPath();
                    }
                }
            }
            // Logic for constant updating of attack range, area of effect and location
            if (character != null)
            {
                if (attackSelected == true)
                {
                    GetAttackRangeTiles();
                    switch (character.weaponsEquipped[WeaponSelected].GetShotType())
                    {
                        case "Linear":
                            attackTiles = pathfinder.FindLinearAttackPath(character.activeTile, overlayTile, character.weaponsEquipped[WeaponSelected].GetWeaponRange(), inRangeTiles);
                            break;
                        case "Lobbing":
                            attackTiles = pathfinder.FindAOEAttackPath(overlayTile, (int)character.weaponsEquipped[WeaponSelected].GetAttackPattern().x, inRangeTiles);
                            break;
                        case "Across":
                            attackTiles = pathfinder.FindAcrossAttackPath(character.activeTile, overlayTile, (int)character.weaponsEquipped[WeaponSelected].GetAttackPattern().x,
                                character.weaponsEquipped[WeaponSelected].GetWeaponRange(), inRangeTiles);
                            break;
                        default:
                            attackTiles = pathfinder.FindLinearAttackPath(character.activeTile, overlayTile, character.weaponsEquipped[WeaponSelected].GetWeaponRange(), inRangeTiles);
                            break;
                    }
                    

                    for (int i = 0; i < attackTiles.Count; i++)
                    {
                        attackTiles[i].ShowAttackTile();
                    }
                }
            }

            if (Input.GetMouseButtonDown(0))
            {

                if (path.Count > 0)
                {
                    isMoving = true;
                }
                else if (attackTiles.Count > 0 && attackSelected == true)
                {
                    DoDamage();
                }

                // OLD CHARACTER SPAWNING CODE (For reference)
                //character = Instantiate(characterPrefab).GetComponent<CharacterBehaviour>();
                //PositionCharacter(overlayTile);
                //GetInRangeTiles();

                // Check to see if clicked on a character:
                Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector2 mousePos2d = new Vector2(mousePos.x, mousePos.y);

                RaycastHit2D[] hits = Physics2D.RaycastAll(mousePos2d, Vector2.zero);
                foreach (RaycastHit2D hit in hits)
                {
                    GameObject objectHit = hit.collider.gameObject;

                    // Check if the collider is a trigger
                    if (hit.collider.isTrigger)
                    {
                       
                        if (objectHit.CompareTag("Character"))
                        {
                            // Set selected character as the clicked one
                            DeselectAction();
                            character = objectHit.GetComponent<CharacterBehaviour>();
                            // Zoom into character
                            StartCoroutine(sceneCameraController.ZoomAtCharacter(character.transform.position));
                            // Logic for showing UI stuffs
                            if (character.finishedMove == false)
                            {
                                moveButton.gameObject.SetActive(true);
                            }
                            if (character.isOverheated == false)
                            {
                                attack1Button.gameObject.SetActive(true);
                                attack1Button.gameObject.GetComponent<Image>().sprite = character.weaponsEquipped[0].GetAttackSprite();
                                if (character.weaponsEquipped.Count > 1 && character.weaponsEquipped[1] != null)
                                {
                                    attack2Button.gameObject.SetActive(true);
                                    attack2Button.gameObject.GetComponent<Image>().sprite = character.weaponsEquipped[1].GetAttackSprite();
                                }
                                else
                                {
                                    attack2Button.gameObject.SetActive(false);
                                }
                            }
                            // Update Character sheet UI
                            characterSheet.gameObject.SetActive(true);
                            characterSheet.gameObject.GetComponentInChildren<Image>().sprite = character.GetComponent<SpriteRenderer>().sprite;
                            characterSheet.gameObject.GetComponentInChildren<TextMeshProUGUI>().text = character.characterName + "\nHP: " 
                                + character.HP + "\nMax F: " + character.maxFuel + "\nCurr F: " + character.currentFuel;
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
            if (inRangeTiles.IndexOf(tile) < (4 + Mathf.Pow(character.overheatAmount, 2)))
            {
                tile.ShowTile();
            }
            else
            {
                tile.ShowOverheatTile();
            }
        }
    }
    private void GetAttackRangeTiles()
    {
        foreach (var tile in inRangeTiles)
        {
            tile.HideTile();
        }

        inRangeTiles = moveRangeFinder.GetTilesInAttackRange(character.activeTile, character.weaponsEquipped[WeaponSelected].GetWeaponRange());

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
            path.RemoveAt(0);
            character.currentFuel -= 1;
        }

        if (path.Count == 0)
        {
            character.finishedMove = true;
            foreach (var tile in inRangeTiles)
            {
                tile.HideTile();
            }
            isMoving = false;
            DeselectAction();
            character = null;
        }
    }

    // Logic for doing damage
    private void DoDamage()
    {
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
                continue;
            }
            if (MapManager.Instance.playerCharacters[affectedCount].grid2DLocation == tilesWithCharacters[i].grid2DLocation)
            {
                MapManager.Instance.playerCharacters[affectedCount].HP -= character.weaponsEquipped[WeaponSelected].GetWeaponDamage();
                MapManager.Instance.playerCharacters[affectedCount].healthBar.SetHealth(MapManager.Instance.playerCharacters[affectedCount].HP);
                affectedCount++;
                tilesWithCharacters.Remove(tilesWithCharacters[i]);
                sceneCameraController.CameraShake();
                i = -1;
            }
            else
            {
                affectedCount++;
                i = -1;
            }
        }
        DeselectAction();
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

        RaycastHit2D[] hits = Physics2D.RaycastAll(mousePos2d, Vector2.zero);

        if (hits.Length > 0)
        {
            return hits.Where(hit => hit.collider.CompareTag("OverlayTile")).OrderByDescending(i => i.collider.transform.position.z).First();
        }

        return null;
    }

    //Method to on the movement selection
    public void SelectMovement()
    {
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
        attackSelected = false;
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

        // Clear the A* path of any remnants
        path.Clear();

        //Hide the character sheet
        characterSheet.gameObject.SetActive(false);
    }
    // Methods to on the attack selection:
    public void SelectAttack(int weaponNumber)
    {
        WeaponSelected = weaponNumber;
        attackSelected = true;
        moveButton.gameObject.SetActive(false);
        cancelButton.gameObject.SetActive(true);

        // Ensure movement is no longer selected
        movementSelected = false;

        // Lighten up the button that was clicked
        switch(weaponNumber)
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
