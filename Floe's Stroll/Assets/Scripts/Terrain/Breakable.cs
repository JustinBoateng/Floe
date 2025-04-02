using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Breakable : MonoBehaviour
{

    [SerializeField] int Health;
    BoxCollider2D bc;
    Rigidbody2D rb;
    //[SerializeField] bool isStill;
    // Start is called before the first frame update
    void Start()
    {
        bc = GetComponent<BoxCollider2D>();
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void Hit(int h)
    {
        //Spawn Hit Sprites
        Health -= h;
        if(Health <= 0)
        {
            Break();
        }
    }

    public void Break()
    {
        //Play Breaking Animation
        Destroy(this.gameObject);
    }
}
