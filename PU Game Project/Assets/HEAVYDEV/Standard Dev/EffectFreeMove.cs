using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MHA.BattleBehaviours;

public class EffectFreeMove : EffectMove
{
    public Vector3 destination;
    public float moveSpeed = 5f;
    public bool destroyAtEnd;

    public EffectFreeMove(EffectDataPacket _effectData, GameObject _moveTarget, List<Vector3> _locations) : base(_effectData, _moveTarget, _locations)
    {

    }

    protected override void MovementWarn()
    {
        throw new System.NotImplementedException();
    }

    protected override void MovementRun()
    {
        //Debug.Log("I'm going here: " + destination);
        new AnimMoveToPos(destination, moveTarget, moveSpeed, destroyAtEnd);
    }

    protected override void CancelEffectImpl()
    {

    }

    protected override bool EffectSpecificCondition()
    {
        if(moveTarget != null)
        {
            return true;
        }
        return false;
    }
}
