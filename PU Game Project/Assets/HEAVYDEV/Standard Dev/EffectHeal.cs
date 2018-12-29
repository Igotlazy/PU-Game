﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectHeal : BattleEffect {

    float healAmount;

    public EffectHeal (EffectDataPacket _effectData) : base(_effectData)
    {

    }


    protected override void RunEffectImpl()
    {
        HealTarget();
    }

    protected override void WarnEffect()
    {
        Debug.Log("Heal: Warning Event Not Implemented");
    }

    private void HealTarget()
    {
        //target.Heal(healAmount);

        //effectData.SetValueAtKey("Healed Amount", (float)effectData.blackboard["Healed Amount"] + healAmount);
    }

    protected override bool EffectSpecificCondition()
    {
        return true;
    }

    protected override void CancelEffectImpl()
    {

    }

}
