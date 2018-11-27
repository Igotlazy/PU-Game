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
    public bool recordDamageAttack;
    public Attack damageAttack;

    string damageTargetKey;
    public string DamageTargetKey
    {
        set
        {
            DamageTargetKey = value;
            recordDamageTarget = true;
        }
    }
    public bool recordDamageTarget;
    public LivingCreature damageTarget;

    

    public EffectDealDamage(EffectDataPacket _effectData) : base(_effectData)
    {

    }


    public override void RunEffectImpl()
    {
        DealDamage();
    }

    private void DealDamage()
    {

        //damageTodeal

        if (!isCancelled)
        {
            if (damageTarget != null)
            {
                damageTarget.CreatureHit(damageAttack);

                new BBDealDamageAnim(damageTarget, damageTarget.currentHealth, damageTarget.maxHealth.Value);

                EventFlags.EVENTTookDamage(this, new EventFlags.ETookDamageArgs
                {
                    damageValue = damageAttack.damageValue,
                    source = (LivingCreature)effectData.GetValueAtKey("caster", false)[0],
                    target = damageTarget
                });

                if (recordDamageAttack)
                {
                    effectData.SetValueAtKey(damageAttackKey, damageAttack);
                }
                if (recordDamageTarget)
                {
                    effectData.SetValueAtKey(damageTargetKey, damageTarget);
                }
            }
            else
            {
                CancelEffect();
            }
        }
    }

    public override void CancelEffectImpl()
    {

    }
}
