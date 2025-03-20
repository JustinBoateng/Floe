using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

public class PlayerInputHandler : MonoBehaviour
{

    [SerializeField] PlayerInput PIRef;
    [SerializeField] private PlayerControl PCRef;
    [SerializeField] private PlayerControllerRep PCllRep;

    [SerializeField] int index;
    

    private void Awake()
    {
        PIRef = GetComponent<PlayerInput>();
        index = PIRef.playerIndex;

        var players = FindObjectsOfType<PlayerControl>();
        if(players.Length != 0)
        PCRef = players.FirstOrDefault(p => p.GetPlayerNumber() == index);
    
        var TeamSelCursors = FindObjectsOfType<PlayerControllerRep>();
        if(TeamSelCursors.Length != 0)
        PCllRep = TeamSelCursors.FirstOrDefault(t => t.GetPlayerNumber() == index);

        PIRef.SwitchCurrentActionMap("Player");
        //FindObjectsOfType gives us a list of all objects in the scene that is of the type in the <>
        //FirstOrDefault here will return the first element in the list that matches the following condition:
        //(let p = the current element in the list of playercontrol objects)
        //return p if p.GetPlayerNumber = index
        //index being retrieved from the PIRef, and set by the PIM when this object is spawned in.


        //Same Logic for PClllRep


    }


    public void OnMove(InputAction.CallbackContext context)
    {
        Debug.Log("Moving Player: " + index);
        PCllRep.OnMove(context);
    }
    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
