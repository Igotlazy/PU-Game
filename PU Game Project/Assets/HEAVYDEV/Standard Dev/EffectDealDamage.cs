using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MHA.BattleBehaviours;
using MHA.Events;

public class EffectDealDamage : BattleEffect {

    string damageAttackKey;
    public string DamageAttackKey
    {
        set
        {
            damageAttackKey = value;
            recordDamageAttack = true;
        }
    }
    bool recordDamageAttack;
    public Attack damageAttack;

    string damageTargetKey;
    public string DamageTargetKey
    {
        set
        {
            damageTargetKey = value;
            recordDamageTarget = true;
        }
    }
    bool recordDamageTarget;
    public LivingCreature damageTarget;

    

    public EffectDealDamage(EffectDataPacket _effectData) : base(_effectData)
    {

    }


    protected override void RunEffectImpl()
    {
        DealDamage();
    }

    protected override void WarnEffect()
    {
        Debug.Log("Deal Damage: Warning Event Not Implemented");
    }

    private void DealDamage()
    {
        damageTarget.CreatureHit(damageAttack);
        Debug.Log("DEALING DAMAGE");

        new BBDealDamageAnim(damageTarget, damageTarget.currentHealth, damageAttack);

        EventFlags.EVENTTookDamage(this, new EventFlags.ETookDamageArgs
        {
            damageValue = damageAttack.damageValue,
            source = (LivingCreature)effectData.GetValue("caster", false)[0],
            target = damageTarget
        });

        if (recordDamageAttack)
        {
            effectData.AppendValue(damageAttackKey, damageAttack);
        }
        if (recordDamageTarget)
        {
            effectData.AppendValue(damageTargetKey, damageTarget);
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
