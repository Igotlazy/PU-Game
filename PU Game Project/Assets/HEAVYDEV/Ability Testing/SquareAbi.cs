using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using MHA.BattleEffects;

[CreateAssetMenu(fileName = "New Box Attack", menuName = "Abilities/General/BoxAttack")]
public class SquareAbi : CharAbility
{

    [Header("Selectors:")]
    [SerializeField]
    SelectorData.Box firstSelector = new SelectorData.Box();
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

        ResolutionManager.instance.LoadBattleEffect(effect);
    }
}
