using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControl : MonoBehaviour
{

    Rigidbody2D rb;
    [SerializeField] BoxCollider2D[] ColliderChecks = new BoxCollider2D[2];
    //0: BaseCollider


    [SerializeField] bool Grounded;
    [SerializeField] float JumpForce = 10;
    [SerializeField] float[] Speeds;
    //0: Fall, 1: Walk, 2: Run

    [SerializeField] int[] facing = new int[2];
    //0: horizontal facing, 1: vertical facing
    
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
        facing[0] = 1;
        facing[1] = 0;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(Input.GetAxisRaw("Horizontal"));

        facingCalc();
        rb.velocity = new Vector2(Input.GetAxisRaw("Horizontal") * Speeds[1], rb.velocity.y);
        
    }

    private void facingCalc()
    {
        if (Input.GetAxisRaw("Horizontal") > 0) facing[0] = 1;
        else if (Input.GetAxisRaw("Horizontal") < 0) facing[0] = -1;

        transform.localScale = new Vector3(facing[0], 1, 1);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground") { Grounded = true; }
    }
    

    private void OnCollisionExit2D(Collision2D collission)
    {
        if (collission.gameObject.tag == "Ground") { Grounded = false; }
    }

    public void onJump(InputAction.CallbackContext context)
    {
        if(context.started)
            if(Grounded)
            {
                Debug.Log("Jumping");
                //rb.AddForce(new Vector2(0,JumpForce));
                rb.velocity = new Vector2(rb.velocity.x,JumpForce);
            }
    }

    public void onShoot(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            GameObject b = Instantiate(BulletReferences[currentBullet].gameObject, ShootPosition.transform.position, this.transform.rotation);
            b.GetComponent<BulletClass>().setfacing(facing[0], facing[1]);
            b.GetComponent<BulletClass>().setSpeed(bulletSpeed[0], bulletSpeed[1]);
            b.GetComponent<BulletClass>().setDeterioration(bulletDeterRate[0]);
        }
    }

}
