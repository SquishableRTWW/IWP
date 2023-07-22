using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityBehaviour : MonoBehaviour
{
    // In-game variables
    public Vector3Int gridLocation;
    public Vector2Int grid2dLocation;
    public OverlayTileBehaviour activeTile;
    public EntityScriptable entityScriptable;
    // GUI components


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ActivateEntity()
    {
        switch (entityScriptable.type)
        {
            case "Obstacle":
                break;
            case "Crate":
                break;
            case "Barrel":
                break;
            default:
                Debug.Log("Entity Error");
                break;
        }
    }

    public void PositionEntity(EntityBehaviour character, OverlayTileBehaviour overlayTile)
    {
        character.transform.position = new Vector3(overlayTile.transform.position.x, overlayTile.transform.position.y + 0.0001f, overlayTile.transform.position.z);
        character.GetComponent<SpriteRenderer>().sortingOrder = overlayTile.GetComponent<SpriteRenderer>().sortingOrder + 1;
        character.activeTile = overlayTile;
    }
}
