using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectApplyBuff : BattleEffect
{

    Unit hitTarget;
    Buff givenBuff;

    public EffectApplyBuff(EffectDataPacket _effectPacket, Unit _hitTarget, Buff _givenBuff) : base(_effectPacket)
    {
        hitTarget = _hitTarget;
        givenBuff = _givenBuff;
    }



    protected override void WarnEffect()
    {
        Debug.Log("Buff Warning");
    }

    protected override void RunEffectImpl()
    {
        if(hitTarget != null)
        {
            hitTarget.AddBuff(givenBuff);
            givenBuff.BuffInitialApplication();
        }
    }

    protected override void CancelEffectImpl()
    {
        
    }

    protected override bool EffectSpecificCondition()
    {
        return true;
    }
}
