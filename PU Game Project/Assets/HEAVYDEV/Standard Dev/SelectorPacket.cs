using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectorPacket {

    public enum SelectionType
    {
        Null,
        Target,
        AreaTarget,
        AoE,
    }
    public SelectionType selectionType;
    public int maxNumOfSelect;
    public bool isPure;



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



    public SelectorPacket(SelectionType _selectionType, bool _isPure)
    {
        this.selectionType = _selectionType;
        this.isPure = _isPure;
    }



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

    public static SelectorPacket Clone(SelectorPacket givenPacket)
    {
        SelectorPacket returnPacket = new SelectorPacket(givenPacket.selectionType, givenPacket.isPure)
        {
            selectorSpecs = new List<float>(givenPacket.selectorSpecs)           
        };

        if(givenPacket.selectionType == SelectionType.Target)
        {
            returnPacket.maxNumOfSelect = givenPacket.maxNumOfSelect;
        }

        return returnPacket;
    }
}
