using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
//using UnityEditor.SearchService;

//using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitionManager : MonoBehaviour
{
    [SerializeField] public static TransitionManager TM;


    [SerializeField] Vector2[] WipeAnchors;
    //0: center, 1: left, 2: right, 3: up, 4: down
    [SerializeField] Vector2[] WipeEndpoints;
    //0: Center, 1: From, 2: To

    [SerializeField] float[] WipeLERPValues;
    //0; Curr, 1: Rate

    [SerializeField] GameObject CurrentWipe;
    [SerializeField] Sprite[] WipeSprites;
    //Wipes are what to have on the screen as you transition from one scene to the next
    [SerializeField] string TransitionFlag = "";
    [SerializeField] string NextScene = "";
    

    [SerializeField] int FileNumber;
    [SerializeField] int NoofLives;
    [SerializeField] int TimeMins;
    [SerializeField] int TimeSecs;
    [SerializeField] string CurrStage;
    [SerializeField] int currSceneindex;
    [SerializeField] Scene currScene;
    [SerializeField] int CollectedFountains;

    //called first
    private void Awake()
    {
        if (TM == null)
        {
            DontDestroyOnLoad(this.gameObject);
            //DontDestroyOnLoad(FadeScreen);
            TM = this;
        }

        else if (TM != this)
            Destroy(this.gameObject);

        
    }

    //called second
    private void OnEnable()
    {
        Debug.Log("OnEnable called");

        //SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }


    //called third
    //OnSceneLoaded will be delegated to the SceneManager.sceneLoaded function. So they need to have the same parameters
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("OnSceneLoaded" + scene.name);

        if (SceneManager.GetActiveScene().name == "Cinema")
        {
            GameObject.Find("Theatre").GetComponent<CinemaManager>().SetFilm(currSceneindex);
        }
        
        if (SceneManager.GetActiveScene().name == "SampleScene" || SceneManager.GetActiveScene().name == "GameScene")
        {
            GameplayManager.GM.TurnOn();
        }
    }


    // called fourth
    void Start()
    {
        
    }

    //called when the game is terminated
    private void OnDisable()
    {
        Debug.Log("OnDisable");
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    // Update is called once per frame
    void Update()
    {
        //When transitioning, 
        //FadeIn();
        //have the wipe go from one side to the center
        //FadeOut();
        //have the wipe go from center to the other side.
      /*  if (TransitionFlag != "")
        {

            WipeLERPValues[0] += WipeLERPValues[1];
            CurrentWipe.transform.position = Vector2.Lerp(WipeEndpoints[1], WipeEndpoints[2], WipeLERPValues[0]);
            if (WipeLERPValues[1] >= 1)
            {
                switch (TransitionFlag)
                {
                    case "Enter":
                        ExitWipe();
                        SceneManager.LoadScene(NextScene);
                        break;
                    case "Exit":
                        TransitionFlag = "";    
                        break;
                    default:
                        break;
                }

            }
        }*/
    }

   

    public void MoveScene(string Stage)
    {
        CurrStage = Stage;

        if (Stage == "NewFile")
        {
            currSceneindex = 0;
            SceneManager.LoadScene("Cinema");
        }

        else
        {
            switch (Stage)
            {
                case "SampleStage1":
                    if(GameplayManager.GM)
                        GameplayManager.GM.SetStage(1);
                    SceneManager.LoadScene("SampleScene");
                    break;

                case "SampleStage2":
                    if (GameplayManager.GM)
                        GameplayManager.GM.SetStage(2);
                    SceneManager.LoadScene("SampleScene");
                    break;

                case "SampleStage3":
                    if (GameplayManager.GM)
                        GameplayManager.GM.SetStage(3);
                    SceneManager.LoadScene("SampleScene");
                    break;

                default:
                    SceneManager.LoadScene("SampleScene");
                    break;
            }
        }
        //In the Cinema Scene, retrieve the currStage. If it says "NewFile" play the intro cutscene

        //when the scene is loaded, if the scene is "Cinema", 
    }



    
    public void setStage(string s)
    {
        CurrStage = s;
    }
    
    public string getStage()
    {
        return CurrStage;
    }
    
    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quitting Game.");
    }

}

