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

        SelectorPacket firstTP = new SelectorPacket(SelectorPacket.SelectionType.AreaTarget, false)
        {
            maxNumOfSelect = 3
        };
        targetPacketBaseData.Add(new List<SelectorPacket> {firstTP});

        targetSelectors.Add(new List<GameObject> { AbilityPrefabRef.instance.GiveNodeSelectorPrefab(AbilityPrefabRef.instance.CircleSelector) }); //Loads selector.
    }

    private void Initialize(EffectDataPacket effectDataPacket)
    {
        SelectorPacket currentPacket = ((SelectorPacket)effectDataPacket.GetValue("Targets", false)[0]); //Gives packet.
        GameObject projectile = AbilityPrefabRef.instance.GiveAbilityPrefab(AbilityPrefabRef.instance.TakahiroBasic);

        TPorterProjectile projectileEffect = new TPorterProjectile(effectDataPacket, currentPacket, projectile);
        projectileEffect.REPORTKEY = "HitTargets";
        projectileEffect.finishedEffectAuxCall += DamageOnImpact;

        ResolutionManager.instance.LoadBattleEffect(projectileEffect);
    }


    private void DamageOnImpact(EffectDataPacket effectDataPacket)
    {
        GameObject relevantObject = ((GameObject)effectDataPacket.GetValue("HitTargets", false).Last()); //Gets access to GameObject Target.

        EffectDealDamage effect = new EffectDealDamage(effectDataPacket, relevantObject.GetComponent<LivingCreature>(), new Attack(100, Attack.DamageType.Physical));

        ResolutionManager.instance.LoadBattleEffect(effect);
    }
}
