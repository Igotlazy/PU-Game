using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BasicMoveSelector : AttackSelection {

    public Vector3[] path;
    int targetIndex;
    public bool hasSuccessfulPath;
    private Node lastHitNode;

    protected override void Update()
    {
        base.Update();

        SetMovePath();
    }

    private void SetMovePath() //Drawing of path happens in the pathfinding script. 
    {
        RaycastHit hitInfo = new RaycastHit();
        bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo, 500f, CombatUtils.gameTerrainMask);

        if (hit && (GridGen.instance.NodeFromWorldPoint(hitInfo.point) != lastHitNode) && hitInfo.transform.gameObject.CompareTag("Tile") && hitInfo.collider.gameObject.activeInHierarchy) //Makes it so the script doesnt run every frame, only when a new tile is hovered over. 
        {
            lastHitNode = GridGen.instance.NodeFromWorldPoint(hitInfo.point);
            //Debug.Log(hitInfo.point);

            if (lastHitNode != null && lastHitNode.IsSelectable)
            {
                SelectionPathRequest(hitInfo.point);
            }
            else
            {
                path = null;
                hasSuccessfulPath = false;
                DrawIndicators.instance.ClearTileMatStates(true, false, false);
            }
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

    AbilityBasicMove moveAbility;


    public override void MadeSelectionImpl()
    {
        if(path.Length > 0)
        {
            foreach (Vector3 currentVector in path)
            {
                collectedNodes.Add(GridGen.instance.NodeFromWorldPoint(currentVector));
            }
        }
        else
        {

        }
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
