using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI : MonoBehaviour
{

    [SerializeField] int facing = 1;
    [SerializeField] Transform GroundCheckLocation;
    [SerializeField] Transform MountCheckLocation;
    [SerializeField] private BoxCollider2D bc;
    [SerializeField] private Rigidbody2D rb;


    [SerializeField] private LayerMask platformLayer;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private LayerMask mountLayer;

    //isGrounded


    // Start is called before the first frame update
    void Start()
    {
        facing = 1;           
    }

    // Update is called once per frame
    void Update()
    {
        //facing = hor >= 0 ? 1 : -1;

        

    }

    private void facingCalc()
    {
        transform.localScale = new Vector3(facing, 1, 1);
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
