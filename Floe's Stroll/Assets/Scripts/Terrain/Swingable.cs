using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Swingable : MonoBehaviour
{

    [SerializeField] GameObject[] Endpoints;
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
        if (collision.tag == "Player") 
        {
            collision.GetComponent<PlayerControl>().SetSwingParameters(collision.GetComponent<PlayerControl>().VelocityRef, Endpoints[0].transform.position, Endpoints[1].transform.position);
            //collision.transform.SetParent(this.transform);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            collision.GetComponent<PlayerControl>().MaintainSwingParameters(Endpoints[0].transform.position, Endpoints[1].transform.position);
        }
    }

}
