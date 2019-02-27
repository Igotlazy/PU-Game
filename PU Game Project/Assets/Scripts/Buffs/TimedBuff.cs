//OUT OF DATE BUFF SYSTEM. TURN INTO BATTLE EVENT.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MHA.BattleEffects;


public class TimedBuff : Buff
{

    protected Attack burnAttack = new Attack();



    public TimedBuff(Unit buffTarget, GameEntity buffSource, string buffName) : base(buffTarget, buffSource, buffName)
    {
        //burnAttack = new Attack(1f, buffTarget.attachedUnit, Attack.DamageType.Regular);
    }

    public TimedBuff(Unit buffTarget, GameEntity buffSource, string buffName, int turnCooldown) : base(buffTarget, buffSource, buffName, turnCooldown)
    {
        //burnAttack = new Attack(1f, buffTarget.attachedUnit, Attack.DamageType.Regular);
    }


    protected override void InitialAppImpl()
    {
        
    }

    protected override void EndTurnAppImpl()
    {
        LoadDamage();
    }

    private void LoadDamage()
    {
        burnAttack.damageValue = 50f;
        BEffectDealDamage damageEffect = new BEffectDealDamage(bSource, bTarget, burnAttack);

        ResolutionManager.instance.LoadBattleEffect(damageEffect);
    }
}
