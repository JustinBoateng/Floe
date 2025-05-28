using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletClass : Hitbox
{

    [SerializeField] private float xSpeed;
    [SerializeField] private float ySpeed;    
    [SerializeField] private float xoffset;
    [SerializeField] private float yoffset;    
    [SerializeField] private float xconstraint = 1;
    [SerializeField] private float yconstraint = 1;

    [SerializeField] private int[] facing = new int[2];

    Rigidbody2D rb;

    [SerializeField] float[] LifeExpectancy = {0,0,0};



    [SerializeField] public bool BreaksOnGround;
    [SerializeField] public bool BreaksOnWall;

    [SerializeField] public bool isRidable = false;
    [SerializeField] public BoxCollider2D RideBox;
    [SerializeField] public BoxCollider2D WallBox;
    [SerializeField] public Transform MountSpot;
    [SerializeField] public Being Rider;

    //Squad is used to prevent enemies from hitting other enemies 


    [SerializeField] public string BulletType;
    [SerializeField] public bool isCrashed;
    [SerializeField] public Transform BasePosition;
    [SerializeField] public Vector2 offset;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        this.transform.position = new Vector2(this.transform.position.x + xoffset * facing[0], this.transform.position.y + yoffset * facing[1]);
    }

    // Update is called once per frame
    void Update()
    {
        if(!isCrashed)
        rb.velocity = new Vector2(xSpeed * facing[0] * xconstraint, ySpeed * facing[1] * yconstraint);
        //if(RideBox)
            //RideBox.GetComponent<Rigidbody2D>().velocity = new Vector2(xSpeed * facing[0] * xconstraint, ySpeed * facing[1] * yconstraint);
        //if(WallBox)
            //WallBox.GetComponent<Rigidbody2D>().velocity = new Vector2(xSpeed * facing[0] * xconstraint, ySpeed * facing[1] * yconstraint);
        //Note; the ySpeed going in must always be positive, since facing[1] can be negative in the air.

        //this bullet's facing[0] can never be negative compared to the player's facing[0] IF the player is in the air.
        //hence the different treatment between facing[0] and facing[1]
        //When in the air, a bullet's facing[0] WILL Equal it's Signature's facing[0]

        //Make sure to set the X and Y constraints to 1 if they belong to enemies

        BulletDeterioration();
    }

    public void setfacing(int x, int y)
    {
        facing[0] = x;
        facing[1] = y;
    }
    
    //Used for Basic Bullets
    public void setDirection(int i)
    {
        // x/y speed determines what direction the ammo heads towards
        // x.y offset determines which axis needs to not be offset
        // x/y constraint determines what axis it is NOT allowed to move on while it is moving
        switch (i)
        {

            //To the right
            case 1:
                xSpeed *= 1;
                ySpeed *= 0;
                xconstraint = 1;
                yconstraint = 0;
                yoffset = 0;
                break;

            //To the left
            case 2:
                xSpeed *= -1;
                ySpeed *= 0;
                xconstraint = 1;
                yconstraint = 0;
                yoffset = 0; 
                break;   
                
            //Upwards
            case 3:
                xSpeed *= 0;
                ySpeed *= 1;
                xconstraint = 0;
                yconstraint = 1;
                xoffset = 0; 
                transform.rotation = Quaternion.Euler(new Vector3(0, 0, 90f));
                break;            

            //Downwards
            case 4:
                xSpeed *= 0;
                ySpeed *= 1;
                xconstraint = 0;
                yconstraint = 1;
                xoffset = 0;
                transform.rotation = Quaternion.Euler(new Vector3(0, 0, -90f));
                break;            

        }
    }

    //Used for Return Bullets
    public void setDirection(float x, float y)
    {
        xSpeed = x; ySpeed = y;
        facing[0] = 1;
        facing[1] = 1;
    }

    public void setDeterioration(float rate)
    {
        LifeExpectancy[0] = 100;
        LifeExpectancy[1] = 0;
        LifeExpectancy[2] = rate;
    }

    //for enemies
    /*
    public void setSignature(Being B, string S)
    {
        Signature = B;
        Squad = S;
    }
    */
    //for players
    public void setSignature(Being B, int i)
    {
        Signature = B;
        SignatureNumber = i;
    }
    private void BulletDeterioration()
    {
        //LifeExpectancy[1] += LifeExpectancy[2];
        LifeExpectancy[1] += Time.deltaTime; ;
        if (LifeExpectancy[1] >= LifeExpectancy[0])
        {
            Signature.AmmoCalc(1);
            Crash();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log(collision.name);

        //Check for collision
        if ((collision.tag == "Player" || collision.tag == "Enemy") && collision.name != Signature.name && collision.GetComponent<Being>().Squad != Squad)
        {
            //Debug.Log(GameplayManager.GM);
            //Debug.Log(SignatureNumber);
            //Debug.Log(Power);
            //Debug.Log(Signature.name + " Hit: " + collision.name);

            //GameplayManager.GM.ScoreUpdate(SignatureNumber, Power);

            if (collision.name != Signature.name && collision.GetComponent<Being>().Squad != Squad)
                //prevent a character from hitting someone else in their squad (prevent same team hitting)
                //if (collision.GetComponent<Being>().Squad != Squad)
                { 
                    collision.GetComponent<Health>().TakeDamage(Power);

                    if (!collision.GetComponent<Health>().isDown())
                    {
                        if (collision.tag == "Enemy")
                        {
                            collision.GetComponent<EnemyAI>().SetHitstun(Power, Signature.gameObject);

                            //collision.GetComponent<Being>().setVelocity(Vector2.zero);
                            if (collision.GetComponent<EnemyAI>().getArmor() <= 0)
                            {
                                collision.GetComponent<Being>().setVelocity(Vector2.zero);
                                collision.GetComponent<Being>().setVelocity(new Vector2(Knockback.x * facing[0], Knockback.y));
                                //collision.GetComponent<EnemyAI>().resetArmor();
                                //set knockback, THEN reset the armor
                            }
                            //Debug.Log("Enemy Destroyed this");

                        }

                        if (collision.tag == "Player")
                        {
                            collision.GetComponent<Being>().setVelocity(Vector2.zero);
                            collision.GetComponent<Being>().setVelocity(new Vector2(Knockback.x * facing[0], Knockback.y));
                        }

                        //Debug.Log("Player did this if Enemy Didn't");
                    }
                    Crash();
                }

            
        }

        //Mount Mechanic
        if ((collision.tag == "Player" || collision.tag == "Enemy") && collision.name == Signature.name && isRidable)
        {
            if (   !collision.GetComponent<Being>().isGrounded()
                && collision.GetComponent<Being>().MountStatus() != this
                && collision.GetComponent<Rigidbody2D>().velocity.y < 0)
            {
                //collision.transform.SetParent(this.transform);
                Rider = collision.GetComponent<Being>();
                collision.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                collision.GetComponent<Being>().Mount(this.gameObject);
            }
        }

        //check if ySpeed != 0 since you dont want it to crash on the floor if it's not aimed directly at the floor
        if ((collision.tag == "Ground" && BreaksOnGround && ySpeed != 0) || (collision.tag == "Wall" && BreaksOnWall) || collision.tag == "Switch" )
        {
            //Debug.Log(GameplayManager.GM);
            //Debug.Log(SignatureNumber);
            //Debug.Log(Power);            
            //Debug.Log("Ground Destroyed this");
            Crash();
        }

        if (collision.tag == "Bullet")
        {

            //Debug.Log("Bullet Destroyed this");
            if (collision.GetComponent<BulletClass>().Signature != Signature)
            {
                //Debug.Log("Because Signatures don't match");
                if (collision.GetComponent<BulletClass>().Power >= Power)
                    Crash();
            }

        }
        //Destroy(this.gameObject);

        if (collision.tag == "Breakable")
        {
            collision.GetComponent<Breakable>().Hit(Power);
            //Debug.Log("Breakable Destroyed this");
            Crash();
        }

        //else Debug.Log("Yeah, it aint in here dawg.");
    }


    //Bullet Mount Mechanic
    public void OnTriggerStay2D(Collider2D collision)
    {
        if(MountSpot)
        if ((collision.tag == "Player" || collision.tag == "Enemy") 
                && collision.name == Signature.name 
                && collision.GetComponent<Being>().MountStatus() == this.gameObject)
        {
            //float x = collision.GetComponent<Rigidbody2D>().velocity.x;
            //float y = collision.GetComponent<Rigidbody2D>().velocity.y;

            //collision.GetComponent<Rigidbody2D>().velocity = new Vector2(x + rb.velocity.x, y + rb.velocity.y);
            collision.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            collision.transform.position = this.MountSpot.transform.position;
            Rider = collision.GetComponent<Being>();
        }
    } 
    
    public void OnTriggerExit2D(Collider2D collision)
    {
        Rider = null;
        //if ((collision.tag == "Player" || collision.tag == "Enemy") && collision.name == Signature.name)
        //{
        //    collision.transform.SetParent(null);
        //}
    }
    public void Crash()
    {

        //play Animation coroutine

        //Debug.Log("Bullets Crashed");

        //Dismount
        if (Rider)
        {
            Rider.Mount(null);
            //GetComponentInChildren<Being>().transform.parent = null;
        }

        //How is the bullet destroyed?
        switch (BulletType)
        {
            case "Basic":
                Signature.AmmoCalc(1);

                Destroy(this.gameObject);
                break;

            case "Return":
                isCrashed = true;
                this.gameObject.SetActive(false);

                if(BasePosition)
                    this.transform.position = BasePosition.transform.position;

                LifeExpectancy[1] = 0;
                break;
        }
    }

    public void setBasePosition(Transform bP)
    {
        BasePosition = bP;
    }
}
