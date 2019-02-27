using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MHA.Events;

namespace MHA.BattleEffects
{
    public class BEffectTPInstant : BEffectTP
    {
        GameObject fireObjectRef;

        public string REPORTKEY;


        public BEffectTPInstant(GameEntity _source, SelectorPacket _givenPacket) : base(_source, _givenPacket)
        {

        }
        public BEffectTPInstant(GameEntity _source, AbilityDataPacket _effectData, SelectorPacket _givenPacket) : base(_source, _effectData, _givenPacket)
        {

        }

        protected override void TPorterWarn()
        {
            Debug.LogWarning("Instant WARN Event");
        }

        protected override void TPorterRun()
        {

            if (runIndex == 0) //If it's the first, instantiate the projectile.
            {
                //EventFlags.ANIMStartCastCALL(this, new EventFlags.ECastAnim());
            }
            EventFlags.ANIMStartCastCALL(this, new EventFlags.ECastAnim());

            InstantLogic();
        }

        private void InstantLogic()
        {

            Unit targetScript = givenTSpecs[runIndex].targetObj.GetComponent<Unit>();
            if (givenPacket.TargetNodes.Contains(targetScript.currentNode))
            {
                if (!givenPacket.selectorData.isPure)
                {
                    float result = CombatUtils.MainFireCalculation(givenTSpecs[runIndex].fireOrigin, targetScript.shotConnecter.transform.position, targetScript.partialCoverCheck.transform.position);
                    if (CombatUtils.PercentageCalculation(result))
                    {
                        effectData.AppendValue(REPORTKEY, givenTSpecs[runIndex].targetObj);

                        EventFlags.ANIMFinishCastCALL(this, new EventFlags.ECastAnim());
                        Debug.LogWarning("Instant HIT Event");
                    }
                    else
                    {
                        //Debug.Log("Instant MISSED Anim Event");
                        Debug.LogWarning("Instant MISSED Event");
                    }
                }
                else
                {
                    effectData.AppendValue(REPORTKEY, givenTSpecs[runIndex].targetObj);

                    EventFlags.ANIMFinishCastCALL(this, new EventFlags.ECastAnim());
                    Debug.LogWarning("Instant HIT Event");
                }
                TPorterFinishActive = true;
            }

            //if AoE foreach

            doEnding = true;
        }


        protected override void CancelEffectImpl()
        {

        }

        public void SoftEffectCancel()
        {
            PeekRecovery();
            runIndex++;

            if (runIndex >= givenTSpecs.Count)
            {
                RemoveSelfFromResolveList();
            }
        }

        protected override bool EffectSpecificCondition()
        {
            return true;
        }
    }
}
