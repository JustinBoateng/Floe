using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControl : Being
{

    //[SerializeField] PlayerInput PI;
    [SerializeField] int PlayerNumber;
    [SerializeField] float hor;
    [SerializeField] float ver;
    [SerializeField] float Deadzone = 0.4f;

    private Animator anim;
    
    //Rigidbody2D rb;


    //[SerializeField] Transform GroundCheckLocation;
    //[SerializeField] Transform MountCheckLocation;
    
    [SerializeField] bool inputLock = false;
    //[SerializeField] BoxCollider2D ColliderCheck;// = new BoxCollider2D[2];
    
    //[SerializeField] private LayerMask platformLayer;
    //[SerializeField] private LayerMask groundLayer;
    //[SerializeField] private LayerMask wallLayer;
    //[SerializeField] private LayerMask mountLayer;

    [SerializeField] private float DropdownTimer = 0.25f;

    [SerializeField] private Vector2 BaseColliderOffset = new Vector2(-0.025177f, -0.1591772f);
    [SerializeField] private Vector2 BaseColliderDimensions =  new Vector2(0.8906784f, 1.777763f);

    [SerializeField] private Vector2 CrouchColliderOffset = new Vector2(0f, -0.95f);
    [SerializeField] private Vector2 CrouchColliderDimensions =  new Vector2(1.629251f, 0.6012003f);

    [SerializeField] private Vector2 VaultForce =  new Vector2(5, 2);
    //[SerializeField] private Vector2 SlideForce =  new Vector2(3, 0);


    [SerializeField] bool isCrouching = false;
    //[SerializeField] bool isOnPlatform = false;
    [SerializeField] bool isWallRunning = false;
    [SerializeField] bool isClimbing = false;
    //[SerializeField] bool isVaulting = false;
    [SerializeField] bool walking;
    //[SerializeField] float JumpForce = 10;
    [SerializeField] float currSpeed;

    [SerializeField] private float[] LockCountDown = new float[2];

    [SerializeField] float[] Speeds;
    [SerializeField] float TopSpeed;
    //0: Falling, 1: Walk, 2: Run,
    //TopSpeed changed Dynamically
    //Should you have speeds for climbing as well?

    [SerializeField] float[] WallJumpInfo;
    //0: Max, 1: Curr, 2: Amount to push away from Wall, 3: Amount to be pushed upwards, 4: WallSliding Speed, 5: Player inputting left or right does nothing for this long


    //[SerializeField] public int[] facing { get; private set; } = new int[2];
    //[SerializeField] public int[] facing = new int[2];
    //0: horizontal facing, 1: vertical facing
    //instead of doing an entire get and set function, we can just do this.


    [SerializeField] float[] isJumping = new float[3];
    ////0: Max, 1: Curr, 2: isJumping, 3: JumpForce
    // 4: minimum jump cutoff point. the higher this value, the higher the short jump
    // 5: Jumping Drag (the rate of decreacing the upwards velocity as you SHORT jump upwards.) 

    [SerializeField] float[] isShooting = new float[2]; //used for the shooting animation
    //0: Max, 1: curr
    [SerializeField] float[] isDashing = new float[6];
    //0: Max, 1: Curr, 2: isDashing, 3: is holding the dash button
    //used for invul frames and the boolean for if the player is, in fact, dashing

    [SerializeField] float[] AirDashInfo = new float[5];
    //0: MaxTimer, 1: Curr, 2: MaxAirDashes 3:NumOfAirDashLeft, 4:AirDashForce


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
     
    //[SerializeField] int[] bulletSpeed = new int[2];
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
        bc = GetComponent<BoxCollider2D>();
        anim = GetComponent<Animator>();

        //ColliderChecks[0] = GetComponent<CapsuleCollider2D>();
        facing[0] = 1;
        facing[1] = 1;

        SetMaxAmmo(StartingAmmo);
        AirDashReplenish();

        //EstablishController();
    }

    /*
    private void EstablishController()
    {
        PI = this.GetComponent<PlayerInput>();
        //Debug.Log(PI.);
        switch(PlayerNumber)
        {
            case 0:
                PI.SwitchCurrentControlScheme("Gamepad");
                PI.SwitchCurrentActionMap("Player");
                break;
            case 1:
                PI.SwitchCurrentControlScheme("Gamepad 1");
                PI.SwitchCurrentActionMap("Player1"); 
                break;
            case 2:
                PI.SwitchCurrentControlScheme("Gamepad 2");
                PI.SwitchCurrentActionMap("Player2"); 
                break;


        }
    }
    */
    private void Update()
    {
        CooldownCalc();
        AnimCalc();

        AimMovement();

    }
    // Update is called once per frame
    void FixedUpdate()
    {
        //Debug.Log(Gamepad.all);
        //Debug.Log("Player " + PlayerNumber + ": " + PI.currentControlScheme);
        //Debug.Log("Player " + PlayerNumber + ": " + PI.currentActionMap);
        //Debug.Log("Player " + PlayerNumber + ": " + Gamepad.current);

        VelocityRef = rb.velocity;

        //hor = Input.GetAxisRaw("Horizontal");
        //ver = Input.GetAxisRaw("Vertical");
        //ver = -1;
        
        //Debug.Log("Horizonal: " + hor);
        //Debug.Log("Vertical: " + ver);

        //horizontalInput = Input.GetAxisRaw("Horizontal");

        Movement();


        SpeedManagement();
        //Debug.Log("Current Ammo: " + GetAmmo());
    }


    public void CooldownStart(string z)
    {
        switch (z)
        {
            case "Jump":
                isJumping[1] = isJumping[0];
                isJumping[2] = 1;
                Debug.Log("Applying Force");
                //rb.AddForce(new Vector2(rb.velocity.x, isJumping[3]), ForceMode2D.Force);
                rb.AddForce(new Vector2(rb.velocity.x, isJumping[3]), ForceMode2D.Impulse);

                break;
            case "Shoot":
                isShooting[1] = isShooting[0];
                break;            
            case "WallJump":
                Debug.Log("WallJumping");
                
                AirDashInfo[1] = 0;
                AirDashReplenish();

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
                cantMove();
                break;
            case "AirDash":
                Debug.Log("AirDashing");

                isClimbing = false;

                AirDashInfo[3]--;
                AirDashInfo[1] = AirDashInfo[0];
                if(hor != 0)
                {
                    facing[0] = hor >= 0 ? 1 : -1;
                    facingCalc();
                }
                cantMove();
                break;

        }
    }
    private void CooldownCalc()
    {



        LockCountDown[1] = Mathf.Clamp(LockCountDown[1] - Time.deltaTime, 0, LockCountDown[0]);
        
        isJumping[1] = Mathf.Clamp(isJumping[1] - Time.deltaTime, 0, isJumping[0]);
        
        isShooting[1] = Mathf.Clamp(isShooting[1] - Time.deltaTime, 0, isShooting[0]);
        WallJumpInfo[1] = Mathf.Clamp(WallJumpInfo[1] - Time.deltaTime, 0, WallJumpInfo[0]);
        //isDashing[1] = Mathf.Clamp(isDashing[1] - Time.deltaTime * isDashing[5], 0, isDashing[3]);
        isSliding[1] = Mathf.Clamp(isSliding[1] - Time.deltaTime * isSliding[3], 0, isSliding[0]);
        
        isVaulting[1] = Mathf.Clamp(isVaulting[1] - Time.deltaTime, 0, isVaulting[0]);
        //isVAulting[2] being set to 0 is handled in the Update function
        //this isVaulting[1] counter handles if you're in a vault state


        //since [3] (for SlideMax) is greater than [0] (for dashing), we use [3] as a start
        //multiply time.deltatime by the SlideRate (isDashing[5])


        AirDashInfo[1] = Mathf.Clamp(AirDashInfo[1] - Time.deltaTime, 0, AirDashInfo[0]);

        //if(AirDashInfo[1] <= 0 && AirDashInfo[5] == 1 ) 
        //{
        //    AirDashInfo[5] = 0;
        //}

        if (isGrounded())
        {
            AirDashReplenish();
        }

        if (isSliding[1] <= 0 || !isGrounded()) //if no longer sliding or dashing and you're not grounded,
        {
            isSliding[2] = 0;         
        }

        if (isVaulting[1] > 0 &&  isGrounded() && isSliding[1] <= 0)
        {
           isVaulting[1] = 0;
        }

        if (isVaulting[1] == 0)
        {
            isVaulting[2] = 0;
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
        //hor input cannot change if you're crouching
        if (isCrouching) hor = 0;
        
        //walking flag
        walking = hor != 0;

        //Higher Gravity if Falling
        if(rb.velocity.y < 0) 
        {
            //rb.gravityScale = 1.5f;

            //cap the spped of which you fall to the ground
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Max(rb.velocity.y, -Speeds[0]));
        }

        /*
        else if(isGrounded())
        {
            rb.gravityScale = 1;
        }
        */

        //InputLock Dependent Checks
        if (!inputLock)
        {
            facingCalc();
            {
                //because CrouchCheck is checked only if we're not inputLocked, we can slide off of platforms smoothly
                //isDashing[4] is set to false because we are not grounded, due to the cooldownCacl
                //Then, since we are both "not sliding" yet "still crouching"...,
                //we go to the default state, with the default rb.velocity calculations, and the crouching dimensions, respectively...
            }
        }
        
        
        //Checking for walls is done at all times
        WallMovementCheck();
        CrouchRollCheck(); 
        //Checks if you're Crouching and Rolling. This should be outside InputLock too, so that we can have hor change while we are vaulting. We want to check if we're crouching EVEN IF we can't move. 


        //if jumping
        if (isJumping[2] > 0)
        {
            //if jumping timer is still up
            //if (isJumping[1] > 0)
            //{
                //Debug.Log("Jumping");
                //rb.velocity = new Vector2(rb.velocity.x, isJumping[3] - (isJumping[3] / 2 - isJumping[1]));
            //}

            //else


            //if you hit the apex of your jump
            if (isJumping[1] <= 0 || rb.velocity.y <= 0)
            {
                //rb.velocity = new Vector2(rb.velocity.x, 0);
                isJumping[2] = 0;
            }
        }

        //isJumping[2] <= 0
        else
        {
            if (((isJumping[1] < isJumping[0] / isJumping[4]) || rb.velocity.y <= 0) && isJumping[1] > 0)
            {
                isJumping[1] -= isJumping[5];

                rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y - isJumping[1]);
            }

            if (isJumping[1] == 0)
            {
                rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y);
            }

            /*
            if (isJumping[1] == 0)
            {
                rb.velocity = new Vector2(rb.velocity.x, 0);
                //isJumping[2] = 0;
            }
            */
        }

        if (WallJumpInfo[1] <= 0)
        {
            GravityChange();
        }




        //---Set up seperate ifelse chain below

        //if climbing
        if (canClimb() && Mathf.Abs(ver) >= Deadzone || isClimbing)
        {
            Debug.Log("Climbing");
            AirDashReplenish(); //replenish airdash when climbing
            isClimbing = true;
            rb.gravityScale = 0;
            rb.velocity = new Vector2(hor * Speeds[1], ver * Speeds[1]);
        }

        // if wall jumping
        else if (WallJumpInfo[1] > 0)
        {
            Debug.Log("Wall Jumping");
            rb.gravityScale = 0;

            //facing is adjusted in facingCalc() a few lines above
            //if (hor == 0) 
            rb.velocity = new Vector2(facing[0] * WallJumpInfo[2] * (currSpeed / 4), WallJumpInfo[3]);
            //else rb.velocity = new Vector2(facing[0] * WallJumpInfo[2] * 1.2f * (currSpeed / 4), WallJumpInfo[3]);
        }

        //if Vaulting
        else if (isVaulting[2] == 1) 
        {
            Debug.Log("Vaulting");
            //rb.AddForce(new Vector2(VaultForce.x * facing[0], VaultForce.y), ForceMode2D.Impulse);
            rb.velocity = new Vector2(VaultForce.x * facing[0], VaultForce.y);
            //rb.AddForce(new Vector2(VaultForce.x * facing[0], VaultForce.y), ForceMode2D.Force);
            GravityChange();
            //we add force once
            //isVaulting[2] = 0;
        }
        //check if vaulting before if sliding. Vaulting is more situational than sliding

        //if sliding
        else if (isSliding[2] == 1) 
        {
            Debug.Log("Sliding");
            rb.velocity = new Vector2(isSliding[1] * facing[0], rb.velocity.y);
            {//rb.AddForce(new Vector2(SlideForce.x * facing[0], SlideForce.y), ForceMode2D.Impulse);
             //isDashing[4] = 0;
             //You'd have to adjust how the game detects sliding after a set time if you want to do rb.addforce. Vaulting depends on recognizing if your sliding. 
            }
            GravityChange();
            //inputLock = true;
        }

        //if rolling
        else if (isRolling) 
        {
            GravityChange();
            SlopeCoefficient = rb.velocity.y < 0 ? .7f : 0;
            rb.velocity = new Vector2(rb.velocity.x + (RollDecay * -facing[0]) + (SlopeCoefficient * facing[0]), rb.velocity.y);
        }

        // if air dashing
        else if (AirDashInfo[1] > 0) 
        {
            //inputLock = true;
            rb.velocity = new Vector2(AirDashInfo[4] * facing[0], 0);
        }

        //if just vibing...
        else
        {
            //rb.gravityScale = 1;
            GravityChange();
            currSpeed = isDashing[2] == 1 ? Speeds[2] : Speeds[1];
            {
                //currSpeed adjustment
                //currSpeed depends on if isDashing[2] is equal to 1. If yes, then Speeds[2], if no, then Speeds[1]
                //currSpeed will adjust our horizontal ground speed depending on the boolean above
            }
            if (!inputLock) {
                {
                    //rb.velocity = new Vector2(Mathf.Clamp(rb.velocity.x + hor, rb.velocity.x, rb.velocity.x), rb.velocity.y);
                    //rb.velocity = new Vector2(Mathf.Clamp(rb.velocity.x + hor * currSpeed, -Speeds[0], Speeds[0]), rb.velocity.y);
                    //having the cap be whatever currSpeed is set to (Speeds[2] or Speeds[1]) allows for consistant aerial speed
                }
                if (!isGrounded())
                    rb.velocity = new Vector2(Mathf.Clamp(rb.velocity.x + (hor * currSpeed), -TopSpeed, TopSpeed), rb.velocity.y);
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
        //Debug.Log("Curr Aim: " + Aim);
        //ShootPosition.transform.position = AimPositions[Aim];

    }

    /*
    private void OnCollisionEnter2D(Collision2D collision)
    {
        //if (collision.gameObject.CompareTag("Platform"))
        if (collision.gameObject.layer == platformLayer)
        {
            isOnPlatform = true;
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        //if (collision.gameObject.CompareTag("Platform"))
        if (collision.gameObject.layer == platformLayer)
        {
            isOnPlatform = false;
        }
    }
    */
    #region DropDown Mechanic
    private IEnumerator DisablePlayerCollider(float disableTime)
    {
        bc.enabled = false;
        yield return new WaitForSeconds(disableTime);
        bc.enabled = true;
    }

    #endregion

    #region Controller Functions

    public void OnMove(InputAction.CallbackContext context)
    {

        float inputx = context.ReadValue<Vector2>().x;
        float inputy = context.ReadValue<Vector2>().y;

        if (inputx < -Deadzone) hor = -1;
        else if (inputx > Deadzone) hor = 1;
        else hor = 0;

        if (inputy < -Deadzone) ver = -1;
        else if (inputy > Deadzone) ver = 1;
        else ver = 0;

        //hor = context.ReadValue<Vector2>().x;
        //ver = context.ReadValue<Vector2>().y;

        
    }


    public void onJump(InputAction.CallbackContext context)
    {


        //Dropdown
        if (context.started && isCrouching && isOnPlatform && bc.enabled == true)
        {
            Debug.Log("Dropping Down");
            StartCoroutine(  DisablePlayerCollider(DropdownTimer)  );
        }

        
        else if ((context.canceled)) //&& !isGrounded())
        {
            //rb.gravityScale = 1.75f;

            isJumping[2] = 0;
        }

        else if (context.performed) 
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
            else if ((isGrounded() && !isCrouching) || isClimbing)
            {
                Debug.Log("Jumping");
                //rb.AddForce(new Vector2(0,JumpForce));
                CooldownStart("Jump");

                isClimbing = false;
                
                
                anim.SetTrigger("Jump");
            }
 
            
        }

        /*
        if (context.performed && rb.velocity.y > 0 && !isGrounded())
        {
            rb.gravityScale = 0.5f;
        }
        */


        Debug.Log(context.phase);
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

        else if (context.started && !isGrounded())
        {
            if (AirDashInfo[3] > 0)
            {
                GravityChange();
                WallJumpInfo[1] = 0; //No Longer WallJumping
                CooldownStart("AirDash");
            }

        }


        if (context.performed)
        {
            isDashing[3] = 1;
        }

        if (context.canceled)
        {
            isDashing[2] = 0; // you aren't performing the action of dashing anymore    
            isDashing[3] = 0; // nor are you holding the button down
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
                b.GetComponent<BulletClass>().setSignature(this, PlayerNumber);
            }
        }
    }
    #endregion



    #region Constant Checks

    //Handles if you can climb or if you are no longer climbing. 
    //"If you are climbing" is handled in Movement()
    private bool canClimb()
    {
        RaycastHit2D ClimbRay = Physics2D.BoxCast(bc.bounds.center, bc.bounds.size, 0, Vector2.down, 0.1f, mountLayer);
        bool c = ClimbRay.collider != null;

        if (c == false) isClimbing = false;
        return c;
    }

    private bool isOnWall()
    {
        RaycastHit2D ray = Physics2D.BoxCast(bc.bounds.center, bc.bounds.size, 0, new Vector2(facing[0], 0), 0.1f, wallLayer);
        //BoxCast(collider's centerpoint, size of the collider, rotation of box <at 0 because we dont wanna rotate, we want to check underneath the player - so point down, distance to position the virtual box, the layer we'll be checking for>

        bool y = ray.collider != null;
        //Debug.Log(y);
        return y;
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
                //AirDashReplenish();
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
        bc.size = CrouchColliderDimensions;
        bc.offset = CrouchColliderOffset;
        //change the collider dimensions to Crouch
    }

    else
    {
        isCrouching = false;
        bc.size = BaseColliderDimensions;
        bc.offset = BaseColliderOffset;
    }

    //change the collider dimensions to regular
}


    #endregion


    private void AirDashReplenish()
    {
        AirDashInfo[3] = AirDashInfo[2];
    }


    public void cantMove()
    {
        LockCountDown[1] = LockCountDown[0];
        inputLock = true;
    }

    private void SpeedManagement()
    {
        if (isVaulting[1] > 0)
        {
            if (!isOnWall())
                TopSpeed = VaultForce.x;
        }

        if (!isGrounded())
        {
            //TopSpeed = Speeds[1];

            if (AirDashInfo[1] > 0)
            {
                TopSpeed = AirDashInfo[4];
            }

            else if (isDashing[3] > 0)
            {
                TopSpeed = Speeds[2];
            }
                      
        }

        else
            TopSpeed = Speeds[1];

    }
}
