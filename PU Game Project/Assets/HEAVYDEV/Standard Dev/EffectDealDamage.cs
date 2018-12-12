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
    Attack damageAttack;

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
    LivingCreature damageTarget;

    

    public EffectDealDamage(EffectDataPacket _effectData) : base(_effectData)
    {

    }


    public override void RunEffectImpl()
    {
        DealDamage();
    }

    public override void WarnEffect()
    {
        Debug.Log("Deal Damage: Warning Event Not Implemented");
    }

    private void DealDamage()
    {

        //damageTodeal

        if (damageTarget != null)
        {
            damageTarget.CreatureHit(damageAttack);
            Debug.Log("DEALING DAMAGE");

            new BBDealDamageAnim(damageTarget, damageTarget.currentHealth, damageAttack);

            EventFlags.EVENTTookDamage(this, new EventFlags.ETookDamageArgs
            {
                damageValue = damageAttack.damageValue,
                source = (LivingCreature)effectData.GetValueAtKey("caster", false)[0],
                target = damageTarget
            });

            if (recordDamageAttack)
            {
                effectData.AppendValueAtKey(damageAttackKey, damageAttack);
            }
            if (recordDamageTarget)
            {
                effectData.AppendValueAtKey(damageTargetKey, damageTarget);
            }
        }
    }

    public override void CancelEffectImpl()
    {

    }
}
