using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace MHA.BattleEffects
{
    public abstract class BEffectMove : BattleEffect
    {
        public GameObject moveTarget;
        public List<Vector3> locations;
        public int moveIndex;

        public BEffectMove(GameEntity _source, GameObject _moveTarget, List<Vector3> _locations) : base(_source)
        {
            moveTarget = _moveTarget;
            locations = _locations;
            TPorterRemoveActive = false;
        }
        public BEffectMove(GameEntity _source, AbilityDataPacket _effectData, GameObject _moveTarget, List<Vector3> _locations) : base(_source, _effectData)
        {
            moveTarget = _moveTarget;
            locations = _locations;
            TPorterRemoveActive = false;
        }

        protected override void WarnEffect()
        {
            MovementWarn();
        }
        protected abstract void MovementWarn();

        protected override void RunEffectImpl()
        {
            ClearResolveStackOfMovement();
            MovementRun();
            moveIndex++;
            if (moveIndex >= locations.Count)
            {
                TPorterRemoveActive = true;
            }

        }
        protected abstract void MovementRun();

        protected override void CancelEffectImpl()
        {

        }

        protected override bool EffectSpecificCondition()
        {
            return true;
        }

        protected virtual void ClearResolveStackOfMovement()
        {
            List<BattleEffect> resolvingEffects = new List<BattleEffect>();
            foreach (BattleEffect currEffect in ResolutionManager.instance.resolvingEffects)
            {
                resolvingEffects.Add(currEffect);
            }


            for (int i = resolvingEffects.Count - 1; i >= 0; i--)
            {
                try
                {
                    BEffectMove movingEffect = (BEffectMove)resolvingEffects[i];
                    if (movingEffect.moveTarget == this.moveTarget && movingEffect != this && resolvingEffects.Contains(movingEffect))
                    {
                        movingEffect.CancelEffect();
                        Debug.Log("REMOVED MOVEMENT EFFECT");
                    }
                }
                catch (InvalidCastException)
                {
                    continue;
                }
            }
        }
    }
}
