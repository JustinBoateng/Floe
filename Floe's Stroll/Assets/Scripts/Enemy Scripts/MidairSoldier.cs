using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditorInternal.Profiling.Memory.Experimental.FileFormat;
using UnityEngine;
using UnityEngine.WSA;
using static UnityEngine.GraphicsBuffer;

public class MidairSoldier : EnemyAI
{
    //[SerializeField] string EnemyType;
    [SerializeField] BulletClass[] BulletAmmo;
    [SerializeField] float[] activeTimer = new float[2];
    [SerializeField] Vector2 TargetPoint;

    [SerializeField] Vector2 ThrowArc;

    [SerializeField] bool Launched = false;
    // Start is called before the first frame update
    void Start()
    {
        BeingStart();

        this.tag = "Enemy";
        CurrState = "Idle";
        HitstunArmor[1] = HitstunArmor[0];

        rb.gravityScale = 0.0f;

        for (int i = 0; i < BulletAmmo.Length; i++)
        {
            BulletAmmo[i].setSignature(this);
            BulletAmmo[i].setBasePosition(transform);
            BulletAmmo[i].gameObject.SetActive(false);
            BulletAmmo[i].tag = "Bullet";
        }
    }

    // Update is called once per frame
    void Update()
    {
        Monitor();
        Hitstun = Mathf.Clamp(Hitstun -= Time.deltaTime, 0, Hitstun);
        StateStatus();

        if (Launched) LaunchedCounter();
    }

    public void StateStatus()
    {
        switch (CurrState)
        {
            case "Hurt":
                if (Hitstun <= 0)
                {
                    CurrState = "Chase";
                    resetArmor();
                }
                //WeaponStatus(false);
                break;

            case "Idle":
                //rb.velocity = new Vector2(0, rb.velocity.y);
                rb.velocity = new Vector2(0, 0);
                
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
                //change where the AI is facing
                Track(Target.transform.position);

                facingCalc(1);

                //Have this character move to the target.
                //Set Speed[1] == 0  if the character is stationary

                rb.velocity = new Vector2(Speeds[1] * facing[0], Speeds[1] * facing[1] * Mathf.Abs( Target.transform.position.y - transform.position.y ));


                //if the Atk_Countdown is finished and the player is in the AtkRange of the enemy, 
                if (Atk_Countdown[1] <= 0 && AtkRange.LockOnStatus())
                {
                    TargetPoint = Target.transform.position;
                    Track(TargetPoint);
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
                Track(TargetPoint);

                switch (EnemyType)
                {

                    case "AType":

                        //Head Towards where the enemy is at that point
                        rb.velocity = new Vector2(Speeds[2] * facing[0], Speeds[2] * facing[1] * Mathf.Abs(Target.transform.position.y - transform.position.y));

                        //Turn on the damage shield

                        break;


                    case "BType":
                        rb.velocity = Vector2.zero;

                        if (!Launched) { 
                            Launched = true;
                            WeaponStatus(true);
                        }
                        break;
                }

                //LaunchedCounter();
                //AtkCounter determines when to call back the weapons.
                //You want this seperate from when the enemy gets hurt.
                break;
        }
    }

    /*
    protected void AttackSwitch(bool b)
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
                //WeaponStick.gameObject.SetActive(false);
                break;

        }

    }
    */
    public void WeaponStatus(bool b)
    {
        //WeaponStick.gameObject.SetActive(b);

        if (!b)
        {
            //If an ammo piece hits a player, the ground, or if [1] hits zero, have the bullet go inactive instead of being destroyed.
            //Make sure to have a function called ClashReturn instead of Clash going on.

            for (int i = 0; i < BulletAmmo.Length; i++)
            {
                //Debug.Log("WeaponStatus Crasher");
                BulletAmmo[i].Crash();
                //If they're return, they'll come back to the player

                Launched = false;
            }
        }

        else
        {

            int m = 1;
            int n = -1;
            for (int i = 0; i < BulletAmmo.Length; i++)
            {
                Debug.Log("Adjusting Velocity of Bullets");
                //BulletAmmo[i].GetComponent<Rigidbody2D>().AddForce(new Vector2(ThrowArc.x * j, ThrowArc.y));
                //BulletAmmo[i].GetComponent<Rigidbody2D>().velocity = new Vector2(ThrowArc.x * j, ThrowArc.y);
                BulletAmmo[i].isCrashed = false;
                BulletAmmo[i].setDirection(ThrowArc.x * m, ThrowArc.y * n);
                BulletAmmo[i].gameObject.SetActive(true);
                
                //alternate between m and n to get the four directions desired
                if(m == n)
                    m *= -1;
                else
                    n *= -1;
            }
        }
    }

    public void LaunchedCounter()
    {
        //countdown on Atk_Countdown

        Launch_Countdown[1] -= Time.deltaTime;

        //Switch Off Atk Mode when you're done
        if (Launch_Countdown[1] <= 0)
        {
            AttackSwitch(false);
            WeaponStatus(false);
        }
    }
    //has access to

    /*
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
    [SerializeField] float Hitstun;
    [SerializeField] float HitstunMultiplier;
    [SerializeField] int[] HitstunArmor;
     */


    //getArmor
    //ResetArmor
    //WeaponStatus
    //SetHitstun
    //AttackSwitch
    //Monitor
    //StateStatus
    //ChangeState

    /*
     Enemy Type B

Bullet[] Ammo // GameObjects, have them cluster at the enemy position.
//then, have one ammo shoot off in an arc.

float[] activeTimer;
//have [1] start at [0], then decrement via [1] -= Time.deltatime using mathf.clamp.
//if [1] hits zero, then get all the Ammo pieces, and make them unactive. Then set their positions back to the Enemy Position

//If an ammo piece hits a player, the ground, or if [1] hits zero, have the bullet go inactive instead of being destroyed.
//Make sure to have a function called ClashReturn instead of Clash going on.

     */
}
