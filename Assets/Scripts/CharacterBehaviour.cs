using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterBehaviour : MonoBehaviour
{
    public int maxFuel;
    public int currentFuel;
    public int overheatAmount;
    public int HP;
    public int maxHP;
    public int defence;
    public int attackIncrease;

    public bool finishedMove;
    public bool isOverheated = false;
    public List<WeaponBehaviour> weaponsEquipped;
    public List<EquipmentScriptable> equipmentList;
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
        currentFuel = maxFuel;
        HP = maxHP;
        healthBar.SetMaxHealth(maxHP);
        defence = 0;
        finishedMove = false;
        realExplosion = null;
    }

    // Update is called once per frame
    void Update()
    {
        // Equipment check for stat buffs
        if (equipmentList[0] != null)
        {
            foreach (EquipmentScriptable equipment in equipmentList)
            {
                switch (equipment.equipmentName)
                {
                    case "Steel Plating":
                        defence = 1;
                        break;
                    case "Pop Shells":
                        attackIncrease = 1;
                        break;
                    default:
                        break;
                }

            }
        }

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
        activeTile.hasCharacter = false;
        yield return new WaitForSeconds(0.4f);
        Destroy(realExplosion);
        for (int i = 0; i < MapManager.Instance.playerCharacters.Count; i++)
        {
            if (MapManager.Instance.playerCharacters[i].characterName == characterName)
            {
                MapManager.Instance.playerCharacters.Remove(MapManager.Instance.playerCharacters[i]);
                Destroy(gameObject);
            }
        }
    }
}
