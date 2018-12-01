using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetPacket {


    private List<HashSet<Node>> targetNodes = new List<HashSet<Node>>();
    public List<HashSet<Node>> TargetNodes
    {
        get
        {
            return targetNodes;
        }
        set
        {
            value = targetNodes;
        }
    }

    public List<GameObject> ReturnObjectsOnNodes(int targetListIndex)
    {
        List<GameObject> returnList = new List<GameObject>();

        foreach(Node currentNode in targetNodes[targetListIndex])
        {
            if (currentNode.IsOccupied)
            {
                returnList.Add(currentNode.occupant);
            }
        }

        return returnList;
    }

    public List<GameObject> ReturnObjectsOnNodes(int targetListIndex, Unit dontInclude)
    {
        List<GameObject> returnList = new List<GameObject>();

        foreach (Node currentNode in targetNodes[targetListIndex])
        {
            if (currentNode.IsOccupied && (dontInclude != currentNode.occupant.GetComponent<Unit>()))
            {
                returnList.Add(currentNode.occupant);
            }
        }

        return returnList;
    }

    public List<GameObject> ReturnObjectsOnNodes(int targetListIndex, int teamInt)
    {
        List<GameObject> returnList = new List<GameObject>();

        foreach (Node currentNode in targetNodes[targetListIndex])
        {
            if (currentNode.IsOccupied)
            {
                Unit unitScript = currentNode.occupant.GetComponent<Unit>();

                if(unitScript.teamValue != teamInt)
                {
                    returnList.Add(currentNode.occupant);
                }
            }
        }

        return returnList;
    }
}
