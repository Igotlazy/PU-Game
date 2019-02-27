using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MHA.BattleAnimations;

namespace MHA.BattleEffects
{
    public class BEffectFreeMove : BEffectMove
    {
        public Vector3 destination;
        public float moveSpeed = 5f;
        public bool destroyAtEnd;

        public BEffectFreeMove(GameEntity _source, GameObject _moveTarget, List<Vector3> _locations) : base(_source, _moveTarget, _locations)
        {

        }
        public BEffectFreeMove(GameEntity _source, AbilityDataPacket _effectPacket, GameObject _moveTarget, List<Vector3> _locations) : base(_source, _effectPacket, _moveTarget, _locations)
        {

        }

        protected override void MovementWarn()
        {
            throw new System.NotImplementedException();
        }

        protected override void MovementRun()
        {
            //Debug.Log("I'm going here: " + destination);
            new BAnimMoveToPos(this, destination, moveTarget, moveSpeed, destroyAtEnd);
        }

        protected override void CancelEffectImpl()
        {

        }

        protected override bool EffectSpecificCondition()
        {
            if (moveTarget != null)
            {
                return true;
            }
            return false;
        }
    }
}
