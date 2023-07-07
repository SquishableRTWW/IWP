using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterBehaviour : MonoBehaviour
{
    // Stats
    public string characterName;
    public int maxFuel;
    public int currentFuel;
    public int overheatAmount;
    public int HP;
    public int maxHP;
    public int defence;
    public int attackIncrease;
    public int directionIndicator;
    // Combat related
    public bool finishedMove;
    public bool isOverheated = false;
    public List<WeaponBehaviour> weaponsEquipped;
    public List<EquipmentScriptable> equipmentList;
    public HealthBar healthBar;
    // Animation effects
    [SerializeField] GameObject explosionEffect;
    private GameObject realExplosion;
    public GameObject shootingEffect;
    private GameObject realShooting;
    public GameObject lobbingEffect;
    private GameObject realLobbing;
    [SerializeField] GameObject floatingTextPrefab;
    //Sprite stuff
    public Sprite normalSprite;
    public Sprite reverseSprite;
    // Location on the gridmap
    public Vector3Int gridLocation;
    public Vector2Int grid2DLocation { get { return new Vector2Int(gridLocation.x, gridLocation.y); } }
    public OverlayTileBehaviour activeTile;



    // Start is called before the first frame update
    void Start()
    {
        //currentFuel = maxFuel;
        HP = maxHP;
        healthBar.SetMaxHealth(maxHP);
        defence = 0;
        finishedMove = false;
        realExplosion = null;
        directionIndicator = 1;
        currentFuel = 0;
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

    public IEnumerator DoAttackAnimation(GameObject effect)
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

    // Method to show the floating damage text.
    public IEnumerator ShowDamage(string text)
    {
        if (floatingTextPrefab)
        {
            GameObject prefab = Instantiate(floatingTextPrefab, transform.position, Quaternion.identity);
            prefab.GetComponentInChildren<TextMesh>().color = new Color(0f, 1f, 0f, 1f);
            prefab.GetComponentInChildren<TextMesh>().text = text;

            yield return new WaitForSeconds(0.6f);
            Destroy(prefab);
        }
    }
}
