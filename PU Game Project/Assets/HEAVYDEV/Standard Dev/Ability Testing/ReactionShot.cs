using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using MHA.Events;
using MHA.BattleBehaviours;

[CreateAssetMenu(fileName = "ReactionShot", menuName = "Abilities/General/Reaction Shot")]
public class ReactionShot : CharAbility
{

    public override void Initialize(GameEntity givenUnit)
    {
        base.Initialize(givenUnit);

        PrepCast();
        PrepSelectorPacket();
    }

    private void PrepCast()
    {
        EventFlags.ProjectileMove += ProjectileMoveReceiver;
        //castableAbilities.Add(new Action<EffectDataPacket>(Run));
    }

    private void ProjectileMoveReceiver(object sender, EventFlags.EProjectileMoveArgs e)
    {
        //EffectDataPacket packet = new EffectDataPacket(associatedEntity, this);
        //SelectorPacket selectorPacket = new SelectorPacket(SelectorPacket.SelectionType.Target, false);
        //TPorterProjectile porter = new TPorterProjectile()
        //packet.
    }

    private void PrepSelectorPacket()
    {

    }

    private void Run(EffectDataPacket effectDataPacket)
    {
        effectDataPacket.AppendValue("Repeat", 0f);
        SelectorPacket currentPacket = ((SelectorPacket)effectDataPacket.GetVarValue("Targets", false)[0]); //Gives packet.
        //GameObject projectile = AbilityPrefabRef.instance.GiveAbilityPrefab(AbilityPrefabRef.TakahiroBasic);

        TPorterInstant projectileEffect = new TPorterInstant(associatedEntity, effectDataPacket, currentPacket);
        projectileEffect.REPORTKEY = "HitTarget";
        //projectileEffect.finishedEffectAuxCall += AoEScratch;
        //projectileEffect.finishedEffectAuxCall += Damage1;

        ResolutionManager.instance.LoadBattleEffect(projectileEffect);
    }
}
