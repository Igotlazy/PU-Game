//General methods a Unit should have access too.

using UnityEngine;
using System.Collections;
using Cinemachine;

public class Unit : MonoBehaviour {

	public float speed = 25f;

	public Vector3[] path;
	int targetIndex;

    public Node currentNode;
    public LayerMask tileLayerMask;

    public bool hasSuccessfulPath;
    public int teamValue;

    public CinemachineVirtualCamera unitCamera;
    public Transform centerPoint;
    public bool[,] test = new bool[5,5];

    [Space]
    [Header("SHOT RESPONDERS:")]
    public GameObject shotConnecter;
    public GameObject partialCoverCheck;

    [Space]
    public GameObject projectile;


    private void Awake()
    {
        ReferenceObjects.AddToPlayerList(this.gameObject);
    }

    void Start()
    {
        StartNodeFind();
    }

    private void StartNodeFind() //Gives the unit reference to the Node below it.
    {
        Node foundNode = GridGen.instance.NodeFromWorldPoint(transform.position);
        currentNode = foundNode;
        if (foundNode.IsWalkable)
        {
            foundNode.IsOccupied = true;
        }
    }

    public void SelectionPathRequest(Vector3 targetPos) //Called by ClickSelection to get a path to the target. 
    {
        PathRequestManager.RequestPath(transform.position, targetPos, OnPathFound);
    }

	public void OnPathFound(Vector3[] newPath, bool pathSuccessful)
    {
        hasSuccessfulPath = pathSuccessful;

		if (pathSuccessful)
        {
			path = newPath;
			targetIndex = 0;
			//StopCoroutine("FollowPath");
			//StartCoroutine("FollowPath");
		}
	}

    public void RunFollowPath() //Used by other scripts to get the player to follow the loaded path.
    {
        if (hasSuccessfulPath)
        {
            currentNode.IsOccupied = false; // Sets last Node to now not be Occupied.
            
            BattleMove battleMove = new BattleMove(this.gameObject, path, speed);
            TurnManager.instance.EventResolutionReceiver(battleMove);
        }
    }


    public void OnDrawGizmos() //Displays path as Gizmo line. 
    {
		if (path != null)
        {
			for (int i = targetIndex; i < path.Length; i ++)
            {
				Gizmos.color = Color.black;
				Gizmos.DrawCube(path[i], Vector3.one);

				if (i == targetIndex)
                {
					Gizmos.DrawLine(transform.position, path[i]);
				}
				else
                {
					Gizmos.DrawLine(path[i-1],path[i]);
				}
			}
		}
	}
}
