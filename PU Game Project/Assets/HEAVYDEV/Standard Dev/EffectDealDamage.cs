using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MHA.BattleBehaviours;
using MHA.Events;

public class EffectDealDamage : BattleEffect {

    public string KEYdamageAttack;
    public Attack damageAttack;

    public string KEYdamageTarget;
    public Unit damageTarget;

    

    public EffectDealDamage(EffectDataPacket _effectData, Unit _damageTarget, Attack _damageAttack) : base(_effectData)
    {
        this.damageTarget = _damageTarget;
        this.damageAttack = _damageAttack;
    }


    protected override void RunEffectImpl()
    {
        DealDamage();
    }

    protected override void WarnEffect()
    {

    }

    private void DealDamage()
    {
        damageTarget.CreatureHit(damageAttack);

        EventFlags.EVENTTookDamage(this, new EventFlags.ETookDamageArgs(damageAttack.damageValue, ((Unit)effectData.GetValue("Caster", false)[0]), damageTarget));

        if (KEYdamageAttack != null)
        {
            effectData.AppendValue(KEYdamageAttack, damageAttack);
        }
        if (KEYdamageTarget != null)
        {
            effectData.AppendValue(KEYdamageTarget, damageTarget);
        }
    }

    protected override bool EffectSpecificCondition()
    {
        if(damageTarget != null)
        {
            return true;
        }
        return false;
    }

    protected override void CancelEffectImpl()
    {

    }
}
