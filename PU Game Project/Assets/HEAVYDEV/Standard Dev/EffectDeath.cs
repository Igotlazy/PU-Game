﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectDeath : BattleEffect
{
    Unit deathTarget; 

    public EffectDeath(EffectDataPacket _effectData) : base(_effectData)
    {
        deathTarget = (Unit)_effectData.GetValue("Caster", false)[0];
    }

    protected override void WarnEffect()
    {
        Debug.Log("Warn Death");
    }

    protected override void RunEffectImpl()
    {
        deathTarget.amDead = true;
        deathTarget.currentNode.IsOccupied = false;
        deathTarget.currentNode.occupant = null;
        ReferenceObjects.RemovePlayerFromLists(deathTarget.gameObject);

        new BBAnimDeath(this, deathTarget);
    }

    protected override void CancelEffectImpl()
    {

    }

    protected override bool EffectSpecificCondition()
    {
        return true;
    }


}
