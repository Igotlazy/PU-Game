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
    public List<LivingCreature> damageTarget = new List<LivingCreature>();

    

    public EffectDealDamage(EffectDataPacket _effectData, int _runAmount) : base(_effectData, _runAmount)
    {

    }


    protected override void RunEffectImpl(int index)
    {
        DealDamage(index);
    }

    protected override void WarnEffect(int index)
    {
        Debug.Log("Deal Damage: Warning Event Not Implemented");
    }

    private void DealDamage(int index)
    {
        damageTarget[index].CreatureHit(damageAttack);
        Debug.Log("DEALING DAMAGE");

        new BBDealDamageAnim(damageTarget[index], damageTarget[index].currentHealth, damageAttack);

        EventFlags.EVENTTookDamage(this, new EventFlags.ETookDamageArgs
        {
            damageValue = damageAttack.damageValue,
            source = (LivingCreature)effectData.GetValue("caster", false)[0],
            target = damageTarget[index]
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

    protected override bool EffectSpecificCondition(int index)
    {
        if(damageTarget[index] != null)
        {
            return true;
        }
        return false;
    }

    protected override void CancelEffectImpl()
    {

    }
}
