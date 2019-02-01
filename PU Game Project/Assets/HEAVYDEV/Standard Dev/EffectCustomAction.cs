using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EffectCustomAction : BattleEffect
{
    Action actionToRun;
    Action actionToWarn;

    public EffectCustomAction(EffectDataPacket _givenPacket, Action _givenAction) : base (_givenPacket)
    {
        setEffectType = EffectType.Custom;

        actionToRun = _givenAction;
        hasWarned = true;
        canBeCancelled = false;
    }
    public EffectCustomAction(EffectDataPacket _givenPacket, Action _givenWarnAction, Action _givenRunAction) : base(_givenPacket)
    {
        setEffectType = EffectType.Custom;

        actionToRun = _givenRunAction;
        actionToWarn = _givenWarnAction;
        canBeCancelled = false;
    }

    protected override void WarnEffect()
    {
        actionToWarn.Invoke();
    }

    protected override void RunEffectImpl()
    {
        actionToRun.Invoke();
    }

    protected override void CancelEffectImpl()
    {
        
    }

    protected override bool EffectSpecificCondition()
    {
        return true;
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
