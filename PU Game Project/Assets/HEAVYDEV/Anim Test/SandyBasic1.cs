using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MHA.BattleBehaviours;

public class SandyBasic1 : AnimAbilityControl
{
    public ParticleSystem leftSquirt;
    public ParticleSystem rightSquirt;

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Update()
    {
        base.Update();
    }

    public void StartSquirt()
    {
        Debug.Log("Squirt");
        leftSquirt.Play();
        rightSquirt.Play();
    }

    public override void FinishAbilityAnimation()
    {
        base.FinishAbilityAnimation();
        CallCameraShake();
    }
}
