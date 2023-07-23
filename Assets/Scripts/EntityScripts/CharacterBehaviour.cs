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
    public int currentHeat;
    public int HP;
    public int maxHP;
    public int defence;
    public int attackIncrease;
    public int directionIndicator;
    // Combat related
    public bool finishedMove;
    public bool isOverheated = false;
    public List<GameObject> weaponsEquipped;
    public List<GameObject> equipmentList;
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
    public Vector3Int ogPosition;
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

        Manager.Instance.GiveEquipmentBuff(this, equipmentList[0].GetComponent<EquipmentBehaviour>());
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

        // Temp way to show character is overheated
        if (isOverheated)
        {
            this.gameObject.GetComponent<SpriteRenderer>().color = new Color(1f, 0.7f, 0.1f, 1f);
        }
        else
        {
            this.gameObject.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);
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
            prefab.GetComponentInChildren<TextMesh>().color = new Color(1f, 0f, 0f, 1f);
            prefab.GetComponentInChildren<TextMesh>().text = text;

            yield return new WaitForSeconds(0.6f);
            Manager.Instance.sheetHPBar.SetHealth(HP);
            Destroy(prefab);
        }
    }

    public IEnumerator ShowHeal(string text)
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

    public void ResetHealthBars()
    {
        healthBar.SetHealth(HP);
    }

    public void ResetPosition()
    {
        foreach(var tile in MapManager.Instance.allTiles)
        {
            if (tile.gridLocation == ogPosition)
            {
                activeTile.hasCharacter = false;
                PositionCharacter(tile);
                break;
            }
        }
    }

    private void PositionCharacter(OverlayTileBehaviour overlayTile)
    {
        transform.position = new Vector3(overlayTile.transform.position.x, overlayTile.transform.position.y + 0.0001f, overlayTile.transform.position.z);
        GetComponent<SpriteRenderer>().sortingOrder = overlayTile.GetComponent<SpriteRenderer>().sortingOrder + 1;
        activeTile = overlayTile;
        activeTile.hasCharacter = true;
        gridLocation = (activeTile.gridLocation);
    }
}
