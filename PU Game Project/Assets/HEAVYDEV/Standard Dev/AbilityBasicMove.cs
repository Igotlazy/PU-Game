using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AbilityBasicMove : CharAbility {

    public List<Vector3> path = new List<Vector3>();
    public float speed = 5f;

    public AbilityBasicMove(LivingCreature livingCreature) : base (livingCreature)
    {
        castableAbilities.Add(new Action<int>(Initialize));
        targetCollectors.Add(new List<GameObject> {CharacterAbilityPrefabRef.instance.NodeCollectors[2]});
    }

    private void Initialize(int castIndex)
    {
        Debug.Log(abilityTargets[0].Count);
        foreach(Node currentNode in abilityTargets[0])
        {
            path.Add(currentNode.worldPosition);
        }
        Debug.Log(path.Count);

        associatedCreature.CurrentEnergy -= path.Count;

        EffectDataPacket effectPacket = new EffectDataPacket(associatedCreature, this, castIndex);

        for (int i = 0; i < path.Count; i++)
        {
            effectPacket.SetValueAtKey("MovePath", path[i]);
        }

        List<BattleEffect> effectsToPass = new List<BattleEffect>();

        for(int i = 0; i < path.Count; i++)
        {
            EffectGridMove moveEffect = new EffectGridMove(effectPacket)
            {
                pathIndex = (Vector3)effectPacket.GetValueAtKey("MovePath", false)[i],
                moveSpeed = speed,
                moveTarget = associatedCreature
            };


            if (previousEffect != null) { previousEffect.TiedCancelEffect += moveEffect.CancelEffect; } //If a move is cancelled, all of them down the line will also be cancelled. 
            previousEffect = moveEffect;

            effectsToPass.Add(moveEffect);
        }

        previousEffect = null;
        ResolutionManager.instance.LoadBattleEffect(effectsToPass);
        path.Clear();
    }
    BattleEffect previousEffect;
}
