using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControl : Being
{

    [SerializeField] int PlayerNumber;
    [SerializeField] float hor;
    [SerializeField] float ver;
    [SerializeField] float inputx;
    [SerializeField] float inputy;
    [SerializeField] float Deadzonex = 0.4f;
    [SerializeField] float Deadzoney = 0.4f;

    [SerializeField] bool inputLock = false;

    [SerializeField] private float DropdownTimer = 0.25f;

    [SerializeField] public Vector2 VelocityRef;

    [SerializeField] private Vector2 BaseColliderOffset = new Vector2(-0.025177f, -0.1591772f);
    [SerializeField] private Vector2 BaseColliderDimensions =  new Vector2(0.8906784f, 1.777763f);

    [SerializeField] private Vector2 CrouchColliderOffset = new Vector2(0f, -0.95f);
    [SerializeField] private Vector2 CrouchColliderDimensions =  new Vector2(1.629251f, 0.6012003f);

    [SerializeField] private Vector2 VaultForce =  new Vector2(5, 2);
    //[SerializeField] private Vector2 SlideForce =  new Vector2(3, 0);

    [SerializeField] Vector2[] BulletSpeeds = new Vector2[5];

    [SerializeField] Vector2[] SwingEndpoints;
    [SerializeField] float SwingLERPValue;
    [SerializeField] Vector2 SwingSpeed = Vector2.zero;



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


    [SerializeField] float[] isJumping = new float[3];
    //  0: Max that [1] can be,
    //  1: Current value of jump counter,
    //  2: isJumping boolean (is the player actually pressing the jump button), 
    //0, 1., have more to do with the short jump physics

    [SerializeField] float[] jumpNuance = new float[3];
    // 0 : JumpForce
    // 1 : minimum jump cutoff point. the higher this value, the higher the short jump    
    // 2 : Jumping Drag (the rate of decreasing the upwards velocity as you SHORT jump upwards.) 
    // 2 might not be necessary

    //if isJumping[1] is lower than jumpNuance[1], then the short jump ends
    //isJumping[0] determines how high isJumping[1] can be
    //keep in mind, the higher isJumping[1] can be, the faster the jump deceleration
    //since it's decreasing y velocity per frame via what isJumping[1] currently is.
    //isJumpingp[0] and jumpNuance[1] tends to be in charge of Short Jump physics

    [SerializeField] float[] isShooting = new float[2]; //used for the shooting animation
    //0: Max, 1: curr
    [SerializeField] float[] isDashing = new float[6];
    //0: Max, 1: Curr, 2: isDashing, 3: is holding the dash button    //used for invul frames and the boolean for if the player is, in fact, dashing

    [SerializeField] float[] AirDashInfo = new float[5];
    //0: MaxTimer, 1: Curr, 2: MaxAirDashes 3:NumOfAirDashLeft, 4:AirDashForce


    [SerializeField] float[] isSliding = new float[4];
    
    [SerializeField] float[] isSwinging = new float[2];
    //Max, Curr
    
    [SerializeField] float[] isVaulting = new float[4];
    //0:Max  //1:curr    //2:is or isnt (initially)    //3:rate

    [SerializeField] bool isRolling = false;
    [SerializeField] float RollDecay = 0.5f;
    [SerializeField] float SlopeCoefficient = 0.7f;
    
    [SerializeField] float[] isHurt = new float[2]; //used for the Hurt animation
    [SerializeField] float[] isKO = new float[4]; //used for the KO animation
    //1: Max, 2: Curr, 3: isKO'd (First three for KO Animation)
    //4: MaxReviveTimer, 5: CurrReviveTimer


    [SerializeField] float[] bulletDeterRate = { 2 };
    //determines how long bullets stay active for

    [SerializeField] GameObject[] ShootPosition;
    [SerializeField] BulletClass[] BulletReferences;
    [SerializeField] int Aim;

    [SerializeField] int StartingAmmo;


    [SerializeField] int currentBullet;
    [SerializeField] float[] ChargeFactor;
    //[Max, Curr, isCharging]

    [SerializeField] public int[] Coins = new int[2];
    //Copper, Nickel

    [SerializeField] Transform TerrainObject;

    // Start is called before the first frame update
    void Start()
    {
        BeingStart();        

        facing[0] = 1; facing[1] = 1;

        SetMaxAmmo(StartingAmmo);
        AirDashReplenish();

        inputLock = false;
        //turns true when cantMove() is called, or when player HP hits zero
    }

    // Update is called once per frame
    private void Update()
    {
        CooldownCalc();
        AnimCalc();

        AimMovement();

        //Debug.Log("Grounded: " + isGrounded());

    }

    //Update for physics
    void FixedUpdate()
    {
        VelocityRef = rb.velocity;

        Movement();

        SpeedManagement();
    }


    public void CooldownStart(string z)
    {
        switch (z)
        {
            case "Jump":
                anim.SetTrigger("Jump");

                AirDashReplenish();

                isJumping[1] = isJumping[0];
                isJumping[2] = 1;

                rb.velocity = Vector2.zero;
                rb.AddForce(new Vector2(rb.velocity.x, jumpNuance[0]), ForceMode2D.Impulse);
                //rb.AddForce(new Vector2(rb.velocity.x, isJumping[3]), ForceMode2D.Impulse);
                //AddForce is used here to set up physics for any jumping.
                //It's needed to get Floe off the ground.
                //Maintaining that velocity is done in the Update Function

                isClimbing = false;
                
                break;
            case "Shoot":
                isShooting[1] = isShooting[0];
                break;            
            case "WallJump":
                //Debug.Log("WallJumping");
                
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
                ResetStates("Vault");
                //isVaulting = true;
                //Debug.Log("Vault Activated");

                isVaulting[1] = isVaulting[0];
                isVaulting[2] = 1; //isVaulting = true
                cantMove();
                break;
            case "Swing":
                ResetStates("Swing");
                isSwinging[1] = isSwinging[0];
                isSwinging[2] = 1;
                break;
            case "AirDash":
                //Debug.Log("AirDashing");
                ResetStates("AirDash");
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
            case "KO'd":
                GameplayManager.GM.CameraCont.disengageTarget();
                bc.enabled = false;
                isKO[1] = isKO[0]; //for physics
                isKO[4] = isKO[3]; //for revive timer
                isKO[2] = 1;
                rb.AddForce(new Vector2(rb.velocity.x, jumpNuance[0]), ForceMode2D.Impulse);
                inputLock = true;
                GameplayManager.GM.PlayerDown(PlayerNumber);
                break;
        }
    }
    private void CooldownCalc()
    {
        isKO[1]          = Mathf.Clamp(isKO[1]          - Time.deltaTime, 0, isKO[0]); //for checing if you're ko'd
        isKO[4]          = Mathf.Clamp(isKO[4]          - Time.deltaTime, 0, isKO[4]); //for revive timer
        LockCountDown[1] = Mathf.Clamp(LockCountDown[1] - Time.deltaTime, 0, LockCountDown[0]);        

        isJumping[1]     = Mathf.Clamp(isJumping[1]     - Time.deltaTime, 0, isJumping[0]);
        WallJumpInfo[1]  = Mathf.Clamp(WallJumpInfo[1]  - Time.deltaTime, 0, WallJumpInfo[0]);
        isVaulting[1]    = Mathf.Clamp(isVaulting[1]    - Time.deltaTime, 0, isVaulting[0]);

        isSwinging[1]    = Mathf.Clamp(isSwinging[1]    - Time.deltaTime, 0, isSwinging[0]);
        isSliding[1]     = Mathf.Clamp(isSliding[1]     - Time.deltaTime * isSliding[3], 0, isSliding[0]);
        AirDashInfo[1]   = Mathf.Clamp(AirDashInfo[1]   - Time.deltaTime, 0, AirDashInfo[0]);

        isShooting[1] = Mathf.Clamp(isShooting[1] - Time.deltaTime, 0, isShooting[0]);
        ChargeFactor[1] = Mathf.Clamp(ChargeFactor[1] + Time.deltaTime, 0, ChargeFactor[0]);


        if (isGrounded())
        {
            AirDashReplenish();
            isSwinging[1] = 0;
            if (isVaulting[1] > 0 ) isVaulting[1] = 0;
        }

        if (isSliding[1] <= 0 || !isGrounded()) //if no longer sliding or you're not grounded,
        {
            isSliding[2] = 0;         
        }


        if (isVaulting[1] <= 0) isVaulting[2] = 0;

        if (isSwinging[1] <= 0) isSwinging[2] = 0;

        //Rolling Cooldown
        if (Mathf.Abs(rb.velocity.x) < Mathf.Abs(facing[0])|| !isGrounded() || !isCrouching)
        {
            isRolling = false;
        }

        //Revive Cooldown
        if (isKO[4] <= 0 && isKO[2] > 0)
        {
            isKO[2] = 0;
            Revive();
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
        anim.SetFloat("isSwinging", isSwinging[1]);

        //anim.SetFloat("isCharging", ChargeFactor[1]);
        anim.SetFloat("isKO", isKO[2]);

    }
    private void Movement()
    {
        //Debug.Log(rb.velocity.y);

        //hor input cannot change if you're crouching
        if (isCrouching) hor = 0;
        
        //walking flag
        walking = hor != 0;

        //Higher Gravity if Falling
        if(rb.velocity.y < 0) 
        {
            //cap the speed of which you fall to the ground
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Max(rb.velocity.y, -Speeds[0]));
        }

        //InputLock Dependent Checks
        if (!inputLock)
        {
            facingCalc();
        }
        
        
        //Checking for walls is done at all times
        WallMovementCheck();
        CrouchRollCheck();
        //Checks if you're Crouching and Rolling. This should be outside InputLock too, so that we can have hor change while we are vaulting. We want to check if we're crouching EVEN IF we can't move. 

        //if KO'd
        if (isKO[2] != 0)
        {
            inputLock = true;
            if (isKO[2] == 1)
            {
                //pop the player up

                if (isKO[1] <= 0 || rb.velocity.y <= 0)
                {
                    isKO[2] = 2;
                }
            }

            if (isKO[2] == 2)
            {

                //drag the player down
                if (((isKO[1] < isJumping[0] / jumpNuance[1]) || rb.velocity.y <= 0) && isKO[1] > 0)
                {
                    isKO[1] -= jumpNuance[2];

                    rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y - isJumping[1]);
                }

                if (isJumping[1] == 0)
                {
                    rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y);
                }

            }
        }


        //if we are pressing the jump button
        if (isJumping[2] > 0)
        {
            //if you hit the apex of your jump, then set the isJumping boolean to false
            //it'd be as if you aren't holding the jump button anymore
            //if (isJumping[1] <= 0 || rb.velocity.y <= 0)
            if (rb.velocity.y <= 0)
            {
                isJumping[2] = 0;
            }
        }

        //if we are NOT pressing the jump button
        else
        {
            //if isJumping[1] is not 0 yet, meaning Floe is still rising in her jump
            //implying a SHORT JUMP is being performed
            //if (((isJumping[1] < isJumping[0] / isJumping[4]) || rb.velocity.y <= 0) && isJumping[1] > 0)
            //if ((rb.velocity.y <= 0) && isJumping[1] > 0)
            //-----------

            //if current jump count < jump cuttoff (ShortJump Mechanic)            
            if (((isJumping[1] < jumpNuance[1])))// || rb.velocity.y <= 0))// && isJumping[1] > 0)
            {
                //isJumping[1] -= isJumping[5];
                //isJumping[1] -= jumpNuance[2];
                //-------------
                { 
                    //reduce the y velocity by what the isJumping[1] value is
                    //since isJumping[1] is constantly decreasing in CooldownCount(),
                    //then the decrease is more dynamic and refined
                    //this means that our upwards acceleration is decreasing,
                    //but the rate of decreasing itself is decreasing as time passes
                    //giving the smooth decrease in speed as the player rises up
                }
                rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y - isJumping[1]); 
            }


            //if you're no longer jumping and are just falling
            //if (isJumping[1] == 0)
            //{
            //    rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y);
            //}

        }

        if (WallJumpInfo[1] <= 0)
        {
            GravityChange();
        }




        //---Set up seperate if-else chain below

        //if climbing
        if (canClimb() && Mathf.Abs(ver) >= Deadzoney || isClimbing)
        {
            //Debug.Log("Climbing");
            AirDashReplenish(); //replenish airdash when climbing
            isClimbing = true;
            rb.gravityScale = 0;
            rb.velocity = new Vector2(hor * Speeds[1], ver * Speeds[1]);
        }

        // if wall jumping
        else if (WallJumpInfo[1] > 0)
        {
            //Debug.Log("Wall Jumping");
            rb.gravityScale = 0;
            //facing is adjusted in facingCalc() a few lines above
            rb.velocity = new Vector2(facing[0] * WallJumpInfo[2] * (currSpeed / 4), WallJumpInfo[3]);

        }

        //if Vaulting
        else if (isVaulting[2] == 1) 
        {
            //Debug.Log("Vaulting");
            rb.velocity = new Vector2(VaultForce.x * facing[0], VaultForce.y);
            GravityChange();

        }
        //check if vaulting before if sliding. Vaulting is more situational than sliding

        //if sliding
        else if (isSliding[2] == 1) 
        {
            //Debug.Log("Sliding");
            rb.velocity = new Vector2(isSliding[1] * facing[0], rb.velocity.y);
            GravityChange();

        }

        //if rolling
        else if (isRolling) 
        {
            GravityChange();
            SlopeCoefficient = rb.velocity.y < 0 ? .7f : 0;
            rb.velocity = new Vector2(rb.velocity.x + (RollDecay * -facing[0]) + (SlopeCoefficient * facing[0]), rb.velocity.y);
        }

        else if (isSwinging[2] > 0)
        {
            rb.gravityScale = 0;
            SwingLERPValue = Mathf.Clamp(SwingLERPValue + (ver * Time.deltaTime), 0, 1);
            this.transform.position = Vector2.Lerp(SwingEndpoints[0], SwingEndpoints[1], SwingLERPValue);

        }
        // if air dashing
        else if (AirDashInfo[1] > 0) 
        {
            rb.velocity = new Vector2(AirDashInfo[4] * facing[0], 0);
        }


        //if just vibing...
        else
        {
            GravityChange();
            currSpeed = isDashing[2] == 1 ? Speeds[2] : Speeds[1];
            {
                //currSpeed adjustment
                //currSpeed depends on if isDashing[2] is equal to 1. If yes, then Speeds[2], if no, then Speeds[1]
                //currSpeed will adjust our horizontal ground speed depending on the boolean above
            }
            if (!inputLock) {
                if (!isGrounded())
                    rb.velocity = new Vector2(Mathf.Clamp(rb.velocity.x + (hor * currSpeed), -TopSpeed, TopSpeed), rb.velocity.y);
                else
                    rb.velocity = new Vector2(hor * currSpeed, rb.velocity.y);

                //The TopSpeed is constantly changing based on Floe's circumstances
                //Regardless, TopSpeed only handles the limit of Floe's Speed, not what her speed currently is
            }
        }


        //GravityChange is being called under specific states and circumstances here.
        //So we can't just call it at the end as a catch-all case
        
    }



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

        inputx = context.ReadValue<Vector2>().x;
        inputy = context.ReadValue<Vector2>().y;

        if (inputx < -Deadzonex) hor = -1;
        else if (inputx > Deadzonex) hor = 1;
        else hor = 0;

        if (inputy < -Deadzoney) ver = -1;
        else if (inputy > Deadzoney) ver = 1;
        else ver = 0;

    }

    public void onJump(InputAction.CallbackContext context)
    {


        //Dropdown
        if (context.started && isCrouching && isOnPlatform && bc.enabled == true)
        {
            //Debug.Log("Dropping Down");
            StartCoroutine(  DisablePlayerCollider(DropdownTimer)  );
        }

        
        else if ((context.canceled))
        {
            isJumping[2] = 0;
            //if you let go of the button, set the isJumping boolean to false
            //This is for high jumping / quick jumping
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

            else //Regular Jump Conditions
            {
                //Swing Jump
                if (isSwinging[2] > 0)
                {
                    //End the swing
                    isSwinging[1] = 0;
                    isSwinging[2] = 0;

                    //Resume the velocity to what it was before going on the swing
                    rb.velocity = SwingSpeed;


                    CooldownStart("Jump");

                }


                //Mount Jump
                if (MountStatus())
                {
                    Mount(null);
                    CooldownStart("Jump");
                }

                //regular jump
                else if ((isGrounded() && !isCrouching) || isClimbing)
                {
                    CooldownStart("Jump");
                    //the other checks are necessary, because you could want to jump even when not grounded
                }

            }
        }



        //Debug.Log(context.phase);
    }

    public void onDash(InputAction.CallbackContext context)
    {
        if (context.started && isGrounded())
        {            

            if (isCrouching && isSliding[2] == 0)
            {   //if you're crouching and not already sliding...
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
        
        if(context.started && isHurt[1] == 0) 
        {
            ChargeFactor[2] = 1;
            ChargeFactor[1] = 0;
            //Make sure ChargeFactor[0] is the same as the amount of different bullets you can charge to

            //play Charge animation
        }

        /*
        if (context.performed && isHurt[1] == 0)
        {
            ChargeFactor[1] = Mathf.Clamp(ChargeFactor[1] + Time.deltaTime, 0, ChargeFactor[0]);
        }
        */
        if (context.canceled && isHurt[1] == 0) //We can shoot because we are not hurt
        {
            if (GetAmmo() > 0)
            {

                currentBullet = (int) ChargeFactor[1];

                AmmoCalc(-1);
                CooldownStart("Shoot");


                GameObject b = Instantiate(
                    BulletReferences[currentBullet].gameObject, 
                    new Vector2(ShootPosition[Aim].transform.position.x, ShootPosition[Aim].transform.position.y),// + BulletReferences[currentBullet].offset, 
                    this.transform.rotation);
                //Adjusted to make the larger bullets spawn further away

                b.GetComponent<BulletClass>().setfacing(facing[0], facing[1]);

                int movSpeed = 0;
                if (walking)
                    movSpeed = (int)Speeds[1];

                //if running, set movSpeed to Speed[2]
                //we want the current bullet to have the same speed as Floe when she's walking, running, or standing still

                b.GetComponent<BulletClass>().setDirection(Aim);

                //b.GetComponent<BulletClass>().setDeterioration(bulletDeterRate[0]);
                b.GetComponent<BulletClass>().setSignature(this, PlayerNumber);

                ChargeFactor[1] = 0;
                ChargeFactor[2] = 0;
            }
        }
    }
    #endregion

    #region Constant Checks
    private void AimMovement()
    {
        if (hor == -facing[0] && !isGrounded())  Aim = 2;

        else if (ver >= Deadzoney) Aim = 3;

        else if (ver <= -Deadzoney) Aim = 4;
        
        else Aim = 1;

    }

    private bool canClimb()
    {   //Handles if you can climb or if you are no longer climbing. 
        //"If you are climbing" is handled in Movement()
        RaycastHit2D ClimbRay = Physics2D.BoxCast(bc.bounds.center, bc.bounds.size, 0, Vector2.down, 0.1f, ClimbLayer);
        bool c = ClimbRay.collider != null;

        if (c == false) isClimbing = false;
        return c;
    }

    private bool isOnWall()
    {
        RaycastHit2D ray = Physics2D.BoxCast(bc.bounds.center, bc.bounds.size, 0, new Vector2(facing[0], 0), 0.1f, wallLayer);
        //BoxCast(collider's centerpoint, size of the collider, rotation of box <at 0 because we dont wanna rotate, we want to check underneath the player - so point down, distance to position the virtual box, the layer we'll be checking for>

        bool y = ray.collider != null;

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

    }

    private void WallMovementCheck()
    {
        //use inputx instead of hor, since hor can be stuck being 0 if you're crouching and cannot refresh while vaulting
        if (isOnWall() && !isGrounded() && Mathf.Sign(inputx) == Mathf.Sign(facing[0]))
        {
            if (isVaulting[1] > 0 || isWallRunning) //if we hit a wall while Vaulting or already WallRUnnning
            {
                //Debug.Log("WallRunning");
                isWallRunning = true;
                rb.velocity = new Vector2(rb.velocity.x, Speeds[1]);
            }
            
            else
            {
                //Debug.Log("WallSliding");
                //AirDashReplenish();
                rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -WallJumpInfo[4], float.MaxValue));
            }
            //clamp makes it so that the min is -wallJumpInfo[4] and the max is the highest float number possible. This makes it so the rb.velocity.y can be no less than -WallJumpInfo[4]
            //We want the speed the player falls at to decrease, thus we need to cap the minimum, hence, limiting how fast the player falls.

        }

        //if (!isOnWall() || isGrounded() || hor != facing[0])
        else isWallRunning = false;
    }

    private void CrouchRollCheck()
    {
        if (ver <= -Deadzoney && isGrounded())
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
        //This function dynamically alters what the current TopSpeed is, in order to give a sense of consistant momentum
        //Note: TopSpeed, not CurrentSpeed

        //Maintaining VaultForce Speed
        if (isVaulting[1] > 0)
        {
            if (!isOnWall())
                TopSpeed = VaultForce.x;
        }

        //Maintaining AirForce Speed and Speed while holding Dash
        if (!isGrounded())
        {
            if (AirDashInfo[1] > 0)
            {
                TopSpeed = AirDashInfo[4];
            }

            else if (isDashing[3] > 0)
            {
                TopSpeed = Speeds[2];
            }
                      
        }

        //Have TopSpeed be the walking speed if grounded.
        else
            TopSpeed = Speeds[1];

    }

    public int GetPlayerNumber()
    {
        return PlayerNumber;
    }

    public void SetSwingParameters(Vector2 v, Vector2 EPA, Vector2 EPB)//, Transform t)
    {
        if (!isGrounded())
        {
            CooldownStart("Swing");
            SwingSpeed = v;

            SwingEndpoints[0] = EPA;
            SwingEndpoints[1] = EPB;

            float A = Vector2.Distance(EPA, this.transform.position);
            float B = Vector2.Distance(this.transform.position, EPB);
            float C = (A + B);

            SwingLERPValue = A / (A + B);

            if( 0.6f < SwingLERPValue  && SwingLERPValue < 0.8f)
            {
                SwingLERPValue = 0.7f;
            }
            if( 0.4f < SwingLERPValue  && SwingLERPValue < 0.6f)
            {
                SwingLERPValue = 0.5f;
            }
            if( 0.2f < SwingLERPValue  && SwingLERPValue < 0.4f)
            {
                SwingLERPValue = 0.3f;
            }
            if( 0.0f < SwingLERPValue  && SwingLERPValue < 0.2f)
            {
                SwingLERPValue = 0.1f;
            }

            //TerrainObject = t;

        }
    }

    public void MaintainSwingParameters(Vector2 EPA, Vector2 EPB)
    {
        SwingEndpoints[0] = EPA;
        SwingEndpoints[1] = EPB;
    }
    public void ResetStates(string ignore = "")
    {
        if( ignore != "Swing" )
            isSwinging[1] = 0;
        if (ignore != "Vault")
            isVaulting[1] = 0;
        if (ignore != "AirDash")
            AirDashInfo[1] = 0;
        if (ignore != "Lock")
            LockCountDown[1] = 0;
        if (ignore != "WallJump")
            WallJumpInfo[1] = 0;
    }
    public void Collect(string s)
    {

        switch (s)
        {

            case "Copper":
                //play Collected Animation for Copper
                Coins[0]++;
                break;

            case "Nickel":
                //play Collected Animation for Nickel
                Coins[1]++;
                break;

            case "Bubble":
                //play Collected Animation for Bubble
                ResetStates();
                AirDashReplenish();
                //rb.AddForce(new Vector2(rb.velocity.x, isJumping[3]), ForceMode2D.Impulse);
                //rb.velocity = Vector2.zero;
                CooldownStart("Jump");
                break;

            case "WaterSmall":
                //Play Healing Animation
                GetComponent<Health>().Heal(3);
    
            break;

            case "WaterLarge":
                //Play Healing Animation
                GetComponent<Health>().Heal(7);
    
            break;
        }

    }



    public void Revive()
    {
        BeingStart(); 
        GetComponent<Health>().HealthRefill();
        Coins[0] = 0;  Coins[1] = 0;
        GameplayManager.GM.PlacePlayer();
        GameplayManager.GM.CameraCont.setTarget(this.transform);
    }
    #endregion
}
