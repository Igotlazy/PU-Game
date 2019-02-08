using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class EffectMove : BattleEffect
{
    public GameObject moveTarget;
    public List<Vector3> locations;
    public int moveIndex;

    public EffectMove(EffectDataPacket _effectData, GameObject _moveTarget, List<Vector3> _locations) : base(_effectData)
    {
        moveTarget = _moveTarget;
        locations = _locations;
        TPorterRemoveOverride = false;
    }

    protected override void WarnEffect()
    {
        MovementWarn();
    }
    protected abstract void MovementWarn();

    protected override void RunEffectImpl()
    {
        ClearResolveStackOfMovement();
        MovementRun();
        moveIndex++;
        if(moveIndex >= locations.Count)
        {
            TPorterRemoveOverride = true;
        }
        
    }
    protected abstract void MovementRun();

    protected override void CancelEffectImpl()
    {
        
    }

    protected override bool EffectSpecificCondition()
    {
        return true;
    }

    protected virtual void ClearResolveStackOfMovement()
    {
        List<BattleEffect> resolvingEffects = new List<BattleEffect>();
        foreach(BattleEffect currEffect in ResolutionManager.instance.resolvingEffects)
        {
            resolvingEffects.Add(currEffect);
        }

                   
        for (int i = resolvingEffects.Count - 1; i >= 0; i--)
        {
            try
            {
                EffectMove movingEffect = (EffectMove)resolvingEffects[i];
                if (movingEffect.moveTarget == this.moveTarget && movingEffect != this && resolvingEffects.Contains(movingEffect))
                {
                    movingEffect.CancelEffect();
                    Debug.Log("REMOVED MOVEMENT EFFECT");
                }
            }
            catch(InvalidCastException)
            {
                continue;
            }
        }
    }
}
