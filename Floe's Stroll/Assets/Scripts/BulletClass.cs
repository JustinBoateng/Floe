using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletClass : MonoBehaviour
{

    [SerializeField] private float xSpeed;
    [SerializeField] private float ySpeed;
    [SerializeField] private int[] facing = new int[2];
    Rigidbody2D rb;

    [SerializeField] float[] LifeExpectancy = {0,0,0}; 
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        rb.velocity = new Vector2(xSpeed * facing[0], ySpeed * facing[1]);

        BulletDeterioration();
    }

    public void setfacing(int x, int y)
    {
        facing[0] = x;
        facing[1] = y;
    }
    
    public void setSpeed(int x, int y)
    {
        xSpeed = x;
        ySpeed= y;
    }

    public void setDeterioration(float rate)
    {
        LifeExpectancy[0] = 100;
        LifeExpectancy[1] = 0;
        LifeExpectancy[2] = rate;
    }

    private void BulletDeterioration()
    {
        LifeExpectancy[1] += LifeExpectancy[2];
        if (LifeExpectancy[1] >= LifeExpectancy[0])
            Destroy(this.gameObject);
    }
}
