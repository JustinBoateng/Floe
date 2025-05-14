using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class FileManager : MonoBehaviour
{
    [SerializeField] File[] Files;
    [SerializeField] FileMenuObject[] FMO;


    [SerializeField] public Sprite[] StageThumbnail;
    [SerializeField] public Sprite EmptyFountain;
    [SerializeField] public string[] StageName;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < FMO.Length; i++)
        {
            FMO[i].SetFileInfo(Files[i]);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
