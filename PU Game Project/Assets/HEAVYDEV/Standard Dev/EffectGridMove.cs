using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MHA.BattleBehaviours;

public class EffectGridMove : BattleEffect {

    public Vector3 pathIndex;
    public float moveSpeed = 5f;
    public LivingCreature moveTarget;

    public EffectGridMove(EffectDataPacket _effectData) : base(_effectData)
    {

    }


    public override void RunEffectImpl()
    {
        GridMove();
    }

    private void GridMove()
    {

        //damageTodeal

        if (!isCancelled)
        {
            if(moveTarget != null)
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

                new BBGridMoveAnim(pathIndex, moveTarget.gameObject, moveSpeed);
            }
            else
            {
                CancelEffect();
            }
            
        }
    }

    public override void CancelEffectImpl()
    {

    }
}
