using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class BattleEffect {

    public EffectDataPacket effectData; //Reference to data for this Effect.

    private Action cancelEffectAuxCalls; //Methods that can be called when this effect is cancelled. For closely related strings of effects that need to all get cancelled should one of them get cancelled. Add the Cancel Effect of the last one to this.
    public Action CancelEffectAuxCalls
    {
        get { return cancelEffectAuxCalls; }
        set
        {
            cancelEffectAuxCalls += value;
            //Makes it so when the effect that it's depending on is cancelled, it also will get cancelled.
        }
    }
    private Action<EffectDataPacket> finishedEffectAuxCall; //Methods that can be called when this is effect finishes. 
    public Action<EffectDataPacket> FinishedEffectAuxCall
    {
        get {return finishedEffectAuxCall;}
        set
        {
            finishedEffectAuxCall += value;
        }
    }
    public bool isCancelled;
    public bool isFinished;


    public BattleEffect(EffectDataPacket _effectData)
    {
        this.effectData = _effectData;   
    }


    public void RunEffect()
    {
        WarnEffect();

        if (!isCancelled)
        {
            RunEffectImpl();

            FinishEffect();
        }        
    }
    public abstract void WarnEffect();
    public abstract void RunEffectImpl();


    public void CancelEffect()
    {
        Debug.Log("Effect Cancelled");
        isCancelled = true;
        RemoveSelfFromResolveList();
        if(cancelEffectAuxCalls != null)
        {
            cancelEffectAuxCalls.Invoke();
        }
        CancelEffectImpl();
    }
    public abstract void CancelEffectImpl();

    private void FinishEffect()
    {
        if (!isCancelled)
        {
            RemoveSelfFromResolveList();
            if(finishedEffectAuxCall != null) { finishedEffectAuxCall.Invoke(effectData); }          
        }
        isFinished = true;
    }


    protected virtual void RemoveSelfFromResolveList()
    {
        if (ResolutionManager.instance.resolvingEffects.Contains(this))
        {
            ResolutionManager.instance.resolvingEffects.Remove(this);
        }
    }
}
