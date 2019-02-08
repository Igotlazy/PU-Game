using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using MHA.Events;
using MHA.BattleBehaviours;

[CreateAssetMenu(fileName = "Sandy Basic", menuName = "Abilities/Heroes/Sandy/SandyBasic")]
public class SandyBasic : CharAbility
{
    [Header("Selectors:")]
    [SerializeField]
    AbilityPrefabRef.CircleSelectorData firstSelector = new AbilityPrefabRef.CircleSelectorData();
    [SerializeField]
    AbilityPrefabRef.CircleSelectorData secondSelector = new AbilityPrefabRef.CircleSelectorData();
    [Space]

    [Header("Properties")]
    [SerializeField]
    Attack damage = new Attack();
    [SerializeField]
    Attack damage2 = new Attack();

    public override void Initialize(Unit givenUnit)
    {
        base.Initialize(givenUnit);

        PrepCast();
        PrepSelectorPacket();
    }

    private void PrepCast()
    {
        castableAbilities.Add(new Action<EffectDataPacket>(Run));
    }
    private void PrepSelectorPacket()
    {
        SelectorPacket firstSP = new SelectorPacket(SelectorPacket.SelectionType.Target, false)
        {
            maxNumOfSelect = 1,
            selectorData = firstSelector
        };
        SelectorPacket secondSP = new SelectorPacket(SelectorPacket.SelectionType.AreaTarget, false)
        {
            selectorData = secondSelector
        };
        selectorPacketBaseData.Add(new List<SelectorPacket> { firstSP, secondSP });
    }

    private void Run(EffectDataPacket effectDataPacket)
    {
        SelectorPacket currentPacket = ((SelectorPacket)effectDataPacket.GetValue("Targets", false)[0]); //Gives packet.
        GameObject projectile = AbilityPrefabRef.instance.GiveAbilityPrefab(AbilityPrefabRef.TakahiroBasic);

        TPorterInstant projectileEffect = new TPorterInstant(effectDataPacket, currentPacket);
        projectileEffect.REPORTKEY = "HitTarget";
        projectileEffect.finishedEffectAuxCall += AoEScratch;
        projectileEffect.finishedEffectAuxCall += Damage1;

        ResolutionManager.instance.LoadBattleEffect(projectileEffect);
    }

    private void Damage1(EffectDataPacket effectDataPacket)
    {
        GameObject relevantObject = ((GameObject)effectDataPacket.GetValue("HitTarget", false).Last()); //Gets access to GameObject Target.
        Attack attack = damage;
        EffectDealDamage chomp = new EffectDealDamage(effectDataPacket, relevantObject.GetComponent<LivingCreature>(), attack);

        ResolutionManager.instance.LoadBattleEffect(chomp);
    }

    private void AoEScratch(EffectDataPacket effectDataPacket)
    {
        SelectorPacket currentPacket = ((SelectorPacket)effectDataPacket.GetValue("Targets", false)[1]); //Gives packet.

        TPorterInstant scratch = new TPorterInstant(effectDataPacket, currentPacket);
        scratch.differentiator = 1;
        scratch.REPORTKEY = "HitTarget1";
        scratch.finishedEffectAuxCall += Damage2;

        ResolutionManager.instance.LoadBattleEffect(scratch);
    }

    private void Damage2(EffectDataPacket effectDataPacket)
    {
        GameObject relevantObject = ((GameObject)effectDataPacket.GetValue("HitTarget1", false).Last()); //Gets access to GameObject Target.
        Attack attack = damage2;
        EffectDealDamage effect = new EffectDealDamage(effectDataPacket, relevantObject.GetComponent<LivingCreature>(), attack);


        List<BattleEffect> sendEffects = new List<BattleEffect>() { effect };
        ResolutionManager.instance.LoadBattleEffect(sendEffects);
    }

    public override void AnimResponse(object givenObject, EventFlags.ECastAnim givenCastAnim)
    {
        TPorter effect = (TPorter)givenObject;
        CharAbility ability = (CharAbility)effect.effectData.GetValue("CharacterAbility", false)[0];
        Vector3 pos = CombatUtils.GiveShotConnector(effect.givenTSpecs[effect.runIndex].targetObj);
        base.AnimResponse(givenObject, givenCastAnim);
        if (this == ability && effect.differentiator == 0) 
        {
            new AnimAbility(AbilityPrefabRef.instance.GiveAbilityPrefab(AbilityPrefabRef.SandyBasic1), pos);
            Debug.Log("CHOMP");
        }
        if (this == ability && effect.differentiator == 1)
        {
            new AnimAbility(AbilityPrefabRef.instance.GiveAbilityPrefab(AbilityPrefabRef.SandyBasic2), pos);
            Debug.Log("CHOMP");
        }
    }
}
