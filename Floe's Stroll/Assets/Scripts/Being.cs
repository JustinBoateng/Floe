using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Being : MonoBehaviour
{

    [SerializeField] protected Transform GroundCheckLocation;
    [SerializeField] protected Transform MountCheckLocation;
    [SerializeField]protected BoxCollider2D bc;
    [SerializeField] protected Rigidbody2D rb;
    [SerializeField] protected Animator anim;

    [SerializeField] protected LayerMask platformLayer;
    [SerializeField] protected LayerMask groundLayer;
    [SerializeField] protected LayerMask wallLayer;
    [SerializeField] protected LayerMask mountLayer;
    [SerializeField] protected LayerMask beingLayer;
    [SerializeField] protected LayerMask PlayerLayer;

    [SerializeField] protected bool isOnPlatform;
    [SerializeField] protected bool isOnBeing;
    [SerializeField] protected bool isOnPlayer;

    [SerializeField] protected float[] gravityAdjust;
    //baserising, currrising, falling, 

    [SerializeField] int[] Ammo = new int[2];

    string Name;

    [SerializeField] public int[] facing = new int[2];


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        bc = GetComponent<BoxCollider2D>();
        anim = GetComponent<Animator>();

        bc.enabled = true;
        facing[0] = 1;
    }

    public void SetMaxAmmo(int i)
    {
        Ammo[0] = i;
        Ammo[1] = Ammo[0] = i;
    }

    public int GetAmmo()
    {
        return Ammo[1];
    }

    public void AmmoCalc(int i)
    {
        Ammo[1] = Mathf.Clamp(Ammo[1] + i, 0, Ammo[0]);
    }

    protected bool isGrounded()
    {
        RaycastHit2D ray3 = Physics2D.BoxCast(GroundCheckLocation.transform.position, new Vector2(bc.size.x, 1), 0, Vector2.down, 0, groundLayer);
        RaycastHit2D ray4 = Physics2D.BoxCast(GroundCheckLocation.transform.position, new Vector2(bc.size.x, 1), 0, Vector2.down, 0, platformLayer);
        RaycastHit2D ray5 = Physics2D.BoxCast(GroundCheckLocation.transform.position, new Vector2(bc.size.x, 1), 0, Vector2.down, 0, beingLayer);


        Debug.DrawRay(GroundCheckLocation.transform.position, Vector2.down, Color.red, 1);

        RaycastHit2D PRay1 = Physics2D.BoxCast(GroundCheckLocation.transform.position, new Vector2(bc.size.x, 1), 0, Vector2.down, 0, PlayerLayer);

        isOnBeing = (ray5.collider != null) && (ray5.collider.name != this.name);
        isOnPlatform = ray4.collider != null;
        isOnPlayer = PRay1.collider;// != null && PRay2.collider && PRay3.collider != null;

        bool y = ray3.collider != null || isOnPlatform || isOnBeing || isOnPlayer;

        return y;
        //returns true if the ray hits a collider in the groundLayer.
    }

    protected void GravityChange()
    {
        if (!isGrounded())
        {
            if (rb.velocity.y > 0)
            {
                gravityAdjust[1] = Mathf.Clamp(gravityAdjust[1] + Time.deltaTime, gravityAdjust[1], gravityAdjust[2]);
                rb.gravityScale = gravityAdjust[1];
            }
            else if (rb.velocity.y <= 0)
            {
                gravityAdjust[1] = gravityAdjust[0];
                rb.gravityScale = gravityAdjust[2];
            }
        }

        else
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
        anim = GetComponent<Animator>();

        bc.enabled = true;
        facing[0] = 1;
    }
    //use this just in case a class that derives from this one has its own Start Function and you don't want these steps to be overwritten
}
