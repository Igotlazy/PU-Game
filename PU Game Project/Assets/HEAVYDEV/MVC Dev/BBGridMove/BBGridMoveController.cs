using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BBGridMoveController : BattleBehaviourController {

    public BBGridMoveModel gridMoveModel;

    public BBGridMoveController(BBGridMoveModel givenModel)
    {
        gridMoveModel = givenModel;
    }

    int targetIndex;

    protected override IEnumerator RunBehaviourImpl()
    {
        Vector3 currentWaypoint = gridMoveModel.path[0];
        int lastPositionIndex = gridMoveModel.path.Length - 1;
        Vector3 lastPosition = gridMoveModel.path[lastPositionIndex];

        while (true)
        {
            yield return attachedBattleEvent.bEventMonoBehaviour.StartCoroutine(attachedBattleEvent.ALLOWINTERRUPT(0f)); //Pauses Coroutine

            if (gridMoveModel.moveTarget.transform.position == currentWaypoint)
            {
                targetIndex++;
                if (targetIndex >= gridMoveModel.path.Length)
                {
                    break;
                }
                currentWaypoint = gridMoveModel.path[targetIndex];
            }

            gridMoveModel.moveTarget.transform.position = Vector3.MoveTowards(gridMoveModel.moveTarget.transform.position, currentWaypoint, gridMoveModel.speed * Time.deltaTime);
        }

        Unit moveTargetScript = gridMoveModel.moveTarget.GetComponent<Unit>();
        moveTargetScript.currentNode = GridGen.instance.NodeFromWorldPoint(lastPosition); //So the player knows which Node they're on. 
        moveTargetScript.currentNode.IsOccupied = true;
        moveTargetScript.currentNode.occupant = gridMoveModel.moveTarget; //Sets last Node to now be Occupied.

        DrawIndicators.instance.ClearTileMatStates(true, true, true);
        ClickSelection.instance.DrawMoveZone();
        FinishBehaviour();
        Debug.Log("Finished Controller");
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
