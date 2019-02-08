using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class BattleEffect {

    public EffectDataPacket effectData; //Reference to data for this Effect.

    public Action cancelEffectAuxCalls; //Methods that can be called when this effect is cancelled. For closely related strings of effects that need to all get cancelled should one of them get cancelled. Add the Cancel Effect of the last one to this.
    public Action<EffectDataPacket> finishedEffectAuxCall; //Methods that can be called when this is effect finishes. 
    public Func<EffectDataPacket, BattleEffect, bool> conditionCheck; //Method that checks wether the condition for activating still applies.
    public bool isCancelled;
    protected bool hasWarned;

    //For TPorters
    protected bool TPorterRemoveOverride = true; //Turn to false for T porters to separate movements. 
    protected bool TPorterFinishOverride = true;
    protected bool TPorterWarnOverride = true;

    protected bool canBeCancelled = true;

    public int differentiator;



    public BattleEffect(EffectDataPacket _effectData)
    {
        effectData = _effectData;
    }


    public void RunEffect()
    {
        if (!hasWarned)
        {
            if (TPorterWarnOverride)
            {
                WarnEffect();
            }
            hasWarned = true;
        }
        else
        {
            bool conditionResult = true;
            if (!isCancelled && conditionCheck != null) //Checks set condition if the effect hasn't already been cancelled.
            {
                conditionResult = conditionCheck.Invoke(effectData, this);
            }

            if (!isCancelled && conditionResult && EffectSpecificCondition()) //Does effect if it hasn't been cancelled, and all conditions have been met. 
            {
                RunEffectImpl();

                if (TPorterFinishOverride)
                {
                    FinishEffect();
                }
            }

            if (TPorterRemoveOverride)
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

    protected void FinishEffect()
    {
        if (!isCancelled)
        {
            if(finishedEffectAuxCall != null) { finishedEffectAuxCall.Invoke(effectData); }          
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
