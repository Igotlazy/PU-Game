using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MHA.BattleBehaviours;

public class EffectFreeMove : BattleEffect
{
    public Vector3 destination;
    public float moveSpeed = 5f;
    public List<GameObject> moveTarget = new List<GameObject>();
    public bool destroyAtEnd;

    public EffectFreeMove(EffectDataPacket _effectData, int _runAmount) : base(_effectData, _runAmount)
    {

    }


    protected override void RunEffectImpl(int index)
    {
        FreeMove(index);
    }

    protected override void WarnEffect(int index)
    {
        //Debug.Log("Grid Move: Warning Event Not Implemented");
    }

    private void FreeMove(int index)
    {
        //Debug.Log("I'm going here: " + destination);
        new AnimMoveToPos(destination, moveTarget[index], moveSpeed, destroyAtEnd);
    }

    protected override bool EffectSpecificCondition(int index)
    {
        if(moveTarget[index] != null)
        {
            return true;
        }
        return false;
    }

    protected override void CancelEffectImpl()
    {

    }
}
