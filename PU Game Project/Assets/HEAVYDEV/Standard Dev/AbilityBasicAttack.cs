using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

[CreateAssetMenu(fileName = "New Basic Attack" , menuName = "Abilities/General/BasicAttack")]
public class AbilityBasicAttack : CharAbility
{

    public override void Initialize(Unit givenUnit)
    {
        base.Initialize(givenUnit);


        castableAbilities.Add(new Action<EffectDataPacket>(Run));

        SelectorPacket firstTP = new SelectorPacket(SelectorPacket.SelectionType.AreaTarget, false)
        {
            maxNumOfSelect = 3
        };
        targetPacketBaseData.Add(new List<SelectorPacket> {firstTP});

        targetSelectors.Add(new List<GameObject> { AbilityPrefabRef.instance.GiveNodeSelectorPrefab(AbilityPrefabRef.instance.CircleSelector) }); //Loads selector.
    }

    private void Run(EffectDataPacket effectDataPacket)
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
        Attack attack = new Attack(100, associatedUnit, Attack.DamageType.Regular);
        EffectDealDamage effect = new EffectDealDamage(effectDataPacket, relevantObject.GetComponent<LivingCreature>(), attack);

        ResolutionManager.instance.LoadBattleEffect(effect);
    }
}
