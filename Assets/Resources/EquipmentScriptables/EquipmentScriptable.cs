using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "EquipmentScriptable", menuName = "Equipment/EquipmentScriptable")]
public class EquipmentScriptable : ScriptableObject
{
    public string equipmentName;
    public string rarity;

    public Sprite equipmentSprite;
}
