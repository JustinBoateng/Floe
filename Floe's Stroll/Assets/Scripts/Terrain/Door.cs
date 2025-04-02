using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] private Transform PrevRoom;
    [SerializeField] private Transform NextRoom;

    [SerializeField] private CameraController CC;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            //if the player is coming from the left, go to the next room
            if(collision.transform.position.x < transform.position.x) 
            {
                CC.MovetoNewRoom(NextRoom);
            }
            //if player is coming from the right, go to the previous room
            else
            {
                CC.MovetoNewRoom(PrevRoom);
            }
        }
    }

}
