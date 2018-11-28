//General methods a Unit should have access too.

using UnityEngine;
using System.Collections;
using Cinemachine;
using MHA.BattleBehaviours;

public class Unit : MonoBehaviour {

	public float speed = 4f;

	public Vector3[] path;
	int targetIndex;

    public Node currentNode;
    public LayerMask tileLayerMask;

    public bool hasSuccessfulPath;
    public int teamValue;

    public CinemachineVirtualCamera unitCamera;
    public Transform centerPoint;
    public bool[,] test = new bool[5,5];

    [Space]
    [Header("SHOT RESPONDERS:")]
    public GameObject shotConnecter;
    public GameObject partialCoverCheck;

    [Space]
    public GameObject projectile;


    private void Awake()
    {
        ReferenceObjects.AddToPlayerList(this.gameObject);
    }

    void Start()
    {
        StartNodeFind();
    }

    private void StartNodeFind() //Gives the unit reference to the Node below it.
    {
        Node foundNode = GridGen.instance.NodeFromWorldPoint(transform.position);
        currentNode = foundNode;
        if (foundNode.IsWalkable)
        {
            foundNode.IsOccupied = true;
            foundNode.occupant = this.gameObject;
        }
    }
}
