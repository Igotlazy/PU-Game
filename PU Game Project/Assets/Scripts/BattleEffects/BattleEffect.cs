using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace MHA.BattleEffects
{
    public abstract class BattleEffect
    {

        public GameEntity source;
        public int castIndex;
        public AbilityDataPacket effectData; //Reference to data for this Effect.

        public Action cancelEffectAuxCalls; //Methods that can be called when this effect is cancelled. For closely related strings of effects that need to all get cancelled should one of them get cancelled. Add the Cancel Effect of the last one to this.
        public Action<AbilityDataPacket> finishedEffectAuxCall; //Methods that can be called when this is effect finishes. 
        public Func<AbilityDataPacket, BattleEffect, bool> conditionCheck; //Method that checks wether the condition for activating still applies.
        public bool isCancelled;
        protected bool hasWarned;

        //For TPorters
        protected bool TPorterRemoveActive = true; //Turn to false for T porters to separate movements. 
        protected bool TPorterFinishActive = true;
        protected bool TPorterWarnActive = true;

        protected bool canBeCancelled = true;

        public int differentiator;



        public BattleEffect(GameEntity _source)
        {
            source = _source;
            CharAbility.totalCastIndex++;
            castIndex = CharAbility.totalCastIndex;
        }
        public BattleEffect(GameEntity _source, AbilityDataPacket _effectData)
        {
            source = _source;
            effectData = _effectData;
            castIndex = CharAbility.totalCastIndex;
        }


        public void RunEffect()
        {
            if (!hasWarned && TPorterWarnActive)
            {
                WarnEffect();
                /*
                if (TPorterWarnActive)
                {
                    WarnEffect();
                }
                */
                hasWarned = true;
            }
            else
            {
                bool conditionResult = true;
                if (!isCancelled && conditionCheck != null) //Checks set condition if the effect hasn't already been cancelled.
                {
                    if (effectData != null)
                    {
                        conditionResult = conditionCheck.Invoke(effectData, this);
                    }
                    else
                    {
                        Debug.LogError("WARNING: CALLING CONDITION CHECK WITHOUT DATA PACKET");
                    }
                }

                if (!isCancelled && conditionResult && EffectSpecificCondition()) //Does effect if it hasn't been cancelled, and all conditions have been met. 
                {
                    RunEffectImpl();

                    if (TPorterFinishActive)
                    {
                        FinishEffect();
                    }
                }

                if (TPorterRemoveActive)
                {
                    RemoveSelfFromResolveList();
                }
                else
                {
                    hasWarned = false;
                }
            }
        }
        protected abstract void WarnEffect();
        protected abstract bool EffectSpecificCondition();
        protected abstract void RunEffectImpl();


        public void CancelEffect()
        {
            if (canBeCancelled)
            {
                Debug.Log("Effect Cancelled");

                isCancelled = true;

                RemoveSelfFromResolveList();

                if (cancelEffectAuxCalls != null)
                {
                    cancelEffectAuxCalls.Invoke();
                }

                CancelEffectImpl();
            }
        }
        protected abstract void CancelEffectImpl();

        protected virtual void FinishEffect()
        {
            if (!isCancelled)
            {
                if (finishedEffectAuxCall != null)
                {
                    if (effectData != null)
                    {
                        finishedEffectAuxCall.Invoke(effectData);
                    }
                    else
                    {
                        Debug.LogError("WARNING: CALLING FINISH EFFECT WITHOUT DATAPACKET");
                    }
                }
            }
        }


        protected virtual void RemoveSelfFromResolveList()
        {
            if (ResolutionManager.instance.resolvingEffects.Contains(this))
            {
                ResolutionManager.instance.resolvingEffects.Remove(this);
            }
        }

        public enum EffectType
        {
            Basic,
            Movement,
            Custom,
        }
    }
}
