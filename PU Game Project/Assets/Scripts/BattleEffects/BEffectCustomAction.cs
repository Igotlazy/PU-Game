using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace MHA.BattleEffects
{
    public class BEffectCustomAction : BattleEffect
    {
        Action actionToRun;
        Action actionToWarn;

        public BEffectCustomAction(GameEntity _source, Action _givenAction) : base(_source)
        {
            actionToRun = _givenAction;
            hasWarned = true;
            canBeCancelled = false;
        }
        public BEffectCustomAction(GameEntity _source, AbilityDataPacket _effectData, Action _givenWarnAction, Action _givenRunAction) : base(_source, _effectData)
        {
            actionToRun = _givenRunAction;
            actionToWarn = _givenWarnAction;
            canBeCancelled = false;
        }

        protected override void WarnEffect()
        {
            actionToWarn.Invoke();
        }

        protected override void RunEffectImpl()
        {
            actionToRun.Invoke();
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
