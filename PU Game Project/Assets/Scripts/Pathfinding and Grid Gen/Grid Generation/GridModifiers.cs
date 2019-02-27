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
            if(partnersNode == null)
            {
                Debug.LogError("WARNING: A Node Connector is not associated with a Node - " + current.gameObject.name);
            }
            else if(!myNode.nodeNeighbors.Contains(partnersNode))
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
