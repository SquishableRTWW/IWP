using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterBehaviour : MonoBehaviour
{
    public int maxFuel;
    public int currentFuel;
    public OverlayTileBehaviour activeTile;
    // Start is called before the first frame update
    void Start()
    {
        currentFuel = maxFuel;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
