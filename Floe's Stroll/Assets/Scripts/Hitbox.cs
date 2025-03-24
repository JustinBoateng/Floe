using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbox : MonoBehaviour
{

    [SerializeField] public Being Signature;
    [SerializeField] public int SignatureNumber;
    [SerializeField] public int Power;
    [SerializeField] public Vector2 Knockback;

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
        if ((collision.tag == "Player" || collision.tag == "Enemy"))
        {
            //Debug.Log(GameplayManager.GM);
            //Debug.Log(SignatureNumber);
            //Debug.Log(Power);
            //GameplayManager.GM.ScoreUpdate(SignatureNumber, Power);

            int f = collision.GetComponent<Being>().facing[0] * -1;

            collision.GetComponent<Health>().TakeDamage(Power);

            //Debug.Log(Signature.name + " Hit: " + collision.name);

            collision.GetComponent<Being>().setVelocity(Vector2.zero);
            collision.GetComponent<Being>().setVelocity(new Vector2(Knockback.x * f, Knockback.y));
        }

        if (collision.tag == "Bullet")
        {
            //if (collision.GetComponent<BulletClass>().Signature != Signature) //if the hitboxes come from different people
            //if (collision.GetComponent<BulletClass>().Power >= Power)
            //Crash();
            Destroy(collision.gameObject);
        }
        //Destroy(this.gameObject);


    }
}
