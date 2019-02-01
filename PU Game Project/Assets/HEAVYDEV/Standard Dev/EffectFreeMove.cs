using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MHA.BattleBehaviours;

public class EffectFreeMove : BattleEffect
{
    public Vector3 destination;
    public float moveSpeed = 5f;
    public GameObject moveTarget;
    public bool destroyAtEnd;

    public EffectFreeMove(EffectDataPacket _effectData) : base(_effectData)
    {
        setEffectType = EffectType.Movement;
    }


    protected override void RunEffectImpl()
    {
        FreeMove();
    }

    protected override void WarnEffect()
    {
        //Debug.Log("Grid Move: Warning Event Not Implemented");
    }

    private void FreeMove()
    {
        //Debug.Log("I'm going here: " + destination);
        new AnimMoveToPos(destination, moveTarget, moveSpeed, destroyAtEnd);
    }

    protected override bool EffectSpecificCondition()
    {
        if(moveTarget != null)
        {
            return true;
        }
        return false;
    }

    protected override void CancelEffectImpl()
    {

    }
}
