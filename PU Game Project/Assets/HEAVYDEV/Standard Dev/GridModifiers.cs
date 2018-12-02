using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridModifiers : MonoBehaviour
{
    public List<GridModifiers> joinedModifiers;

    void Start()
    {
        Node myNode = GridGen.instance.NodeFromWorldPoint(transform.position);
        foreach (GridModifiers current in joinedModifiers)
        {
            Node partnersNode = GridGen.instance.NodeFromWorldPoint(current.transform.position);
            if (!myNode.nodeNeighbors.Contains(partnersNode))
            {
                myNode.nodeNeighbors.Add(partnersNode);
            }
        }
    }

    /*
    void Update()
    {

    }
    */
}
