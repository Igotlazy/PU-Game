using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AbilityBasicMove : CharAbility {

    public Vector3[] path;
    public LivingCreature moveTarget;
    public float speed = 5f;

    public AbilityBasicMove(LivingCreature livingCreature) : base (livingCreature)
    {
        castableAbilities.Add(new Action<int>(Initialize));


    }

    private void Initialize(int castIndex)
    {
        EffectDataPacket effectPacket = new EffectDataPacket(moveTarget, this, castIndex);

        for (int i = 0; i < path.Length; i++)
        {
            effectPacket.SetValueAtKey("MovePath", path[i]);
        }

        List<BattleEffect> effectsToPass = new List<BattleEffect>();

        for(int i = 0; i < path.Length; i++)
        {
            EffectGridMove moveEffect = new EffectGridMove(effectPacket)
            {
                pathIndex = (Vector3)effectPacket.GetValueAtKey("MovePath", false)[i],
                moveSpeed = speed,
                moveTarget = moveTarget,                 
            };


            if (previousEffect != null) { previousEffect.TiedCancelEffect += moveEffect.CancelEffect; } //If a move is cancelled, all of them down the line will also be cancelled. 
            previousEffect = moveEffect;

            effectsToPass.Add(moveEffect);
        }

        previousEffect = null;
        ResolutionManager.instance.LoadBattleEffect(effectsToPass);
    }
    BattleEffect previousEffect;
}
