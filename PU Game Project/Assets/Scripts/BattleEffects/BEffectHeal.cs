using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MHA.BattleEffects
{
    public class BEffectHeal : BattleEffect
    {

        float healAmount;

        public BEffectHeal(GameEntity _source) : base(_source)
        {

        }
        public BEffectHeal(GameEntity _source, AbilityDataPacket _effectData) : base(_source, _effectData)
        {

        }


        protected override void RunEffectImpl()
        {
            HealTarget();
        }

        protected override void WarnEffect()
        {
            Debug.Log("Heal: Warning Event Not Implemented");
        }

        private void HealTarget()
        {
            //target.Heal(healAmount);

            //effectData.SetValueAtKey("Healed Amount", (float)effectData.blackboard["Healed Amount"] + healAmount);
        }

        protected override bool EffectSpecificCondition()
        {
            return true;
        }

        protected override void CancelEffectImpl()
        {

        }

    }
}
