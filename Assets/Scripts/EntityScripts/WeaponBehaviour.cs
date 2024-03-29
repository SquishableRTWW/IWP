using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponBehaviour : MonoBehaviour
{
    public WeaponScriptable weaponScriptable;
    public bool isInInventory;
    public bool isInstantiated;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Sprite GetAttackSprite()
    {
        return weaponScriptable.attackSprite;
    }
    public Sprite GetWeaponSprite()
    {
        return weaponScriptable.weaponSprite;
    }

    public int GetWeaponRange()
    {
        return weaponScriptable.range;
    }
    public int GetWeaponDamage()
    {
        return weaponScriptable.damage;
    }
    public string GetWeaponName()
    {
        return weaponScriptable.weaponName;
    }
    public string GetShotType()
    {
        return weaponScriptable.shotType;
    }
    public Vector2 GetAttackPattern()
    {
        return weaponScriptable.attackPattern;
    }
    public int GetCPCost()
    {
        return weaponScriptable.CPCost;
    }
    public GameObject GetAnimation()
    {
        return weaponScriptable.animation;
    }
}
