using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MHA.BattleAnimations;

namespace MHA.BattleEffects
{
    public class BEffectDeath : BattleEffect
    {
        Unit deathTarget;

        public BEffectDeath(GameEntity _source, Unit _deathTarget) : base(_source)
        {
            deathTarget = _deathTarget;
        }
        public BEffectDeath(GameEntity _source, AbilityDataPacket _effectData, Unit _deathTarget) : base(_source, _effectData)
        {
            deathTarget = _deathTarget;
        }

        protected override void WarnEffect()
        {
            Debug.Log("Warn Death");
        }

        protected override void RunEffectImpl()
        {
            deathTarget.amDead = true;
            deathTarget.currentNode.IsOccupied = false;
            deathTarget.currentNode.occupant = null;
            ReferenceObjects.RemovePlayerFromLists(deathTarget.gameObject);

            new BAnimDeath(this, deathTarget);
        }

        protected override void CancelEffectImpl()
        {

        }

        protected override bool EffectSpecificCondition()
        {
            return true;
        }


    }
}
