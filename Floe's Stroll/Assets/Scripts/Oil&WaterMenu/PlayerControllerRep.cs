using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public class PlayerControllerRep : MonoBehaviour
{

    [SerializeField] int PlayerNumber;

    [SerializeField] PlayerInput PIRef;

    [SerializeField] Transform[] TeamPositions;
    //0: Team Water,  1: Undecided, 2: Team Oil

    [SerializeField] int TeamNumber;
    //0: Team Water,  1: Undecided, 2: Team Oil

    [SerializeField] GameObject ControllerSprite;

    [SerializeField] float Deadzone;
    [SerializeField] float hor;
    [SerializeField] bool canMove;


    public int GetPlayerNumber()
    {
        return PlayerNumber;
    }

    // Start is called before the first frame update
    void Start()
    {
        canMove = true;
        TeamNumber = 1;
        ShowHideSprite(false);
        PIRef = GetComponent <PlayerInput> ();
    }

    // Update is called once per frame
    void Update()
    {
        if(ControllerSprite.activeInHierarchy)
            ControllerSprite.transform.position = TeamPositions[TeamNumber].transform.position;
        //Debug.Log(PIRef.currentActionMap);

    }

    public void ShowHideSprite(bool b)
    {
        ControllerSprite.SetActive(b);
    }

    public void Move(Vector2 Direction)
    {
        //hor = Direction.x;

        if (MenuManager.MM && MenuManager.MM.TransitionFlag == "OilAndWater")
        {
            if (Direction.x < -Deadzone) hor = -1;
            else if (Direction.x > Deadzone) hor = 1;
            else hor = 0;

            if (canMove)
            {
                if (hor == -1)
                {
                    Debug.Log("in canMove Check -1");

                    TeamNumber = Mathf.Clamp(TeamNumber - 1, 0, 2);
                    canMove = false;
                }

                if (hor == 1)
                {
                    Debug.Log("in canMove Check +1");

                    TeamNumber = Mathf.Clamp(TeamNumber + 1, 0, 2);
                    canMove = false;
                }
            }

            if (hor == 0)
            {
                canMove = true;
            }
        }
    }



    public void OnMove(InputAction.CallbackContext context)
    {
        Debug.Log("Moving Here too...");
        if (MenuManager.MM && MenuManager.MM.TransitionFlag == "OilAndWater")
        {
            float inputx = context.ReadValue<Vector2>().x;
            if (inputx < -Deadzone) hor = -1;
            else if (inputx > Deadzone) hor = 1;
            else hor = 0;

            if (canMove)
            {
                if (hor == -1)
                {
                    Debug.Log("in canMove Check -1");

                    TeamNumber = Mathf.Clamp(TeamNumber - 1, 0, 2);
                    canMove = false;
                }

                if (hor == 1)
                {
                    Debug.Log("in canMove Check +1");

                    TeamNumber = Mathf.Clamp(TeamNumber + 1, 0, 2);
                    canMove = false;
                }
            }

            if (hor == 0)
            {
                canMove = true;
            }
        }
    }

    public void OnSubmit(InputAction.CallbackContext context)
    {
        if (MenuManager.MM && MenuManager.MM.TransitionFlag == "OilAndWater")
        {

            if (context.performed)
            {
                ControllerSprite.SetActive(true);

            }
        }

        if(MenuManager.MM && MenuManager.MM.TransitionFlag == "Title")
        {
            MenuManager.MM.MainMenuOpen();
        }
    }
}
