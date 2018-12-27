using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class AbilityBasicMove : CharAbility {


    public AbilityBasicMove(LivingCreature livingCreature) : base (livingCreature)
    {
        castableAbilities.Add(new Action<EffectDataPacket>(Initialize));

        targetPacketBaseData.Add(new List<TargetPacket> { new TargetPacket() });

        targetSelectors.Add(new List<GameObject> {AbilityPrefabRef.instance.GiveNodeCollectorPrefab(AbilityPrefabRef.instance.BasicMoveSelector)});
    }


    private void Initialize(EffectDataPacket effectPacket)
    {
        List<Vector3> path = new List<Vector3>();

        TargetPacket relevantTargets = (TargetPacket)effectPacket.GetValue("Targets", false)[0];
        foreach (Node currentNode in relevantTargets.TargetNodes) //Get Path Location Data
        {
            path.Add(currentNode.worldPosition);
        }

        associatedCreature.CurrentEnergy -= path.Count;



        for (int i = 0; i < path.Count; i++) //Store Path Location Data (not really needed in this case)
        {
            effectPacket.AppendValue("MovePath", path[i]);
        }
        effectPacket.AppendValue("MovingTarget", associatedCreature);



        List<BattleEffect> effectsToPass = new List<BattleEffect>(); //Effects to send to the Resolver/
        for (int i = 0; i < path.Count; i++) //Creation of Effects
        {
            EffectGridMove moveEffect = new EffectGridMove(effectPacket, 1)
            {
                pathIndex = (Vector3)effectPacket.GetValue("MovePath", false)[i],
                moveSpeed = 3.5f,
            };
            moveEffect.moveTarget.Add((LivingCreature)effectPacket.GetValue("MovingTarget", false)[0]);
            //moveEffect.conditionCheck += FreeMoveCONDITION;

            effectsToPass.Add(moveEffect);
        }

        CombatUtils.MakeEffectsDependent(effectsToPass);
        ResolutionManager.instance.LoadBattleEffect(effectsToPass);
    }

    /*
    private bool FreeMoveCONDITION(EffectDataPacket givenPacket, BattleEffect givenEffect)
    {
        LivingCreature moveTarget = (LivingCreature)givenPacket.GetValueAtKey("MovingTarget", false)[givenEffect.runTracker];
        if(moveTarget != null)
        {
            return true;
        }
        return false;
    }
    */
}
