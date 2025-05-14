using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class File : MonoBehaviour
{

    [SerializeField] public int FileNumber;
    [SerializeField] public int NoofLives;
    [SerializeField] public int TimeMins;
    [SerializeField] public int TimeSecs;
    [SerializeField] public string CurrStage = "NewFile"; 
    [SerializeField] public int CurrStageInt;
    [SerializeField] public bool[] CollectedFountains;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


}
