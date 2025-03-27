using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    //Room Camera Movement
    [SerializeField] private float Speed;
    private float currentPosX;
    private Vector3 velocity = Vector3.zero;


    //Follow Player
    [SerializeField] private Transform Player;
    [SerializeField] private float aheadDistance;
    [SerializeField] private float CameraSpeed;
    private float LookAhead;

    [SerializeField] private float xMax;
    [SerializeField] private float xMin;
    [SerializeField] private float yMax;
    [SerializeField] private float yMin;

    [SerializeField] private Transform[] BoundaryPoints;


    private void Start()
    {
        BoundarySet();   
    }

    void Update()
    {
        //transform.position = Vector3.SmoothDamp(transform.position, new Vector3(currentPosX, transform.position.y, transform.position.z), ref velocity, Speed * Time.deltaTime);
        //Speed * Time.deltaTime makes it so that the speed of the movement would not be affected by frame rate. But apparently, the below code works better
        
        //Room Camera Movement
        //transform.position = Vector3.SmoothDamp(transform.position, new Vector3(currentPosX, transform.position.y, transform.position.z), ref velocity, Speed);// * Time.deltaTime);    
        //ref is the C++ equivalent to calling by reference

        if(Player)
            transform.position = new Vector3(Player.position.x + LookAhead, Player.position.y, transform.position.z);
        BoundaryCheck();
        if(Player)
            LookAhead = Mathf.Lerp(LookAhead, (aheadDistance * Player.localScale.x), Time.deltaTime * CameraSpeed);
        //when player.localScale.x is 1, you're facing right, and will look to the right. When localScale.x is -1, you'll face left, and will look to the left

    }

    public void MovetoNewRoom(Transform _newRoom)
    {
        currentPosX = _newRoom.position.x;
    }
    private void BoundarySet()
    {
        xMax = BoundaryPoints[0].position.x;
        xMin = BoundaryPoints[1].position.x;
        yMax = BoundaryPoints[2].position.y;
        yMin = BoundaryPoints[3].position.y;
    }

    public void BoundaryCheck()
    {
        if (transform.position.x < xMin) transform.position = new Vector3(xMin, transform.position.y, transform.position.z);
        if (transform.position.x > xMax) transform.position = new Vector3(xMax, transform.position.y, transform.position.z);
        if (transform.position.y < yMin) transform.position = new Vector3(transform.position.x, yMin, transform.position.z);
        if (transform.position.y > yMax) transform.position = new Vector3(transform.position.x, yMax, transform.position.z);

    }

    public void setTarget(Transform g)
    {
        Player = g;
    }

    public void setAhead(float a)
    {
        aheadDistance = a;
    }

    public void disengageTarget()
    {
        Player = null;
    }
}
