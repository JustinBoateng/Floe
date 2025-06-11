using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] public static HealthBar Instance;

    [SerializeField] private Health playerHealth;
    [SerializeField] private Image totalHealthBar;
    [SerializeField] private Image currHealthBar;

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

    private void Start()
    {
    
    }
    private void Update()
    {
        if(SceneManager.GetActiveScene().name == "Main Menu" || SceneManager.GetActiveScene().name == "Cinema")
            gameObject.SetActive(false);
        else gameObject.SetActive(true);

        if (playerHealth) 
            currHealthBar.fillAmount = playerHealth.currentHealth / playerHealth.StartingHealth;
        //Fill Amount can be between 1 and 0
    }


    public void setHealth(Health hRef)
    {
        playerHealth = hRef;
        //Debug.Log("Current Health Fill: " + playerHealth.currentHealth / playerHealth.StartingHealth);
        totalHealthBar.fillAmount = playerHealth.currentHealth / playerHealth.StartingHealth;
    }


}
