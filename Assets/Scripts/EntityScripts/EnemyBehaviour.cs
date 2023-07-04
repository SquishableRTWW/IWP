using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{
    // Stats
    public string characterName;
    public EnemyScriptable enemyScriptable;
    public int HP;
    public int maxHP;
    public int movementRange;
    public HealthBar healthBar;
    // Effects and animations
    [SerializeField] GameObject explosionEffect;
    private GameObject realExplosion;
    public Sprite normalSprite;
    public Sprite reverseSprite;
    // For combat
    public OverlayTileBehaviour targetTile;
    public GameObject shootingEffect;
    public int directionIndicator;
    private GameObject realShooting;
    public bool shouldAttack;
    public bool hasAttacked;
    // Location on gridmap
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
        hasAttacked = false;
        realExplosion = null;
        pathfinder = new Pathfinder();
        rangeFinder = new MoveRangeFinder();
        directionIndicator = 1;
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
                Manager.Instance.enemyPath.Remove(Manager.Instance.enemyPath[i]);
                Destroy(gameObject);
            }
        }
    }

    public IEnumerator DoAttackAnimation()
    {
        if (realShooting == null && shootingEffect != null)
        {
            realShooting = Instantiate(shootingEffect,
            new Vector3(gameObject.transform.position.x + (0.3f * directionIndicator), gameObject.transform.position.y + 0.2f, gameObject.transform.position.z),
            Quaternion.identity);

            realShooting.GetComponent<SpriteRenderer>().sortingOrder = GetComponent<SpriteRenderer>().sortingOrder + 1;
            //realShooting.transform.rotation.Set(0f, 0f, 1f, -180f * directionIndicator);
        }

        yield return new WaitForSeconds(0.75f);
        Destroy(realShooting);
    }

    // AI methods here:========================================================================================
    public bool InAttackRange(OverlayTileBehaviour targetTile)
    {
        switch (enemyScriptable.weapon.GetShotType())
        {
            case "Linear":
                List<OverlayTileBehaviour> targetTiles = rangeFinder.GetTilesInAttackRange(activeTile, enemyScriptable.weapon.GetWeaponRange());
                if (targetTiles.Count > 1)
                {
                    //Debug.Log("Target is in sight");
                    if (targetTiles.Contains(targetTile))
                    {
                        //Debug.Log("Target is also in range");
                        return true;
                    }
                    else
                    {
                        //Debug.Log("But not in range");
                    }
                }
                break;
            case "Lobbing":
                break;
            default:
                break;
        }

        Debug.Log("Target not in Sight");
        return false;
    }
}
