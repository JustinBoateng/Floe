using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class EnemyAI : AI
{
    // Start is called before the first frame update

    //States:
    //Idle, Patrol, Atk, Hurt
    [SerializeField] protected Sight LineOfSight;
    [SerializeField] protected Sight FarSight;
    [SerializeField] protected Sight AtkRange;

    [SerializeField] protected string CurrState = "Idle";
    [SerializeField] protected float stateTimer;
    [SerializeField] protected float[] Speeds;
    [SerializeField] protected float[] Atk_Countdown;
    //o: Max Cooldown, Curr Cooldown, Cooldown Rate 

    [SerializeField] protected float[] Launch_Countdown;
    //o: Max Cooldown, Curr Cooldown, Cooldown Rate 

    [SerializeField] protected string EnemyType;
    [SerializeField] protected GameObject Target;
    //[SerializeField] protected GameObject WeaponStick;
    [SerializeField] protected float Hitstun;
    [SerializeField] protected float HitstunMultiplier;
    [SerializeField] protected int[] HitstunArmor;
    //0:Max Armor, 1: Curr Armor
    void Start()
    {
        BeingStart();

        this.tag = "Enemy";
        CurrState = "Idle";
        HitstunArmor[1] = HitstunArmor[0];

    }

    // Update is called once per frame
    void Update()
    {
        Monitor();
        Hitstun = Mathf.Clamp(Hitstun-= Time.deltaTime, 0, Hitstun);
        //StateStatus();        
    }

    /*
     * public void StateStatus()
    {
        switch (CurrState)
        {
            case "Hurt":
                if (isGrounded() && Hitstun <= 0)
                {
                    CurrState = "Chase";
                    resetArmor();
                }
                WeaponStatus(false);
                break;

            case "Idle":
                rb.velocity = new Vector2(0, rb.velocity.y);
                stateTimer -= Time.deltaTime;
                if (stateTimer <= 0)
                {
                    facingCalc(-1);
                    CurrState = "Patrol";
                    stateTimer = Random.Range(3.0f, 6.0f);
                }

                break;
            case "Patrol":
                rb.velocity = new Vector2(Speeds[0] * facing[0], rb.velocity.y);
                stateTimer -= Time.deltaTime;
                if (stateTimer <= 0)
                {
                    CurrState = "Idle";
                    stateTimer = Random.Range(2f, 6f);
                    ;
                }
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
    */
    public void ChangeState(string s)
    {
        CurrState = s;
    }

    protected void Monitor()
    {
        if (CurrState != "Hurt")
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
    }

    
    protected void AttackSwitch(bool b)
    {
        Atk_Countdown[1] = Atk_Countdown[0];
        if(Launch_Countdown.Length > 0)
            Launch_Countdown[1] = Launch_Countdown[0];
    
        switch (b)
        {
            case true:
                CurrState = "Attack";
                break;

            case false:
                CurrState = "Idle";
                //WeaponStick.gameObject.SetActive(false);
                break;
                    
        }

    }
    

    public void SetHitstun(float x, GameObject T)
    {
        Target = T;
        CurrState = "Chase";
        //Now the AI knows who shot them

        //Suprise Attack
        if (CurrState == "Idle" && CurrState == "Patrol")
        {
            HitstunArmor[1] = 0;
        }
        //Regular Armor Deterioration
        else
        {
            HitstunArmor[1]--;
            if (x >= 5) HitstunArmor[1]--;
            if (x >= 10) HitstunArmor[1]--;
        }


        if (HitstunArmor[1] <= 0)
        {
            CurrState = "Hurt";
            Hitstun = x * HitstunMultiplier;
        }
    }

    public int getArmor()
    {
        return HitstunArmor[1];
    }

    public void resetArmor()
    {
        HitstunArmor[1] = HitstunArmor[0];
    }

    public void Track(Vector2 Tar)
    {
        if (Tar.x < transform.position.x) facing[0] = -1;
        else facing[0] = 1;

        if (Tar.y < transform.position.y) facing[1] = -1;
        else facing[1] = 1;

        facingCalc(1);
    }

    /*
     * public void WeaponStatus(bool b)
    {
        WeaponStick.gameObject.SetActive(b);
    }
    */
}
