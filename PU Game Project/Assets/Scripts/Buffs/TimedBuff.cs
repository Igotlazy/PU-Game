//OUT OF DATE BUFF SYSTEM. TURN INTO BATTLE EVENT.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TimedBuff : Buff
{

    protected Attack burnAttack = new Attack();



    public TimedBuff(Unit buffTarget, GameObject buffSource, string buffName) : base(buffTarget, buffSource, buffName)
    {
        //burnAttack = new Attack(1f, buffTarget.attachedUnit, Attack.DamageType.Regular);
    }

    public TimedBuff(Unit buffTarget, GameObject buffSource, string buffName, int turnCooldown) : base(buffTarget, buffSource, buffName, turnCooldown)
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
        EffectDataPacket packet = new EffectDataPacket(bSource.GetComponent<Unit>(), null);
        EffectDealDamage damageEffect = new EffectDealDamage(packet, bTarget, burnAttack);

        ResolutionManager.instance.LoadBattleEffect(damageEffect);
    }
}
