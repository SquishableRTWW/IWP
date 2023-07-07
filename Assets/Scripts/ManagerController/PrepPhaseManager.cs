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
            changeButtons[i].GetComponent<Image>().color = new Color(0, 0, 0, 0.6f);
        }

        characterImage.sprite = characterSelected.GetComponent<SpriteRenderer>().sprite;
        characterName.text = characterSelected.characterName;
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
            changeButtons[i].GetComponent<Image>().color = new Color(0, 0, 0, 1);

            characterImage.sprite = MapManager.Instance.playerCharacters[i].GetComponent<SpriteRenderer>().sprite;
            characterName.text = MapManager.Instance.playerCharacters[i].characterName;
        }
    }

    public void AssignFuel(int amount)
    {
        if (characterSelected.currentFuel < characterSelected.maxFuel)
        {
            characterSelected.currentFuel += amount;
            Manager.Instance.fuelPool -= amount;
        }
    }
    public void DeassignFuel(int amount)
    {
        if (characterSelected.currentFuel > 0)
        {
            characterSelected.currentFuel -= amount;
            Manager.Instance.fuelPool += amount;
        }
    }

    public void MaxAssignFuel()
    {
        characterSelected.currentFuel = characterSelected.maxFuel;
        Manager.Instance.fuelPool -= characterSelected.maxFuel;
    }
    public void MaxDeassignFuel()
    {
        int difference = characterSelected.maxFuel - characterSelected.currentFuel;
        characterSelected.currentFuel = 0;
        Manager.Instance.fuelPool += difference;
    }
}
