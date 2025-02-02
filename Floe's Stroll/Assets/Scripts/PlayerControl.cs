using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControl : MonoBehaviour
{

    float hor;
    float ver;

    [SerializeField] bool inputLock = false;

    Rigidbody2D rb;
    //[SerializeField] BoxCollider2D[] ColliderChecks = new BoxCollider2D[2];
    [SerializeField] BoxCollider2D ColliderCheck;// = new BoxCollider2D[2];
    private Animator anim;
    //[SerializeField] CapsuleCollider2D[] ColliderChecks = new CapsuleCollider2D[2];
    //0: BaseCollider

    [SerializeField] private LayerMask platformLayer;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask wallLayer;

    [SerializeField] private float DropdownTimer = 0.25f;

    [SerializeField] private Vector2 BaseColliderOffset = new Vector2(-0.025177f, -0.1591772f);
    [SerializeField] private Vector2 BaseColliderDimensions =  new Vector2(0.8906784f, 1.777763f);

    [SerializeField] private Vector2 CrouchColliderOffset = new Vector2(0f, -0.95f);
    [SerializeField] private Vector2 CrouchColliderDimensions =  new Vector2(1.629251f, 0.6012003f);


    [SerializeField] bool isCrouching = false;
    [SerializeField] bool isOnPlatform = false;
    [SerializeField] bool walking;
    [SerializeField] float JumpForce = 10;
    [SerializeField] float currSpeed;

    [SerializeField] private float[] LockCountDown = new float[2];

    [SerializeField] float[] Speeds;
    //0: Fall, 1: Walk, 2: Run

    [SerializeField] float[] WallJumpInfo;
    //0: Max, 1: Curr, 2: Amount to push away from Wall, 3: Amount to be pushed upwards, 4: WallSliding Speed, 5: Player inputting left or right does nothing for this long

    
    //[SerializeField] public int[] facing { get; private set; } = new int[2];
    [SerializeField] public int[] facing = new int[2];
    //0: horizontal facing, 1: vertical facing
    //instead of doing an entire get and set function, we can just do this.


    [SerializeField] float[] isShooting = new float[2]; //used for the shooting animation
    //0: Max, 1: curr
    [SerializeField] float[] isDashing = new float[6];
    //0: Max, 1: Curr,
    //2: isDashing,
    //3: SlideMax (have Curr start at SlideMax instead of Max),
    //4: isSliding
    //5: Slide rate
    //used for invul frames and the boolean for if the player is, in fact, dashing
    
    [SerializeField] float[] isHurt = new float[2]; //used for the Hurt animation
    [SerializeField] float[] isKO = new float[2]; //used for the KO animation
     
    [SerializeField] int[] bulletSpeed = new int[2];
    //0: horizontal Speed, 1: vertical Speed
    
    [SerializeField] float[] bulletDeterRate = { 2 };
    //determines how long bullets stay active for

    [SerializeField] GameObject ShootPosition;
    [SerializeField] BulletClass[] BulletReferences;
    
    [SerializeField] int currentBullet;
    // Start is called before the first frame update


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        //ColliderChecks[0] = GetComponent<BoxCollider2D>();
        ColliderCheck = GetComponent<BoxCollider2D>();
        anim = GetComponent<Animator>();

        //ColliderChecks[0] = GetComponent<CapsuleCollider2D>();
        facing[0] = 1;
        facing[1] = 0;
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(Input.GetAxisRaw("Horizontal"));



        hor = Input.GetAxisRaw("Horizontal");
        ver = Input.GetAxisRaw("Vertical");
        //ver = -1;
        
        //Debug.Log("Horizonal: " + hor);
        //Debug.Log("Vertical: " + ver);

        //horizontalInput = Input.GetAxisRaw("Horizontal");
        Movement();

        CooldownCalc();
        AnimCalc();
    }


    public void CooldownStart(string z)
    {
        switch (z)
        {
            case "Shoot":
                isShooting[1] = isShooting[0];
                break;            
            case "WallJump":
                facing[0] = -facing[0];
                WallJumpInfo[1] = WallJumpInfo[0];
                break;            
            case "KO":
                isKO[1] = isKO[0];
                break;            
            case "Hurt":
                isHurt[1] = isHurt[0];
                break;
            case "Dash":
                isDashing[1] = isDashing[0];
                isDashing[2] = 1;
                break;
            case "Slide":
                isDashing[1] = isDashing[3];
                cantMove();
                break;

        }
    }
    private void CooldownCalc()
    {



        LockCountDown[1] = Mathf.Clamp(LockCountDown[1] - Time.deltaTime, 0, LockCountDown[0]);
        
        isShooting[1] = Mathf.Clamp(isShooting[1] - Time.deltaTime, 0, isShooting[0]);
        WallJumpInfo[1] = Mathf.Clamp(WallJumpInfo[1] - Time.deltaTime, 0, WallJumpInfo[0]);
        isDashing[1] = Mathf.Clamp(isDashing[1] - Time.deltaTime * isDashing[5], 0, isDashing[3]); 
        
        //since [3] (for SlideMax) is greater than [0] (for dashing), we use [3] as a start
        //multiply time.deltatime by the SlideRate (isDashing[5])
        if (isDashing[1] <= 0 || !isGrounded()) //if no longer sliding or dashing and you're not grounded,
        {
            //then you're not sliding
            isDashing[4] = 0;
        }


        if (LockCountDown[1] <= 0)
        {
            inputLock = false;
        }
        //we want to do -= instead of += due to being easier to work with the animator. We can change states depending on if the current value of something is greater than 0 instead of less than some arbitrary number
        
    }

    private void AnimCalc()
    {
        //Set Animator Parameters
        anim.SetBool("Walking", walking);
        anim.SetFloat("Dashing", isDashing[2]);

        anim.SetBool("inputLocked", inputLock);
        anim.SetBool("Grounded", isGrounded());
        anim.SetBool("Crouching", isCrouching);
        
        anim.SetFloat("Vertical Speed", rb.velocity.y);
        anim.SetFloat("isShooting", isShooting[1]);
    }
    private void Movement()
    {
        if (isCrouching) hor = 0;
        //hor cannot change if you're crouching

        walking = hor != 0;
        //walking flag

        //General Checks
        if (!inputLock)
        {
            facingCalc();
            WallSlideCheck();
            CrouchCheck();

            //because CrouchCheck is checked only if we're not inputLocked, we can slide off of platforms smoothly
            //isDashing[4] is set to false because we are not grounded, due to the cooldownCacl
            //Then, since we are both "not sliding" yet "still crouching"...,
            //we go to the default state, with the default rb.velocity calculations, and the crouching dimensions, respectively...
        }


        if (WallJumpInfo[1] > 0) // if wall jumping
        {
            Debug.Log("Wall Jumping");
            rb.gravityScale = 0;

            //facing is adjusted in facingCalc() a few lines above
            if(hor == 0) rb.velocity = new Vector2(facing[0] * WallJumpInfo[2] * (currSpeed / 4), WallJumpInfo[3]);
            else rb.velocity = new Vector2(facing[0] * WallJumpInfo[2] * 1.2f * (currSpeed / 4), WallJumpInfo[3]);


        }

        else if (isDashing[4] == 1) //if sliding
        {
            Debug.Log("Sliding");
            rb.velocity = new Vector2(isDashing[1] * facing[0], rb.velocity.y);
            inputLock = true;
        }

        
        else //if just vibing...
        {
            rb.gravityScale = 1;

            currSpeed = isDashing[2] == 1 ? Speeds[2] : Speeds[1];
            //currSpeed adjustment
            //currSpeed depends on if isDashing[2] is equal to 1. If yes, then Speeds[2], if no, then Speeds[1]
            //currSpeed will adjust our horizontal ground speed depending on the boolean above

            if (!inputLock)
                rb.velocity = new Vector2(hor * currSpeed, rb.velocity.y);

        }



        
    }



    #region DropDown Mechanic
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Platform"))
        {
            isOnPlatform = true;
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Platform"))
        {
            isOnPlatform = false;
        }
    }

    private IEnumerator DisablePlayerCollider(float disableTime)
    {
        ColliderCheck.enabled = false;
        yield return new WaitForSeconds(disableTime);
        ColliderCheck.enabled = true;
    }

    #endregion

    #region Controller Functions
        public void onJump(InputAction.CallbackContext context)
        {

            if (context.started) 
            {
                if (WallJumpInfo[1] <=0)
                {
                    if (isDashing[4] == 1)
                    {
                        //slide jump
                    }

                    //regular jump
                    else if (isGrounded() && !isCrouching)
                    {
                        Debug.Log("Jumping");
                        //rb.AddForce(new Vector2(0,JumpForce));
                        rb.velocity = new Vector2(rb.velocity.x, JumpForce);
                        anim.SetTrigger("Jump");
                    }
 
                    //Wall Jump
                    else if (isOnWall() && !isGrounded())
                    {
                        //rb.gravityScale = 0;
                        //rb.velocity = Vector2.zero;
                        CooldownStart("WallJump");
                        //anim.SetTrigger("Jump");

                        //float wjForce = -facing[0] * WallJumpInfo[2];

                        //Debug.Log(wjForce);
                        /*
                        if (hor == 0)
                        {
                            Debug.Log("Regular Wall Jump");
                            //rb.velocity = new Vector2(-Mathf.Sign(facing[0]) * WallJumpInfo[2] * 2 * (currSpeed / 4), WallJumpInfo[3]);
                            //rb.velocity = new Vector2(wjForce * 2, WallJumpInfo[3]);
                            //jump farther if you're not holding towards the wall
                        }

                        else
                        {
                            Debug.Log("Held Wall Jump");
                            //rb.velocity = new Vector2(-Mathf.Sign(facing[0]) * WallJumpInfo[2] * (currSpeed / 4), WallJumpInfo[3]);
                            //rb.velocity = new Vector2(wjForce , WallJumpInfo[3]);
                                
                        }

                        //transform.localScale = new Vector3(-1 * facing[0], transform.localScale.y, transform.localScale.z);
                        */
                    }
                }
            }

            if (context.performed && isCrouching && isOnPlatform &&  ColliderCheck.enabled == true)
            {
                StartCoroutine(DisablePlayerCollider(DropdownTimer));
            }
        }

        public void onDash(InputAction.CallbackContext context)
        {
            if (context.started && isGrounded())
            {

                if (isCrouching && isDashing[4] == 0)
                {   //if you're crashing and not already sliding...
                    //boost forward by applying a force in the way you are facing
                    CooldownStart("Slide");
                    isDashing[4] = 1;

                }
                else if (!inputLock)//not crouching
                {
                    CooldownStart("Dash");
                }
            }


            if (context.canceled)
            {
                isDashing[2] = 0;
            }
        }

        public void onShoot(InputAction.CallbackContext context)
        {
            if (context.started && isHurt[1] == 0) //We can shoot because we are not hurt
            {
                CooldownStart("Shoot");

                GameObject b = Instantiate(BulletReferences[currentBullet].gameObject, ShootPosition.transform.position, this.transform.rotation);
                b.GetComponent<BulletClass>().setfacing(facing[0], facing[1]);
    
                int movSpeed = 0;
                if(walking)
                    movSpeed = (int) Speeds[1];
            
                //if running, set movSpeed to Speed[2]
                //we want the current bullet to have the same speed as Floe when she's walking, running, or standing still
                b.GetComponent<BulletClass>().setSpeed(bulletSpeed[0] + movSpeed, bulletSpeed[1]);
                b.GetComponent<BulletClass>().setDeterioration(bulletDeterRate[0]);
            }
        }
    #endregion



    #region Constant Checks
    private bool isGrounded()
    {
        RaycastHit2D ray = Physics2D.BoxCast(ColliderCheck.bounds.center, ColliderCheck.bounds.size, 0, Vector2.down, 0.1f, groundLayer);
        RaycastHit2D ray2 = Physics2D.BoxCast(ColliderCheck.bounds.center, ColliderCheck.bounds.size, 0, Vector2.down, 0.1f, platformLayer);
        //BoxCast(collider's centerpoint, size of the collider, rotation of box <at 0 because we dont wanna rotate, we want to check underneath the player - so point down, distance to position the virtual box, the layer we'll be checking for>

        //Debug.DrawRay(ColliderCheck.bounds.center, Vector2.down, Color.red, 100);

        return ray.collider != null || ray2.collider != null;
        //returns true if the ray hits a collider in the groundLayer.
    }
    
    private bool isOnWall()
    {
        RaycastHit2D ray = Physics2D.BoxCast(ColliderCheck.bounds.center, ColliderCheck.bounds.size, 0, new Vector2(facing[0], 0), 0.1f, wallLayer);
        //BoxCast(collider's centerpoint, size of the collider, rotation of box <at 0 because we dont wanna rotate, we want to check underneath the player - so point down, distance to position the virtual box, the layer we'll be checking for>

        return ray.collider != null;
        //returns true if the ray hits a collider in the groundLayer.
    }

    private void facingCalc()
    {
        if (isGrounded())
        {
            if (hor > 0) facing[0] = 1;
            else if (hor < 0) facing[0] = -1;
        }
        transform.localScale = new Vector3(facing[0], 1, 1);
        //Debug.Log(facing[0]);
    }

    private void WallSlideCheck()
    {
        if (isOnWall() && !isGrounded() && hor == facing[0])
        {
            Debug.Log("WallSliding");
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -WallJumpInfo[4], float.MaxValue));
            //clamp makes it so that the min is -wallJumpInfo[4] and the max is the highest float number possible. This makes it so the rb.velocity.y can be no less than -WallJumpInfo[4]
            //We want the speed the player falls at to decrease, thus we need to cap the minimum, hence, limiting how fast the player falls.

        }
    }

    private void CrouchCheck()
{
    if (ver <= -0.8f && isGrounded())
    {
        isCrouching = true;
        ColliderCheck.size = CrouchColliderDimensions;
        ColliderCheck.offset = CrouchColliderOffset;
        //change the collider dimensions to Crouch
    }

    else
    {
        isCrouching = false;
        ColliderCheck.size = BaseColliderDimensions;
        ColliderCheck.offset = BaseColliderOffset;
    }

    //change the collider dimensions to regular
}


    #endregion


    public void cantMove()
    {
        LockCountDown[1] = LockCountDown[0];
        inputLock = true;
    }
}
