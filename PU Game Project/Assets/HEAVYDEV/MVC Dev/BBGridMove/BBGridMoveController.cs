using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MHA.BattleBehaviours
{
    public class BBGridMoveController : BattleBehaviourController
    {

        public BBGridMoveModel gridMoveModel;

        public BBGridMoveController(BBGridMoveModel givenModel) : base(givenModel)
        {
            gridMoveModel = givenModel;
            for (int i = 0; i < givenModel.path.Length; i++)
            {
                RunBehaviourImplList.Add(GridMove);
            }
        }

        int targetIndex;

        protected void GridMove()
        {
            Vector3 newGridPos = gridMoveModel.path[targetIndex];
            targetIndex++;

            Unit moveTargetScript = gridMoveModel.moveTarget.GetComponent<Unit>();
            Node newNode = GridGen.instance.NodeFromWorldPoint(newGridPos);

            if (moveTargetScript != null)
            {
                moveTargetScript.currentNode.IsOccupied = false;
                moveTargetScript.currentNode = newNode; //So the player knows which Node they're on. 
            }
            newNode.IsOccupied = true;
            newNode.occupant = gridMoveModel.moveTarget; //Sets last Node to now be Occupied.

            new BBGridMoveAnim(newGridPos, gridMoveModel.moveTarget, gridMoveModel.speed);

            //EVENT FOR MOVEMENT



            DrawIndicators.instance.ClearTileMatStates(true, true, true);
            //ClickSelection.instance.DrawMoveZone();
        }

        protected override void CancelBehaviourImpl()
        {
            throw new System.NotImplementedException();
        }

        protected override void FinishBehaviourImpl()
        {
            //throw new System.NotImplementedException();
        }

    }
}
