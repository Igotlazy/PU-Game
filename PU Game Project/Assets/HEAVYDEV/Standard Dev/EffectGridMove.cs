using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MHA.BattleBehaviours;

public class EffectGridMove : BattleEffect {

    public Vector3 pathIndex;
    public float moveSpeed = 3.5f;
    public List<LivingCreature> moveTarget = new List<LivingCreature>();
    public bool destroyAtEnd;

    public EffectGridMove(EffectDataPacket _effectData, int _runAmount) : base(_effectData, _runAmount)
    {

    }


    protected override void RunEffectImpl(int index)
    {
        GridMove(index);
    }

    protected override void WarnEffect(int index)
    {
        Debug.Log("Grid Move: Warning Event Not Implemented");
    }

    private void GridMove(int index)
    {
        Debug.Log("Index: " + index);
        if (moveTarget[index] != null)
        {
            Unit moveTargetScript = moveTarget[index].gameObject.GetComponent<Unit>();
            Node newNode = GridGen.instance.NodeFromWorldPoint(pathIndex);

            if (moveTargetScript != null)
            {
                moveTargetScript.currentNode.IsOccupied = false;
                moveTargetScript.currentNode = newNode; //So the player knows which Node they're on. 
            }
            newNode.IsOccupied = true;
            newNode.occupant = moveTarget[index].gameObject; //Sets last Node to now be Occupied.

            new AnimMoveToPos(pathIndex, moveTarget[index].gameObject, moveSpeed, destroyAtEnd);
        }
    }

    protected override bool EffectSpecificCondition(int index)
    {
        if(moveTarget[runTracker] != null)
        {
            return true;
        }
        return false;
    }

    protected override void CancelEffectImpl()
    {

    }


}
