using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyOfWater : MonoBehaviour
{

    [SerializeField] float gravityStrength;
    [SerializeField] float windStrength;
    [SerializeField] Vector2 windDirection;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.GetComponent<Rigidbody2D>())
        collision.GetComponent<Rigidbody2D>().gravityScale = gravityStrength;
    }  
    
    public void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.GetComponent<Rigidbody2D>())
        {
            collision.GetComponent<Rigidbody2D>().gravityScale = gravityStrength;

            collision.GetComponent<Rigidbody2D>().AddForce(windDirection);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<Rigidbody2D>())
            collision.GetComponent<Rigidbody2D>().gravityScale = 1f;
        
        if (collision.GetComponent<Being>())
            collision.GetComponent<Being>().GravityChange();

    }
}
