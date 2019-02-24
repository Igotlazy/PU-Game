using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BasicMoveSelector : AbilitySelection {

    public Vector3[] path;
    int targetIndex;
    public bool hasSuccessfulPath;
    private RaycastHit hitInfo;
    private List<Node> allNodes = new List<Node>();

    protected void Start()
    {
        CursorController.instance.CursorNewNodeEVENT += SetMovePath;
    }

    protected override void InitializeImpl()
    {
        if(givenAbility.associatedEntity.entityType == GameEntity.EntityType.Unit)
        {
            Unit unit = (Unit)givenAbility.associatedEntity;

            if (unit.CurrentEnergy <= 0)
            {
                CancelSelection();
                return;
            }

            allNodes = Pathfinding.instance.DisplayAvailableMoves(unit.currentNode, unit.CurrentEnergy);
            foreach (Node currentNode in allNodes)
            {
                currentNode.IsSelectable = true;
            }
        }
        else
        {
            Debug.Log("NONE UNIT USING UNIT MOVEMENT");
            CancelSelection();
            return;
        }
    }

    protected override void Update()
    {
        base.Update();
        if (!isAIControlled)
        {
            if (Input.GetMouseButtonDown(1))
            {
                MadeSelection();
            }
        }
    }

    public void SetMovePath(Node givenNode) //Drawing of path happens in the pathfinding script. 
    {
        if (givenNode.IsSelectable)
        {
            //DrawIndicators.instance.ClearTileMatStates(true, false, false);
            SelectionPathRequest(givenNode.worldPosition);
        }
    }

    public void SelectionPathRequest(Vector3 targetPos) //Called by ClickSelection to get a path to the target. 
    {
        PathRequestManager.RequestPath(transform.position, targetPos, OnPathFound);
    }

    public void OnPathFound(Vector3[] newPath, bool pathSuccessful) //Loads script with relevant path data should the path return successful. 
    {
        hasSuccessfulPath = pathSuccessful;

        if (pathSuccessful)
        {
            path = newPath;
            targetIndex = 0;
        }
    }

    protected override void MadeSelectionImpl()
    {
        CursorController.instance.CursorNewNodeEVENT -= SetMovePath;
        if (path.Length > 0)
        {
            foreach (Vector3 currentVector in path)
            {
                collectedNodes.Add(GridGen.instance.NodeFromWorldPoint(currentVector));
            }
        }
        else
        {
            Debug.LogWarning("Path of Length 0 Given");
        }

        foreach (Node currentNode in allNodes)
        {
            currentNode.IsSelectable = false;
        }
    }
    protected override void CancelSelectionImpl()
    {
        foreach (Node currentNode in allNodes)
        {
            currentNode.IsSelectable = false;
        }
    }
    private void OnDestroy()
    {
        CursorController.instance.CursorNewNodeEVENT -= SetMovePath;
    }

    public void OnDrawGizmos() //Displays path as Gizmo line. 
    {
        if (path != null)
        {
            for (int i = targetIndex; i < path.Length; i++)
            {
                Gizmos.color = Color.black;
                Gizmos.DrawCube(path[i], Vector3.one);

                if (i == targetIndex)
                {
                    Gizmos.DrawLine(transform.position, path[i]);
                }
                else
                {
                    Gizmos.DrawLine(path[i - 1], path[i]);
                }
            }
        }
    }


}
