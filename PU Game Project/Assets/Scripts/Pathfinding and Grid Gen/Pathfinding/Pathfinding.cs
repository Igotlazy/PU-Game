//All the Pathfinding algorithms. A* pathfinding and all below it (up to but not including BFS) is not my code. May need to look it over. 

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Pathfinding : MonoBehaviour {
	
	PathRequestManager requestManager;
    DrawIndicators pathDrawingScript;
    public List<Node> lastPath;
	GridGen gridGenScript;

    public LineRenderer givenRenderer;

    public static Pathfinding instance;
	
	void Awake() {

        instance = this;

		requestManager = GetComponent<PathRequestManager>();
		gridGenScript = GetComponent<GridGen>();
        pathDrawingScript = GetComponent<DrawIndicators>();
	}
	
	
	public void StartFindPath(Vector3 startPos, Vector3 targetPos)
    {
		StartCoroutine(FindPath(startPos,targetPos));
	}

    //A* Pathfinding	
	IEnumerator FindPath(Vector3 startPos, Vector3 targetPos)
    {

		Vector3[] waypoints = new Vector3[0];
		bool pathSuccess = false;
		
		Node startNode = gridGenScript.NodeFromWorldPoint(startPos);
		Node targetNode = gridGenScript.NodeFromWorldPoint(targetPos);

		if (startNode.IsWalkable && (targetNode.IsWalkable && !targetNode.IsOccupied)) {
			Heap<Node> openSet = new Heap<Node>(gridGenScript.MaxSize);
			HashSet<Node> closedSet = new HashSet<Node>();
			openSet.Add(startNode);
			
			while (openSet.Count > 0) {
				Node currentNode = openSet.RemoveFirst();
				closedSet.Add(currentNode);
				
				if (currentNode == targetNode) {
                    pathSuccess = true;
					break;
				}
				
				foreach (Node neighbour in currentNode.nodeNeighbors)
                {
					if (!neighbour.IsWalkable || neighbour.IsOccupied || closedSet.Contains(neighbour))
                    {
						continue;
					}
					
					int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);
					if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                    {
						neighbour.gCost = newMovementCostToNeighbour;
						neighbour.hCost = GetDistance(neighbour, targetNode);
                        
						neighbour.parent = currentNode;
                        
						
						if (!openSet.Contains(neighbour))
							openSet.Add(neighbour);
					}
				}
			}
		}
		yield return null;

		if (pathSuccess) {
            //Debug.LogAssertion("PATH SUCCESS");
			waypoints = RetracePath(startNode,targetNode);
		}
		requestManager.FinishedProcessingPath(waypoints,pathSuccess);	
	}
	
	Vector3[] RetracePath(Node startNode, Node endNode) {
		List<Node> path = new List<Node>();
		Node currentNode = endNode;
		
		while (currentNode != startNode) {
			path.Add(currentNode);
			currentNode = currentNode.parent;
		}
		Vector3[] waypoints = ConvertPath(path);
		Array.Reverse(waypoints);
		return waypoints;
		
	}


	// Turns the list of retrieved nodes into Vector3 Waypoints that the Unit will move towards. 
	Vector3[] ConvertPath(List<Node> path) {

		List<Vector3> waypoints = new List<Vector3>();
		//Vector2 directionOld = Vector2.zero;
		for (int i = 0; i < path.Count; i ++) {

            //THE FOLLOWING WAS REMOVED AS IT SIMPLIFIED THE PATH TO SIMPLY GO TO CORNERS AS OPPOSED TO MOVING FROM NODE TO NODE. 
            /*
			Vector2 directionNew = new Vector2(path[i-1].gridX - path[i].gridX,path[i-1].gridY - path[i].gridY);
			if (directionNew != directionOld) {
				waypoints.Add(path[i].worldPosition);
			}
			directionOld = directionNew;
            */

            //This simplified the path by breaking it up into corners. Made the unit stray off the path exact path (not useful for grid-based movement). 

            waypoints.Add(path[i].worldPosition);
        }
        pathDrawingScript.OnPathClear();
        pathDrawingScript.OnPathSet(path);           //Loads drawPathfinding with relavent nodes. 
		return waypoints.ToArray();
	}
	
	int GetDistance(Node nodeA, Node nodeB) {
		//int dstX = Mathf.Clamp(Mathf.Abs(nodeA.gridX - nodeB.gridX), 0, 2);
		//int dstZ = Mathf.Clamp(Mathf.Abs(nodeA.gridZ - nodeB.gridZ), 0 , 2);

        int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        if(dstX > Mathf.Cos(gridGenScript.nodeRadius * 2.1f)){
            dstX = 1;
        }

        int dstZ = Mathf.Abs(nodeA.gridZ - nodeB.gridZ);
        if (dstZ > Mathf.Sin(gridGenScript.nodeRadius * 2.1f)){
            dstZ = 1;
        }

        //Debug.Log(dstX);
        //Debug.Log(dstZ);

        if (dstX > dstZ)
        {           
            return 14 * dstZ + 10 * (dstX - dstZ);
        }

        return 14*dstX + 10 * (dstZ-dstX);
	}

    //BFS Pathfinding
    public List<Node> DisplayAvailableMoves(Node startNode, int movePoints)
    {
        List<Node> validNodes = new List<Node>();
        HashSet<Node> visitedNodes = new HashSet<Node>();

        Queue<Node> queue = new Queue<Node>();
        queue.Enqueue(startNode);
        startNode.BFSDepth = 0;
        while (queue.Count > 0)
        {
            Node currentNode = queue.Dequeue();
            visitedNodes.Add(currentNode);
            if (currentNode.BFSDepth < movePoints)
            {
                List<Node> currentNodeNeigh = currentNode.nodeNeighbors;
                foreach (Node currentNeigh in currentNodeNeigh)
                {
                    if (currentNode.IsWalkable && !visitedNodes.Contains(currentNeigh))
                    {
                        if(!currentNode.IsOccupied || currentNode == startNode) //Makes it so it avoids Occupied nodes but not the one the player is standing on. 
                        {
                            if (queue.Contains(currentNeigh))
                                continue;

                            queue.Enqueue(currentNeigh);
                            currentNeigh.BFSDepth = currentNode.BFSDepth + 1;
                        }
                    }
                }
            }
            if (currentNode.IsWalkable && !validNodes.Contains(currentNode))
            {
                validNodes.Add(currentNode);
            }
        }

        //DrawLine(startNode, validNodes);

        return validNodes;
    }

    private void DrawLine(Node startNode, List<Node> givenNodes) 
    {
        Node currentNode = startNode;
        float length = GridGen.instance.nodeRadius;

        Vector3 currentPos = new Vector3(startNode.worldPosition.x, startNode.worldPosition.y, startNode.worldPosition.z);
        currentPos += Vector3.right * length * 2; //Extends out to check for any nodes. 

        while (true)
        {
            bool foundNeighbour = false;

            foreach(Node neighbour in currentNode.nodeNeighbors)
            {
                if(neighbour.worldPosition.Equals(currentPos))
                {
                    currentNode = neighbour;
                    currentPos += Vector3.right * length * 2; //Extends out to check again.
                    foundNeighbour = true;
                    break;
                }
            }
            if (!foundNeighbour)
            {
                currentPos += Vector3.left * length * 2; //Pulls back as nothing was found.
                break;
            }
        }

        StartCoroutine(TracePath(currentPos, givenNodes));

        /*
        Vector3[] posArray = new Vector3[] {startNode.worldPosition, currentPos };

        givenRenderer.positionCount = posArray.Length;
        givenRenderer.SetPositions(posArray);
        */

    }
    
    IEnumerator TracePath(Vector3 startPos, List<Node> givenNodes)
    {
        WaitForSeconds wait = new WaitForSeconds(0.25f);
        float length = GridGen.instance.nodeRadius;
        Node currentNode = GridGen.instance.NodeFromWorldPoint(startPos);
        Vector3 currentPos = currentNode.worldPosition;

        List<Vector3> pointList = new List<Vector3>();
        pointList.Add(new Vector3(currentPos.x + length, currentPos.y, currentPos.z + length));

        while (true)
        {
            givenRenderer.positionCount = pointList.Count;
            givenRenderer.SetPositions(pointList.ToArray());
            Debug.Log("Hello");
            yield return wait;
            Debug.Log("Hello2");

            bool foundNeighbour = false;
            Vector3 forwardCompare = currentPos + (Vector3.back *length * 2);
            foreach (Node neighbour in currentNode.nodeNeighbors)
            {
                if (neighbour.worldPosition.Equals(forwardCompare))
                {
                    currentNode = neighbour;
                    pointList.Add(new Vector3(currentPos.x + length, currentPos.y, currentPos.z + length));
                    foundNeighbour = true;
                    break;
                }
            }
            if (foundNeighbour)
            {
                if (currentPos.Equals(startPos))
                {
                    break;
                }
                continue;
            }

        }

        Vector3[] posArray = new Vector3[] { startPos, currentPos };

        givenRenderer.positionCount = posArray.Length;
        givenRenderer.SetPositions(posArray);
    }

}
