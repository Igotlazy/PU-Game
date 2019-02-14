using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AnimAbilityControl : MonoBehaviour
{
    public bool shouldWait;
    public bool animFinished;
    public float clipSpeed = 1f;

    protected Animation animComp;

    protected virtual void Awake()
    {
        animComp = GetComponent<Animation>();
    }

    protected virtual void Update()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            BeginAbilityAnimation();
        }
    }

    public virtual void BeginAbilityAnimation()
    {
        animComp.Play();
        foreach(AnimationState state in animComp)
        {
            state.speed = clipSpeed;
        }
    }


    public virtual void FinishAbilityAnimation()
    {
        animFinished = true;
    }

    protected void CallCameraShake()
    {
        CameraManager.instance.CameraShake();
    }


}
