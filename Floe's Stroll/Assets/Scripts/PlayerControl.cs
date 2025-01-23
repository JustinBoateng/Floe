using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControl : MonoBehaviour
{

    Rigidbody2D rb;
    [SerializeField] BoxCollider2D[] ColliderChecks = new BoxCollider2D[2];
    private Animator anim;
    //[SerializeField] CapsuleCollider2D[] ColliderChecks = new CapsuleCollider2D[2];
    //0: BaseCollider

    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask wallLayer;

    //[SerializeField] bool Grounded;
    [SerializeField] bool walking;
    [SerializeField] float JumpForce = 10;
    float currSpeed;
    [SerializeField] float[] Speeds;
    //0: Fall, 1: Walk, 2: Run

    [SerializeField] float[] WallJumpInfo;
    //0: Max, 1: Curr, 2: Amount to push away from Wall, 3: Amount to be pushed upwards, 4: WallSliding Speed, 5: Player inputting left or right does nothing for this long

    private float horizontalInput;

    [SerializeField] public int[] facing { get; private set; } = new int[2];
    //0: horizontal facing, 1: vertical facing
    //instead of doing an entire get and set function, we can just do this.


    [SerializeField] float[] isShooting = new float[2]; //used for the shooting animation
    //0: Max, 1: curr
    [SerializeField] float[] isDashing = new float[3];
    //0: Max, 1: Curr, 2: isDashing
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
        ColliderChecks[0] = GetComponent<BoxCollider2D>();
        anim = GetComponent<Animator>();

        //ColliderChecks[0] = GetComponent<CapsuleCollider2D>();
        facing[0] = 1;
        facing[1] = 0;
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(Input.GetAxisRaw("Horizontal"));

        facingCalc();
        horizontalInput = Input.GetAxisRaw("Horizontal");
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

        }
    }
    private void CooldownCalc()
    {

        isShooting[1] = Mathf.Clamp(isShooting[1] - Time.deltaTime, 0, isShooting[0]);
        WallJumpInfo[1] = Mathf.Clamp(WallJumpInfo[1] - Time.deltaTime, 0, WallJumpInfo[0]); ;
        //we want to do -= instead of += due to being easier to work with the animator. We can change states depending on if the current value of something is greater than 0 instead of less than some arbitrary number
        
    }

    private void AnimCalc()
    {
        //Set Animator Parameters
        anim.SetBool("Walking", walking);
        anim.SetFloat("Dashing", isDashing[2]);

        anim.SetBool("Grounded", isGrounded());
        
        anim.SetFloat("Vertical Speed", rb.velocity.y);
        anim.SetFloat("isShooting", isShooting[1]);
    }
    private void Movement()
    {
        walking = Input.GetAxisRaw("Horizontal") != 0;

        if (WallJumpInfo[1] <= 0)
        {
            rb.gravityScale = 1;
            currSpeed = isDashing[2] == 1 ? Speeds[2] : Speeds[1];
            //currSpeed equals if isDash[2] is equal to 1. If yes, then Speeds[2], if no, then Speeds[1]

            rb.velocity = new Vector2(Input.GetAxisRaw("Horizontal") * currSpeed, rb.velocity.y);

        }

        WallSlide();


    }
    private void facingCalc()
    {
        if (isGrounded())
        {
            if (Input.GetAxisRaw("Horizontal") > 0) facing[0] = 1;
            else if (Input.GetAxisRaw("Horizontal") < 0) facing[0] = -1;
        }
        transform.localScale = new Vector3(facing[0], 1, 1);
        Debug.Log(facing[0]);
    }


    private void WallSlide()
    {
        if(isOnWall() && !isGrounded() && horizontalInput == facing[0])
        {
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -WallJumpInfo[4], float.MaxValue));
            //clamp makes it so that the min is -wallJumpInfo[4] and the max is the highest float number possible. This makes it so the rb.velocity.y can be no less than -WallJumpInfo[4]
            //We want the speed the player falls at to decrease, thus we need to cap the minimum, hence, limiting how fast the player falls.

        }
    }

    public void onJump(InputAction.CallbackContext context)
    {

        if (context.started) 
        {
            if (WallJumpInfo[1] <=0)
            {
                //regular jump
                if (isGrounded())
                {
                    Debug.Log("Jumping");
                    //rb.AddForce(new Vector2(0,JumpForce));
                    rb.velocity = new Vector2(rb.velocity.x, JumpForce);
                    anim.SetTrigger("Jump");
                }
 
                //Wall Jump
                else if (isOnWall() && !isGrounded())
                {
                    rb.gravityScale = 0;
                    rb.velocity = Vector2.zero;

                    if (horizontalInput == 0)
                    {
                        rb.velocity = new Vector2(-Mathf.Sign(facing[0]) * WallJumpInfo[2] * 2 * (currSpeed / 4), WallJumpInfo[3]);
                        //jump farther if you're not holding towards the wall
                    }

                    else
                    {
                        rb.velocity = new Vector2(-Mathf.Sign(facing[0]) * WallJumpInfo[2] * (currSpeed / 4), WallJumpInfo[3]);
                    }

                    facing[0] = -facing[0];
                    //transform.localScale = new Vector3(-1 * facing[0], transform.localScale.y, transform.localScale.z);
                    CooldownStart("WallJump");
                    
                }
            }
        }
    }

    public void onDash(InputAction.CallbackContext context)
    {
        if (context.started && isGrounded())
        {
            CooldownStart("Dash");
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

    private bool isGrounded()
    {
        RaycastHit2D ray = Physics2D.BoxCast(ColliderChecks[0].bounds.center, ColliderChecks[0].bounds.size, 0, Vector2.down, 0.1f, groundLayer);
        //BoxCast(collider's centerpoint, size of the collider, rotation of box <at 0 because we dont wanna rotate, we want to check underneath the player - so point down, distance to position the virtual box, the layer we'll be checking for>

        return ray.collider != null;
        //returns true if the ray hits a collider in the groundLayer.
    }
    
    private bool isOnWall()
    {
        RaycastHit2D ray = Physics2D.BoxCast(ColliderChecks[0].bounds.center, ColliderChecks[0].bounds.size, 0, new Vector2(facing[0], 0), 0.1f, wallLayer);
        //BoxCast(collider's centerpoint, size of the collider, rotation of box <at 0 because we dont wanna rotate, we want to check underneath the player - so point down, distance to position the virtual box, the layer we'll be checking for>

        return ray.collider != null;
        //returns true if the ray hits a collider in the groundLayer.
    }
}
