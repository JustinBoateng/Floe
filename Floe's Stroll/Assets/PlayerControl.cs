using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControl : MonoBehaviour
{

    Rigidbody2D rb;
    [SerializeField] BoxCollider2D[] ColliderChecks = new BoxCollider2D[2];
    //0: BaseCollider

    int StrictHor;

    [SerializeField] bool Grounded;
    [SerializeField] float JumpForce = 10;
    [SerializeField] float[] Speeds;
    //0: Fall, 1: Walk, 2: Run


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        ColliderChecks[0] = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(Input.GetAxis("Horizontal"));
        //if (Input.GetAxis("Horizontal") > 0) StrictHor = 1;
        //else if (Input.GetAxis("Horizontal") < 0) StrictHor = -1;
        //else if (StrictHor < Input.GetAxis("Horizontal") && Input.GetAxis("Horizontal") < 0) StrictHor = 0;
        //else if (StrictHor > Input.GetAxis("Horizontal") && Input.GetAxis("Horizontal") > 0) StrictHor = 0;
        //else StrictHor = 0;
        //Debug.Log(StrictHor);

        //rb.velocity = new Vector2(StrictHor * Speeds[1], rb.velocity.y);
        Debug.Log(Input.GetAxisRaw("Horizontal"));
        rb.velocity = new Vector2(Input.GetAxisRaw("Horizontal") * Speeds[1], rb.velocity.y);

        
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
}
