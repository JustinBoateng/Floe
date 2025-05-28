using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoMoveInstructions : MonoBehaviour
{
    [SerializeField] string[] commands;
    [SerializeField] float[] timer;
    [SerializeField] GameObject[] positions; //0: Floe, 1: Boss, 2: etc.

    public string getCommand(int i)
    {
        if (i >= commands.Length)
            return null;
        return commands[i];
    }

    public float getTimer(int i)
    {
        if (i >= timer.Length)
            return timer[0];
        return timer[i];
    }

    public GameObject getPos(int i)
    {
        if (i >= positions.Length)
            return positions[0];
        return positions[i];
    }

}
