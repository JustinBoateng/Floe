using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBatch : MonoBehaviour
{

    [SerializeField] EnemyAI[] Enemies;

    //[SerializeField] Camera CamRef;
    [SerializeField] float[] CameraPanCooldown = {2, 2, 0};
    //Max Pan In, Max Pan Out, curr Value

    [SerializeField] float TriggerViewSize;
    [SerializeField] GameObject CameraPos;
    [SerializeField] SwitchSystem SSRef;

    // Start is called before the first frame update
    void Start()
    {
        //CamRef =        

    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(CameraController.CC.name);

        if (!AtLeastOneAlive() && CameraPanCooldown[2] == 0)
        {
            //have the 
            //Do a cooldown for the camera to move to the platforms

            CameraPanCooldown[2] = CameraPanCooldown[0];
            
            CameraController.CC.setTarget(CameraPos.transform);
            CameraController.CC.setViewSize(TriggerViewSize);

            SSRef.PerformAction();

        }

        if (CameraPanCooldown[2] > 0)
        {
            CameraPanCooldown[2] -= Time.deltaTime;
            if (CameraPanCooldown[2] <= 0)
            {
                CameraController.CC.setTarget(GameplayManager.GM.GetPlayer().transform);
                //CameraCont.setAhead(0);
                CameraController.CC.ResetViewSize();
            }
        }
    }

    bool AtLeastOneAlive()
    {
        foreach(EnemyAI enemy in Enemies)
        {
            if (enemy != null) return true;
        }

        //Debug.Log("All Enemies Defeated");
        return false;
    }
}
