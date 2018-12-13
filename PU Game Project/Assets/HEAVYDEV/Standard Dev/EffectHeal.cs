using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectHeal : BattleEffect {

    float healAmount;

    public EffectHeal (EffectDataPacket _effectData, int _runAmount) : base(_effectData, _runAmount)
    {

    }


    protected override void RunEffectImpl(int index)
    {
        HealTarget(index);
    }

    protected override void WarnEffect(int index)
    {
        Debug.Log("Heal: Warning Event Not Implemented");
    }

    private void HealTarget(int index)
    {
        //target.Heal(healAmount);

        //effectData.SetValueAtKey("Healed Amount", (float)effectData.blackboard["Healed Amount"] + healAmount);
    }

    protected override bool EffectSpecificCondition(int index)
    {
        return true;
    }

    protected override void CancelEffectImpl()
    {

    }

}
