//Data associated with the Tiles. Comes into existence through the Grid Generation. Change in bools results in changes to the TilePrefab.

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Node : IHeapItem<Node> {

    //Node Properties;
    public Vector3 worldPosition;
	public int gridX;
	public int gridY;
    public int gridZ;
    public List<Node> nodeNeighbors = new List<Node>();
    public GameObject occupant;

    public int gCost;
	public int hCost;
	public Node parent;
    public int BFSDepth;
    public GameObject tilePrefab;
    private Tile tileScript;

    int heapIndex;
	
	public Node(bool _walkable, Vector3 _worldPos, int _gridX, int _gridY, int _gridZ, GameObject _tilePrefab) {
		worldPosition = _worldPos;
		gridX = _gridX;
		gridY = _gridY;
        gridZ = _gridZ;
        tilePrefab = _tilePrefab;

        tileScript = tilePrefab.GetComponent<Tile>();
        tileScript.carryingNode = this;

        IsWalkable = _walkable;
    }

	public int fCost {
		get {
			return gCost + hCost;
		}
	}

	public int HeapIndex {
		get {
			return heapIndex;
		}
		set {
			heapIndex = value;
		}
	}

	public int CompareTo(Node nodeToCompare) {
		int compare = fCost.CompareTo(nodeToCompare.fCost);
		if (compare == 0) {
			compare = hCost.CompareTo(nodeToCompare.hCost);
		}
		return -compare;
	}

    //Node States
    private bool isWalkable;
    public bool IsWalkable
    {
        get
        {
            return isWalkable;
        }
        set
        {
            isWalkable = value;
            if (value == true)
            {
                tileScript.SetBaseTileMaterial(0);
            }
            else
            {
                tileScript.SetBaseTileMaterial(1);
            }

            tileScript.ReconfigureMats();
        }
    }

    private bool isOccupied;
    public bool IsOccupied
    {
        get
        {
            return isOccupied;
        }
        set
        {
            isOccupied = value;
            if(value == false)
            {
                occupant = null;
            }
            tileScript.ReconfigureMats();
        }
    }

    private bool isSelectable;
    public bool IsSelectable
    {
        get
        {
            return isSelectable;
        }
        set
        {
            isSelectable = value;
            tileScript.ReconfigureMats();
        }
    }

    private bool isAttackable;
    public bool IsAttackable
    {
        get
        {
            return isAttackable;
        }
        set
        {
            isAttackable = value;
            tileScript.ReconfigureMats();
        }
    }

    private bool isOnPath;
    public bool IsOnPath
    {
        get
        {
            return isOnPath;
        }
        set
        {
            isOnPath = value;
            tileScript.ReconfigureMats();
        }
    }
}
