using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Being : MonoBehaviour
{
    int[] Ammo = new int[2];

    string Name;

    [SerializeField] public int[] facing = new int[2];

    public void SetMaxAmmo(int i)
    {
        Ammo[0] = i; 
        Ammo[1] = Ammo[0] = i;
    }

    public int GetAmmo()
    {
        return Ammo[1];
    }

    public void AmmoCalc(int i)
    {
        Ammo[1] = Mathf.Clamp(Ammo[1] + i, 0, Ammo[0]);
    }
}
