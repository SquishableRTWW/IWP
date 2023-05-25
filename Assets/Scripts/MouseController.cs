using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MouseController : MonoBehaviour
{
    public float speed;
    private List<OverlayTileBehaviour> path = new List<OverlayTileBehaviour>();
    private List<OverlayTileBehaviour> inRangeTiles = new List<OverlayTileBehaviour>();

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
            // Logic for constant updating of attack range and area of effect
            if (character != null)
            {
                if (attackSelected == true)
                {
                    GetAttackRangeTiles();
                }
            }

            if (Input.GetMouseButtonDown(0))
            {

                if (path.Count > 0)
                {
                    isMoving = true;
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
                            DeselectMovement();
                            character = objectHit.GetComponent<CharacterBehaviour>();
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
            if (inRangeTiles.IndexOf(tile) > (4 + Mathf.Pow(2, character.overheatAmount)))
            {
                tile.ShowOverheatTile();
            }
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

        inRangeTiles = moveRangeFinder.GetTilesInAttackRange(character.activeTile, character.weaponsEquipped[WeaponSelected].GetWeaponRange());

        foreach (var tile in inRangeTiles)
        {
            tile.ShowTile();
        }
    }

    private void MoveAlongPath()
    {
        List<int> overheatValue = new List<int>();
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
            DeselectMovement();
            character = null;
        }
    }

    private void PositionCharacter(OverlayTileBehaviour overlayTile)
    {
        character.transform.position = new Vector3(overlayTile.transform.position.x, overlayTile.transform.position.y + 0.0001f, overlayTile.transform.position.z);
        character.GetComponent<SpriteRenderer>().sortingOrder = overlayTile.GetComponent<SpriteRenderer>().sortingOrder + 1;
        character.activeTile = overlayTile;
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
    public void DeselectMovement()
    {
        // Reset the booleans
        movementSelected = false;
        attackSelected = false;
        // Make buttons disappear
        moveButton.gameObject.SetActive(false);
        cancelButton.gameObject.SetActive(false);
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
        cancelButton.gameObject.SetActive(true);

        // Ensure movement is no longer selected
        movementSelected = false;
    }
}
