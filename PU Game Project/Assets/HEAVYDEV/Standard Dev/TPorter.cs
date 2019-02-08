using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MHA.Events;
using MHA.BattleBehaviours;

public abstract class TPorter : BattleEffect
{
    public int runIndex;
    protected SelectorPacket givenPacket;
    public List<TargetSpecs> givenTSpecs;
    protected bool warnOnce;
    protected bool doEnding;

    public TPorter(EffectDataPacket _effectData, SelectorPacket _givenPacket) : base(_effectData)
    {
        this.givenPacket = _givenPacket;
        this.givenTSpecs = givenPacket.targetObjectSpecs;

        if (givenPacket.selectionType == SelectorPacket.SelectionType.AoE)
        {
            this.warnOnce = true;
        }
    }

    protected override void WarnEffect()
    {
        //Anim Event for Very Start of Cast
        if (!doEnding)
        {
            if(givenTSpecs.Count > 0)
            {
                CameraMove();
                PeekCheck();
            }
            TPorterWarn();

            if (warnOnce)
            {
                TPorterWarnOverride = false;
            }
        }
    }
    protected abstract void TPorterWarn();
    protected virtual void CameraMove()
    {
        Unit unit = (Unit)(effectData.GetValue("Caster", false)[0]);
        new BBAnimCameraMove(unit.gameObject.transform);
    }

    protected override void RunEffectImpl()
    {
        TPorterFinishOverride = false;
        TPorterRemoveOverride = false;

        if (!doEnding)
        {
            if(givenTSpecs.Count > 0)
            {
                TPorterRun();
            }
            if(givenPacket.selectionType == SelectorPacket.SelectionType.AoE)
            {
                //Update Area with foreach. 
            }

        }
        else
        {
            PeekRecovery();
            runIndex++;
            doEnding = false;
        }

        if (runIndex > givenTSpecs.Count - 1)
        {
            //Anim Event for very End of Cast
            TPorterRemoveOverride = true;
        }
    }
    protected abstract void TPorterRun();

    protected override void CancelEffectImpl()
    {
        
    }

    protected override bool EffectSpecificCondition()
    {
        return true;
    }

    //Called in the TPorterWarn.
    protected virtual void PeekCheck()
    {
        if (!givenPacket.isPure && (givenTSpecs[runIndex].selectionType == SelectorPacket.SelectionType.Target || givenTSpecs[runIndex].selectionType == SelectorPacket.SelectionType.AreaTarget))
        {
            TargetSpecs currentSpec = givenTSpecs[runIndex];
            Vector3 targetShot = CombatUtils.GiveShotConnector(currentSpec.targetObj);
            Vector3 targetPartial = CombatUtils.GivePartialCheck(currentSpec.targetObj);
            CombatUtils.MainFireCalculation(currentSpec.fireOriginPoint, targetShot, targetPartial, out currentSpec.didPeek, out currentSpec.fireOriginPoint);

            if (currentSpec.didPeek)
            {
                Unit sourceObj = ((Unit)effectData.GetValue("Caster", false)[0]);
                EventFlags.ANIMStartPeekCALL(this, new EventFlags.EPeekStart(sourceObj, currentSpec.fireOriginPoint, sourceObj.gameObject.transform.position)); //EVENT
            }
        }
    }

    protected void PeekRecovery()
    {
        if (givenTSpecs[runIndex].didPeek && (givenTSpecs[runIndex].selectionType == SelectorPacket.SelectionType.Target || givenTSpecs[runIndex].selectionType == SelectorPacket.SelectionType.AreaTarget))
        {
            EventFlags.ANIMEndPeekCALL(this, new EventFlags.EPeekEnd(((Unit)effectData.GetValue("Caster", false)[0])));
        }
    }

}
