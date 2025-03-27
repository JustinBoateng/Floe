using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class GameplayManager : MonoBehaviour
{
    [SerializeField] public static GameplayManager GM;

    [SerializeField] int numPlayers;
    [SerializeField] GameObject[] PlayerPrefabs;
    [SerializeField] int currPF = 0;
    [SerializeField] PlayerInputManager PIU;
    [SerializeField] PlayerInput[] PlayerInputs;
    [SerializeField] public CameraController CameraCont;
    [SerializeField] Transform CameraFocus;
    [SerializeField] Transform MainCharacter;

    [SerializeField] float ViewSize;
    [SerializeField] float Time;
    [SerializeField] int[] Score;
    [SerializeField] PlayerControl[] Players;

    [SerializeField] bool GameOn = false;
    [SerializeField] int Winner = 0;
    [SerializeField] Health[] PlayerHealth; 
    [SerializeField] HealthBar[] PlayerHealthBars;

    [SerializeField] float[] Timer = new float[4];
    [SerializeField] Clock ClockRef;
    //[Max Minutes, Minutes, Seconds, Milliseconds]

    [SerializeField] Checkpoints[] Checkpoints;
    [SerializeField] int FurthestCheckpoint;
    [SerializeField] GameObject CurrentMainPlayer;


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

        if(numPlayers <= 1)
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

        PlayerHealth[0] = Players[0].GetComponent<Health>();
        PlayerHealth[1] = Players[1].GetComponent<Health>();
        PlayerHealthBars[0].setHealth(PlayerHealth[0]);
        PlayerHealthBars[1].setHealth(PlayerHealth[1]);

        Timer[1] = Timer[0];

        PlacePlayer();
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(Input.GetJoystickNames());
        TimeCalc();
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

    private void TimeCalc()
    {
        //if (Timer[3] <= 0)
        if ((int)(Timer[3]) > (int)(Timer[3] - UnityEngine.Time.deltaTime) || Timer[3] < 0)
        {
            if (Timer[2] <= 0)
            {
                if (Timer[1] <= 0)
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
                Timer[3] = 99;
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


    public void setCheckpoint(int n)
    {
        FurthestCheckpoint = Mathf.Max(FurthestCheckpoint, n);

    }

    public void PlacePlayer()
    {
        CurrentMainPlayer.transform.position = Checkpoints[FurthestCheckpoint].transform.position;
        CurrentMainPlayer.GetComponent<Health>().HealthRefill();
    }

}
