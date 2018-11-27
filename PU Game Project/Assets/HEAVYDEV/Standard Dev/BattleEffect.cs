using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class BattleEffect {

    public EffectDataPacket effectData; //Reference to data for this Effect.
    private Action tiedCancelEffect; //For closely related strings of effects that need to all get cancelled should one of them get cancelled. Add the Cancel Effect of the last one. 
    public Action TiedCancelEffect
    {
        get { return tiedCancelEffect; }
        set
        {
            tiedCancelEffect += value;
            //Makes it so when the effect that it's depending on is cancelled, it also will get cancelled.
        }
    }
    private Action<EffectDataPacket> dependentEffectCall; //Effect that this Effect is dependent on resolving in order to be called.
    public Action<EffectDataPacket> DependentEffectCall
    {
        get {return dependentEffectCall;}
        set
        {
            if(dependentEffectCall == null)
            {
                dependentEffectCall += value;
                //Makes it so when the effect that it's depending on is cancelled, it also will get cancelled.
            }
        }
    }
    public bool isCancelled;


    public BattleEffect(EffectDataPacket _effectData)
    {
        this.effectData = _effectData;   
    }


    public void RunEffect()
    {
        RunEffectImpl();
        if (!isCancelled)
        {
            FinishEffect();
        }
        
    }
    public abstract void RunEffectImpl();


    public void CancelEffect()
    {
        isCancelled = true;
        RemoveSelfFromResolveList();
        if(tiedCancelEffect != null)
        {
            tiedCancelEffect.Invoke();
        }
        CancelEffectImpl();
    }
    public abstract void CancelEffectImpl();

    public void FinishEffect()
    {
        if (!isCancelled)
        {
            RemoveSelfFromResolveList();
            if(dependentEffectCall != null) { dependentEffectCall.Invoke(effectData); }          
        }
    }


    public virtual void RemoveSelfFromResolveList()
    {
        if (ResolutionManager.instance.resolvingEffects.Contains(this))
        {
            ResolutionManager.instance.resolvingEffects.Remove(this);
        }
    }
}
