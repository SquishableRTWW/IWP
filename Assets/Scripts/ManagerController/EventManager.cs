using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
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
    public Sprite eventImage;
    public List<GameObject> itemsToAdd;
    public CharacterBehaviour characterToAdd;
}


public class EventManager : MonoBehaviour
{
    private static EventManager _instance;
    public static EventManager Instance { get { return _instance; } }

    // Event variables
    public int rng;
    public eventNo decidedLeftEvent;
    public eventNo decidedRightEvent;
    public Event leftEvent;
    public Event rightEvent;
    public List<Sprite> eventImageList;
    // GUI components
    public TextMeshProUGUI leftText;
    public TextMeshProUGUI rightText;
    public Image leftImage;
    public Image rightImage;
    public Canvas eventCanvas;
    public Canvas prepCanvas;

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
    private void Start()
    {
        //RandomiseEvent();
    }

    void Update()
    {
        if (!Manager.Instance.isInEvent)
        {
            eventCanvas.gameObject.SetActive(false);
        }
        else
        {
            eventCanvas.gameObject.SetActive(true);
        }
    }

    public void RandomiseEvent()
    {
        // Randomise for left side
        if (MapManager.Instance.level == 3)
        {
            decidedLeftEvent = eventNo.AddCharacter;
        }
        else
        {
            rng = UnityEngine.Random.Range(0, 100);
            if (rng <= 50)
            {
                decidedLeftEvent = GetRandomEnumValue<eventNo>(0, 4);
            }
            else if ((rng > 50 && rng <= 90) || MapManager.Instance.playerCharacters.Count >= 3)
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
            else if ((rng > 50 && rng <= 90) || MapManager.Instance.playerCharacters.Count >= 3)
            {
                decidedRightEvent = GetRandomEnumValue<eventNo>(5, 6);
            }
            else
            {
                decidedRightEvent = GetRandomEnumValue<eventNo>(7, 7);
            }
        }

        // Switch case to create event based on random number:
        leftEvent = new Event();
        leftEvent.eventImage = eventImageList[(int)(decidedLeftEvent)];
        leftImage.sprite = leftEvent.eventImage;
        rightEvent = new Event();
        rightEvent.eventImage = eventImageList[(int)(decidedRightEvent)];
        rightImage.sprite = rightEvent.eventImage;

        switch (decidedLeftEvent)
        {
            case eventNo.AddSmallFuel:
                leftEvent.eventName = "AddSmallFuel";
                leftEvent.eventType = "Add fuel";
                leftEvent.eventMessage = "We spotted a few cans of fuel left by the enemy. They could slightly increase our fuel pool.";
                leftEvent.amount = 10;
                break;
            case eventNo.AddMediumFuel:
                leftEvent.eventName = "AddMediumFuel";
                leftEvent.eventType = "Add fuel";
                leftEvent.eventMessage = "We found a gas station to the North of the battlefield. Could hold a lot of fuel...";
                leftEvent.amount = 20;
                break;
            case eventNo.AddCP:
                leftEvent.eventName = "AddCP";
                leftEvent.eventType = "Add cp";
                leftEvent.eventMessage = "We found some comms equipment that could be salvaged. This could allow you to give us more commands.";
                leftEvent.amount = 1;
                break;
            case eventNo.AddWeapon:
                leftEvent.eventName = "AddWeapon";
                leftEvent.eventType = "Add item";
                leftEvent.eventMessage = "The team thinks some of the enemy's weapons can be reused, we could use more guns.";
                leftEvent.itemsToAdd = new List<GameObject>();
                // Choose random weapon;
                int randomItem = UnityEngine.Random.Range(0, PrepPhaseManager.Instance.weaponsInGame.Count);
                leftEvent.itemsToAdd.Add(PrepPhaseManager.Instance.weaponsInGame[randomItem]);
                break;
            case eventNo.AddEquipment:
                leftEvent.eventName = "AddEquipment";
                leftEvent.eventType = "Add item";
                leftEvent.eventMessage = "The enemy has some useful equipment that we could utilise in future skirmishes...";
                leftEvent.itemsToAdd = new List<GameObject>();
                // Choose random weapon;
                randomItem = UnityEngine.Random.Range(0, PrepPhaseManager.Instance.equipmentInGame.Count);
                leftEvent.itemsToAdd.Add(PrepPhaseManager.Instance.equipmentInGame[randomItem]);
                break;
            case eventNo.AddSupplyDrop:
                leftEvent.eventName = "AddSupplyDrop";
                leftEvent.eventType = "Add item";
                leftEvent.eventMessage = "Theres an enemy supply drop further from the battlefield. Could hold multiple useful items";
                leftEvent.itemsToAdd = new List<GameObject>();
                // Choose random weapon;
                int itemCount = UnityEngine.Random.Range(1, 3);
                for (int i = 0; i < itemCount; i++)
                {
                    randomItem = UnityEngine.Random.Range(0, PrepPhaseManager.Instance.itemsInGame.Count);
                    leftEvent.itemsToAdd.Add(PrepPhaseManager.Instance.itemsInGame[randomItem]);
                }
                break;
            case eventNo.Add2CP:
                leftEvent.eventName = "Add2CP";
                leftEvent.eventType = "Add cp";
                leftEvent.eventMessage = "We found some comms equipment in mint condition. This could allow you to give us many more commands.";
                leftEvent.amount = 2;
                break;
            case eventNo.AddCharacter:
                leftEvent.eventName = "AddCharacter";
                leftEvent.eventType = "Add character";
                leftEvent.eventMessage = "Theres an emergency smoke signal in the distance. Could be a survivor and a salvagable mech to use.";
                int randomChar = UnityEngine.Random.Range(0, MapManager.Instance.characterList.Count);
                leftEvent.characterToAdd = MapManager.Instance.characterList[randomChar];
                break;
            default:
                Debug.Log("event decision error...");
                break;
        }
        switch (decidedRightEvent)
        {
            case eventNo.AddSmallFuel:
                rightEvent.eventName = "AddSmallFuel";
                rightEvent.eventType = "Add fuel";
                rightEvent.eventMessage = "We spotted a few cans of fuel left by the enemy. They could slightly increase our fuel pool.";
                rightEvent.amount = 10;
                break;
            case eventNo.AddMediumFuel:
                rightEvent.eventName = "AddMediumFuel";
                rightEvent.eventType = "Add fuel";
                rightEvent.eventMessage = "We found a gas station to the North of the battlefield. Could hold a lot of fuel...";
                rightEvent.amount = 20;
                break;
            case eventNo.AddCP:
                rightEvent.eventName = "AddCP";
                rightEvent.eventType = "Add cp";
                rightEvent.eventMessage = "We found some comms equipment that could be salvaged. This could allow you to give us more commands.";
                rightEvent.amount = 1;
                break;
            case eventNo.AddWeapon:
                rightEvent.eventName = "AddWeapon";
                rightEvent.eventType = "Add item";
                rightEvent.eventMessage = "The team thinks some of the enemy's weapons can be reused, we could use more guns.";
                rightEvent.itemsToAdd = new List<GameObject>();
                // Choose random weapon;
                int randomItem = UnityEngine.Random.Range(0, PrepPhaseManager.Instance.weaponsInGame.Count);
                rightEvent.itemsToAdd.Add(PrepPhaseManager.Instance.weaponsInGame[randomItem]);
                break;
            case eventNo.AddEquipment:
                rightEvent.eventName = "AddEquipment";
                rightEvent.eventType = "Add item";
                rightEvent.eventMessage = "The enemy has some useful equipment that we could utilise in future skirmishes...";
                rightEvent.itemsToAdd = new List<GameObject>();
                // Choose random weapon;
                randomItem = UnityEngine.Random.Range(0, PrepPhaseManager.Instance.equipmentInGame.Count);
                rightEvent.itemsToAdd.Add(PrepPhaseManager.Instance.equipmentInGame[randomItem]);
                break;
            case eventNo.AddSupplyDrop:
                rightEvent.eventName = "AddSupplyDrop";
                rightEvent.eventType = "Add item";
                rightEvent.eventMessage = "Theres an enemy supply drop further from the battlefield. Could hold multiple useful items";
                rightEvent.itemsToAdd = new List<GameObject>();
                // Choose random weapon;
                int itemCount = UnityEngine.Random.Range(1, 3);
                for (int i = 0; i < itemCount; i++)
                {
                    randomItem = UnityEngine.Random.Range(0, PrepPhaseManager.Instance.itemsInGame.Count);
                    rightEvent.itemsToAdd.Add(PrepPhaseManager.Instance.itemsInGame[randomItem]);
                }
                break;
            case eventNo.Add2CP:
                rightEvent.eventName = "Add2CP";
                rightEvent.eventType = "Add cp";
                rightEvent.eventMessage = "We found some comms equipment in mint condition. This could allow you to give us many more commands.";
                rightEvent.amount = 2;
                break;
            case eventNo.AddCharacter:
                rightEvent.eventName = "AddCharacter";
                rightEvent.eventType = "Add character";
                rightEvent.eventMessage = "Theres an emergency smoke signal in the distance. Could be a survivor and a salvagable mech to use.";
                int randomChar = UnityEngine.Random.Range(0, MapManager.Instance.characterList.Count);
                rightEvent.characterToAdd = MapManager.Instance.characterList[randomChar];
                break;
            default:
                Debug.Log("event decision error...");
                break;
        }


        // Set the GUI components
        leftText.text = leftEvent.eventMessage;
        rightText.text = rightEvent.eventMessage;
    }

    public void ActivateLeftDecision()
    {
        switch(leftEvent.eventType)
        {
            case "Add fuel":
                Manager.Instance.maxfuelPool = Manager.Instance.maxfuelPool + leftEvent.amount;
                break;
            case "Add cp":
                Manager.Instance.maxCP = Manager.Instance.maxCP + leftEvent.amount;
                Debug.Log(Manager.Instance.maxCP);
                break;
            case "Add item":
                for (int i = 0; i < leftEvent.itemsToAdd.Count; i++)
                {
                    Manager.Instance.playerItemList.Add(leftEvent.itemsToAdd[i]);
                }
                break;
            case "Add character":
                int count = MapManager.Instance.playerCharacters.Count;
                MapManager.Instance.playerCharacters.Add(leftEvent.characterToAdd);
                MapManager.Instance.playerCharacters[count] = Instantiate(MapManager.Instance.playerCharacters[count]);
                MapManager.Instance.SetOGPosition();
                for (int i = 0; i < MapManager.Instance.playerCharacters.Count; i++)
                {
                    PrepPhaseManager.Instance.changeButtons[i].GetComponent<Image>().sprite = MapManager.Instance.playerCharacters[i].GetComponent<SpriteRenderer>().sprite;
                    PrepPhaseManager.Instance.changeButtons[i].GetComponent<Image>().color = new Color(1, 1, 1, 0.6f);
                }
                break;
            default:
                Debug.Log("Event activation error...");
                break;
        }
        TransitionToPrep();
    }

    public void ActivateRightDecision()
    {
        switch (rightEvent.eventType)
        {
            case "Add fuel":
                Manager.Instance.maxfuelPool = Manager.Instance.maxfuelPool + rightEvent.amount;
                break;
            case "Add cp":
                Manager.Instance.maxCP = Manager.Instance.maxCP + rightEvent.amount;
                Debug.Log(Manager.Instance.maxCP);
                break;
            case "Add item":
                for (int i = 0; i < rightEvent.itemsToAdd.Count; i++)
                {
                    Manager.Instance.playerItemList.Add(rightEvent.itemsToAdd[i]);
                }
                break;
            case "Add character":
                int count = MapManager.Instance.playerCharacters.Count;
                MapManager.Instance.playerCharacters.Add(rightEvent.characterToAdd);
                MapManager.Instance.playerCharacters[count] = Instantiate(MapManager.Instance.playerCharacters[count]);
                MapManager.Instance.SetOGPosition();
                for (int i = 0; i < MapManager.Instance.playerCharacters.Count; i++)
                {
                    PrepPhaseManager.Instance.changeButtons[i].GetComponent<Image>().sprite = MapManager.Instance.playerCharacters[i].GetComponent<SpriteRenderer>().sprite;
                    PrepPhaseManager.Instance.changeButtons[i].GetComponent<Image>().color = new Color(1, 1, 1, 0.6f);
                }
                break;
            default:
                Debug.Log("Event activation error...");
                break;
        }
        TransitionToPrep();
    }

    public void TransitionToPrep()
    {
        //MapManager.Instance.ReloadMap();
        eventCanvas.gameObject.SetActive(false);
        prepCanvas.gameObject.SetActive(true);
        Manager.Instance.isInEvent = false;
        PrepPhaseManager.Instance.HideCharacterPrep();
        PrepPhaseManager.Instance.DisplayInventoryItems();

        MapManager.Instance.SetOGPosition();
        //Reset all characters:
        foreach (CharacterBehaviour character in MapManager.Instance.playerCharacters)
        {
            character.currentFuel = 0;
            character.HP = character.maxHP;
            character.ResetHealthBars();
            character.ResetPosition();
        }

        Manager.Instance.fuelPool = Manager.Instance.maxfuelPool;
        PrepPhaseManager.Instance.ResetBars();
        PrepPhaseManager.Instance.UpdateCharacterButtons();
        //PrepPhaseManager.Instance.ChangeSelectedCharacter(0);
        //Debug.Log("Moving to prep");


    }

    public static T GetRandomEnumValue<T>(int start, int end)
    {
        Array enumValues = Enum.GetValues(typeof(T));
        int randomIndex = UnityEngine.Random.Range(start, end);
        T randomEnumValue = (T)enumValues.GetValue(randomIndex);
        return randomEnumValue;
    }
}
