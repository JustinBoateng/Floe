using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameplayManager : MonoBehaviour
{
    [SerializeField] int numPlayers;
    [SerializeField] GameObject[] PlayerPrefabs;
    [SerializeField] int currPF = 0;
    [SerializeField] PlayerInputManager PIU;
    [SerializeField] PlayerInput[] PlayerInputs;
    [SerializeField] CameraController Camera;
    [SerializeField] Transform CameraFocus;
    [SerializeField] Transform MainCharacter;
    // Start is called before the first frame update
    void Start()
    {
        if(numPlayers <= 1)
        {
            Debug.Log("Multiplayer Mode");
            Camera.setTarget(MainCharacter);
        }
        else
        {
            Debug.Log("Multiplayer Mode");
            Camera.setTarget(CameraFocus);
        }

        PIU.playerPrefab = PlayerPrefabs[currPF];

        //if (PIU.playerJoinedEvent)

        if (numPlayers > 1)
        {
            //var p1 = PlayerInput.Instantiate(PlayerPrefabs, controlScheme:"Gamepad 1", device: Gamepad.current);
            //var p2 = PlayerInput.Instantiate(PlayerPrefabs, controlScheme:"Gamepad 1", device: Gamepad.current);
        }
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
}
