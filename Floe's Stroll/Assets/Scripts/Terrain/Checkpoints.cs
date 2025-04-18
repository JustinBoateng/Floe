using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoints : MonoBehaviour
{
    [SerializeField] int CheckNumber;
    [SerializeField] int numSprites;
    [SerializeField] Sprite[] Visuals;
    [SerializeField] public bool isPassed, isGoal;
    [SerializeField] BoxCollider2D bc;
    [SerializeField] SpriteRenderer SR;
    
    

    // Start is called before the first frame update
    void Start()
    {
        if(!SR)
            SR = GetComponent<SpriteRenderer>();
        if (!bc)
            bc = GetComponent<BoxCollider2D>();

        SR.sprite = Visuals[0];

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            GameplayManager.GM.setCheckpoint(CheckNumber);
            //play checkpoint animation
            if(numSprites == 2) 
                SR.sprite = Visuals[1];
            isPassed = true;

        }
    }
}
