using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControl : Being
{

    [SerializeField] float hor;
    [SerializeField] float ver;
    [SerializeField] float Deadzone = 0.4f;

    private Animator anim;
    
    Rigidbody2D rb;


    [SerializeField] Transform GroundCheckLocation;

    [SerializeField] bool inputLock = false;
    [SerializeField] BoxCollider2D ColliderCheck;// = new BoxCollider2D[2];
    
    [SerializeField] private LayerMask platformLayer;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask wallLayer;

    [SerializeField] private float DropdownTimer = 0.25f;

    [SerializeField] private Vector2 BaseColliderOffset = new Vector2(-0.025177f, -0.1591772f);
    [SerializeField] private Vector2 BaseColliderDimensions =  new Vector2(0.8906784f, 1.777763f);

    [SerializeField] private Vector2 CrouchColliderOffset = new Vector2(0f, -0.95f);
    [SerializeField] private Vector2 CrouchColliderDimensions =  new Vector2(1.629251f, 0.6012003f);

    [SerializeField] private Vector2 VaultForce =  new Vector2(5, 2);
    //[SerializeField] private Vector2 SlideForce =  new Vector2(3, 0);


    [SerializeField] bool isCrouching = false;
    [SerializeField] bool isOnPlatform = false;
    [SerializeField] bool isWallRunning = false;
    //[SerializeField] bool isVaulting = false;
    [SerializeField] bool walking;
    [SerializeField] float JumpForce = 10;
    [SerializeField] float currSpeed;

    [SerializeField] private float[] LockCountDown = new float[2];

    [SerializeField] float[] Speeds;
    //0: Max, 1: Walk, 2: Run

    [SerializeField] float[] WallJumpInfo;
    //0: Max, 1: Curr, 2: Amount to push away from Wall, 3: Amount to be pushed upwards, 4: WallSliding Speed, 5: Player inputting left or right does nothing for this long

    
    //[SerializeField] public int[] facing { get; private set; } = new int[2];
    [SerializeField] public int[] facing = new int[2];
    //0: horizontal facing, 1: vertical facing
    //instead of doing an entire get and set function, we can just do this.


    [SerializeField] float[] isShooting = new float[2]; //used for the shooting animation
    //0: Max, 1: curr
    [SerializeField] float[] isDashing = new float[6];
    //0: Max,
    //1: Curr,
    //2: isDashing,
    //used for invul frames and the boolean for if the player is, in fact, dashing

    [SerializeField] float[] isSliding = new float[4];
    
    [SerializeField] float[] isVaulting = new float[4];
    //0:Max
    //1:curr
    //2:is or isnt (initially)
    //3:rate

    [SerializeField] bool isRolling = false;
    [SerializeField] float RollDecay = 0.5f;
    [SerializeField] float SlopeCoefficient = 0.7f;
    
    [SerializeField] float[] isHurt = new float[2]; //used for the Hurt animation
    [SerializeField] float[] isKO = new float[2]; //used for the KO animation
     
    [SerializeField] int[] bulletSpeed = new int[2];
    //0: horizontal Speed, 1: vertical Speed
    
    [SerializeField] float[] bulletDeterRate = { 2 };
    //determines how long bullets stay active for

    [SerializeField] GameObject[] ShootPosition;
    [SerializeField] BulletClass[] BulletReferences;
    //[SerializeField] Vector2[] AimPositions = new Vector2[5];
    //This brings complications, since you're changing the position of an object that's attached to another object
    [SerializeField] Vector2[] BulletSpeeds = new Vector2[5];
    [SerializeField] int Aim;

    [SerializeField] int StartingAmmo;


    [SerializeField] int currentBullet;
    // Start is called before the first frame update

    [SerializeField] Vector2 VelocityRef;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        //ColliderChecks[0] = GetComponent<BoxCollider2D>();
        ColliderCheck = GetComponent<BoxCollider2D>();
        anim = GetComponent<Animator>();

        //ColliderChecks[0] = GetComponent<CapsuleCollider2D>();
        facing[0] = 1;
        facing[1] = 1;

        SetMaxAmmo(StartingAmmo);
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(Input.GetAxisRaw("Horizontal"));

        VelocityRef = rb.velocity;

        hor = Input.GetAxisRaw("Horizontal");
        ver = Input.GetAxisRaw("Vertical");
        //ver = -1;
        
        //Debug.Log("Horizonal: " + hor);
        //Debug.Log("Vertical: " + ver);

        //horizontalInput = Input.GetAxisRaw("Horizontal");
        Movement();

        CooldownCalc();
        AnimCalc();

        AimMovement();

        //Debug.Log("Current Ammo: " + GetAmmo());
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
                isSliding[2] = 1;
                isSliding[1] = isSliding[0];
                cantMove();
                break;
            case "Vault":
                //isVaulting = true;
                Debug.Log("Vault Activated");

                isVaulting[1] = isVaulting[0];
                isVaulting[2] = 1; //isVaulting = true
                break;

        }
    }
    private void CooldownCalc()
    {



        LockCountDown[1] = Mathf.Clamp(LockCountDown[1] - Time.deltaTime, 0, LockCountDown[0]);
        
        isShooting[1] = Mathf.Clamp(isShooting[1] - Time.deltaTime, 0, isShooting[0]);
        WallJumpInfo[1] = Mathf.Clamp(WallJumpInfo[1] - Time.deltaTime, 0, WallJumpInfo[0]);
        //isDashing[1] = Mathf.Clamp(isDashing[1] - Time.deltaTime * isDashing[5], 0, isDashing[3]);
        isSliding[1] = Mathf.Clamp(isSliding[1] - Time.deltaTime * isSliding[3], 0, isSliding[0]);
        
        isVaulting[1] = Mathf.Clamp(isVaulting[1] - Time.deltaTime, 0, isVaulting[0]);
        //isVAulting[2] being set to 0 is handled in the Update function
        //this isVaulting[1] counter handles if you're in a vault state


        //since [3] (for SlideMax) is greater than [0] (for dashing), we use [3] as a start
        //multiply time.deltatime by the SlideRate (isDashing[5])

        //then you're not sliding
        if (isSliding[1] <= 0 || !isGrounded()) //if no longer sliding or dashing and you're not grounded,
        {
            isSliding[2] = 0;

         
        }

        if (isVaulting[1] > 0 &&  isGrounded() && isSliding[1] <= 0)
        {
           isVaulting[1] = 0;
        }

        if (Mathf.Abs(rb.velocity.x) < Mathf.Abs(facing[0])|| !isGrounded() || !isCrouching)
        {
            isRolling = false;
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

        anim.SetFloat("isVaulting", isVaulting[1]);
        anim.SetFloat("isSliding", isSliding[1]);
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
            WallMovementCheck(); // Handles Wall Sliding and Wall Running
            CrouchRollCheck(); //Handles Crouching and Rolling

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
            if (hor == 0) rb.velocity = new Vector2(facing[0] * WallJumpInfo[2] * (currSpeed / 4), WallJumpInfo[3]);
            else rb.velocity = new Vector2(facing[0] * WallJumpInfo[2] * 1.2f * (currSpeed / 4), WallJumpInfo[3]);


        }

        else if (isVaulting[2] == 1) //if Vaulting
        {
            Debug.Log("Vaulting");
            //rb.velocity = new Vector2(VaultForce.x * facing[0], VaultForce.y);
            rb.AddForce(new Vector2(VaultForce.x * facing[0], VaultForce.y), ForceMode2D.Impulse);

            //we add force once
            isVaulting[2] = 0;


        }
        //check if vaulting before if sliding. Vaulting is more situational than sliding

        else if (isSliding[2] == 1) //if sliding
        {
            Debug.Log("Sliding");
            rb.velocity = new Vector2(isSliding[1] * facing[0], rb.velocity.y);
            //rb.AddForce(new Vector2(SlideForce.x * facing[0], SlideForce.y), ForceMode2D.Impulse);
            //isDashing[4] = 0;
            //You'd have to adjust how the game detects sliding after a set time if you want to do rb.addforce. Vaulting depends on recognizing if your sliding. 
            inputLock = true;
        }

        else if (isRolling) 
        {
            SlopeCoefficient = rb.velocity.y < 0 ? .7f : 0;

            rb.velocity = new Vector2(rb.velocity.x + (RollDecay * -facing[0]) + (SlopeCoefficient * facing[0]), rb.velocity.y);
        }


        else //if just vibing...
        {
            rb.gravityScale = 1;

            currSpeed = isDashing[2] == 1 ? Speeds[2] : Speeds[1];
            //currSpeed adjustment
            //currSpeed depends on if isDashing[2] is equal to 1. If yes, then Speeds[2], if no, then Speeds[1]
            //currSpeed will adjust our horizontal ground speed depending on the boolean above

            if (!inputLock) {
                if (!isGrounded())
                    //rb.velocity = new Vector2(Mathf.Clamp(rb.velocity.x + hor, rb.velocity.x, rb.velocity.x), rb.velocity.y);
                    //rb.velocity = new Vector2(Mathf.Clamp(rb.velocity.x + hor * currSpeed, -Speeds[0], Speeds[0]), rb.velocity.y);
                    //having the cap be whatever currSpeed is set to (Speeds[2] or Speeds[1]) allows for consistant aerial speed
                    rb.velocity = new Vector2(Mathf.Clamp(rb.velocity.x + hor * currSpeed, -currSpeed, currSpeed), rb.velocity.y);

                else
                    rb.velocity = new Vector2(hor * currSpeed, rb.velocity.y);
            }
        }



        
    }

    private void AimMovement()
    {


        if(hor == -facing[0] && !isGrounded())
        {
            Aim = 2;
        }

        else if(ver >= Deadzone)
        {
            Aim = 3;
        }
        
        else if(ver <= -Deadzone)
        {
            Aim = 4;
        }


        else 
        {
            Aim = 1;
        }
        Debug.Log("Curr Aim: " + Aim);
        //ShootPosition.transform.position = AimPositions[Aim];

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
            //vault
            if (isSliding[2] == 1) //if sliding and you jump
            {
                CooldownStart("Vault");
            }


            //Wall Jump
            else if (isOnWall() && !isGrounded())
            {
                CooldownStart("WallJump");
            }

            //regular jump
            else if (isGrounded() && !isCrouching)
            {
                Debug.Log("Jumping");
                //rb.AddForce(new Vector2(0,JumpForce));
                rb.velocity = new Vector2(rb.velocity.x, JumpForce);
                anim.SetTrigger("Jump");
            }
 
            
        }

        //Dropdown
        if (context.performed && isCrouching && isOnPlatform &&  ColliderCheck.enabled == true)
        {
            StartCoroutine(DisablePlayerCollider(DropdownTimer));
        }
    }

    public void onDash(InputAction.CallbackContext context)
    {
        if (context.started && isGrounded())
        {

            if (isCrouching && isSliding[2] == 0)
            {   //if you're crashing and not already sliding...
                //boost forward by applying a force in the way you are facing   
                CooldownStart("Slide");

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
            if (GetAmmo() > 0)
            {
                AmmoCalc(-1);
                CooldownStart("Shoot");


                GameObject b = Instantiate(BulletReferences[currentBullet].gameObject, ShootPosition[Aim].transform.position, this.transform.rotation);
                b.GetComponent<BulletClass>().setfacing(facing[0], facing[1]);

                int movSpeed = 0;
                if (walking)
                    movSpeed = (int)Speeds[1];

                //if running, set movSpeed to Speed[2]
                //we want the current bullet to have the same speed as Floe when she's walking, running, or standing still


                //b.GetComponent<BulletClass>().setSpeed(bulletSpeed[0] + movSpeed, bulletSpeed[1]);
                //b.GetComponent<BulletClass>().setSpeed(BulletSpeeds[Aim].x + (movSpeed * facing[0]), BulletSpeeds[Aim].y);
                b.GetComponent<BulletClass>().setSpeed(BulletSpeeds[Aim].x , BulletSpeeds[Aim].y);

                b.GetComponent<BulletClass>().setDeterioration(bulletDeterRate[0]);
                b.GetComponent<BulletClass>().setSignature(this);
            }
        }
    }
    #endregion



    #region Constant Checks
    private bool isGrounded()
    {
        RaycastHit2D ray = Physics2D.BoxCast(ColliderCheck.bounds.center, ColliderCheck.bounds.size, 0, Vector2.down, 0.1f, groundLayer);
        RaycastHit2D ray2 = Physics2D.BoxCast(ColliderCheck.bounds.center, ColliderCheck.bounds.size, 0, Vector2.down, 0.1f, platformLayer);
        RaycastHit2D ray3 = Physics2D.BoxCast(GroundCheckLocation.transform.position, new Vector2(ColliderCheck.size.x, 1), 0, Vector2.down, 0.1f, groundLayer);
        RaycastHit2D ray4 = Physics2D.BoxCast(GroundCheckLocation.transform.position, new Vector2(ColliderCheck.size.x, 1), 0, Vector2.down, 0.1f, platformLayer);
        //BoxCast(collider's centerpoint, size of the collider, rotation of box <at 0 because we dont wanna rotate, we want to check underneath the player - so point down, distance to position the virtual box, the layer we'll be checking for>

        Debug.DrawRay(GroundCheckLocation.transform.position, Vector2.down, Color.red, 1);

        //bool y = ray.collider != null || ray2.collider != null;
        bool y = ray3.collider != null|| ray4.collider != null;
        
        //Debug.Log(y);
        
        return y;
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
        //use isGrounded so that you turn only when grounded
        if (isGrounded())
        {
            if (hor > 0) facing[0] = 1;
            else if (hor < 0) facing[0] = -1;
        }


        if(ver > 0) facing[1] = 1;
        else if (ver < 0) facing[1] = -1;

        transform.localScale = new Vector3(facing[0], 1, 1);
        //Debug.Log(facing[0]);
    }

    private void WallMovementCheck()
    {
        if (isOnWall() && !isGrounded() && hor == facing[0])
        {
            if (isVaulting[1] > 0 || isWallRunning) //if we hit a wall while Vaulting or already WallRUnnning
            {
                Debug.Log("WallRunning");
                isWallRunning = true;
                rb.velocity = new Vector2(rb.velocity.x, Speeds[1]);

                //now to find a time to turn isWallRunning off
            }
            
            else
            {
                Debug.Log("WallSliding");
                rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -WallJumpInfo[4], float.MaxValue));
            }
            //clamp makes it so that the min is -wallJumpInfo[4] and the max is the highest float number possible. This makes it so the rb.velocity.y can be no less than -WallJumpInfo[4]
            //We want the speed the player falls at to decrease, thus we need to cap the minimum, hence, limiting how fast the player falls.

        }

        if (!isOnWall() || isGrounded() || hor != facing[0])
        {
            isWallRunning = false;
        }
    }

    
    private void CrouchRollCheck()
{
    if (ver <= -Deadzone && isGrounded())
    {
            if (isDashing[2] == 1 && Mathf.Abs(rb.velocity.x) > Mathf.Abs(facing[0]))
            {
                isRolling = true;
            }
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
