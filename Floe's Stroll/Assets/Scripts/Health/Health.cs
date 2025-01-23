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

    [SerializeField] float[] DamageRecoil = new float[2]; //0: x-vector, 1: y-vector

    //[SerializeField] private Health playerHealth;
    [SerializeField] private Image totalHealthBar;
    [SerializeField] private Image currHealthBar;


    PlayerControl playerRef;
    Rigidbody2D rbRef;
    [SerializeField] float[] tookDamageRecoil = new float[2];

    private void Awake()
    {
        currentHealth = StartingHealth;
        dead = false;
    }
    private void Start()
    {
        totalHealthBar.fillAmount = currentHealth / 10;

        rbRef = GetComponent<Rigidbody2D>();
        playerRef = GetComponent<PlayerControl>();
    }

    private void Update()
    {
        currHealthBar.fillAmount = currentHealth / 10;
        //Fill Amount can be between 1 and 0

        if (tookDamageRecoil[1] > 0)
            KnockbackPhysics();
    }

    private void KnockbackPhysics()
    {
        rbRef.velocity = Vector2.zero;
        Debug.Log(-1 * playerRef.facing[0] * DamageRecoil[0]);
        rbRef.velocity = new Vector2(-1 * playerRef.facing[0] * DamageRecoil[0], DamageRecoil[1]);
        //rbRef.AddForce(new Vector2(playerRef.facing[0] * DamageRecoil[0] * -1, DamageRecoil[1]));
        tookDamageRecoil[1] -= Time.deltaTime;
    }

    public void TakeDamage(float _damage)
    {
        currentHealth = Mathf.Clamp(currentHealth - _damage, 0, StartingHealth);
        //currentHealth will become cHealth - _damage, the least it can be is 0, the most it can be is startingHealth

        PlayerControl playerRef = GetComponent<PlayerControl>();
        Rigidbody2D rbRef = GetComponent<Rigidbody2D>();


        if (currentHealth > 0)
        {
            //Player Hurt Animation
            playerRef.CooldownStart("Hurt");
            anim.SetTrigger("Hurt");

            tookDamageRecoil[1] = tookDamageRecoil[0];
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
