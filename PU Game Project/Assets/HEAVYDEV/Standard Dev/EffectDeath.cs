using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectDeath : BattleEffect
{
    Unit deathTarget; 

    public EffectDeath(GameEntity _source, Unit _deathTarget) : base (_source)
    {
        deathTarget = _deathTarget;
    }
    public EffectDeath(GameEntity _source, EffectDataPacket _effectData, Unit _deathTarget) : base(_source, _effectData)
    {
        deathTarget = _deathTarget;
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
