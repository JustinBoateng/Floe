using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletClass : Hitbox
{

    [SerializeField] private float xSpeed;
    [SerializeField] private float ySpeed;
    [SerializeField] private int[] facing = new int[2];
    Rigidbody2D rb;

    [SerializeField] float[] LifeExpectancy = {0,0,0};



    [SerializeField] public bool BreaksOnGround;
    [SerializeField] public bool BreaksOnWall;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        rb.velocity = new Vector2(xSpeed * facing[0], ySpeed * facing[1]);
        //Note; the ySpeed going in must always be positive, since facing[1] can be negative in the air.

        //this bullet's facing[0] can never be negative compared to the player's facing[0] IF the player is in the air.
        //hence the different treatment between facing[0] and facing[1]
        //When in the air, a bullet's facing[0] WILL Equal it's Signature's facing[0]
 


        BulletDeterioration();
    }

    public void setfacing(int x, int y)
    {
        facing[0] = x;
        facing[1] = y;
    }
    
    public void setDirection(int i)
    {
        switch (i)
        {

            case 1:
                xSpeed *= 1;
                ySpeed *= 0;
                break;
            case 2:
                xSpeed *= -1;
                ySpeed *= 0;
                break;            
            case 3:
                xSpeed *= 0;
                ySpeed *= 1;
                break;            
            case 4:
                xSpeed *= 0;
                ySpeed *= 1;
                break;            

        }
    }

    public void setDeterioration(float rate)
    {
        LifeExpectancy[0] = 100;
        LifeExpectancy[1] = 0;
        LifeExpectancy[2] = rate;
    }

    public void setSignature(Being B, int i)
    {
        Signature = B;
        SignatureNumber = i;
    }
    private void BulletDeterioration()
    {
        LifeExpectancy[1] += LifeExpectancy[2];
        if (LifeExpectancy[1] >= LifeExpectancy[0])
        {
            Signature.AmmoCalc(1);
            Crash();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if((collision.tag == "Player" || collision.tag == "Enemy") && collision.name != Signature.name)
        {
            //Debug.Log(GameplayManager.GM);
            //Debug.Log(SignatureNumber);
            //Debug.Log(Power);
            //Debug.Log(Signature.name + " Hit: " + collision.name);

            //GameplayManager.GM.ScoreUpdate(SignatureNumber, Power);

            collision.GetComponent<Health>().TakeDamage(Power);
            

            if(collision.tag == "Enemy")
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
            }

            Crash();
        } 
        
        if((collision.tag == "Ground"  && BreaksOnGround) || (collision.tag == "Wall" && BreaksOnWall))
        {
            //Debug.Log(GameplayManager.GM);
            //Debug.Log(SignatureNumber);
            //Debug.Log(Power);            
            Crash();
        }

        if(collision.tag == "Bullet")
        {
            if(collision.GetComponent<BulletClass>().Signature != Signature)
                if (collision.GetComponent<BulletClass>().Power >= Power)
                    Crash();
        }
        //Destroy(this.gameObject);

    }

    private void Crash()
    {
        //play Animation coroutine
        Signature.AmmoCalc(1);
        Destroy(this.gameObject);
    }

}
