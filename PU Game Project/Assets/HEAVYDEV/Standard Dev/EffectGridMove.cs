using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MHA.BattleBehaviours;

public class EffectGridMove : EffectMove {

    public float moveSpeed = 3.5f;
    public bool destroyAtEnd;

    public EffectGridMove(GameEntity _source, GameObject _moveTarget, List<Vector3> _locations) : base(_source, _moveTarget, _locations)
    {

    }
    public EffectGridMove(GameEntity _source, EffectDataPacket _effectData, GameObject _moveTarget, List<Vector3> _locations) : base(_source, _effectData, _moveTarget, _locations)
    {

    }


    protected override void MovementWarn()
    {
        //Debug.Log("Grid Move: Warning Event Not Implemented");
    }

    protected override void MovementRun()
    {
        if (moveTarget != null)
        {
            Unit moveTargetScript = moveTarget.gameObject.GetComponent<Unit>();
            Node newNode = GridGen.instance.NodeFromWorldPoint(locations[moveIndex]);

            if (moveTargetScript != null)
            {
                moveTargetScript.currentNode.IsOccupied = false;
                moveTargetScript.currentNode = newNode; //So the player knows which Node they're on. 
            }
            newNode.IsOccupied = true;
            newNode.occupant = moveTarget.gameObject; //Sets last Node to now be Occupied.

            new AnimMoveToPos(this, locations[moveIndex], moveTarget.gameObject, moveSpeed, destroyAtEnd);

        }
    }

    protected override void CancelEffectImpl()
    {

    }

    protected override bool EffectSpecificCondition()
    {
        if (moveTarget != null)
        {
            return true;
        }
        return false;
    }
}
