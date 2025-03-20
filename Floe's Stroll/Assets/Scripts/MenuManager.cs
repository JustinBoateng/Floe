using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
public class MenuManager : MonoBehaviour
{
    [SerializeField] public static MenuManager MM;

    [SerializeField] PlayerInput PIRef;

    [SerializeField] EventSystem ESRef;
    [SerializeField] Button ArcadeButton;

    [SerializeField] GameObject TitleScreenRef;
    [SerializeField] GameObject MainMenuRef;
    [SerializeField] GameObject OilAndWaterRef;
    [SerializeField] GameObject StageSelectRef;
    [SerializeField] GameObject SettingsRef;
    [SerializeField] GameObject QuitGameRef;
    // Start is called before the first frame update

    [SerializeField] Vector2[] WipeAnchors;
    //0: center, 1: left, 2: right, 3: up, 4: down
    [SerializeField] Vector2[] WipeEndpoints;
    //0: Center, 1: From, 2: To

    [SerializeField] float[] WipeLERPValues;
    //0; Curr, 1: Rate


    [SerializeField] GameObject CurrentWipe;
    [SerializeField] Sprite[] WipeSprites;
    // Start is called before the first frame update

    [SerializeField] public string TransitionFlag = "Title";
    [SerializeField] string NextScene = "";

    private void Awake()
    {
        if (MM == null)
        {
            DontDestroyOnLoad(this.gameObject);
            //DontDestroyOnLoad(FadeScreen);
            MM = this;
            //MenuManager.MM.FindCanvas();

        }

        else if (MM != this)
            Destroy(this.gameObject);
    }
    void Start()
    {
        Awake();

        //Assuming that we are in the Title Screen
        TitleScreenRef.SetActive(true);
        MainMenuRef.SetActive(false);
        OilAndWaterRef.SetActive(false);
        StageSelectRef.SetActive(false);
        SettingsRef.SetActive(false);
        QuitGameRef.SetActive(false);

        //PIRef = this.GetComponent<PlayerInput>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void MainMenuOpen()
    {
        Debug.Log("Button Pressed");
        if (TransitionFlag == "Title")
        {
            TransitionFlag = "Main";
            MenuSwitch(TransitionFlag);
            ESRef.SetSelectedGameObject(ArcadeButton.gameObject);

            //PIRef.SwitchCurrentActionMap("Player");

            Debug.Log("In Main Menu");
        }
    }

    public void MenuSwitch(string s)
    {
        TransitionFlag = s;
        switch (TransitionFlag)
        {
            case "Title":
                TitleScreenRef.SetActive(true);
                MainMenuRef.SetActive(false);
                OilAndWaterRef.SetActive(false); 
                StageSelectRef.SetActive(false);
                SettingsRef.SetActive(false);
                QuitGameRef.SetActive(false);
                break;

            case "Main":
                TitleScreenRef.SetActive(false);
                MainMenuRef.SetActive(true);
                OilAndWaterRef.SetActive(false);
                StageSelectRef.SetActive(false);
                SettingsRef.SetActive(false);
                QuitGameRef.SetActive(false);
                break;

            case "OilAndWater":
                TitleScreenRef.SetActive(false);
                MainMenuRef.SetActive(false);
                OilAndWaterRef.SetActive(true);
                StageSelectRef.SetActive(false);
                SettingsRef.SetActive(false);
                QuitGameRef.SetActive(false);
                break;

            case "Settings":
                TitleScreenRef.SetActive(false);
                MainMenuRef.SetActive(false);
                OilAndWaterRef.SetActive(false);
                StageSelectRef.SetActive(false);
                SettingsRef.SetActive(true);
                QuitGameRef.SetActive(false);
                break;  
            


            case "StageSelectOn":
                StageSelectRef.SetActive(true);
                break;

            case "StageSelectOff":
                StageSelectRef.SetActive(false);
                break;

            case "QuitGameOn":
                QuitGameRef.SetActive(true);
                break;

            case "QuitGameOff":
                QuitGameRef.SetActive(false);
                break;

        }
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quitting Game.");
    }

}
