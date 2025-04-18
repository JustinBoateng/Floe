using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Switch : MonoBehaviour
{
    [SerializeField] string Type;
    //Switch for switches that need to be hit
    //Sensor for when the trigger is activated by walking through it

    [SerializeField] bool isHit;
    [SerializeField] bool isOneWay;
    [SerializeField] float[] hitTimer = new float[2];
    [SerializeField] SpriteRenderer SR;
    [SerializeField] Sprite[] images = new Sprite[2];
    [SerializeField] SwitchSystem SSRef;
    [SerializeField] BoxCollider2D bc;
    [SerializeField] string Target;
    // Start is called before the first frame update
    void Start()
    {
        bc = GetComponent<BoxCollider2D>();

        if (GetComponent<SpriteRenderer>())
        {
            SR = GetComponent<SpriteRenderer>();
            SR.sprite = images[0];
        }

        hitTimer[1] = hitTimer[0];

    }

    // Update is called once per frame
    void Update()
    {
        if(!isOneWay && isHit)
        {
            hitTimer[1] = Mathf.Clamp(hitTimer[1] - Time.deltaTime, 0, hitTimer[0]);
            if (hitTimer[1] <= 0) { 
                ResetSwitch();
            }
        }   
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        switch (Type) {

            case "Switch":
                if (collision.tag == "Player" || collision.tag == "Bullet")
                {
                    HitSwitch();
                    if(collision.tag == "Bullet")
                    {
                        collision.GetComponent<BulletClass>().Crash();
                    }
                }
                break;

            case "Censor":
                if (collision.tag == "Player")
                {
                    //if (!isOneWay)
                        //hitTimer[1] = hitTimer[0]; //set hitTimer to max

                    SSRef.PerformAction(Target, true); // set the Action attached to the SwitchSystem to true


                }

                break;
        }
    }

    public void ResetSwitch()
    {
        isHit = false;
        hitTimer[1] = hitTimer[0];
        SR.sprite = images[0];
        bc.enabled = true;

        SSRef.PerformAction(Target, false);
    }

    public void HitSwitch()
    {
        //if (!isOneWay)  hitTimer[1] = hitTimer[0]; //set hitTimer to max

        Debug.Log("Switch Hit");
        SR.sprite = images[1]; //set button to appear hit

        SSRef.PerformAction(Target, true); // set the Action attached to the SwitchSystem to true

        isHit = true;

        bc.enabled = false;
    }
}
