using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI : Being
{
    // Start is called before the first frame update
    void Start()
    {
      
    }

    protected void facingCalc(int i)
    {
        if (i != 0)
            facing[0] *= i;
        transform.localScale = new Vector3(facing[0], 1, 1);
    }

    private bool isGrounded()
    {
        RaycastHit2D ray3 = Physics2D.BoxCast(GroundCheckLocation.transform.position, new Vector2(bc.size.x, 1), 0, Vector2.down, 0, groundLayer);
        RaycastHit2D ray4 = Physics2D.BoxCast(GroundCheckLocation.transform.position, new Vector2(bc.size.x, 1), 0, Vector2.down, 0, platformLayer);
    
        Debug.DrawRay(GroundCheckLocation.transform.position, Vector2.down, Color.red, 1);

        bool y = ray3.collider != null || ray4.collider != null;


        return y;
    }
}
