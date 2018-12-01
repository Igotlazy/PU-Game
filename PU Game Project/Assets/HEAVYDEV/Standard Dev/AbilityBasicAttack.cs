using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AbilityBasicAttack : CharAbility
{

    public AbilityBasicAttack(LivingCreature livingCreature) : base(livingCreature)
    {
        castableAbilities.Add(new Action<EffectDataPacket>(Initialize));
        targetSelectors.Add(new List<GameObject> { CharacterAbilityPrefabRef.instance.NodeCollectors[3] }); //Loads selector.
    }

    private void Initialize(EffectDataPacket effectDataPacket)
    {
        List<GameObject> relevantObjects = ((TargetPacket)effectDataPacket.GetValueAtKey("Targets", false)[0]).ReturnObjectsOnNodes(0); //Gets access to GameObject Targets from Nodes.
        List<Vector3> movePositions = CombatUtils.ProjectilePathSplicer(associatedCreature.gameObject.GetComponent<Unit>(), relevantObjects[0].GetComponent<Unit>()); //Get the positions the projectile should travel.

        GameObject projectile = GameObject.Instantiate(CharacterAbilityPrefabRef.instance.TakahiroPrefabs[0], movePositions[0], Quaternion.LookRotation(movePositions[1] - movePositions[0])); //Ceates projectile.



        List<BattleEffect> sendList = new List<BattleEffect>();

        for(int i = 1; i < movePositions.Count; i++)
        {
            EffectFreeMove effect = new EffectFreeMove(effectDataPacket)
            {
                destination = movePositions[i],
                moveSpeed = 5f + 0.5f*i,
                moveTarget = projectile
            };

            if(i == movePositions.Count - 1)
            {
                effect.destroyAtEnd = true;
                effect.FinishedEffectAuxCall += new Action<EffectDataPacket>(DamageOnImpact);
            }

            sendList.Add(effect);
        }

        CombatUtils.MakeEffectsDependent(sendList);

        ResolutionManager.instance.LoadBattleEffect(sendList);
    }


    private void DamageOnImpact(EffectDataPacket effectDataPacket)
    {
        Debug.Log("dealing damage");
        List<GameObject> relevantObjects = ((TargetPacket)effectDataPacket.GetValueAtKey("Targets", false)[0]).ReturnObjectsOnNodes(0); //Gets access to GameObject Targets from Nodes.

        EffectDealDamage effect = new EffectDealDamage(effectDataPacket)
        {
            damageAttack = new Attack(100, Attack.DamageType.Physical),
            damageTarget = relevantObjects[0].GetComponent<LivingCreature>()
        };

        ResolutionManager.instance.LoadBattleEffect(effect);
    }
}
