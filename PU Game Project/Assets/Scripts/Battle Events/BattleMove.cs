//BattleEvent that handles pathfinding movement. Receives path data from Unit script. 

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleMove : BattleEvent {


    public int targetIndex;
    public GameObject moveTarget;
    public Vector3[] path = new Vector3[] { };
    public float speed;


    public BattleMove(GameObject receivedTarget, Vector3[] receivedPath, float receivedSpeed) : base()
    {
        this.moveTarget = receivedTarget;
        this.path = (Vector3[])receivedPath.Clone();
        this.speed = receivedSpeed;
    }


    protected override void BattleEventRunImpl()
    {
        bEventMonoBehaviour.StartCoroutine(FollowPath());
	}
    protected override void BattleEventPauseImpl()
    {

    }
    protected override void BattleEventResumeImpl()
    {

    }
    protected override void BattleEventCancelImpl()
    {
        bEventMonoBehaviour.StopCoroutine(FollowPath());
    }



    IEnumerator FollowPath()
    {
        Vector3 currentWaypoint = path[0];
        int lastPositionIndex = path.Length - 1;
        Vector3 lastPosition = path[lastPositionIndex];

        while (true)
        {
            while (IsPaused) {yield return null;} //Pauses Coroutine

            if (moveTarget.transform.position == currentWaypoint)
            {
                targetIndex++;
                if (targetIndex >= path.Length)
                {
                    break;
                }
                currentWaypoint = path[targetIndex];
            }

            moveTarget.transform.position = Vector3.MoveTowards(moveTarget.transform.position, currentWaypoint, speed * Time.deltaTime);
            yield return null;

        }

        Unit moveTargetScript = moveTarget.GetComponent<Unit>();
        moveTargetScript.currentNode = GridGen.instance.NodeFromWorldPoint(lastPosition); //So the player knows which Node they're on. 
        moveTargetScript.currentNode.IsOccupied = true; //Sets last Node to now be Occupied.

        IsFinished = true;
    }
}
