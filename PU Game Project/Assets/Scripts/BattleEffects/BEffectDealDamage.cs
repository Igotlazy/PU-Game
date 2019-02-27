using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MHA.Events;

namespace MHA.BattleEffects
{
    public class BEffectDealDamage : BattleEffect
    {

        public string KEYdamageAttack;
        public Attack damageAttack;

        public string KEYdamageTarget;
        public Unit damageTarget;



        public BEffectDealDamage(GameEntity _source, Unit _damageTarget, Attack _damageAttack) : base(_source)
        {
            this.damageTarget = _damageTarget;
            this.damageAttack = _damageAttack;
        }
        public BEffectDealDamage(GameEntity _source, AbilityDataPacket _effectData, Unit _damageTarget, Attack _damageAttack) : base(_source, _effectData)
        {
            this.damageTarget = _damageTarget;
            this.damageAttack = _damageAttack;
        }


        protected override void RunEffectImpl()
        {
            DealDamage();
        }

        protected override void WarnEffect()
        {

        }

        private void DealDamage()
        {
            damageTarget.CreatureHit(damageAttack);

            EventFlags.EVENTTookDamage(this, new EventFlags.ETookDamageArgs(damageAttack.damageValue, (Unit)source, damageTarget));

            if (KEYdamageAttack != null)
            {
                effectData.AppendValue(KEYdamageAttack, damageAttack);
            }
            if (KEYdamageTarget != null)
            {
                effectData.AppendValue(KEYdamageTarget, damageTarget);
            }
        }

        protected override bool EffectSpecificCondition()
        {
            if (damageTarget != null)
            {
                return true;
            }
            return false;
        }

        protected override void CancelEffectImpl()
        {

        }
    }
}
