using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterBehaviour : MonoBehaviour
{
    public int maxFuel;
    public int currentFuel;
    public int overheatAmount;
    public int HP;
    public bool finishedMove;
    public bool isOverheated = false;
    public List<WeaponBehaviour> weaponsEquipped;

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
        finishedMove = false;
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
    }

    private IEnumerator DestroyCharacter()
    {
        yield return new WaitForSeconds(0.4f);
        Destroy(realExplosion);
        Destroy(gameObject);
    }
}
