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
    [SerializeField] CameraController CameraCont;
    [SerializeField] Transform CameraFocus;
    [SerializeField] Transform MainCharacter;

    [SerializeField] float ViewSize;
    [SerializeField] float Time;
    [SerializeField] int[] Score;
    [SerializeField] PlayerControl[] Players;

    [SerializeField] bool GameOn = false;
    [SerializeField] float[] Timer = new float [4];
    [SerializeField] int Winner = 0;
    [SerializeField] Health[] PlayerHealth; 
    [SerializeField] HealthBar[] PlayerHealthBars;
    
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
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(Input.GetJoystickNames());
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
}
