using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Being : MonoBehaviour
{

    [SerializeField] protected Transform GroundCheckLocation;
    [SerializeField] protected Transform MountCheckLocation;
    [SerializeField] protected BoxCollider2D bc;
    [SerializeField] protected SpriteRenderer sr;
    [SerializeField] protected Rigidbody2D rb;
    [SerializeField] protected Animator anim;

    [SerializeField] protected LayerMask platformLayer;
    [SerializeField] protected LayerMask groundLayer;
    [SerializeField] protected LayerMask wallLayer;
    [SerializeField] protected LayerMask ClimbLayer;
    [SerializeField] protected LayerMask WaterfallLayer;
    [SerializeField] protected LayerMask beingLayer;
    [SerializeField] protected LayerMask underwaterLayer;
    [SerializeField] protected LayerMask PlayerLayer;
    //[SerializeField] protected LayerMask InvulnurableLayer;
    //[SerializeField] protected LayerMask InvincibleLayer;

    [SerializeField] protected bool isOnPlatform;
    [SerializeField] protected bool isOnBeing;
    [SerializeField] protected bool isOnPlayer;
    [SerializeField] protected bool isUnderwater;
    [SerializeField] protected GameObject currentMount;

    [SerializeField] protected float[] gravityAdjust;
    //baserising, currrising, falling, 

    [SerializeField] int[] AmmoCount = new int[2];
    //0: Max Amount, 1: Current Amount
    
    //string Name;
    [SerializeField] public string Squad;

    [SerializeField] public int[] facing = new int[2];


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        bc = GetComponent<BoxCollider2D>();
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();

        bc.enabled = true;
        facing[0] = 1;
    }

    public void SetMaxAmmo(int i)
    {
        AmmoCount[0] = i;
        AmmoCount[1] = AmmoCount[0] = i;
    }

    public int GetAmmo()
    {
        return AmmoCount[1];
    }

    public void AmmoCalc(int i)
    {
        AmmoCount[1] = Mathf.Clamp(AmmoCount[1] + i, 0, AmmoCount[0]);
    }

    public bool isGrounded()
    {
        RaycastHit2D ray3 = Physics2D.BoxCast(GroundCheckLocation.transform.position, new Vector2(bc.size.x, 1), 0, Vector2.down, 0, groundLayer);
        RaycastHit2D ray4 = Physics2D.BoxCast(GroundCheckLocation.transform.position, new Vector2(bc.size.x, 1), 0, Vector2.down, 0, platformLayer);
        RaycastHit2D ray5 = Physics2D.BoxCast(GroundCheckLocation.transform.position, new Vector2(bc.size.x, 1), 0, Vector2.down, 0, beingLayer);
        RaycastHit2D ray6 = Physics2D.BoxCast(GroundCheckLocation.transform.position, new Vector2(bc.size.x, 1), 0, Vector2.down, 0, underwaterLayer);


        Debug.DrawRay(GroundCheckLocation.transform.position, Vector2.down, Color.red, 1);

        RaycastHit2D PRay1 = Physics2D.BoxCast(GroundCheckLocation.transform.position, new Vector2(bc.size.x, 1), 0, Vector2.down, 0, PlayerLayer);

        isOnBeing = (ray5.collider != null) && (ray5.collider.name != this.name);
        isOnPlatform = ray4.collider != null;
        isOnPlayer = PRay1.collider;// != null && PRay2.collider && PRay3.collider != null;
        isUnderwater = ray6.collider != null;

        bool y = ray3.collider != null || isOnPlatform || isOnBeing || isOnPlayer || isUnderwater;

        return y;
        //returns true if the ray hits a collider in the groundLayer.
    }

    public void GravityChange()
    {
        //this occurs if you're in the air
        if (!isGrounded())
        {
            if (rb.velocity.y > 0)
            {
                gravityAdjust[1] = Mathf.Clamp(gravityAdjust[1] + Time.deltaTime, gravityAdjust[1], gravityAdjust[2]);
                rb.gravityScale = gravityAdjust[1]; //current gravity is gA[1], which rises up to gA[2] (which is being used for convenience's sake)
            }
            else if (rb.velocity.y <= 0)
            {
                gravityAdjust[1] = gravityAdjust[0]; // set gA[1] back to normal (gA[0])
                rb.gravityScale = gravityAdjust[2]; //current gravity is original gravity 
            }
        }

        else
            if(!isUnderwater)
                rb.gravityScale = 1;
    }

    public void setVelocity(Vector2 V)
    {
        rb.velocity = V;
    }


    public void BeingStart()
    {
        rb = GetComponent<Rigidbody2D>();
        bc = GetComponent<BoxCollider2D>();
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();

        bc.enabled = true;
        facing[0] = 1;
    }
    //use this just in case a class that derives from this one has its own Start Function and you don't want these steps to be overwritten

    public void Mount(GameObject b)
    {
        currentMount = b;
        if (b == false)
        {
            rb.gravityScale = 1;
        }

        else
        {
            rb.gravityScale = 0;
        }
    }

    public GameObject MountStatus()
    {
        return currentMount;
    }
}
