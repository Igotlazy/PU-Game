using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AbilityBasicAttack : CharAbility
{

    public AbilityBasicAttack(LivingCreature livingCreature) : base(livingCreature)
    {
        castableAbilities.Add(new Action<EffectDataPacket>(Initialize));
        targetSelectors.Add(new List<GameObject> { AbilityPrefabRef.instance.GiveNodeCollectorPrefab(AbilityPrefabRef.instance.BasicAttackSelector) }); //Loads selector.
    }

    private void Initialize(EffectDataPacket effectDataPacket)
    {
        List<GameObject> relevantObjects = ((TargetPacket)effectDataPacket.GetValueAtKey("Targets", false)[0]).ReturnObjectsOnNodes(0); //Gets access to GameObject Targets from Nodes.
        List<Vector3> movePositions = CombatUtils.ProjectilePathSplicer(associatedCreature.gameObject.GetComponent<Unit>(), relevantObjects[0].GetComponent<Unit>()); //Get the positions the projectile should travel.

        GameObject projectile = GameObject.Instantiate(AbilityPrefabRef.instance.GiveAbilityPrefab(AbilityPrefabRef.instance.TakahiroBasic), movePositions[0], Quaternion.LookRotation(movePositions[1] - movePositions[0])); //Ceates projectile.



        List<BattleEffect> sendList = new List<BattleEffect>();

        for(int i = 1; i < movePositions.Count; i++)
        {
            EffectFreeMove effect = new EffectFreeMove(effectDataPacket, 1)
            {               
                destination = movePositions[i],
                moveSpeed = 5f + 0.5f*i,
            };
            effect.moveTarget.Add(projectile);

            if(i == movePositions.Count - 1)
            {
                effect.destroyAtEnd = true;
                effect.finishedEffectAuxCall += DamageOnImpact;
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
        List<LivingCreature> creatureList = new List<LivingCreature>();
        foreach (GameObject obj in relevantObjects)
        {
            creatureList.Add(obj.GetComponent<LivingCreature>());
        }


        EffectDealDamage effect = new EffectDealDamage(effectDataPacket, creatureList.Count)
        {
            damageAttack = new Attack(100, Attack.DamageType.Physical),
            damageTarget = creatureList
        };

        ResolutionManager.instance.LoadBattleEffect(effect);
    }
}
