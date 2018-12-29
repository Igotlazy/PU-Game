using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class AbilityBasicAttack : CharAbility
{

    public AbilityBasicAttack(LivingCreature livingCreature) : base(livingCreature)
    {
        castableAbilities.Add(new Action<EffectDataPacket>(Initialize));

        TargetPacket firstTP = new TargetPacket(TargetPacket.SelectionType.Single)
        {
            maxNumOfSelect = 3
        };
        Debug.Log(firstTP.maxNumOfSelect);
        targetPacketBaseData.Add(new List<TargetPacket> {firstTP});

        targetSelectors.Add(new List<GameObject> { AbilityPrefabRef.instance.GiveNodeCollectorPrefab(AbilityPrefabRef.instance.BasicAttackSelector) }); //Loads selector.
    }

    private void Initialize(EffectDataPacket effectDataPacket)
    {
        List<TargetSpecs> relevantTargets = ((TargetPacket)effectDataPacket.GetValue("Targets", false)[0]).targetObjectSpecs; //Gets access to GameObject Targets.
        GameObject projectile = AbilityPrefabRef.instance.GiveAbilityPrefab(AbilityPrefabRef.instance.TakahiroBasic);

        TPorterProjectile projectileEffect = new TPorterProjectile(effectDataPacket, relevantTargets, projectile);
        projectileEffect.REPORTKEY = "HitTargets";
        projectileEffect.finishedEffectAuxCall += DamageOnImpact;

        ResolutionManager.instance.LoadBattleEffect(new List<BattleEffect>() { projectileEffect });
    }


    private void DamageOnImpact(EffectDataPacket effectDataPacket)
    {
        GameObject relevantObject = ((GameObject)effectDataPacket.GetValue("HitTargets", false).Last()); //Gets access to GameObject Target.


        EffectDealDamage effect = new EffectDealDamage(effectDataPacket)
        {
            damageAttack = new Attack(100, Attack.DamageType.Physical),
            damageTarget = relevantObject.GetComponent<LivingCreature>()
        };


        ResolutionManager.instance.LoadBattleEffect(effect);
    }
}
