using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
public class OWMenuManager : MonoBehaviour
{
    [SerializeField] PlayerInputManager PIM;
    [SerializeField] PlayerControllerRep PCRPrefab;

    [SerializeField] int PlayerIndex;

    [SerializeField] Image[] ActiveControllers;

    // Start is called before the first frame update
    void Start()
    {
        PIM.playerPrefab = PCRPrefab.gameObject;


        //Debug.Log(Input.GetJoystickNames());
        string[] joystickNames = Input.GetJoystickNames();
        foreach (string joystickName in joystickNames)
        {
            Debug.Log(joystickName);
        }
        PlayerIndex = 0;
    }



    // Update is called once per frame
    void Update()
    {
    }

    public void SpawnNewConRep()
    {
        //Debug.Log(PlayerIndex);
        Debug.Log(InputSystem.devices[PlayerIndex]);

        if (MenuManager.MM && MenuManager.MM.TransitionFlag == "OilAndWater")
        {
            ActiveControllers[PlayerIndex].color = Color.red;
            PIM.JoinPlayer(PlayerIndex, PlayerIndex, "Player", InputSystem.devices[PlayerIndex]);
            PlayerIndex++;
        }
    }
}
