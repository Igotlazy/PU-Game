using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MHA.BattleBehaviours;
using Cinemachine;

public class BBAnimCameraMove : BattleAnimation
{
    Transform cameraTransform;
    public BBAnimCameraMove (object _source, Transform givenTransform) : base(_source)
    {
        cameraTransform = givenTransform;
    }

    protected override void PlayBattleAnimationImpl()
    {
        if (CameraManager.instance.cameraTracker.transform.position != cameraTransform.position)
        {
            mono.StartCoroutine(MoveToCamera());
        }
        else
        {
            AnimFinished = true;
        }
        
    }

    private IEnumerator MoveToCamera()
    {
        CameraManager.instance.SetCameraTargetBasic(cameraTransform);
        yield return new WaitForSeconds(Camera.main.GetComponent<CinemachineBrain>().m_DefaultBlend.m_Time);
        AnimFinished = true;
    }


}
