using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponBehaviour : MonoBehaviour
{
    [SerializeField] private WeaponScriptable weaponScriptable;
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
}