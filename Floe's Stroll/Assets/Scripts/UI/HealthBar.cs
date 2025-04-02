using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Health playerHealth;
    [SerializeField] private Image totalHealthBar;
    [SerializeField] private Image currHealthBar;

    private void Start()
    {
    
    }
    private void Update()
    {
        currHealthBar.fillAmount = playerHealth.currentHealth / 10;
        //Fill Amount can be between 1 and 0
    }


    public void setHealth(Health hRef)
    {
        playerHealth = hRef;
        totalHealthBar.fillAmount = playerHealth.currentHealth / playerHealth.StartingHealth;
    }
}
