﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectHeal : BattleEffect {

    float healAmount;

    public EffectHeal (EffectDataPacket _effectData) : base(_effectData)
    {

    }


    public override void RunEffectImpl()
    {
        HealTarget();
    }

    private void HealTarget()
    {
        //target.Heal(healAmount);

        //effectData.SetValueAtKey("Healed Amount", (float)effectData.blackboard["Healed Amount"] + healAmount);
    }

    public override void CancelEffectImpl()
    {

    }

}
