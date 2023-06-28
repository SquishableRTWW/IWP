using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{
    public EnemyScriptable enemyScriptable;
    public int HP;
    public int maxHP;
    public int movementRange;
    public HealthBar healthBar;

    [SerializeField] GameObject explosionEffect;
    private GameObject realExplosion;
    public string characterName;
    public Vector3Int gridLocation;
    public Vector2Int grid2DLocation { get { return new Vector2Int(gridLocation.x, gridLocation.y); } }
    public OverlayTileBehaviour activeTile;

    private Pathfinder pathfinder;
    private MoveRangeFinder rangeFinder;

    // Start is called before the first frame update
    void Start()
    {
        maxHP = enemyScriptable.HP;
        HP = maxHP;
        movementRange = enemyScriptable.movementRange;
        healthBar.SetMaxHealth(maxHP);
        realExplosion = null;
        pathfinder = new Pathfinder();
        rangeFinder = new MoveRangeFinder();
    }

    // Update is called once per frame
    void Update()
    {
        if (HP <= 0)
        {
            if (realExplosion == null)
            {
                realExplosion = Instantiate(explosionEffect, gameObject.transform.position, Quaternion.identity);
                realExplosion.GetComponent<SpriteRenderer>().sortingOrder = GetComponent<SpriteRenderer>().sortingOrder + 1;
            }
            StartCoroutine(DestroyCharacter());
        }
        if (HP >= maxHP)
        {
            healthBar.gameObject.SetActive(false);
        }
        else
        {
            healthBar.gameObject.SetActive(true);
        }
    }

    private IEnumerator DestroyCharacter()
    {
        activeTile.hasEnemy = false;
        yield return new WaitForSeconds(0.4f);
        Destroy(realExplosion);
        for (int i = 0; i < MapManager.Instance.enemyList.Count; i++)
        {
            if (MapManager.Instance.enemyList[i].grid2DLocation == grid2DLocation)
            {
                MapManager.Instance.enemyList.Remove(MapManager.Instance.enemyList[i]);
                Destroy(gameObject);
            }
        }
    }

    // AI methods here:========================================================================================
    public bool InAttackRange(OverlayTileBehaviour targetTile)
    {
        switch (enemyScriptable.weapon.GetShotType())
        {
            case "Linear":
                List<OverlayTileBehaviour> rangeTiles = rangeFinder.GetTilesInAttackRange(targetTile, enemyScriptable.weapon.GetWeaponRange());
                List<OverlayTileBehaviour> targetTiles = pathfinder.FindLinearAttackPath(activeTile, targetTile, enemyScriptable.weapon.GetWeaponRange(), rangeTiles);
                if (targetTiles.Contains(targetTile))
                {
                    Debug.Log("Target is in range");
                    return true;
                }
                break;
            case "Lobbing":
                break;
            default:
                break;
        }

        Debug.Log("Target not in Range");
        return false;
    }
}
