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

    }


    public override void RunEffectImpl()
    {
        FreeMove();
    }

    public override void WarnEffect()
    {
        //Debug.Log("Grid Move: Warning Event Not Implemented");
    }

    private void FreeMove()
    {
        if (moveTarget != null)
        {
            //Debug.Log("I'm going here: " + destination);
            new AnimMoveToPos(destination, moveTarget.gameObject, moveSpeed, destroyAtEnd);
        }
    }

    public override void CancelEffectImpl()
    {

    }
}
