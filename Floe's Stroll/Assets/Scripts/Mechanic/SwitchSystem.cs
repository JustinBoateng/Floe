using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchSystem : MonoBehaviour
{
    [SerializeField] string Type;
    //Switch (The system activates when a button is pressed)
    //Trigger (like, reaching a certain area wil trigger some event)
    //Solution (system activates when a condition is met, say, if all the enemies in an area are defeated)

    [SerializeField] bool switchstate;
    [SerializeField] bool isOneWay;

    [SerializeField] Switch S;
    
    [SerializeField] MovingTerrain[] MT;
    [SerializeField] float[] Timer = new float[2];
    



    // Start is called before the first frame update
    void Start()
    {
        switchstate = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /*
    public void FlipState(string s = "")
    {
        if (!isOneWay) { 
            switchstate = true;
            PerformAction();
        }
        else { switchstate = !switchstate; PerformAction(s, switchstate); }
    }
    */
    public void PerformAction(string s = "", bool state = true)
    {
        switch (s)
        {
            case "MovingTerrain":
                if (state)
                    foreach (MovingTerrain t in MT) { 
                    t.TerrainActivate(1);
                    }

                else
                    foreach (MovingTerrain t in MT)
                    {
                        t.TerrainActivate(-1); 
                    }

                break;
        }
    }

}
