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

    // Start is called before the first frame update
    void Start()
    {
        maxHP = enemyScriptable.HP;
        HP = maxHP;
        movementRange = enemyScriptable.movementRange;
        healthBar.SetMaxHealth(maxHP);
        realExplosion = null;
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
            if (MapManager.Instance.enemyList[i].characterName == characterName)
            {
                MapManager.Instance.enemyList.Remove(MapManager.Instance.enemyList[i]);
                Destroy(gameObject);
            }
        }
    }

    // AI methods here:========================================================================================
}
