using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PrepPhaseManager : MonoBehaviour
{
    [SerializeField] CharacterBehaviour characterSelected;

    // GUI Components
    [SerializeField] Canvas prepCanvas;
    [SerializeField] List<Button> changeButtons;
    [SerializeField] Image characterImage;
    [SerializeField] TextMeshProUGUI characterName;
    [SerializeField] FuelBar characterFuelbar;
    [SerializeField] FuelBar poolFuelBar;
    [SerializeField] GameObject slotContainer;

    [SerializeField] List<ItemSlot> weaponSlots;
    [SerializeField] List<ItemSlot> equipmentSlots;


    // Start is called before the first frame update
    void Start()
    {
        if (MapManager.Instance.playerCharacters.Count > 0)
        {
            characterSelected = MapManager.Instance.playerCharacters[0];
        }
        for (int i = 0; i < MapManager.Instance.playerCharacters.Count; i++)
        {
            changeButtons[i].GetComponent<Image>().sprite = MapManager.Instance.playerCharacters[i].GetComponent<SpriteRenderer>().sprite;
            changeButtons[i].GetComponent<Image>().color = new Color(1, 1, 1, 0.6f);
        }

        // Set stats
        characterImage.sprite = characterSelected.GetComponent<SpriteRenderer>().sprite;
        characterName.text = characterSelected.characterName;
        characterFuelbar.SetBarLimit(characterSelected.maxFuel);
        poolFuelBar.SetBarLimit(Manager.Instance.fuelPool);
        //characterFuelbar.SetFuel(characterSelected.currentFuel);
        poolFuelBar.SetFuel(Manager.Instance.fuelPool);
        UpdateEquipSlots();
    }
    private void Update()
    {
        if (Manager.Instance.isInCombat)
        {
            prepCanvas.gameObject.SetActive(false);
        }
        else
        {
            prepCanvas.gameObject.SetActive(true);
        }
    }

    public void ChangeSelectedCharacter(int i)
    {
        if (MapManager.Instance.playerCharacters.Count > i)
        {
            characterSelected = MapManager.Instance.playerCharacters[i];

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
        }
    }

    public void AssignFuel(int amount)
    {
        if (characterSelected.currentFuel < characterSelected.maxFuel && Manager.Instance.fuelPool > 0)
        {
            characterSelected.currentFuel += amount;
            Manager.Instance.fuelPool -= amount;
            characterFuelbar.SetFuel(characterSelected.currentFuel);
            poolFuelBar.SetFuel(Manager.Instance.fuelPool);
        }
    }
    public void DeassignFuel(int amount)
    {
        if (characterSelected.currentFuel > 0)
        {
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
        characterSelected.currentFuel = characterSelected.maxFuel;
        Manager.Instance.fuelPool -= characterSelected.maxFuel;
        characterFuelbar.SetFuel(characterSelected.currentFuel);
        poolFuelBar.SetFuel(Manager.Instance.fuelPool);
    }
    public void MaxDeassignFuel()
    {
        int difference = 0 - characterSelected.currentFuel;
        if (Manager.Instance.fuelPool <= Manager.Instance.maxfuelPool - characterSelected.currentFuel)
        {
            Manager.Instance.fuelPool += difference;
        }
        else
        {
            Manager.Instance.fuelPool += Manager.Instance.maxfuelPool - characterSelected.currentFuel;
        }

        characterSelected.currentFuel = 0;
        characterFuelbar.SetFuel(characterSelected.currentFuel);
        poolFuelBar.SetFuel(Manager.Instance.fuelPool);
    }

    public void UpdateEquipSlots()
    {
        // Remove all previously shown equip slots
        foreach(Transform child in slotContainer.transform)
        {
            if (child.gameObject.GetComponent<WeaponBehaviour>() != null)
            {
                Destroy(child.gameObject);
            }
            if (child.gameObject.GetComponent<EquipmentBehaviour>() != null)
            {
                Destroy(child.gameObject);
            }
        }
        // Show all weapons equipped on character
        for (int i = 0; i < characterSelected.weaponsEquipped.Count; i++)
        {
            GameObject weapon = Instantiate(characterSelected.weaponsEquipped[i], weaponSlots[i].transform.position, Quaternion.identity);
            weapon.transform.SetParent(slotContainer.transform);
            weapon.GetComponent<RectTransform>().anchoredPosition = weaponSlots[i].GetComponent<RectTransform>().anchoredPosition;
            //Debug.Log("Weapon Updated");
        }
        // Show all equipment equipped on character
        for (int i = 0; i < characterSelected.equipmentList.Count; i++)
        {
            if (characterSelected.equipmentList[i] != null)
            {
                GameObject equipment = Instantiate(characterSelected.equipmentList[i], equipmentSlots[i].transform.position, Quaternion.identity);
                equipment.transform.SetParent(slotContainer.transform);
                equipment.GetComponent<RectTransform>().anchoredPosition = equipmentSlots[i].GetComponent<RectTransform>().anchoredPosition;
                //Debug.Log("Equipment Updated");
            }
        }
    }
}
