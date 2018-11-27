using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicMoveSelector : MonoBehaviour {

    public Vector3[] path;
    int targetIndex;
    public bool hasSuccessfulPath;

    public GameObject movingTarget;
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SelectionPathRequest(Vector3 targetPos) //Called by ClickSelection to get a path to the target. 
    {
        PathRequestManager.RequestPath(movingTarget.transform.position, targetPos, OnPathFound);
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

    public void RunFollowPath() 
    {
        if (hasSuccessfulPath)
        {
            moveAbility = new AbilityBasicMove(GetComponent<LivingCreature>())
            {
                moveTarget = GetComponent<LivingCreature>(),
                path = path
            };
            moveAbility.CastAbility(0);
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
