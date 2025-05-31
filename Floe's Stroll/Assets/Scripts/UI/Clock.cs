using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Clock : MonoBehaviour
{
    [SerializeField] public static Clock ClockInstance;

    [SerializeField] TextMeshProUGUI Minutes;
    [SerializeField] TextMeshProUGUI Seconds;
    [SerializeField] TextMeshProUGUI Milliseconds;
    [SerializeField] float msSpecification;

    private void Awake()
    {
        if (ClockInstance == null)
        {
            DontDestroyOnLoad(this.gameObject);
            //DontDestroyOnLoad(FadeScreen);
            ClockInstance = this;
        }

        else if (ClockInstance != this)
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

    public void ClockUpdate(float m, float s, float ms)
    {
        //Debug.Log(ms);
        msSpecification = (int)(ms * 100);// % 100;

        Minutes.text = m.ToString();

        if(s <  10) { Seconds.text = "0" + s.ToString(); }
        else Seconds.text = s.ToString();

        if(msSpecification < 10) { Milliseconds.text = "0" + msSpecification.ToString(); }
        else Milliseconds.text = msSpecification.ToString();
    }
}
