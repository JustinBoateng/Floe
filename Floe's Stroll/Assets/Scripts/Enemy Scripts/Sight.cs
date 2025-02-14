using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sight : MonoBehaviour
{

    [SerializeField] private BoxCollider2D BC;
    [SerializeField] private List<Being> beings = new List<Being>();
    [SerializeField] private bool PlayerLockOn;
    // Start is called before the first frame update
    void Start()
    {
        BC = GetComponent<BoxCollider2D>();
        PlayerLockOn = false;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        beings.Add(collision.GetComponent<Being>());

        if (collision.tag == "Player")
        {
            PlayerLockOn = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        beings.Remove(collision.GetComponent<Being>());
        if (collision.tag == "Player")
        {
            PlayerLockOn = false;
        }

    }

    public bool LockOnStatus()
    {
        return PlayerLockOn;
    }
}
