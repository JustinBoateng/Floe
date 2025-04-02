using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingTerrain : MonoBehaviour
{
    [SerializeField] int Type;
    //1 for moving back and forth
    //2 for moving in one direction and then dissapearing
    //3 for rotating

    [SerializeField] Transform[] Endpoints;
    [SerializeField] float LerpValue;
    [SerializeField] int direction; //1 or -1
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        LerpValue += Time.deltaTime * direction;
        if (LerpValue < 0 || LerpValue > 1)
        {
            direction *= -1;
            LerpValue += Time.deltaTime * direction;
        }

        this.transform.position = Vector2.Lerp(Endpoints[0].position, Endpoints[1].position, LerpValue);

    }
}
