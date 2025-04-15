using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Switch : MonoBehaviour
{
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
        SR = GetComponent<SpriteRenderer>();

        SR.sprite = images[0];
    }

    // Update is called once per frame
    void Update()
    {
        if(!isOneWay && isHit)
        {
            hitTimer[1] = Mathf.Clamp(hitTimer[1] - Time.deltaTime, 0, hitTimer[0]);
        }   
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player" || collision.tag == "Bullet")
        {
            if(!isOneWay)
                hitTimer[1] = hitTimer[0]; //set hitTimer to max
            
            SR.sprite = images[1]; //set button to appear hit

            SSRef.PerformAction(Target); // set the Action attached to the SwitchSystem to true


        }
    }
}
