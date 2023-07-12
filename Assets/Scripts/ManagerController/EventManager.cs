using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Event
{
    public string eventName;
    public string eventType;
    public int amount;
}

public class EventManager : MonoBehaviour
{
    private static EventManager _instance;
    public static EventManager Instance { get { return _instance; } }

    public int randomEventNumber;
    public List<Event> eventList;

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
        randomEventNumber = Random.Range(0, 100);
    }

    public void ActivateLeftDecision()
    {

    }

    public void ActivateRightDecision()
    {

    }
}
