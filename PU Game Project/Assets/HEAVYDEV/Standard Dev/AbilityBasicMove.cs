using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AbilityBasicMove : CharAbility {


    public AbilityBasicMove(LivingCreature livingCreature) : base (livingCreature)
    {
        castableAbilities.Add(new Action<EffectDataPacket>(Initialize));
        targetSelectors.Add(new List<GameObject> {CharacterAbilityPrefabRef.instance.NodeCollectors[2]});
    }


    private void Initialize(EffectDataPacket effectPacket)
    {
        List<Vector3> path = new List<Vector3>();

        TargetPacket relevantTargets = (TargetPacket)effectPacket.GetValueAtKey("Targets", false)[0];
        foreach (Node currentNode in relevantTargets.TargetNodes[0]) //Get Path Location Data
        {
            path.Add(currentNode.worldPosition);
        }

        associatedCreature.CurrentEnergy -= path.Count;

        for (int i = 0; i < path.Count; i++) //Store Path Location Data (not really needed in this case)
        {
            effectPacket.SetValueAtKey("MovePath", path[i]);
        }

        List<BattleEffect> effectsToPass = new List<BattleEffect>(); //Effects to send to the Resolver

        for(int i = 0; i < path.Count; i++) //Creation of Effects
        {
            EffectGridMove moveEffect = new EffectGridMove(effectPacket)
            {
                pathIndex = (Vector3)effectPacket.GetValueAtKey("MovePath", false)[i],
                moveSpeed = 3f,
                moveTarget = associatedCreature
            };


            if (previousEffect != null) { previousEffect.CancelEffectAuxCalls += moveEffect.CancelEffect; } //If a move is cancelled, all of them down the line will also be cancelled. 
            previousEffect = moveEffect;

            effectsToPass.Add(moveEffect);
        }

        previousEffect = null;
        ResolutionManager.instance.LoadBattleEffect(effectsToPass);
    }
    BattleEffect previousEffect;
}
