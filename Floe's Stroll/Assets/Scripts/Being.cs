using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Being : MonoBehaviour
{

    [SerializeField] protected Transform GroundCheckLocation;
    [SerializeField] protected Transform MountCheckLocation;
    [SerializeField] protected BoxCollider2D bc;
    [SerializeField] protected Rigidbody2D rb;
    //[SerializeField] BoxCollider2D bc;// = new BoxCollider2D[2];

    [SerializeField] protected LayerMask platformLayer;
    [SerializeField] protected LayerMask groundLayer;
    [SerializeField] protected LayerMask wallLayer;
    [SerializeField] protected LayerMask mountLayer;

    [SerializeField] protected bool isOnPlatform;

    [SerializeField] protected float[] gravityAdjust;
    //baserising, currrising, falling, 

    int[] Ammo = new int[2];

    string Name;

    [SerializeField] public int[] facing = new int[2];

    public void SetMaxAmmo(int i)
    {
        Ammo[0] = i; 
        Ammo[1] = Ammo[0] = i;
    }

    public int GetAmmo()
    {
        return Ammo[1];
    }

    public void AmmoCalc(int i)
    {
        Ammo[1] = Mathf.Clamp(Ammo[1] + i, 0, Ammo[0]);
    }

    protected bool isGrounded()
    {
        //RaycastHit2D ray = Physics2D.BoxCast(ColliderCheck.bounds.center, ColliderCheck.bounds.size, 0, Vector2.down, 0.1f, groundLayer);
        //RaycastHit2D ray2 = Physics2D.BoxCast(ColliderCheck.bounds.center, ColliderCheck.bounds.size, 0, Vector2.down, 0.1f, platformLayer);
        RaycastHit2D ray3 = Physics2D.BoxCast(GroundCheckLocation.transform.position, new Vector2(bc.size.x, 1), 0, Vector2.down, 0, groundLayer);
        RaycastHit2D ray4 = Physics2D.BoxCast(GroundCheckLocation.transform.position, new Vector2(bc.size.x, 1), 0, Vector2.down, 0, platformLayer);
        //BoxCast(collider's centerpoint, size of the collider, rotation of box <at 0 because we dont wanna rotate, we want to check underneath the player - so point down, distance to position the virtual box, the layer we'll be checking for>

        Debug.DrawRay(GroundCheckLocation.transform.position, Vector2.down, Color.red, 1);

        //bool y = ray.collider != null || ray2.collider != null;
        bool y = ray3.collider != null || ray4.collider != null;
        isOnPlatform = ray4.collider != null;
        //Debug.Log(y);

        return y;
        //returns true if the ray hits a collider in the groundLayer.
    }

    protected void GravityChange()
    {
        if (!isGrounded())
        {
            if (rb.velocity.y > 0)
            {
                gravityAdjust[1] = Mathf.Clamp(gravityAdjust[1] + Time.deltaTime, gravityAdjust[1], gravityAdjust[2]);
                rb.gravityScale = gravityAdjust[1];
            }
            else if (rb.velocity.y <= 0)
            {
                gravityAdjust[1] = gravityAdjust[0];
                rb.gravityScale = gravityAdjust[2];
            }
        }

        else
            rb.gravityScale = 1;
    }

}
