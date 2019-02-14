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
        SelectorPacket secondSP = new SelectorPacket(SelectorPacket.SelectionType.AoE, false)
        {
            selectorData = secondSelector
        };
        selectorPacketBaseData.Add(new List<SelectorPacket> { firstSP, secondSP });
    }

    private void Run(EffectDataPacket effectDataPacket)
    {
        effectDataPacket.AppendValue("Repeat", 0f);
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
        EffectDealDamage chomp = new EffectDealDamage(effectDataPacket, relevantObject.GetComponent<Unit>(), attack);

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
        Attack attack = damage2;
        List<BattleEffect> sendEffects = new List<BattleEffect>();

        foreach (object hitTarget in effectDataPacket.GetValue("HitTarget1", false))
        {
            GameObject target = (GameObject)hitTarget;
            EffectDealDamage effect = new EffectDealDamage(effectDataPacket, target.GetComponent<Unit>(), attack);
            sendEffects.Add(effect);

        }

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
            new AnimAbility(this, AbilityPrefabRef.instance.GiveAbilityPrefab(AbilityPrefabRef.SandyBasic1), pos);
        }

        if (this == ability && effect.differentiator == 1)
        {
            try
            {
                AnimAbility oldAnim = (AnimAbility)ResolutionManager.instance.animationQueue.Last();
                if(this == (object)oldAnim.source && oldAnim.animObject == AbilityPrefabRef.instance.GiveAbilityPrefab(AbilityPrefabRef.SandyBasic2))
                {
                    oldAnim.forceGo = true;
                    new AnimAbility(this, AbilityPrefabRef.instance.GiveAbilityPrefab(AbilityPrefabRef.SandyBasic2), pos);
                }
                else
                {
                    new AnimAbility(this, AbilityPrefabRef.instance.GiveAbilityPrefab(AbilityPrefabRef.SandyBasic2), pos);
                }

            }
            catch (InvalidCastException)
            {
                new AnimAbility(this, AbilityPrefabRef.instance.GiveAbilityPrefab(AbilityPrefabRef.SandyBasic2), pos);
            }
        }
    }
}
