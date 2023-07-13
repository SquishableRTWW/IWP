using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum eventNo
{ 
    AddSmallFuel,
    AddMediumFuel,
    AddCP,
    AddWeapon,
    AddEquipment,
    AddSupplyDrop,
    Add2CP,
    AddCharacter,
}

public struct Event
{
    public string eventName;
    public string eventType;
    public string eventMessage;
    public int amount;
    public GameObject thingToAdd;
}


public class EventManager : MonoBehaviour
{
    private static EventManager _instance;
    public static EventManager Instance { get { return _instance; } }

    public int rng;
    public eventNo decidedLeftEvent;
    public eventNo decidedRightEvent;
    public Event leftEvent;
    public Event rightEvent;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RandomiseEvent()
    {
        // Randomise for left side
        {
            rng = UnityEngine.Random.Range(0, 100);
            if (rng <= 50)
            {
                decidedLeftEvent = GetRandomEnumValue<eventNo>(0, 4);
            }
            else if (rng > 50 && rng <= 80)
            {
                decidedLeftEvent = GetRandomEnumValue<eventNo>(5, 6);
            }
            else
            {
                decidedLeftEvent = GetRandomEnumValue<eventNo>(7, 7);
            }
        }
        // Randomise for right side
        {
            rng = UnityEngine.Random.Range(0, 100);
            if (rng <= 50)
            {
                decidedRightEvent = GetRandomEnumValue<eventNo>(0, 4);
            }
            else if (rng > 50 && rng <= 80)
            {
                decidedRightEvent = GetRandomEnumValue<eventNo>(5, 6);
            }
            else
            {
                decidedRightEvent = GetRandomEnumValue<eventNo>(7, 7);
            }
        }
        // Switch case to create event based on random number:
        switch (decidedLeftEvent)
        {
            case eventNo.AddSmallFuel:
                break;
            case eventNo.AddMediumFuel:
                break;
            case eventNo.AddCP:
                break;
            case eventNo.AddWeapon:
                break;
            case eventNo.AddEquipment:
                break;
            case eventNo.AddSupplyDrop:
                break;
            case eventNo.AddCharacter:
                break;
            default:
                break;
        }
        switch (decidedRightEvent)
        {
            case eventNo.AddSmallFuel:
                break;
            case eventNo.AddMediumFuel:
                break;
            case eventNo.AddCP:
                break;
            case eventNo.AddWeapon:
                break;
            case eventNo.AddEquipment:
                break;
            case eventNo.AddSupplyDrop:
                break;
            case eventNo.AddCharacter:
                break;
            default:
                break;
        }
    }

    public void ActivateLeftDecision()
    {

    }

    public void ActivateRightDecision()
    {

    }

    public static T GetRandomEnumValue<T>(int start, int end)
    {
        Array enumValues = Enum.GetValues(typeof(T));
        int randomIndex = UnityEngine.Random.Range(start, end);
        T randomEnumValue = (T)enumValues.GetValue(randomIndex);
        return randomEnumValue;
    }
}
