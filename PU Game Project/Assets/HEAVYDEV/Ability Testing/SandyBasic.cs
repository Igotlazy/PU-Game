using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using MHA.Events;
using MHA.BattleEffects;
using MHA.BattleAnimations;

[CreateAssetMenu(fileName = "Sandy Basic", menuName = "Abilities/Heroes/Sandy/SandyBasic")]
public class SandyBasic : CharAbility
{
    [Header("Selectors:")]
    [SerializeField]
    SelectorData.Circle firstSelector = new SelectorData.Circle();
    [SerializeField]
    SelectorData.Circle secondSelector = new SelectorData.Circle();
    [Space]

    [Header("Properties")]
    [SerializeField]
    Attack damage = new Attack();
    [SerializeField]
    Attack damage2 = new Attack();

    public override void Initialize(GameEntity givenUnit)
    {
        base.Initialize(givenUnit);

        PrepCast();
        PrepSelectorPacket();
    }

    private void PrepCast()
    {
        castableAbilities.Add(new Action<AbilityDataPacket>(Run));
    }
    private void PrepSelectorPacket()
    {
        selectorData.Add(new List<SelectorData>() { firstSelector, secondSelector });
    }

    private void Run(AbilityDataPacket effectDataPacket)
    {
        effectDataPacket.AppendValue("Repeat", 0f);
        SelectorPacket currentPacket = ((SelectorPacket)effectDataPacket.GetVarValue("Targets", false)[0]); //Gives packet.

        BEffectTPInstant bite = new BEffectTPInstant(associatedEntity, effectDataPacket, currentPacket);
        bite.REPORTKEY = "HitTarget";
        bite.finishedEffectAuxCall += Damage1;


        SelectorPacket currentPacket2 = ((SelectorPacket)effectDataPacket.GetVarValue("Targets", false)[1]); //Gives packet.

        BEffectTPInstant scratch = new BEffectTPInstant(associatedEntity, effectDataPacket, currentPacket2);
        scratch.differentiator = 1;
        scratch.REPORTKEY = "HitTarget1";
        scratch.finishedEffectAuxCall += Damage2;

        ResolutionManager.instance.LoadBattleEffect(new List<BattleEffect>() { bite, scratch });
    }

    private void Damage1(AbilityDataPacket effectDataPacket)
    {
        GameObject relevantObject = ((GameObject)effectDataPacket.GetVarValue("HitTarget", false).Last()); //Gets access to GameObject Target.
        Attack attack = damage;
        BEffectDealDamage chomp = new BEffectDealDamage(associatedEntity, effectDataPacket, relevantObject.GetComponent<Unit>(), attack);

        ResolutionManager.instance.LoadBattleEffect(chomp);
    }

    private void Damage2(AbilityDataPacket effectDataPacket)
    {
        Debug.Log("Attack 2 beign called");
        Attack attack = damage2;
        List<BattleEffect> sendEffects = new List<BattleEffect>();

        foreach (object hitTarget in effectDataPacket.GetVarValue("HitTarget1", false))
        {
            GameObject target = (GameObject)hitTarget;
            BEffectDealDamage effect = new BEffectDealDamage(associatedEntity, effectDataPacket, target.GetComponent<Unit>(), attack);
            effect.effectData = effectDataPacket;
            sendEffects.Add(effect);

        }

        ResolutionManager.instance.LoadBattleEffect(sendEffects);
    }

    public override void AnimResponse(object givenObject, EventFlags.ECastAnim givenCastAnim)
    {
        BEffectTP effect = (BEffectTP)givenObject;
        CharAbility ability = (CharAbility)effect.effectData.GetStaticValue("CharacterAbility", false);

        Vector3 pos = CombatUtils.GiveShotConnector(effect.givenTSpecs[effect.runIndex].targetObj);

        base.AnimResponse(givenObject, givenCastAnim);


        if (this == ability && effect.differentiator == 0) 
        {
            new BAnimAbility(this, AbilityPrefabRef.GiveAbilityPrefab(AbilityPrefabRef.SandyBasic1), pos);
        }

        if (this == ability && effect.differentiator == 1)
        {
            try
            {
                BAnimAbility oldAnim = (BAnimAbility)ResolutionManager.instance.animationQueue.Last();
                if(this == (object)oldAnim.source && oldAnim.animObject == AbilityPrefabRef.GiveAbilityPrefab(AbilityPrefabRef.SandyBasic2))
                {
                    oldAnim.forceGo = true;
                    new BAnimAbility(this, AbilityPrefabRef.GiveAbilityPrefab(AbilityPrefabRef.SandyBasic2), pos);
                }
                else
                {
                    new BAnimAbility(this, AbilityPrefabRef.GiveAbilityPrefab(AbilityPrefabRef.SandyBasic2), pos);
                }

            }
            catch (InvalidCastException)
            {
                new BAnimAbility(this, AbilityPrefabRef.GiveAbilityPrefab(AbilityPrefabRef.SandyBasic2), pos);
            }
        }
    }
}
