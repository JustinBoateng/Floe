using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WipeManager : MonoBehaviour
{

    [SerializeField] public static WipeManager WM;

    [SerializeField] Vector2[] WipeAnchors;
    //0: center, 1: left, 2: right, 3: up, 4: down
    [SerializeField] Vector2[] WipeEndpoints;
    //0: Center, 1: From, 2: To

    [SerializeField] float[] WipeLERPValues;
    //0; Curr, 1: Rate


    [SerializeField] GameObject CurrentWipe;
    [SerializeField] Sprite[] WipeSprites;
    // Start is called before the first frame update

    [SerializeField] string TransitionFlag = "";
    [SerializeField] string NextScene = "";

    private void Awake()
    {
        if (WM == null)
        {
            DontDestroyOnLoad(this.gameObject);
            //DontDestroyOnLoad(FadeScreen);
            WM = this;
           WipeManager.WM.FindCanvas();

        }

        else if (WM != this)
            Destroy(this.gameObject);
    }

    void Start()
    {
        FindCanvas();   
    }

    // Update is called once per frame
    void Update()
    {

        if (TransitionFlag != "")
        {

            WipeLERPValues[0] += WipeLERPValues[1];
            CurrentWipe.transform.position = Vector2.Lerp(WipeEndpoints[1], WipeEndpoints[2], WipeLERPValues[0]);
            if (WipeLERPValues[0] >= 1)
            {
                switch (TransitionFlag)
                {
                    case "Enter":
                        ExitWipe();
                        leaveCanvas();
                        TransitionManager.TM.MoveScene(NextScene);
                        //SceneManager.LoadScene(NextScene);
                        break;
                    case "Exit":
                        TransitionFlag = "";
                        break;
                    default:
                        break;
                }

            }
        }
    }

    public void EnterWipe(string SceneName)
    {
        WipeLERPValues[0] = 0;
        NextScene = SceneName;
        TransitionFlag = "Enter";
        Debug.Log("Changing Scene");

        //set current anchors from left to right
        //From
        WipeEndpoints[1] = WipeAnchors[1];
        //To
        WipeEndpoints[2] = WipeAnchors[0];

        //WipeEndpoints[0] = WipeAnchors[0];




    }
    public void ExitWipe()
    {
        WipeLERPValues[0] = 0;
        TransitionFlag = "Exit";

        //finish from left to right
        //From
        WipeEndpoints[1] = WipeAnchors[0];
        //To
        WipeEndpoints[2] = WipeAnchors[2];

    }

    public void FindCanvas()
    {
        this.gameObject.transform.SetParent(GameObject.Find("Canvas").transform, false);
    }

    public void leaveCanvas()
    {
        this.gameObject.transform.SetParent(null,true);
        DontDestroyOnLoad(this.gameObject);

    }

    public void OnEnable()
    {
        FindCanvas();
    }

}
