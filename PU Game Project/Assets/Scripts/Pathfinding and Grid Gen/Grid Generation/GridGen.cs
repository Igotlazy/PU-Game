//Generates the Grid. Forms a grid of size equal to gridWorldSize. Divides the region into nodes based on the given Node radius. Currently it is set to 0.5. Note, TilePrefabs are spawned here and
//WILL NOT change size based on the Node radius. 

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GridGen : MonoBehaviour {

	public bool displayGridGizmos;
    public bool doDiagonal;
	public LayerMask unwalkableMask;
	public Vector2 gridWorldSize;
    public float nodeRadius = 0.5f;
    public GameObject tilePrefab;
	public Node[,] grid;
    private GameObject gridParent;

	float nodeDiameter;
	int gridSizeX, gridSizeY;

    public static GridGen instance;

	void Awake() {

        instance = this;

        if(nodeRadius < 0.05f) //Just to make sure no one can plug in 0 and break the grid gen. 
        {
            nodeRadius = 0.05f;
        }
		nodeDiameter = nodeRadius*2;
		gridSizeX = Mathf.RoundToInt(gridWorldSize.x/nodeDiameter);
		gridSizeY = Mathf.RoundToInt(gridWorldSize.y/nodeDiameter);
		CreateGrid();
	}

	public int MaxSize {
		get {
			return gridSizeX * gridSizeY;
		}
	}

    void CreateGrid()
    {
        grid = new Node[gridSizeX, gridSizeY];
        Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2;

        gridParent = new GameObject("Map Hierarchy");

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);
                bool walkable = !(Physics.CheckSphere(worldPoint, (nodeRadius * 1.1f), unwalkableMask));

                GameObject tileObject = Instantiate(tilePrefab, new Vector3(worldPoint.x, worldPoint.y + 0.55f, worldPoint.z), Quaternion.identity);
                tileObject.name = "Tile " + x + "," + y;
                tileObject.transform.SetParent(gridParent.transform);

                grid[x, y] = new Node(walkable, worldPoint, x, y, tileObject);
            }
        }

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                grid[x, y].nodeNeighbors = GetNeighbours(grid[x, y]);
            }

        }
    }

	public List<Node> GetNeighbours(Node node) //Finds all nodes around the given node in a 3v3 square.
    {
		List<Node> neighbours = new List<Node>();

        if (doDiagonal)
        {
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    if (x == 0 && y == 0) 
                        continue;

                    int checkX = node.gridX + x;
                    int checkY = node.gridY + y;

                    if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
                    {
                        neighbours.Add(grid[checkX, checkY]);
                    }
                }
            }
        }
        else
        {
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    if ((x == 0 && y == 0) || (x == 1 && y == 1) || (x == -1 && y == 1) || (x == -1 && y == -1) || (x == 1 && y == -1)) // Removes corners and the center (center is the original Node).
                        continue;

                    int checkX = node.gridX + x;
                    int checkY = node.gridY + y;

                    if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
                    {
                        neighbours.Add(grid[checkX, checkY]);
                    }
                }
            }
        }

		return neighbours;
	}
	

	public Node NodeFromWorldPoint(Vector3 worldPosition) {

        float posX = ((worldPosition.x - transform.position.x) + gridWorldSize.x * 0.5f) / nodeDiameter;
        float posY = ((worldPosition.z - transform.position.z) + gridWorldSize.y * 0.5f) / nodeDiameter;

        posX = Mathf.Clamp(posX, 0, gridWorldSize.x - 1);
        posY = Mathf.Clamp(posY, 0, gridWorldSize.y - 1);

        int x = Mathf.FloorToInt(posX);
        int y = Mathf.FloorToInt(posY);

        return grid[x, y];
    }

    /*
	void OnDrawGizmos() {
		Gizmos.DrawWireCube(transform.position,new Vector3(gridWorldSize.x,1,gridWorldSize.y));
		if (grid != null && displayGridGizmos) {
			foreach (Node n in grid) {
				Gizmos.color = (n.IsWalkable)?Color.white:Color.red;
				Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter-.1f));
			}
		}
	}
    */
    
}