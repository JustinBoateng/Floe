using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingTerrain : MonoBehaviour
{
    [SerializeField] int Type;
    //1 for moving back and forth
    //2 for moving in one direction and then dissapearing
    //3 for rotating
    //4 for being activated via a switch (moving only when activated)

    [SerializeField] Transform[] Endpoints = new Transform[2];
    [SerializeField] float LerpValue;
    [SerializeField] float[] PauseCountdown = new float[3];
    //0: Max, 1: Curr, 2:isPaused
    [SerializeField] int direction = 1; //1 or -1
    [SerializeField] float Speed = 1; //1 or -1
    // Start is called before the first frame update

    [SerializeField] Transform Platform;

    [SerializeField] bool canMove;

    void Start()
    {
        PauseCountdown[1] = PauseCountdown[0];
        LerpValue = 0;
        Platform.transform.position = Vector2.Lerp(Endpoints[0].position, Endpoints[1].position, LerpValue);
    }

    // Update is called once per frame
    void Update()
    {
        switch (Type) {
            case 1:
                if (PauseCountdown[2] == 0)
                {
                    LerpValue = Mathf.Clamp(LerpValue += (Time.deltaTime * Speed * direction), 0, 1);

                    if (LerpValue <= 0 || LerpValue >= 1)
                    {
                        direction *= -1;
                        PauseCountdown[2] = 1;
                    }

                    Platform.transform.position = Vector2.Lerp(Endpoints[0].position, Endpoints[1].position, LerpValue);
                }
                if (PauseCountdown[2] == 1) {
            Halt();
        }
                break;

            case 4:
                if (canMove)
                {
                    LerpValue = Mathf.Clamp(LerpValue += (Time.deltaTime * Speed * direction), 0, 1);
                    Platform.transform.position = Vector2.Lerp(Endpoints[0].position, Endpoints[1].position, LerpValue);
                    if(LerpValue == 1 || LerpValue == 0)
                    {
                        TerrainDeActivate();
                    }
                }
                break;
        }
    }

    private void Halt()
    {
        PauseCountdown[1] -= Time.deltaTime;
        if (PauseCountdown[1] <= 0)
        {
            PauseCountdown[1] = PauseCountdown[0];
            PauseCountdown[2] = 0;
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        //if (collision.GetComponent<Being>())
        if (collision.tag == "Player")
        {
            //Debug.Log("Entered Platform");
            collision.transform.SetParent(this.transform);
        }
    }

    public void OnTriggerStay2D(Collider2D collision)
    {
        
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        //if (collision.GetComponent<Being>())
        if (collision.tag == "Player")
        {
            //Debug.Log("Entered Platform");
            collision.transform.SetParent(null);
        }
    }
    //even if the platform already has colliders, as long as one of them is a trigger, the above two functions should work.
    //just make sure this script is attached to an object with that very same collider

    public void TerrainActivate(int i)
    {
        canMove = true;

        direction = i;
    }

    public void TerrainDeActivate()
    {
        canMove = false;
    }
}
