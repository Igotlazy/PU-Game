using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MHA.BattleEffects;

namespace MHA.BattleEffects
{
    public class BEffectApplyBuff : BattleEffect
    {

        Unit hitTarget;
        Buff givenBuff;

        public BEffectApplyBuff(GameEntity _source, Unit _hitTarget, Buff _givenBuff) : base(_source)
        {
            hitTarget = _hitTarget;
            givenBuff = _givenBuff;
        }
        public BEffectApplyBuff(GameEntity _source, AbilityDataPacket _effectData, Unit _hitTarget, Buff _givenBuff) : base(_source, _effectData)
        {
            hitTarget = _hitTarget;
            givenBuff = _givenBuff;
        }



        protected override void WarnEffect()
        {
            Debug.Log("Buff Warning");
        }

        protected override void RunEffectImpl()
        {
            if (hitTarget != null)
            {
                hitTarget.AddBuff(givenBuff);
                givenBuff.BuffInitialApplication();
            }
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
