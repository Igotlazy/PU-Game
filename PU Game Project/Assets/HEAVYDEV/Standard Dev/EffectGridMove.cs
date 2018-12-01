using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MHA.BattleBehaviours;

public class EffectGridMove : BattleEffect {

    public Vector3 pathIndex;
    public float moveSpeed = 5f;
    public LivingCreature moveTarget;
    public bool destroyAtEnd;

    public EffectGridMove(EffectDataPacket _effectData) : base(_effectData)
    {

    }


    public override void RunEffectImpl()
    {
        GridMove();
    }

    public override void WarnEffect()
    {
        Debug.Log("Grid Move: Warning Event Not Implemented");
    }

    private void GridMove()
    {
        if (moveTarget != null)
        {
            Unit moveTargetScript = moveTarget.gameObject.GetComponent<Unit>();
            Node newNode = GridGen.instance.NodeFromWorldPoint(pathIndex);

            if (moveTargetScript != null)
            {
                moveTargetScript.currentNode.IsOccupied = false;
                moveTargetScript.currentNode = newNode; //So the player knows which Node they're on. 
            }
            newNode.IsOccupied = true;
            newNode.occupant = moveTarget.gameObject; //Sets last Node to now be Occupied.

            new AnimMoveToPos(pathIndex, moveTarget.gameObject, moveSpeed, destroyAtEnd);
        }
    }

    public override void CancelEffectImpl()
    {

    }
}
