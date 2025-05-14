using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class FileMenuObject : MonoBehaviour
{

    //[SerializeField] File F;
    [SerializeField] FileManager FMRef;

    //[SerializeField] Image background;
    [SerializeField] TextMeshProUGUI FileNumber;
    [SerializeField] TextMeshProUGUI LivesLeft;
    [SerializeField] TextMeshProUGUI TimeMins;
    [SerializeField] TextMeshProUGUI TimeSecs;

    [SerializeField] Image[] FountainsCollected;
    [SerializeField] Sprite[] FountainSpriteRef;
    [SerializeField] Sprite EmptyFountain;

    [SerializeField] Button BRef;

    [SerializeField] bool isNew;
    [SerializeField] string currStage;
    [SerializeField] int currrStageIndex;
    // Start is called before the first frame update
    void Start()
    {
        //SetFileInfo();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetFileInfo(File F)
    {
        currStage = F.CurrStage;

        FileNumber.text = "0" + F.FileNumber.ToString();
        LivesLeft.text = F.NoofLives.ToString();
        
        if(F.TimeMins < 10)
            TimeMins.text = "0" + F.TimeMins.ToString();
        else TimeMins.text = F.TimeMins.ToString();

        if (F.TimeSecs < 10)
            TimeSecs.text = F.TimeSecs.ToString();
        else TimeSecs.text = F.TimeSecs.ToString();

        BRef.image.sprite = FMRef.StageThumbnail[F.CurrStageInt];

        for (int f = 0; f < F.CollectedFountains.Length; f++)
        {
            if (F.CollectedFountains[f])
            {
                FountainsCollected[f].sprite = FountainSpriteRef[f];
            }

            else FountainsCollected[f].sprite = FMRef.EmptyFountain;
        }
    }

    public void ButtonClick()
    {
        TransitionManager.TM.setStage(currStage);

        if (isNew)
        {
            //TransitionManager.TM.setStage("Stage1");
            TransitionManager.TM.MoveScene("NewFile");
        }

        else
            TransitionManager.TM.MoveScene("SampleScene");
    }
}
