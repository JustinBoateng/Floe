using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StageClear : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI TimeScore;
    [SerializeField] TextMeshProUGUI CoinScore;
    [SerializeField] TextMeshProUGUI TotalScore;

    [SerializeField] int time;
    [SerializeField] int coin;
    [SerializeField] int score;


    [SerializeField] public GameObject OnScreenPosition;
    [SerializeField] public GameObject OffScreenPosition;
    [SerializeField] float SCUILerpValue = 0;
    [SerializeField] float Speed = 0;
    [SerializeField] float direction = 1;

    [SerializeField] float[] calcCooldown = { 3, 0 };


    //GOAL:
    // Populate the time, coin, and score values from GameplayManager. 
    // Then calculate them to their score variants:
    // TimeScore = Mins
    // Then, put them into the TimeScore, CoinScore, TotalScore fields
    // Have the Stage Clear UI slide into the frame

    // Start is called before the first frame update
    void Start()
    {
        SCUILerpValue = 0;
        transform.position = Vector2.Lerp(OffScreenPosition.transform.position, OnScreenPosition.transform.position, SCUILerpValue);

        calcCooldown[1] = calcCooldown[0];
    }

    // Update is called once per frame
    void Update()
    {
        if(GameplayManager.GM.StageFinishedEndlag[1] <= 0 && SCUILerpValue < 1)
        {
            SlideIn(); 
        }

        //we check both scuiLerpValue and calcCooldown[1] so that once the calcCooldown was finished, it won't call PopulateandCalculate over and over 
        if(SCUILerpValue == 1 && calcCooldown[1] > 0)
        {
            calcCooldown[1] = Mathf.Clamp(calcCooldown[1] -= Time.deltaTime, 0, calcCooldown[0]);

            if (calcCooldown[1] <= 0)
            {
                PopulateandCalculate(0, 0, 0, coin + time + score);
            }
        }

    }


    public void SlideIn()
    {
        SCUILerpValue = Mathf.Clamp(SCUILerpValue += (Time.deltaTime * Speed * direction), 0, 1);

        transform.position = Vector2.Lerp(OffScreenPosition.transform.position, OnScreenPosition.transform.position, SCUILerpValue);
    }

    public void PopulateandCalculate(int m, int se, int c, int sc)
    {
        score = sc * 100;
        coin = c * 100;

        time =  (100 - m * 10) + (60 - se);

        TotalScore.text = score.ToString();
        CoinScore.text = coin.ToString();
        TimeScore.text = time.ToString();

    }
}
