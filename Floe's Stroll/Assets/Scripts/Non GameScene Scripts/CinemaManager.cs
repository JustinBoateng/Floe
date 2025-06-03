using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Video;

public class CinemaManager : MonoBehaviour
{

    [SerializeField] VideoClip[] FilmDictionary;
    [SerializeField] int currFilm = 0;
    [SerializeField] string nextStage = "SampleStage1";
    [SerializeField] VideoPlayer Theatre;

    // Start is called before the first frame update
    void Start()
    {
        Theatre.loopPointReached += DoSomethingWhenVideoFinish;
        //Delegate Function
        //As soon as the video is finished (hit the loop point)
        //it will trigger then next function.
        //The function on thhe left is subscribed to the function on the right
        //Since the function on the left returns an event handler
    }


    void DoSomethingWhenVideoFinish(VideoPlayer vp)
    {
        Debug.Log("Video Over.");
        //currFilm++;
        //SetFilm(currFilm);


        if(TransitionManager.TM.getStage() == "NewFile");
        TransitionManager.TM.MoveScene(nextStage);
    }

    public void SetFilm(int i)
    {
        Theatre.clip = FilmDictionary[i];
    }

}
