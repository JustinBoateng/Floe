using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : AI
{
    // Start is called before the first frame update

    [SerializeField] private string CurrState = "Idle";
    //States:
    //Idle, Patrol, Atk, Hurt
    [SerializeField] private Sight LineOfSight;
    [SerializeField] private Sight FarSight;



    void Start()
    {
        CurrState = "Idle";
    }

    // Update is called once per frame
    void Update()
    {
        if (LineOfSight.LockOnStatus())
        {
            CurrState = "Chase";
        }

        else if (!FarSight.LockOnStatus())
        {
            CurrState = "Idle";
        }
    }

    public void ChangeState(string s)
    {
        CurrState = s;
    }
}
