using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GameplayManager : MonoBehaviour
{
    [SerializeField] public static GameplayManager GM;

    [SerializeField] Stage[] StageList;
    [SerializeField] int currStage;

    [SerializeField] int numPlayers;
    [SerializeField] GameObject[] PlayerPrefabs;
    [SerializeField] int currPF = 0;
    [SerializeField] PlayerInputManager PIU;
    [SerializeField] PlayerInput[] PlayerInputs;
    [SerializeField] public CameraController CameraCont;
    [SerializeField] Transform CameraFocus;
    [SerializeField] Transform MainCharacter;

    [SerializeField] float ViewSize;
    //[SerializeField] float currTime;
    [SerializeField] int[] Score;
    [SerializeField] PlayerControl[] Players;

    [SerializeField] int Winner = 0;
    [SerializeField] Health[] PlayerHealth; 
    [SerializeField] HealthBar[] PlayerHealthBars;

    [SerializeField] float[] Timer = new float[4];
    [SerializeField] Clock ClockRef;
    [SerializeField] CollectionTracker CollectRef;
    //[Max Minutes, CurrMinutes, Seconds, Milliseconds]



    [SerializeField] Checkpoints[] Checkpoints;
    [SerializeField] int FurthestCheckpoint;
    [SerializeField] public bool StageFinished = false;
    [SerializeField] public float[] StageFinishedEndlag = { 3f, 0f };
    [SerializeField] StageClear StageClearUI;
    [SerializeField] GameObject SCUILerpValue;


    [SerializeField] GameObject CurrentMainPlayer;


    [SerializeField] public bool TimeCountUp = true; //true for TimeCalcUp, false for TimeCalcDown
    [SerializeField] public bool GameOn = true;
    [SerializeField] public bool PauseOn = false;
    [SerializeField] GameObject UIPanel;
    [SerializeField] Button ResumeButton;

    private void Awake()
    {
        if (GM == null)
        {
            DontDestroyOnLoad(this.gameObject);
            //DontDestroyOnLoad(FadeScreen);
            GM = this;
        }

        else if (GM != this)
            Destroy(this.gameObject);
    }
    

    void Start()
    {
        //if(CameraCont == null)
        //    CameraCont = GameObject.Find("Camera").GetComponent<CameraController>();

        TurnOn();

        if (numPlayers <= 1)
        {
            //Debug.Log("Singleplayer Mode");
            CameraCont.setTarget(MainCharacter);
        }
        else
        {
            //Debug.Log("Multiplayer Mode");
            CameraCont.setTarget(CameraFocus);
            CameraCont.setAhead(0);
            CameraCont.GetComponent<Camera>().orthographicSize = ViewSize;
        }

        PIU.playerPrefab = PlayerPrefabs[currPF];

        //PlayerHealth[0] = Players[0].GetComponent<Health>();
        //PlayerHealth[1] = Players[1].GetComponent<Health>();
        //PlayerHealthBars[0].setHealth(PlayerHealth[0]);
        //PlayerHealthBars[1].setHealth(PlayerHealth[1]);


        PlacePlayer();

    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(Input.GetJoystickNames());
        if (GM == null) Awake();
        //Debug.Log(GM.name);
        
        

        //We check if StageFinished and the Endlag are correct so that when the endlag finishes, we dont call P&C over and over again
        if (StageFinished && StageFinishedEndlag[1] > 0)
        {
            StageFinishedEndlag[1] = Mathf.Clamp(StageFinishedEndlag[1] -= Time.deltaTime, 0, StageFinishedEndlag[0]);

            if (StageFinishedEndlag[1] <= 0)
            {
                StageClearUI.PopulateandCalculate((int)Timer[1], (int)Timer[2], CollectRef.getCoins(), Score[0]);
            }
        }

        if(!StageFinished)
        {
            if(TimeCountUp) TimeCalcUp();
            else TimeCalcDown();

            if (numPlayers == 1)
                SinglePlayerScoreCalc();
        }
    }

    public void NextPlayerSpawn()
    {
        currPF++;
        if(currPF >= PlayerPrefabs.Length)
            currPF = 0;

        
        PIU.playerPrefab = PlayerPrefabs[currPF];

    }

    public void ScoreUpdate(int player, int points)
    {
        Score[player] += points;
    }

    
    public void PlayerDown(int i)
    {
        if(Winner == 0)
            switch (i)
            {
                case 1:
                    Winner = 2;
                    break;
                case 2:
                    Winner = 1;
                    break;
            }

        if (Winner != 0)
        {
            GameOn = false;
            if(numPlayers <= 1)
            {
                //dont have the camera track the player
                CameraCont.setTarget(null);
            }
        }
    }

    private void TimeCalcDown()
    {
        //if (Timer[3] <= 0)

        if ((int)(Timer[3]) > (int)(Timer[3] + UnityEngine.Time.deltaTime) || Timer[3] <= 0)
        {
            if (Timer[2] < 0)
            {
                if (Timer[1] < 0)
                {
                    GameOn = false;
                }

                else Timer[1] -= 1;


                if (GameOn)
                {
                    Timer[2] = 59;
                }

            }

            else
            {
                if (GameOn)
                    Timer[2] -= 1;
            }

            if (GameOn)
            {
                Timer[3] = 59;
            }
        }

        if (GameOn)
        {
            //if ()
            //{
            //    Timer[2] -= 1;
            //}
            Timer[3] -= UnityEngine.Time.deltaTime;
        }

        ClockRef.ClockUpdate(Timer[1], Timer[2], Timer[3]);
    }

    private void TimeCalcUp()
    {
        //if (Timer[3] <= 0)

        if ((int)(Timer[3]) < (int)(Timer[3] + UnityEngine.Time.deltaTime) || Timer[3] >= 60)
        {
            if (Timer[2] > 59)
            {
                if (Timer[1] >= Timer[0])
                {
                    GameOn = false;
                    Timer[2] = 59;
                    Timer[1] = 59;
                }

                else Timer[1] += 1;


                if (GameOn)
                {
                    Timer[2] = 0;
                }

            }

            else
            {
                if (GameOn)
                    Timer[2] += 1;
            }

            if (GameOn)
            {
                Timer[3] = 0;
            }
        }

        if (GameOn)
        {
            //if ()
            //{
            //    Timer[2] -= 1;
            //}
            Timer[3] += UnityEngine.Time.deltaTime;
        }

        ClockRef.ClockUpdate(Timer[1], Timer[2], Timer[3]);
    }

    private void SinglePlayerScoreCalc()
    {
        int[] c = CurrentMainPlayer.GetComponent<PlayerControl>().Coins;
        //Copper, Nickels

        CollectRef.ScoreUpdate(c[0], c[1], c[0] + c[1] * 5);
    }

    private void CoinCalc()
    {
        if (GameOn)
        {

        }
    }
    public void setCheckpoint(int n)
    {
        FurthestCheckpoint = Mathf.Max(FurthestCheckpoint, n);

        if (Checkpoints[FurthestCheckpoint].isGoal)
        {
            StageFinished = true;
        }
    }

    public void PlacePlayer()
    {
        CurrentMainPlayer.transform.position = Checkpoints[FurthestCheckpoint].transform.position;
        CurrentMainPlayer.GetComponent<Health>().HealthRefill();
    }

    public void TurnOn()
    {
        if(TimeCountUp) Timer[1] = 0;
        else Timer[1] = Timer[0];

        StageFinished = false;
        StageFinishedEndlag[1] = StageFinishedEndlag[0];

        GameOn = true;
        PauseOn = true;
        PauseButton();

        Instantiate(StageList[currStage]);
        Checkpoints = StageList[currStage].GetCheckpoints();


    }

    public void SetStage(int i)
    {
        currStage = i;
    }

    public void PauseButton()
    {
        if (!StageFinished)
        {
            PauseOn = !PauseOn;

            UIPanel.SetActive(PauseOn);

            if (PauseOn)
                Time.timeScale = 0;

            else Time.timeScale = 1;

            EventSystem.current.SetSelectedGameObject(ResumeButton.gameObject);
        }
    }
}
