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
    [SerializeField] int CP;
    public bool isInCombat = true;
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
        timeLimit = 20f;
        TimerSlider.SetMaxTime(timeLimit);
        CP = 2;
    }

    // Update is called once per frame
    void Update()
    {
        if (isInCombat)
        {
            timeLimit -= Time.deltaTime;
            timerText.text = string.Format("{0:0.00}", timeLimit);
            CPText.text = "CP Left: " + CP.ToString();
            TimerSlider.SetTime(timeLimit);
        }
        if (timeLimit <= 0)
        {
            // Insert function call to start enemy AI here;
            StartCoroutine(TempEnemyTurn());
            timeLimit = 20f;
        }
    }

    public IEnumerator TempEnemyTurn()
    {
        infoText.text = "Turn Ended, wait 3 seconds";
        MapManager.Instance.turnEnded();
        yield return new WaitForSeconds(3f);
    }

    public void ChangeCP(int amount)
    {
        CP -= amount;
    }

    public float GetTimeLimit()
    {
        return timeLimit;
    }
}
