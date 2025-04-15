using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchSystem : MonoBehaviour
{
    [SerializeField] bool switchstate;
    [SerializeField] bool isOneWay;

    [SerializeField] Switch S;
    
    [SerializeField] MovingTerrain MT;
    


    // Start is called before the first frame update
    void Start()
    {
        switchstate = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void FlipState()
    {
        if (!isOneWay) { 
            switchstate = true;
            PerformAction();
        }
        else { switchstate = !switchstate; PerformAction(); }
    }

    public void PerformAction(string s = "")
    {
        switch (s)
        {
            case "MovingTerrain":
                MT.TerrainActivate();
                break;
        }
    }

}
