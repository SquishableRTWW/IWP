using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyScriptable", menuName = "Enemies/EnemyScriptable")]
public class EnemyScriptable : ScriptableObject
{
    public int HP;
    public string enemyName;
    public int movementRange;
    public WeaponBehaviour weapon;
}
