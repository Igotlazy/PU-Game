using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

[CreateAssetMenu(fileName = "New Basic Attack" , menuName = "Abilities/General/BasicAttack")]
public class AbilityBasicAttack : CharAbility
{
    [Header("Selectors:")]
    [SerializeField]
    AbilityPrefabRef.CircleSelector firstSelector = new AbilityPrefabRef.CircleSelector();

    public override void Initialize(Unit givenUnit)
    {
        base.Initialize(givenUnit);

        PrepCast();
        PrepSelectorPacket();
        PrepSelector();
    }

    private void PrepCast()
    {
        castableAbilities.Add(new Action<EffectDataPacket>(Run));
    }
    private void PrepSelectorPacket()
    {
        SelectorPacket firstTP = new SelectorPacket(SelectorPacket.SelectionType.Target, false)
        {
            maxNumOfSelect = 3
        };
        targetPacketBaseData.Add(new List<SelectorPacket> { firstTP });
    }
    private void PrepSelector()
    {
        targetSelectors.Add(new List<GameObject> { AbilityPrefabRef.instance.GiveNodeSelectorPrefab(firstSelector) }); //Loads selector.
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
        Attack attack = new Attack(100f, 20f, associatedUnit, Attack.DamageType.Pure);
        EffectDealDamage effect = new EffectDealDamage(effectDataPacket, relevantObject.GetComponent<LivingCreature>(), attack);

        ResolutionManager.instance.LoadBattleEffect(effect);
    }
}
