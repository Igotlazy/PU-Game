//Generates the Grid. Forms a grid of size equal to gridWorldSize. Divides the region into nodes based on the given Node radius. Currently it is set to 0.75. Note, TilePrefabs are spawned here and
//WILL NOT change size based on the Node radius. They need to be changed manually.  

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class GridGen : MonoBehaviour {

	public bool displayGridGizmos;
    public bool doDiagonal;
	private Vector3 gridWorldSize;
    public float nodeRadius = 0.5f;
    public readonly float nodeHeightDif = 1f;
    public int minCielingHeight = 4;
    public float tileGraphicSpawnHeight = 0.05f;
    public GameObject tilePrefab;
	public Node[,,] grid;
    private GameObject gridParent;
    RaycastHit[] hitObjects;

	float nodeDiameter;
    public float NodeDiameter
    {
        get
        {
            return nodeDiameter;
        }
    }
    public Vector3 GridWorldSize
    {
        get
        {
            return gridWorldSize;
        }
    }

    int gridSizeX, gridSizeY, gridSizeZ;

    public static GridGen instance;

	void Awake() {

        instance = this;

        BoxCollider boxCol = GetComponent<BoxCollider>();
        gridWorldSize.x = boxCol.size.x;
        gridWorldSize.y = boxCol.size.y;
        gridWorldSize.z = boxCol.size.z;


        if (nodeRadius < 0.05f) //Just to make sure no one can plug in 0 and break the grid gen. 
        {
            nodeRadius = 0.05f;
        }
		nodeDiameter = nodeRadius*2;

		gridSizeX = Mathf.RoundToInt(gridWorldSize.x/nodeDiameter);
		gridSizeY = Mathf.RoundToInt(gridWorldSize.y /nodeHeightDif);
        gridSizeZ = Mathf.RoundToInt(gridWorldSize.z/nodeDiameter);

        //StartCoroutine(CreateGrid());
        CreateGrid();
	}

	public int MaxSize {
		get {
			return gridSizeX * gridSizeY * gridSizeZ;
		}
	}

    private void CreateGrid()
    {
        grid = new Node[gridSizeX, gridSizeY, gridSizeZ];
        Vector3 worldBottomTopLeft = transform.position - (Vector3.right * gridWorldSize.x / 2) + (Vector3.up * gridWorldSize.y / 2) - (Vector3.forward * gridWorldSize.z / 2);

        gridParent = new GameObject("Map Hierarchy");

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int z = 0; z < gridSizeZ; z++)
            {
                Vector3 worldPoint = worldBottomTopLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (z * nodeDiameter + nodeRadius);
                hitObjects = Physics.RaycastAll(worldPoint, Vector3.down, gridSizeY, CombatUtils.gameTerrainMask);
                Debug.DrawRay(worldPoint, Vector3.down * gridSizeY, Color.blue, 2f);

                hitObjects = hitObjects.OrderBy(hitObject => Vector3.Distance(worldPoint, hitObject.collider.bounds.max)).ToArray();

                Collider lastCollider = null;
                foreach (RaycastHit currentHit in hitObjects)
                {
                    if (!currentHit.collider.gameObject.CompareTag("Map Base") && !currentHit.collider.gameObject.CompareTag("Floor"))
                    {
                        continue;
                    }
                    if (lastCollider != null &&
                        (lastCollider.bounds.min.y < currentHit.collider.bounds.max.y + minCielingHeight)) 
                    {
                        continue;
                    }
                    lastCollider = currentHit.collider;

                    Vector3 nodePoint = new Vector3(worldPoint.x, (int)currentHit.point.y, worldPoint.z);

                    RaycastHit[] obstacleHits = Physics.RaycastAll(new Vector3(nodePoint.x, (nodePoint.y - (nodeHeightDif/4f)), nodePoint.z), Vector3.up, nodeHeightDif/2f);
                    bool walkable = true;
                    foreach(RaycastHit obHit in obstacleHits)
                    {
                        if (obHit.collider.gameObject.CompareTag("Map") || obHit.collider.gameObject.CompareTag("Obstacle"))
                        {
                            walkable = false;
                            break;
                        }
                    }

                    //Debug.Log("Current Hit Y: " + currentHit.point.y);
                    //Debug.Log("Node Point Y: " + nodePoint.y);
                    int yGridPos = (int)(gridSizeY - (currentHit.distance - 0.05f)); // Magic number as raycast reports strange distance on rotated colliders. 

                    GameObject tileObject = Instantiate(tilePrefab, new Vector3(nodePoint.x, nodePoint.y + tileGraphicSpawnHeight, nodePoint.z), Quaternion.identity);
                    tileObject.name = "Tile " + x + "," + yGridPos + "," + z;
                    tileObject.transform.SetParent(gridParent.transform);

                    //Debug.Log(obstacleHits.Length);

                    grid[x, yGridPos, z] = new Node(walkable, nodePoint, x, yGridPos, z, tileObject);


                    //yield return null;
                }

                //yield return null;
            }
        }

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                for( int z = 0; z < gridSizeZ; z++)
                {
                    if(grid[x,y,z] != null)
                    {
                        grid[x, y, z].nodeNeighbors = GetNeighbours(grid[x, y, z]);
                    }
                }
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
                for (int z = -1; z <= 1; z++)
                {
                    if (x == 0 && z == 0) 
                        continue;

                    int checkX = node.gridX + x;
                    int checkZ = node.gridZ + z;

                    if (checkX >= 0 && checkX < gridSizeX && checkZ >= 0 && checkZ < gridSizeZ)
                    {
                        if(grid[checkX, node.gridY, checkZ] != null)
                        {
                            neighbours.Add(grid[checkX, node.gridY, checkZ]);
                        }
                    }
                }
            }
        }
        else
        {
            for (int x = -1; x <= 1; x++)
            {
                for (int z = -1; z <= 1; z++)
                {
                    if ((x == 0 && z == 0) || (x == 1 && z == 1) || (x == -1 && z == 1) || (x == -1 && z == -1) || (x == 1 && z == -1)) // Removes corners and the center (center is the original Node).
                        continue;

                    int checkX = node.gridX + x;
                    int checkZ = node.gridZ + z;

                    if (checkX >= 0 && checkX < gridSizeX && checkZ >= 0 && checkZ < gridSizeZ)
                    {
                        if (grid[checkX, node.gridY, checkZ] != null)
                        {
                            neighbours.Add(grid[checkX, node.gridY, checkZ]);
                        }
                    }
                }
            }
        }

		return neighbours;
	}
	

	public Node NodeFromWorldPoint(Vector3 worldPosition)
    {
        return NodeFromWorldPoint(worldPosition, false);
    }
    public Node NodeFromWorldPoint(Vector3 worldPosition, bool fromtTile)
    {
        float posX = ((worldPosition.x - transform.position.x) + gridWorldSize.x * 0.5f) / nodeDiameter;

        float posY = 0;
        if (fromtTile)
        {
            posY = ((worldPosition.y - transform.position.y - tileGraphicSpawnHeight) + gridWorldSize.y * 0.5f) / nodeHeightDif;
        }
        else
        {
            posY = ((worldPosition.y - transform.position.y) + gridWorldSize.y * 0.5f) / nodeHeightDif;
        }

        float posZ = ((worldPosition.z - transform.position.z) + gridWorldSize.z * 0.5f) / nodeDiameter;

        posX = Mathf.Clamp(posX, 0, gridWorldSize.x - 1);
        posY = Mathf.Clamp(posY, 0, gridWorldSize.y - 1);
        posZ = Mathf.Clamp(posZ, 0, gridWorldSize.z - 1);

        int x = Mathf.FloorToInt(posX);
        int y = Mathf.FloorToInt(posY);
        int z = Mathf.FloorToInt(posZ);

        Node resultNode = grid[x, y, z];
        return resultNode;
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