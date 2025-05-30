using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage : MonoBehaviour
{

    [SerializeField] Checkpoints[] CheckpointReference;

    
    //Try playing around with spawning enemies 
    //[SerializeField] GameObject[] EnemySpawnpoints;
    //[SerializeField] GameObject[] Enemies;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Checkpoints[] GetCheckpoints()
    {
        return CheckpointReference;
    }


}
