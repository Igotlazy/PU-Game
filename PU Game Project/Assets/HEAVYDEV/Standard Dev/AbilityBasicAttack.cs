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
    AbilityPrefabRef.CircleSelectorData firstSelector = new AbilityPrefabRef.CircleSelectorData();
    [Space]

    [Header("Properties")]
    [SerializeField]
    Attack damage = new Attack();

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
        SelectorPacket firstTP = new SelectorPacket(SelectorPacket.SelectionType.Target, false)
        {
            maxNumOfSelect = 3,
            selectorData = firstSelector           
        };
        selectorPacketBaseData.Add(new List<SelectorPacket> { firstTP });
    }

    private void Run(EffectDataPacket effectDataPacket)
    {
        SelectorPacket currentPacket = ((SelectorPacket)effectDataPacket.GetValue("Targets", false)[0]); //Gives packet.
        GameObject projectile = AbilityPrefabRef.instance.GiveAbilityPrefab(AbilityPrefabRef.TakahiroBasic);

        TPorterProjectile projectileEffect = new TPorterProjectile(effectDataPacket, currentPacket, projectile);
        projectileEffect.REPORTKEY = "HitTargets";
        projectileEffect.finishedEffectAuxCall += DamageOnImpact;

        ResolutionManager.instance.LoadBattleEffect(projectileEffect);
    }


    private void DamageOnImpact(EffectDataPacket effectDataPacket)
    {
        GameObject relevantObject = ((GameObject)effectDataPacket.GetValue("HitTargets", false).Last()); //Gets access to GameObject Target.
        Attack attack = damage;
        EffectDealDamage effect = new EffectDealDamage(effectDataPacket, relevantObject.GetComponent<Unit>(), attack);

        TimedBuff buff = new TimedBuff(relevantObject.GetComponent<Unit>(), this.associatedUnit.gameObject, "fire", 3);
        EffectApplyBuff effect2 = new EffectApplyBuff(effectDataPacket, relevantObject.GetComponent<Unit>(), buff); 


        List<BattleEffect> sendEffects = new List<BattleEffect>() { effect, effect2 };
        ResolutionManager.instance.LoadBattleEffect(sendEffects);
    }
}
