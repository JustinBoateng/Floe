using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SinglePlayerCanvas : MonoBehaviour
{
    public static SinglePlayerCanvas Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            DontDestroyOnLoad(this.gameObject);
            //DontDestroyOnLoad(FadeScreen);
            Instance = this;
        }

        else if (Instance != this)
            Destroy(this.gameObject);


    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
