using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : AI
{
    // Start is called before the first frame update

    //States:
    //Idle, Patrol, Atk, Hurt
    [SerializeField] private Sight LineOfSight;
    [SerializeField] private Sight FarSight;
    [SerializeField] private Sight AtkRange;

    [SerializeField] private string CurrState = "Idle";
    [SerializeField] float stateTimer;
    [SerializeField] float[] Speeds;
    [SerializeField] float[] Atk_Countdown;
    [SerializeField] string EnemyType;
    [SerializeField] GameObject Target;
    [SerializeField] GameObject WeaponStick;




    void Start()
    {
        CurrState = "Idle";
        WeaponStick.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        Monitor();

        switch (CurrState)
        {
            case "Idle":
                rb.velocity = new Vector2(0, rb.velocity.y);
                stateTimer -= Time.deltaTime; 
                if(stateTimer <= 0)
                {
                    facingCalc(-1);
                    CurrState = "Patrol";
                    stateTimer = Random.Range(3.0f, 6.0f);
                }

                break;
            case "Patrol":
                rb.velocity = new Vector2(Speeds[0] * facing[0], rb.velocity.y);
                stateTimer -= Time.deltaTime;
                if(stateTimer <= 0)
                {
                    CurrState = "Idle";
                    stateTimer = Random.Range(2f, 6f);
;               }
                break;
            case "Chase":
                if (Target.transform.position.x < transform.position.x)
                {
                    facing[0] = -1;
                    facingCalc(1);
                    //we want to turn around once. So manually flip facing[0] once, then flip the character themsleves
                }
                else
                {
                    facing[0] = 1;
                    facingCalc(1);
                }

                rb.velocity = new Vector2(Speeds[1] * facing[0], rb.velocity.y);

                if (Atk_Countdown[1] <= 0 && AtkRange.LockOnStatus())
                {
                    AttackSwitch(true);
                }

                else if (AtkRange.LockOnStatus())
                {
                    Atk_Countdown[1] -= Time.deltaTime;
                }

                else if (!AtkRange.LockOnStatus() && FarSight.LockOnStatus())
                    Atk_Countdown[1] -= Time.deltaTime;

                else if (!FarSight.LockOnStatus())
                {
                    Atk_Countdown[1] = Atk_Countdown[0];
                    CurrState = "Idle";
                }
                break;

            case "Attack":
                switch (EnemyType)
                {
                    case "FootSoldier":
                        WeaponStick.gameObject.SetActive(true);
                        rb.velocity = new Vector2(Speeds[2] * facing[0], rb.velocity.y);
                        Atk_Countdown[1] -= Time.deltaTime;
                        if (Atk_Countdown[1] <= 0)
                        {
                            AttackSwitch(false);
                        } 
                        break;
                }
                break;
        }
    }

    public void ChangeState(string s)
    {
        CurrState = s;
    }

    private void Monitor()
    {
        if (LineOfSight.LockOnStatus() && CurrState != "Attack")
        {
            CurrState = "Chase";
            Target = LineOfSight.CurrentTarget();
        }

        else if (!FarSight.LockOnStatus() && CurrState != "Patrol")
        {
            CurrState = "Idle";
            Target = null;
        }
    }

    private void AttackSwitch(bool b)
    {
        switch (b)
        {
            case true:
                CurrState = "Attack";
                Atk_Countdown[1] = Atk_Countdown[0];
                break;

            case false:
                Atk_Countdown[1] = Atk_Countdown[0];
                CurrState = "Idle";
                WeaponStick.gameObject.SetActive(false);
                break;
                    
        }

    }
}
