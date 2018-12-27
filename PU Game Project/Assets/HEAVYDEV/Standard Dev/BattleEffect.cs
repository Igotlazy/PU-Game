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
    bool hasWarned;

    public int runTracker;
    public int runAmount;
    public bool isInternalCancelDependent;

    public List<bool> startHitCheck = new List<bool>();
    public List<bool> finishHitCheck = new List<bool>();


    public BattleEffect(EffectDataPacket _effectData, int _runAmount)
    {
        this.effectData = _effectData;
        this.runAmount = _runAmount;

        for(int i = 0; i < _runAmount; i++)
        {
            startHitCheck.Add(true);
            finishHitCheck.Add(true);
        }
    }


    public void RunEffect()
    {
        if(runAmount <= 0)
        {
            Debug.LogAssertion("WARNING: Effect Was Given Run Amount of 0 or less");
            RemoveSelfFromResolveList();
            return;
        }
        if (!hasWarned)
        {
            WarnEffect(runTracker);
            hasWarned = true;
        }
        else
        {
            if (startHitCheck[runTracker] == false)
            {
                runTracker++;
                if (runTracker > runAmount - 1)
                {
                    RemoveSelfFromResolveList();
                }
                else
                {
                    hasWarned = false;
                }

                isCancelled = false;
                Debug.LogWarning("MISSED");
                //Add Miss Animation;
                return;
            }

            bool conditionResult = true;
            if (!isCancelled && conditionCheck != null) //Checks set condition if the effect hasn't already been cancelled.
            {
                conditionResult = conditionCheck.Invoke(effectData, this);
            }

            if (!isCancelled && conditionResult && EffectSpecificCondition(runTracker)) //Does effect if it hasn't been cancelled, and all conditions have been met. 
            {
                RunEffectImpl(runTracker);

                if (finishHitCheck[runTracker] == true)
                {
                    FinishEffect();
                }
                else
                {
                    Debug.LogWarning("MISSED");
                }
                
            }
            else
            {
                isCancelled = false;
            }

            runTracker++;
            if (runTracker > runAmount - 1)
            {
                RemoveSelfFromResolveList();
            }
            else
            {
                hasWarned = false;
            }
        }
    }
    protected abstract void WarnEffect(int index);
    protected abstract bool EffectSpecificCondition(int index);
    protected abstract void RunEffectImpl(int index);


    public void CancelEffect()
    {
        Debug.Log("Effect Cancelled");

        isCancelled = true;
        if (isInternalCancelDependent)
        {
            RemoveSelfFromResolveList();
        }

        if(cancelEffectAuxCalls!= null)
        {
            cancelEffectAuxCalls.Invoke();
        }

        CancelEffectImpl();
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
}
