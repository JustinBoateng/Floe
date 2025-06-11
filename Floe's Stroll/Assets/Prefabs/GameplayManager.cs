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
    [SerializeField] string nextStage;
    [SerializeField] int currStage;

    [SerializeField] int numPlayers;
    [SerializeField] GameObject CurrentMainPlayer;
    [SerializeField] GameObject[] PlayerPrefabs;


    //[SerializeField] int currPF = 0;
    //[SerializeField] PlayerInputManager PIU;
    //[SerializeField] PlayerInput[] PlayerInputs;
    [SerializeField] public CameraController CameraCont;

    [SerializeField] float ViewSize;
    [SerializeField] int[] Score;
    //[SerializeField] PlayerControl[] Players;

    //[SerializeField] int Winner = 0;
    //[SerializeField] Health[] PlayerHealth; 
    //[SerializeField] HealthBar[] PlayerHealthBars;

    [SerializeField] float[] Timer = new float[4];
    [SerializeField] Clock ClockRef;
    [SerializeField] CollectionTracker CollectRef;
    //[Max Minutes, CurrMinutes, Seconds, Milliseconds]



    [SerializeField] Checkpoints[] Checkpoints;
    [SerializeField] int FurthestCheckpoint;
    [SerializeField] public bool StageFinished = false;
    [SerializeField] public float[] StageFinishedEndlag = { 3f, 0f };
    [SerializeField] StageClear StageClearUI;
    //[SerializeField] GameObject SCUILerpValue;


    [SerializeField] public bool TimeCountUp = true; //true for TimeCalcUp, false for TimeCalcDown
    [SerializeField] public bool GameOn = true;
    [SerializeField] public bool PauseOn = false;
    [SerializeField] GameObject PauseUIPanel;
    [SerializeField] Button ResumeButton;

    private void Awake()
    {
        if (GM == null)
        {
            DontDestroyOnLoad(this.gameObject);
            GM = this;
        }

        else if (GM != this)
            Destroy(this.gameObject);
    }
    

    void Start()
    {
        Debug.Log("GameplayManager's Start Function Activated");
        TurnOn();
    }

    // Update is called once per frame
    void Update()
    {
        if (GM == null) Awake();

        //ScoreCalc Countdown
        //We check if StageFinished and the Endlag are correct so that when the endlag finishes, we dont call StageClear.P&C over and over again
        if (StageFinished && StageFinishedEndlag[1] > 0)
        {
            StageFinishedEndlag[1] = Mathf.Clamp(StageFinishedEndlag[1] -= Time.deltaTime, 0, StageFinishedEndlag[0]);

            if (StageFinishedEndlag[1] <= 0)
            {
                StageClearUI.PopulateandCalculate((int)Timer[1], (int)Timer[2], CollectRef.getCoins(), Score[0]);
            }
        }

        //Time and Score Calc
        if(!StageFinished)
        {            
            TimeCalc();

            if (numPlayers == 1 && CurrentMainPlayer != null)
                SinglePlayerScoreCalc();
        }
    }

    #region Score and Time Functions
    public void ScoreUpdate(int player, int points)
    {
        Score[player] += points;
    }

    private void TimeCalc()
    {
        //Timer [Max, Min, Sec, Milli]
        //Timer[3] contains the deltaTime, so the summation should be less than 1, usually. 
        //Make it > 0.98 so that you don't accidentally include 1.00 in the millisecond timer
        if(TimeCountUp)
        {
            if (Timer[3] > 0.98)
            {
                if (Timer[2] >= 59)
                {
                    if (Timer[1] >= Timer[0])
                    { GameOn = false; Timer[2] = 59; Timer[1] = 59; }

                    else Timer[1] += 1;


                    if (GameOn) Timer[2] = 0;

                }

                else if (GameOn) Timer[2] += 1;



                if (GameOn) Timer[3] = 0;
            }

            if (GameOn)
            {
                Timer[3] += UnityEngine.Time.deltaTime;
            }
        }

        if (!TimeCountUp)
        {
            if (Timer[3] < 0.01)
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
                Timer[3] -= UnityEngine.Time.deltaTime;
            }

        }
        
        ClockRef.ClockUpdate(Timer[1], Timer[2], Timer[3]);

    }

    private void SinglePlayerScoreCalc()
    {
        int[] c = CurrentMainPlayer.GetComponent<PlayerControl>().Coins;
        CollectRef.ScoreUpdate(c[0], c[1], c[0] + c[1] * 5);
    }

    #endregion

    #region Other Functions for other classes to use
    public void PauseButton()
    {
        if (!StageFinished)
        {
            PauseOn = !PauseOn;

            PauseUIPanel.SetActive(PauseOn);

            if (PauseOn)
                Time.timeScale = 0;

            else Time.timeScale = 1;

            EventSystem.current.SetSelectedGameObject(ResumeButton.gameObject);
        }
    }

    public void setCheckpoint(int n)
    {
        FurthestCheckpoint = Mathf.Max(FurthestCheckpoint, n);
    }

    public PlayerControl GetPlayer()
    {
        return CurrentMainPlayer.GetComponent<PlayerControl>();
    }
    #endregion

    #region Loading Game Functions
    public IEnumerator LoadTransition(float LoadTime, string nS)
    {
        nextStage = nS;

        yield return new WaitForSeconds(LoadTime);

        ResetVariables();

        TransitionManager.TM.MoveScene(nextStage);

    }

    public void ResetVariables()
    {
        StageFinished = false;
        StageFinishedEndlag[1] = StageFinishedEndlag[0];
        StageClearUI.resetPos();

        if (TimeCountUp) Timer[1] = 0;
        else Timer[1] = Timer[0];
        Timer[2] = Timer[3] = 0;


        PauseOn = true;
        PauseButton();
    }
    public void TurnOn()
    {
        Debug.Log("TurnOn Function Activated");

        ClockRef = GameObject.Find("ClockObject").GetComponent<Clock>();

        if (TimeCountUp) Timer[1] = 0;
        else Timer[1] = Timer[0];

        StageFinished = false;
        StageFinishedEndlag[1] = StageFinishedEndlag[0];
        FurthestCheckpoint = 0;

        GameOn = true;
        PauseOn = true;
        if(!PauseUIPanel) PauseUIPanel = SinglePlayerCanvas.Instance.getPauseMenu();
        PauseButton();

        Debug.Log("Spawning Stage");
        Instantiate(StageList[currStage]);
        Checkpoints = StageList[currStage].GetCheckpoints();

        Debug.Log(CurrentMainPlayer);

        if (CurrentMainPlayer == null)
        {
            CurrentMainPlayer = Instantiate(PlayerPrefabs[0]);
            CameraController.CC.setTarget(CurrentMainPlayer.transform);
            PlacePlayer();
            SinglePlayerCanvas.Instance.ConnectPlayertoPlayerHealth(
                CurrentMainPlayer.GetComponent<Health>()
                );
        }

        SinglePlayerCanvas.Instance.ConnectPlayertoPlayerHealth(
            CurrentMainPlayer.GetComponent<Health>()
            );

    }
    public void SetStage(int i)
    {
        currStage = i;
    }
    public void PlacePlayer()
    {
        CurrentMainPlayer.transform.position = Checkpoints[FurthestCheckpoint].transform.position;
        CurrentMainPlayer.GetComponent<Health>().HealthRefill();
    }
    
    #endregion    

    #region Multiplayer Scripts
    /*public void NextPlayerSpawn()
    {
        currPF++;
        if (currPF >= PlayerPrefabs.Length)
            currPF = 0;
    }

    public void PlayerDown(int i)
    {
        if (Winner == 0)
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
            if (numPlayers <= 1)
            {
                //dont have the camera track the player
                CameraCont.setTarget(null);
            }
        }
    }
    */
    #endregion

}
