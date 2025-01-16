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
        totalHealthBar.fillAmount = playerHealth.currentHealth / 10;
    }
    private void Update()
    {
        currHealthBar.fillAmount = playerHealth.currentHealth / 10;
        //Fill Amount can be between 1 and 0
    }

}
