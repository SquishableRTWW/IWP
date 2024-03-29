using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PrepPhaseManager : MonoBehaviour
{
    private static PrepPhaseManager _instance;
    public static PrepPhaseManager Instance { get { return _instance; } }

    public CharacterBehaviour characterSelected;

    // GUI Components
    [SerializeField] Canvas prepCanvas;
    [SerializeField] Canvas eventCanvas;
    [SerializeField] Canvas CombatCanvas;
    public List<Button> changeButtons;
    public List<Button> fuelButtons;
    public Image characterImage;
    public TextMeshProUGUI characterName;
    [SerializeField] TextMeshProUGUI fuelPoolText;
    [SerializeField] TextMeshProUGUI reminderText;
    public FuelBar characterFuelbar;
    public FuelBar poolFuelBar;
    [SerializeField] GameObject slotContainer;
    public TextMeshProUGUI levelText;

    public List<GameObject> itemsInGame;
    public List<GameObject> weaponsInGame;
    public List<GameObject> equipmentInGame;
    public List<ItemSlot> weaponSlots;
    [SerializeField] List<ItemSlot> equipmentSlots;
    [SerializeField] List<GameObject> inventorySlots;
    [SerializeField] List<Button> characterSelectButtons;

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
        for (int i = 0; i < MapManager.Instance.playerCharacters.Count; i++)
        {
            changeButtons[i].GetComponent<Image>().sprite = MapManager.Instance.playerCharacters[i].GetComponent<SpriteRenderer>().sprite;
            changeButtons[i].GetComponent<Image>().color = new Color(1, 1, 1, 0.6f);
        }

        // Fill up inventory List
        foreach (Transform slotChild in slotContainer.transform)
        {
            if (slotChild.gameObject.GetComponent<ItemSlot>() != null)
            {
                if (slotChild.gameObject.GetComponent<ItemSlot>().slotType == "Item")
                {
                    inventorySlots.Add(slotChild.gameObject);
                }
            }
        }
        DisplayInventoryItems();
        UpdateCharacterButtons();

        //ChangeSelectedCharacter(0);
        //// Set stats
        //characterImage.sprite = characterSelected.GetComponent<SpriteRenderer>().sprite;
        //characterName.text = characterSelected.characterName;
        //characterFuelbar.SetBarLimit(characterSelected.maxFuel);
        poolFuelBar.SetBarLimit(Manager.Instance.maxfuelPool);
        ////characterFuelbar.SetFuel(characterSelected.currentFuel);
        poolFuelBar.SetFuel(Manager.Instance.fuelPool);
        //UpdateEquipSlots();
        //ChangeSelectedCharacter(0);
    }
    private void Update()
    {
        if (Manager.Instance.isInCombat)
        {
            prepCanvas.gameObject.SetActive(false);
        }
        levelText.text = "LV:" + MapManager.Instance.levelTier + "-" + MapManager.Instance.level;
        fuelPoolText.text = "FUEL POOL: " + Manager.Instance.fuelPool + "/" + Manager.Instance.maxfuelPool;
    }

    public void ChangeSelectedCharacter(int i)
    {
        SoundManager.Instance.PlaySound(SoundManager.Sound.ButtonClick);
        // For tutorial purposes
        if (StateNameController.isInTutorial && Manager.Instance.tutorialNumber == 1)
        {
            SoundManager.Instance.PlaySound(SoundManager.Sound.TutorialClick);
            Manager.Instance.tutorialNumber++;
            TTTextBoxBehaviour firstGameObject = FindObjectOfType<TTTextBoxBehaviour>();
            if (firstGameObject != null)
            {
                Destroy(firstGameObject.gameObject);
            }
            Manager.Instance.SpawnNextTutorial(Manager.Instance.tutorialNumber);
        }

        if (MapManager.Instance.playerCharacters.Count > i)
        {
            characterSelected = MapManager.Instance.playerCharacters[i];

            ShowCharacterPrep();
            // Re-highlight button pressed
            foreach (Button button in changeButtons)
            {
                button.GetComponent<Image>().color = new Color(1, 1, 1, 0.6f);
            }
            changeButtons[i].GetComponent<Image>().color = new Color(1, 1, 1, 1);

            characterImage.sprite = characterSelected.GetComponent<SpriteRenderer>().sprite;
            characterName.text = characterSelected.characterName;
            characterFuelbar.SetBarLimit(characterSelected.maxFuel);
            characterFuelbar.SetFuel(characterSelected.currentFuel);
            UpdateEquipSlots();
            ShowCharacterPrep();

            if (characterSelected.weaponsEquipped.Count < 2)
            {
                weaponSlots[1].gameObject.SetActive(false);
            }
            else
            {
                weaponSlots[1].gameObject.SetActive(true);
            }
        }
    }

    public void AssignFuel(int amount)
    {
        if (characterSelected != null && characterSelected.currentFuel < characterSelected.maxFuel && Manager.Instance.fuelPool > 0)
        {
            SoundManager.Instance.PlaySound(SoundManager.Sound.ButtonClick);
            characterSelected.currentFuel += amount;
            Manager.Instance.fuelPool -= amount;
            characterFuelbar.SetFuel(characterSelected.currentFuel);
            poolFuelBar.SetFuel(Manager.Instance.fuelPool);
        }
    }
    public void DeassignFuel(int amount)
    {
        if (characterSelected != null && characterSelected.currentFuel > 0)
        {
            SoundManager.Instance.PlaySound(SoundManager.Sound.ButtonClick);
            characterSelected.currentFuel -= amount;
            if (Manager.Instance.fuelPool < Manager.Instance.maxfuelPool)
            {
                Manager.Instance.fuelPool += amount;
            }
            characterFuelbar.SetFuel(characterSelected.currentFuel);
            poolFuelBar.SetFuel(Manager.Instance.fuelPool);
        }
    }

    public void MaxAssignFuel()
    {
        if (characterSelected != null)
        {
            SoundManager.Instance.PlaySound(SoundManager.Sound.ButtonClick);
            if (Manager.Instance.fuelPool >= characterSelected.maxFuel)
            {
                characterSelected.currentFuel = characterSelected.maxFuel;
                Manager.Instance.fuelPool -= characterSelected.maxFuel;
                characterFuelbar.SetFuel(characterSelected.currentFuel);
                poolFuelBar.SetFuel(Manager.Instance.fuelPool);
            }
            else
            {
                characterSelected.currentFuel += Manager.Instance.fuelPool;
                Manager.Instance.fuelPool = 0;
                characterFuelbar.SetFuel(characterSelected.currentFuel);
                poolFuelBar.SetFuel(Manager.Instance.fuelPool);
            }
        }
    }
    public void MaxDeassignFuel()
    {
        if (characterSelected != null)
        {
            SoundManager.Instance.PlaySound(SoundManager.Sound.ButtonClick);
            Manager.Instance.fuelPool += characterSelected.currentFuel;
            if (Manager.Instance.fuelPool > Manager.Instance.maxfuelPool)
            {
                Manager.Instance.fuelPool = Manager.Instance.maxfuelPool;
            }

            characterSelected.currentFuel = 0;
            characterFuelbar.SetFuel(characterSelected.currentFuel);
            poolFuelBar.SetFuel(Manager.Instance.fuelPool);
        }
    }

    public void UpdateEquipSlots()
    {
        // Remove all previously shown equip slots
        foreach(Transform child in slotContainer.transform)
        {
            if (child.gameObject.GetComponent<WeaponBehaviour>() != null)
            {
                if (!child.gameObject.GetComponent<WeaponBehaviour>().isInInventory)
                {
                    Destroy(child.gameObject);
                }
            }
            if (child.gameObject.GetComponent<EquipmentBehaviour>() != null)
            {
                if (!child.gameObject.GetComponent<EquipmentBehaviour>().isInInventory)
                {
                    Destroy(child.gameObject);
                }
            }
        }
        // Show all weapons equipped on character
        for (int i = 0; i < characterSelected.weaponsEquipped.Count; i++)
        {
            if (characterSelected.weaponsEquipped[i] != null)
            {
                GameObject weapon = Instantiate(characterSelected.weaponsEquipped[i], weaponSlots[i].transform.position, Quaternion.identity);
                weapon.transform.SetParent(slotContainer.transform);
                weapon.GetComponent<RectTransform>().anchoredPosition = weaponSlots[i].GetComponent<RectTransform>().anchoredPosition;
                weapon.GetComponent<DragDrop>().prevSlot = weaponSlots[i].gameObject;
                weapon.GetComponent<WeaponBehaviour>().isInInventory = false;
                weapon.GetComponent<WeaponBehaviour>().isInstantiated = true;
                //Debug.Log("Weapon Updated");
            }
        }
        // Show all equipment equipped on character
        for (int i = 0; i < characterSelected.equipmentList.Count; i++)
        {
            if (characterSelected.equipmentList[i] != null)
            {
                GameObject equipment = Instantiate(characterSelected.equipmentList[i], equipmentSlots[i].transform.position, Quaternion.identity);
                equipment.transform.SetParent(slotContainer.transform);
                equipment.GetComponent<RectTransform>().anchoredPosition = equipmentSlots[i].GetComponent<RectTransform>().anchoredPosition;
                equipment.GetComponent<DragDrop>().prevSlot = equipmentSlots[i].gameObject;
                equipment.GetComponent<EquipmentBehaviour>().isInInventory = false;
                equipment.GetComponent<EquipmentBehaviour>().isInstantiated = true;
                //Debug.Log("Equipment Updated");
            }
        }
    }

    public void ResetBars()
    {
        characterSelected = null;
        poolFuelBar.SetMaxFuel(Manager.Instance.fuelPool);
    }

    public void DisplayInventoryItems()
    {
        foreach (Transform item in slotContainer.transform)
        {
            if (item.gameObject.GetComponent<WeaponBehaviour>())
            {
                if (!item.gameObject.GetComponent<WeaponBehaviour>().isInInventory)
                {
                    Destroy(item.gameObject);
                }
            }
            else if (item.gameObject.GetComponent<EquipmentBehaviour>())
            {
                if (!item.gameObject.GetComponent<EquipmentBehaviour>().isInInventory)
                {
                    Destroy(item.gameObject);
                }
            }
            else
            {
                continue;
            }
        }


        for (int i = 0; i < Manager.Instance.playerItemList.Count; i++)
        {
            if (Manager.Instance.playerItemList[i] != null)
            {
                if (Manager.Instance.playerItemList[i].GetComponent<WeaponBehaviour>())
                {
                    if (!Manager.Instance.playerItemList[i].GetComponent<WeaponBehaviour>().isInstantiated)
                    {
                        GameObject item = Instantiate(Manager.Instance.playerItemList[i], inventorySlots[i].transform.position, Quaternion.identity);
                        Manager.Instance.playerItemList[i] = item;
                        item.transform.SetParent(slotContainer.transform);
                        item.GetComponent<RectTransform>().anchoredPosition = inventorySlots[i].GetComponent<RectTransform>().anchoredPosition;
                        item.GetComponent<DragDrop>().prevSlot = inventorySlots[i];
                        item.GetComponent<WeaponBehaviour>().isInInventory = true;
                        item.GetComponent<WeaponBehaviour>().isInstantiated = true;
                    }
                }
                else if (Manager.Instance.playerItemList[i].GetComponent<EquipmentBehaviour>())
                {
                    if (!Manager.Instance.playerItemList[i].GetComponent<EquipmentBehaviour>().isInstantiated)
                    {
                        GameObject item = Instantiate(Manager.Instance.playerItemList[i], inventorySlots[i].transform.position, Quaternion.identity);
                        Manager.Instance.playerItemList[i] = item;
                        item.transform.SetParent(slotContainer.transform);
                        item.GetComponent<RectTransform>().anchoredPosition = inventorySlots[i].GetComponent<RectTransform>().anchoredPosition;
                        item.GetComponent<DragDrop>().prevSlot = inventorySlots[i];
                        item.GetComponent<EquipmentBehaviour>().isInInventory = true;
                        item.GetComponent<EquipmentBehaviour>().isInstantiated = true;
                    }
                }
            }
        }
    }

    public void UpdateCharacterButtons()
    {
        for (int i = 0; i < characterSelectButtons.Count; i++)
        {
            characterSelectButtons[i].gameObject.SetActive(false);
        }
        for (int i = 0; i < MapManager.Instance.playerCharacters.Count; i++)
       {
            characterSelectButtons[i].gameObject.SetActive(true);
       }
    }

    public void ShowCharacterPrep()
    {
        reminderText.gameObject.SetActive(false);
        characterFuelbar.gameObject.SetActive(true);
        characterName.gameObject.SetActive(true);
        characterImage.gameObject.SetActive(true);
        foreach (var slot in equipmentSlots)
        {
            slot.gameObject.SetActive(true);
        }
        foreach (var slot in weaponSlots)
        {
            slot.gameObject.SetActive(true);
        }
        foreach (var button in fuelButtons)
        {
            button.gameObject.SetActive(true);
        }
    }
    public void HideCharacterPrep()
    {
        reminderText.gameObject.SetActive(true);
        characterFuelbar.gameObject.SetActive(false);
        characterName.gameObject.SetActive(false);
        characterImage.gameObject.SetActive(false);
        foreach (var slot in equipmentSlots)
        {
            slot.gameObject.SetActive(false);
        }
        foreach (var slot in weaponSlots)
        {
            slot.gameObject.SetActive(false);
        }
        foreach (var button in fuelButtons)
        {
            button.gameObject.SetActive(false);
        }
    }

    public void ShowBattleField()
    {
        SoundManager.Instance.PlaySound(SoundManager.Sound.ButtonClick);
        // Hide all the canvases stuff except the return button
        foreach (Transform child in prepCanvas.transform)
        {
            if (child.gameObject.name != "Button_HideLevel")
            {
                child.gameObject.SetActive(false);
            }
            else
            {
                child.gameObject.SetActive(true);
            }
        }
        foreach (Transform child in eventCanvas.transform)
        {
            child.gameObject.SetActive(false);
        }
        CombatCanvas.gameObject.SetActive(false);
    }
    public void HideBattleField()
    {
        SoundManager.Instance.PlaySound(SoundManager.Sound.ButtonClick);
        // Show all the canvases again and hide the ShowBattleField() Button
        foreach (Transform child in CombatCanvas.transform)
        {
            if (child.gameObject.name == "Button_move" || child.gameObject.name == "Button_Cancel" || child.gameObject.name == "Canvas_CharacterInfo" ||
                child.gameObject.name == "Button_Attack1" || child.gameObject.name == "Button_Attack2" || child.gameObject.name == "ToolTipBox")
            {
                child.gameObject.SetActive(false);
            }
            else
            {
                child.gameObject.SetActive(true);
            }
        }
        foreach (Transform child in eventCanvas.transform)
        {
            child.gameObject.SetActive(true);
        }
        foreach (Transform child in prepCanvas.transform)
        {
            if (child.gameObject.name != "Button_HideLevel" && child.gameObject.name != "Text_SelectionReminder")
            {
                child.gameObject.SetActive(true);
            }
            else
            {
                child.gameObject.SetActive(false);
            }
            if (characterSelected == null)
            {
                if (child.gameObject.name != "Button_HideLevel" && child.gameObject.name != "Image_CharacterSelected" && child.gameObject.name != "Button_AF" && child.gameObject.name != "Button_DF"
                && child.gameObject.name != "Button_MAX" && child.gameObject.name != "Button_MIN" && child.gameObject.name != "PrepFuelBar")
                {
                    child.gameObject.SetActive(true);
                }
                else
                {
                    child.gameObject.SetActive(false);
                }
                if (child.gameObject.name == "Text_SelectionReminder")
                {
                    child.gameObject.SetActive(true);
                }
                
            }
        }
        Manager.Instance.warningGUI.SetActive(false);
        UpdateCharacterButtons();
    }
}
