using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EntityScriptable", menuName = "Entity/EntityScriptable")]
public class EntityScriptable : ScriptableObject
{
    public string entityName;
    public string type;
    public string description;
    public int durability;
    public GameObject destroyEffect;
}
