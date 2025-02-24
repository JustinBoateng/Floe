using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletClass : MonoBehaviour
{

    [SerializeField] private float xSpeed;
    [SerializeField] private float ySpeed;
    [SerializeField] private int[] facing = new int[2];
    Rigidbody2D rb;

    [SerializeField] float[] LifeExpectancy = {0,0,0};

    [SerializeField] public Being Signature;
    [SerializeField] public int SignatureNumber;
    [SerializeField] public int Power;
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
    
    public void setSpeed(float x, float y)
    {
        xSpeed = x;
        ySpeed= y;
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
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player" && collision.name != Signature.name)
        {
            //Debug.Log(GameplayManager.GM);
            //Debug.Log(SignatureNumber);
            //Debug.Log(Power);
            GameplayManager.GM.ScoreUpdate(SignatureNumber, Power);
            Debug.Log(Signature.name + " Hit: " + collision.name);
            Signature.AmmoCalc(1);
            Destroy(this.gameObject);
        } 
        
        if(collision.tag == "Ground" || collision.tag == "Wall")
        {
            //Debug.Log(GameplayManager.GM);
            //Debug.Log(SignatureNumber);
            //Debug.Log(Power);
            Signature.AmmoCalc(1);
            Destroy(this.gameObject);
        }

        //Destroy(this.gameObject);

    }
}
