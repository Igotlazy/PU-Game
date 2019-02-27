using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using MHA.BattleEffects;

[CreateAssetMenu(fileName = "New Basic Attack" , menuName = "Abilities/General/BasicAttack")]
public class AbilityBasicAttack : CharAbility
{
    [Header("Selectors:")]
    [SerializeField]
    SelectorData.Circle firstSelector = new SelectorData.Circle();
    [Space]

    [Header("Properties")]
    [SerializeField]
    Attack damage = new Attack();

    public override void Initialize(GameEntity givenEntity)
    {
        base.Initialize(givenEntity);

        PrepCast();
        PrepSelectorPacket();
    }

    private void PrepCast()
    {
        castableAbilities.Add(new Action<AbilityDataPacket>(Run));
    }
    private void PrepSelectorPacket()
    {
        selectorData.Add(new List<SelectorData>() { firstSelector });
    }

    private void Run(AbilityDataPacket effectDataPacket)
    {
        SelectorPacket currentPacket = ((SelectorPacket)effectDataPacket.GetVarValue("Targets", false)[0]); //Gives packet.
        GameObject projectile = AbilityPrefabRef.GiveAbilityPrefab(AbilityPrefabRef.TakahiroBasic);

        BEffectTPProjectile projectileEffect = new BEffectTPProjectile(associatedEntity, effectDataPacket, currentPacket, projectile);
        projectileEffect.REPORTKEY = "HitTargets";
        projectileEffect.finishedEffectAuxCall += DamageOnImpact;

        ResolutionManager.instance.LoadBattleEffect(projectileEffect);
    }


    private void DamageOnImpact(AbilityDataPacket effectDataPacket)
    {
        GameObject relevantObject = ((GameObject)effectDataPacket.GetVarValue("HitTargets", false).Last()); //Gets access to GameObject Target.
        Attack attack = damage;
        BEffectDealDamage effect = new BEffectDealDamage(associatedEntity, effectDataPacket, relevantObject.GetComponent<Unit>(), attack);

        TimedBuff buff = new TimedBuff(relevantObject.GetComponent<Unit>(), associatedEntity, "fire", 3);
        BEffectApplyBuff effect2 = new BEffectApplyBuff(associatedEntity, effectDataPacket, relevantObject.GetComponent<Unit>(), buff);


        List<BattleEffect> sendEffects = new List<BattleEffect>() { effect, effect2 };
        ResolutionManager.instance.LoadBattleEffect(sendEffects);
    }
}
