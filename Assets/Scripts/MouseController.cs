using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MouseController : MonoBehaviour
{
    public float speed;
    private List<OverlayTileBehaviour> path = new List<OverlayTileBehaviour>();
    private List<OverlayTileBehaviour> inRangeTiles = new List<OverlayTileBehaviour>();

    private Pathfinder pathfinder;
    private MoveRangeFinder moveRangeFinder;

    public GameObject characterPrefab;
    private CharacterBehaviour character;

    private bool isMoving = false;

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

            if (inRangeTiles.Contains(overlayTile) && !isMoving)
            {
                path = pathfinder.FindPath(character.activeTile, overlayTile, inRangeTiles);

                foreach (var item in inRangeTiles)
                {
                    item.ShowTile();
                }

                for (int i = 0; i < path.Count; i++)
                {
                    path[i].ShowTileInPath();
                }
            }

            if (Input.GetMouseButtonDown(0))
            {
                //overlayTile.ShowTile();

                if (character == null)
                {
                    character = Instantiate(characterPrefab).GetComponent<CharacterBehaviour>();
                    PositionCharacter(overlayTile);
                    GetInRangeTiles();
                }
                else
                {
                    isMoving = true;
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
            tile.ShowTile();
        }
    }

    private void MoveAlongPath()
    {
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
            GetInRangeTiles();
            isMoving = false;
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
            return hits.OrderByDescending(i => i.collider.transform.position.z).First();
        }

        return null;
    }
}
