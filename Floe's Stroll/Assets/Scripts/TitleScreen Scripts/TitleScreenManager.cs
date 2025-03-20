using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TitleScreenManager : MonoBehaviour
{
    [SerializeField] EventSystem ESRef;
    [SerializeField] Button[] ButtRefs;

    // Start is called before the first frame update
    void Start()
    {
        ESRef.SetSelectedGameObject(ButtRefs[0].gameObject);

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SceneChange(int i)
    {

    }
}
