using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class Manager : MonoBehaviour
{
    private static Manager _instance;
    public static Manager Instance { get { return _instance; } }


    public MouseController mouseController;
    [SerializeField] TextMeshProUGUI timerText;
    [SerializeField] TextMeshProUGUI infoText;
    [SerializeField] TextMeshProUGUI CPText;
    [SerializeField] float timeLimit;
    [SerializeField] float originalTime;
    [SerializeField] int CP;
    public bool isInCombat = true;
    public bool playerTurn = true;
    [SerializeField] TimerBar TimerSlider;

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
        timeLimit = originalTime;
        TimerSlider.SetMaxTime(timeLimit);
        CP = 2;
    }

    // Update is called once per frame
    void Update()
    {
        if (isInCombat)
        {
            if (playerTurn)
            {
                timeLimit -= Time.deltaTime;
            }
            timerText.text = string.Format("{0:0.00}", timeLimit);
            CPText.text = "CP Left: " + CP.ToString();
            TimerSlider.SetTime(timeLimit);
        }
        if (timeLimit <= 0)
        {
            // Insert function call to start enemy AI here;
            playerTurn = false;
            StartCoroutine(TempEnemyTurn());
            timeLimit = originalTime;

        }
    }

    public IEnumerator TempEnemyTurn()
    {
        infoText.text = "Enemy turn. Wait 3 S";
        yield return new WaitForSeconds(3f);
        MapManager.Instance.turnEnded();
        CP = 2;
        timeLimit = originalTime;
        playerTurn = true;
        mouseController.DeselectAction();
    }
    public void EndTurn()
    {
        playerTurn = false;
        StartCoroutine(TempEnemyTurn());
    }

    public void ChangeCP(int amount)
    {
        CP -= amount;
    }
    public int GetCP()
    {
        return CP;
    }
    public void SetTimeLimit(float newLimit)
    {
        timeLimit = newLimit;
    }

    public float GetTimeLimit()
    {
        return timeLimit;
    }
}
