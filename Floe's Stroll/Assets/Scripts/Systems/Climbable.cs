using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Climbable : MonoBehaviour
{
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
        //if (collision.GetComponent<Being>())
        /*if (collision.tag == "Player")
        {
            Debug.Log("Entered Platform");
            collision.transform.SetParent(this.transform);
        }
        */
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        //if (collision.GetComponent<Being>())
        if (collision.tag == "Player")
        {
            //Debug.Log("Exited Platform");
            collision.transform.SetParent(null);
        }
    }

    public void StartClimb(Transform t)
    {
        t.SetParent(this.transform);
    }

    public void EndClimb(Transform t)
    {
        t.SetParent(null);
    }
}
