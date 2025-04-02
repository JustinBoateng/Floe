using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blade : Hitbox
{ 

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ((collision.tag == "Player" || collision.tag == "Enemy") && collision.name != Signature.name)
        {
            //Debug.Log(GameplayManager.GM);
            //Debug.Log(SignatureNumber);
            //Debug.Log(Power);
            //GameplayManager.GM.ScoreUpdate(SignatureNumber, Power);

            int f = collision.GetComponent<Being>().facing[0] * -1;

            collision.GetComponent<Health>().TakeDamage(Power);

            Debug.Log(Signature.name + " Hit: " + collision.name);

            collision.GetComponent<Being>().setVelocity(Vector2.zero);
            collision.GetComponent<Being>().setVelocity(new Vector2(Knockback.x * f, Knockback.y));
        }

        if (collision.tag == "Bullet")
        {
            if (collision.GetComponent<BulletClass>().Signature != Signature) //if the hitboxes come from different people
                if (collision.GetComponent<BulletClass>().Power >= Power)
                    Crash();
        }
        //Destroy(this.gameObject);


    }
    private void Crash()
    {
        //play Animation coroutine
        //Signature.AmmoCalc(1);
        gameObject.SetActive(false);
    }

}
