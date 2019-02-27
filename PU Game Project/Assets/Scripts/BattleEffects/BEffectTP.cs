using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MHA.Events;
using MHA.BattleAnimations;

namespace MHA.BattleEffects
{
    public abstract class BEffectTP : BattleEffect
    {
        public int runIndex;
        protected SelectorPacket givenPacket;
        public List<TargetSpecs> givenTSpecs;
        protected bool warnOnce;
        protected bool doEnding;

        public BEffectTP(GameEntity _source, SelectorPacket _givenPacket) : base(_source)
        {
            this.givenPacket = _givenPacket;
            this.givenTSpecs = givenPacket.targetObjectSpecs;

            if (givenPacket.selectorData.selectionType == SelectorData.SelectionType.AoE)
            {
                this.warnOnce = true;
            }
        }
        public BEffectTP(GameEntity _source, AbilityDataPacket _effectData, SelectorPacket _givenPacket) : base(_source, _effectData)
        {
            this.givenPacket = _givenPacket;
            this.givenTSpecs = givenPacket.targetObjectSpecs;

            if (givenPacket.selectorData.selectionType == SelectorData.SelectionType.AoE)
            {
                this.warnOnce = true;
            }
        }

        protected override void WarnEffect()
        {
            //Anim Event for Very Start of Cast
            if (!doEnding)
            {
                if (givenTSpecs.Count > 0)
                {
                    CameraMove();
                    PeekCheck();
                }
                TPorterWarn();

                if (warnOnce)
                {
                    TPorterWarnActive = false;
                }
            }
        }
        protected abstract void TPorterWarn();

        protected virtual void CameraMove()
        {
            new BAnimCameraMove(this, source.transform);
        }

        protected override void RunEffectImpl()
        {
            TPorterFinishActive = false;
            TPorterRemoveActive = false;

            if (!doEnding)
            {
                if (givenTSpecs.Count > 0)
                {
                    TPorterRun();
                }
                if (givenPacket.selectorData.selectionType == SelectorData.SelectionType.AoE)
                {
                    //Update Area with foreach. 
                }

            }
            else
            {
                PeekRecovery();
                runIndex++;
                doEnding = false;
            }

            if (runIndex > givenTSpecs.Count - 1)
            {
                //Anim Event for very End of Cast
                TPorterRemoveActive = true;
                Debug.Log("RemoveOverride - Normal: " + TPorterRemoveActive);
            }
        }
        protected abstract void TPorterRun();

        protected override void CancelEffectImpl()
        {

        }

        protected override bool EffectSpecificCondition()
        {
            return true;
        }

        //Called in the TPorterWarn.
        protected virtual void PeekCheck()
        {
            if (!givenPacket.selectorData.isPure && (givenPacket.selectorData.selectionType == SelectorData.SelectionType.Pick || givenPacket.selectorData.selectionType == SelectorData.SelectionType.AreaPick))
            {
                TargetSpecs currentSpec = givenTSpecs[runIndex];
                Vector3 targetShot = CombatUtils.GiveShotConnector(currentSpec.targetObj);
                Vector3 targetPartial = CombatUtils.GivePartialCheck(currentSpec.targetObj);
                CombatUtils.MainFireCalculation(currentSpec.fireOrigin, targetShot, targetPartial, out currentSpec.didPeek, out currentSpec.fireOrigin);

                if (currentSpec.didPeek)
                {
                    Unit sourceObj = (Unit)source;
                    EventFlags.ANIMStartPeekCALL(this, new EventFlags.EPeekStart(sourceObj, currentSpec.fireOrigin, sourceObj.gameObject.transform.position)); //EVENT
                }
            }
        }

        protected void PeekRecovery()
        {
            if (givenTSpecs[runIndex].didPeek && (givenPacket.selectorData.selectionType == SelectorData.SelectionType.Pick || givenPacket.selectorData.selectionType == SelectorData.SelectionType.AreaPick))
            {
                EventFlags.ANIMEndPeekCALL(this, new EventFlags.EPeekEnd((Unit)source));
            }
        }

        protected override void FinishEffect()
        {
            if (givenPacket.selectorData.selectionType == SelectorData.SelectionType.AoE)
            {
                if (runIndex + 1 > givenTSpecs.Count - 1) // <= Needs to know if the last TSpecs has been evaluated. runIndex updates too late, and PeekRecovery needs runIndex as well. 
                {
                    Debug.Log("Finish Porter AOE");
                    base.FinishEffect();
                }
            }
            else
            {
                Debug.Log("Finish Porter Target");
                base.FinishEffect();
            }
        }

    }
}
