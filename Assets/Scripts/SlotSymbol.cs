using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotSymbol : MonoBehaviour
{
    [SpineAnimation(dataField: "skeletonDataAsset")]
    public string _landAnimation;

    [SpineAnimation(dataField: "skeletonDataAsset")]
    public string _winAnimation;

    public string _landAudio;
    public void PlayLandAnim()
    {
        GetComponent<SkeletonGraphic>().AnimationState.SetAnimation(0,_landAnimation,false);
        
        if(_landAudio != string.Empty)
            GameController.Instance.PlayAudio(_landAudio);
    }
    public void PlayWinAnim()
    {
        GetComponent<SkeletonGraphic>().AnimationState.SetAnimation(0, _winAnimation, true);
    }
}
