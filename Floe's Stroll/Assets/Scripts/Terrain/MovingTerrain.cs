using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingTerrain : MonoBehaviour
{
    [SerializeField] int Type;
    //1 for moving back and forth
    //2 for moving in one direction and then dissapearing
    //3 for rotating

    [SerializeField] Transform[] Endpoints = new Transform[2];
    [SerializeField] float LerpValue;
    [SerializeField] float[] PauseCountdown = new float[3];
    //0: Max, 1: Curr, 2:isPaused
    [SerializeField] int direction = 1; //1 or -1
    [SerializeField] float Speed = 1; //1 or -1
    // Start is called before the first frame update
    void Start()
    {
        PauseCountdown[1] = PauseCountdown[0];
    }

    // Update is called once per frame
    void Update()
    {
        if (PauseCountdown[2] == 0)
        {
            LerpValue = Mathf.Clamp(LerpValue += (Time.deltaTime * Speed * direction), 0, 1);

            if (LerpValue <= 0 || LerpValue >= 1)
            {
                direction *= -1;
                PauseCountdown[2] = 1;
            }

            this.transform.position = Vector2.Lerp(Endpoints[0].position, Endpoints[1].position, LerpValue);
        }
        if (PauseCountdown[2] == 1) {
            Halt();
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
}
