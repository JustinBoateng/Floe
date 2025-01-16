using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class Health : MonoBehaviour
{
    [SerializeField] private float StartingHealth;
    [SerializeField] public float currentHealth { get; private set; }
    //instead of doing an entire get and set function, we can just do this.

    [SerializeField] private Animator anim;
    [SerializeField] private bool dead;


    //[SerializeField] private Health playerHealth;
    [SerializeField] private Image totalHealthBar;
    [SerializeField] private Image currHealthBar;

    private void Awake()
    {
        currentHealth = StartingHealth;
        dead = false;
    }
    private void Start()
    {
        totalHealthBar.fillAmount = currentHealth / 10;
    }

    private void Update()
    {
        currHealthBar.fillAmount = currentHealth / 10;
        //Fill Amount can be between 1 and 0
    }
    public void TakeDamage(float _damage)
    {
        currentHealth = Mathf.Clamp(currentHealth - _damage, 0, StartingHealth);
        //currentHealth will become cHealth - _damage, the least it can be is 0, the most it can be is startingHealth

        if (currentHealth > 0)
        {
            //Player Hurt Animation
            //anim.SetTrigger("Hurt")
        }

        else
        {
            if(!dead) { }
            {
                //Player dead Animation
                //anim.SetTrigger("Die")
                GetComponent<PlayerControl>().enabled = false;
                //make it so that the player can't control the character anymore
                dead = true;
            }
        }
    }



    public void onDmgTest(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            TakeDamage(1);
            Debug.Log("Health is now " + currentHealth);
        }
    }
}
