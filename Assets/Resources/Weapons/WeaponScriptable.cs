using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponScriptable", menuName = "Weapons/WeaponScriptable")]
public class WeaponScriptable : ScriptableObject
{
    public int damage;
    public int range;
    public int CPCost;
    public string shotType;
    public Vector2Int attackPattern;
}
