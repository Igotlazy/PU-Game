using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetPacket {

    public enum SelectionType
    {
        Single,
        AoE,
        HardAOE
    }
    public SelectionType selectionTypes;



    private HashSet<Node> targetNodes = new HashSet<Node>();
    public HashSet<Node> TargetNodes
    {
        get
        {
            return targetNodes;
        }
        set
        {
            targetNodes = value;
        }
    }

    public List<TargetSpecs> targetObjectSpecs = new List<TargetSpecs>();
    public List<float> selectorSpecs = new List<float>();
    public Vector3 spawnLocation;



    public List<GameObject> ReturnObjectsOnNodes()
    {
        List<GameObject> returnList = new List<GameObject>();

        foreach(Node currentNode in targetNodes)
        {
            if (currentNode.IsOccupied)
            {
                returnList.Add(currentNode.occupant);
            }
        }

        return returnList;
    }

    public List<GameObject> ReturnObjectsOnNodes(Unit dontInclude)
    {
        List<GameObject> returnList = new List<GameObject>();

        foreach (Node currentNode in targetNodes)
        {
            if (currentNode.IsOccupied && (dontInclude != currentNode.occupant.GetComponent<Unit>()))
            {
                returnList.Add(currentNode.occupant);
            }
        }

        return returnList;
    }

    public List<GameObject> ReturnObjectsOnNodes(int teamInt)
    {
        List<GameObject> returnList = new List<GameObject>();

        foreach (Node currentNode in targetNodes)
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

    public static TargetPacket Clone(TargetPacket givenPacket)
    {
        TargetPacket returnPacket = new TargetPacket();
        //returnPacket.TargetNodes = new List<HashSet<Node>>(givenPacket.TargetNodes);
        returnPacket.selectorSpecs = new List<float>(givenPacket.selectorSpecs);

        return returnPacket;
    }
}
