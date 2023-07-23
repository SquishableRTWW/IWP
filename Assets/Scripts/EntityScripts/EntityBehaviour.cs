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
    [SerializeField] GameObject explosionEffect;
    private GameObject realExplosion;

    // Start is called before the first frame update
    void Start()
    {
        explosionEffect = entityScriptable.destroyEffect;
        realExplosion = null;
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
                int itemCount = Random.Range(1, 2);
                for (int i = 0; i < itemCount; i++)
                {
                    int randomItem = Random.Range(0, PrepPhaseManager.Instance.itemsInGame.Count);
                    Manager.Instance.playerItemList.Add(PrepPhaseManager.Instance.itemsInGame[randomItem]);
                }
                Destroy(gameObject);
                break;
            case "Barrel":
                List<OverlayTileBehaviour> damageTiles = MapManager.Instance.Get8DirectionTiles(activeTile, 3);
                foreach(var tile in damageTiles)
                {
                    if (MapManager.Instance.GetEnemyAt(tile) != null)
                    {
                        var enemy = MapManager.Instance.GetEnemyAt(tile);
                        int damageDealt = 4;
                        enemy.HP -= damageDealt;
                        enemy.healthBar.SetHealth(enemy.HP);
                        StartCoroutine(enemy.ShowDamage(damageDealt.ToString()));
                    }
                }
                if (realExplosion == null)
                {
                    realExplosion = Instantiate(explosionEffect, gameObject.transform.position, Quaternion.identity);
                    realExplosion.GetComponent<SpriteRenderer>().sortingOrder = GetComponent<SpriteRenderer>().sortingOrder + 1;
                }
                StartCoroutine(DestroyEntity());
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

    private IEnumerator DestroyEntity()
    {
        activeTile.hasCharacter = false;
        yield return new WaitForSeconds(0.4f);
        Destroy(realExplosion);
        Destroy(gameObject);
    }
}
