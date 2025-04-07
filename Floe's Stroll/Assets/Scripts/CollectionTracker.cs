using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CollectionTracker : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI CopperDisplay;
    [SerializeField] TextMeshProUGUI NickelDisplay;
    [SerializeField] TextMeshProUGUI CoinsDisplay;

    [SerializeField] int CopperAmount = 0;
    [SerializeField] int NickelAmount = 0;
    [SerializeField] int CoinsAmount = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ScoreUpdate(int c, int n, int coins)
    {
        CopperAmount = c;
        NickelAmount = n;
        CoinsAmount = coins;

        /*
        switch (type)
        {
            case "Copper":
                CopperAmount++;
                break;
            case "Nickel":
                NickelAmount++;
                break;
            case "Coin":
                CoinsAmount++;
                break;

        }
        */
        if (CopperDisplay)
        {
            CopperDisplay.text = CopperAmount.ToString();
        }
        
        if (NickelDisplay)
        {
            NickelDisplay.text = NickelAmount.ToString();
        }
        
        if (CoinsDisplay)
        {
            CoinsDisplay.text = CoinsAmount.ToString();
        }
    }
}
