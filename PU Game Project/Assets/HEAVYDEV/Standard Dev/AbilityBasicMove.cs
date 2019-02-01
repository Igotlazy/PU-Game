using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

[CreateAssetMenu(fileName = "New Basic Move", menuName = "Abilities/General/BasicMove")]
public class AbilityBasicMove : CharAbility {

    [Header("Selectors:")]
    [SerializeField]
    AbilityPrefabRef.BasicMoveSelectorData moveSelector = new AbilityPrefabRef.BasicMoveSelectorData();

    public override void Initialize(Unit givenUnit)
    {
        base.Initialize(givenUnit);

        castableAbilities.Add(new Action<EffectDataPacket>(Run));

        SelectorPacket firstSP = new SelectorPacket(SelectorPacket.SelectionType.Null, false)
        {
            selectorData = moveSelector
        };

        selectorPacketBaseData.Add(new List<SelectorPacket> { firstSP });
    }


    private void Run(EffectDataPacket effectPacket)
    {
        List<Vector3> path = new List<Vector3>();

        SelectorPacket relevantTargets = (SelectorPacket)effectPacket.GetValue("Targets", false)[0];
        foreach (Node currentNode in relevantTargets.TargetNodes) //Get Path Location Data
        {
            path.Add(currentNode.worldPosition);
        }

        //associatedUnit.CreatureScript.CurrentEnergy -= path.Count;



        for (int i = 0; i < path.Count; i++) //Store Path Location Data (not really needed in this case)
        {
            effectPacket.AppendValue("MovePath", path[i]);
        }
        effectPacket.AppendValue("MovingTarget", associatedUnit);



        List<BattleEffect> effectsToPass = new List<BattleEffect>(); //Effects to send to the Resolver/
        for (int i = 0; i < path.Count; i++) //Creation of Effects
        {
            EffectGridMove moveEffect = new EffectGridMove(effectPacket)
            {
                pathIndex = (Vector3)effectPacket.GetValue("MovePath", false)[i],
                moveSpeed = 5f,
            };
            moveEffect.moveTarget = (Unit)effectPacket.GetValue("MovingTarget", false)[0];
            moveEffect.finishedEffectAuxCall += base.PayEnergyCost;
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

    protected override void PayEnergyCost(EffectDataPacket givenPacket)
    {

    }
}
