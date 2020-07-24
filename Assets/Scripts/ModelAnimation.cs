using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelAnimation : MonoBehaviour
{
    public string AnimName;

    void Start()
    {
        ResetAnimation();
    }

    public void ResetAnimation()
    {
        SetAnimation(AnimName);
    }

    public void SetAnimation(string animname)
    {
        AnimName = animname;
        AnimationClip lclip = (AnimationClip)AnimationClip.Instantiate(Resources.Load("Animations/" + animname));
        UpdatePose(lclip);
    }

    void UpdatePose(AnimationClip clip)
    {
        GameObject vrm = GameObject.Find("VRM");
        Animator model = vrm.GetComponent<Animator>();
        RuntimeAnimatorController controller = (RuntimeAnimatorController)RuntimeAnimatorController.Instantiate(Resources.Load("Animations/AnimController"));
        AnimatorOverrideController newanim = new AnimatorOverrideController(controller);
        newanim["00_tpose"] = clip;
        model.runtimeAnimatorController = newanim;
    }

}
