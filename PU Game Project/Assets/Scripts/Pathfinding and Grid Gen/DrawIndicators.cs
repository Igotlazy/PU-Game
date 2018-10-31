//Large scale changes in Tile colors. 
//This is a SHIT script filled with repetitive code. I'll fix it later.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawIndicators : MonoBehaviour {

    public List<Node> currentPathNodes = new List<Node>();
    public List<Node> currentBFSNodes = new List<Node>();
    public List<Node> currentAttackableNodes = new List<Node>();

    public static DrawIndicators instance;

    public void Awake()
    {
        instance = this;
    }

    

    public void OnPathSet(List<Node> onPathNodes)
    {
        currentPathNodes = onPathNodes;

        foreach(Node currentNode in currentPathNodes)
        {
            currentNode.IsOnPath = true;
        }
    }

    public void OnPathReturn()
    {
        foreach(Node onPathNode in currentPathNodes)
        {
            onPathNode.IsOnPath = true;
        }
    }
    public void OnPathClear()
    {
        foreach (Node onPathNode in currentPathNodes)
        {
            onPathNode.IsOnPath = false;
        }
    }


    public void BFSSelectableSet(List<Node> BFSNodes)
    {
        currentBFSNodes = BFSNodes;

        foreach(Node currentNode in currentBFSNodes)
        {
            currentNode.IsSelectable = true;
        }
    }
    public void BFSSelectableReturn()
    {
        foreach (Node currentNode in currentBFSNodes)
        {
            currentNode.IsSelectable = true;
        }
    }
    public void BFSSelectableClear()
    {
        foreach (Node currentNode in currentBFSNodes)
        {
            currentNode.IsSelectable = false;
        }
    }


    public void AttackableSet(List<Node> attackableNodes)
    {
        currentAttackableNodes = attackableNodes;

        foreach (Node currentNode in currentAttackableNodes)
        {
            currentNode.IsAttackable = true;
        }
    }
    public void AttackableReturn()
    {
        foreach (Node currentNode in currentAttackableNodes)
        {
            currentNode.IsAttackable = true;
        }
    }
    public void AttackableSetClear()
    {
        foreach (Node currentNode in currentAttackableNodes)
        {
            currentNode.IsAttackable = false;
        }
    }

    public void ClearTileMatStates(bool includePath, bool includeSelectable, bool includeAttackable)
    {
        if (includePath)
        {
            OnPathClear();
        }

        if (includeSelectable)
        {
            BFSSelectableClear();
        }

        if (includeAttackable)
        {
            AttackableSetClear();

        }
    }
}
