using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CursorController : MonoBehaviour
{
    public GameObject visual;

    public Node currentNode;
    public Vector3 nodePos;

    public float catchupSpeed = 20f;

    private bool isActive;

    private bool attemptFloorUp;
    private bool attemptFloorDown;

    public delegate void CursorNewNode(Node givenNode);
    public event CursorNewNode CursorNewNodeEVENT;

    public static CursorController instance;

    private void Awake()
    {
        if (instance == null) { instance = this; } else { Destroy(this); }

        visual.SetActive(false);
    }


    void Start()
    {
        ClickSelection.instance.NewSelectionEvent += NewSelection;
        ClickSelection.instance.ClearSelectionEvent += ClearedSelection;
    }

    private void Update()
    {
        if (isActive && transform.position != nodePos)
        {
            visual.transform.Translate((nodePos - visual.transform.position) * Time.deltaTime * catchupSpeed);
        }
    }

    void LateUpdate()
    {
        if (isActive)
        {
            SetPositionOfCursor();
        }
    }


    private void SetPositionOfCursor()
    {
        Node foundNode = null;

        RaycastHit[] hitInfos = Physics.RaycastAll(Camera.main.ScreenPointToRay(Input.mousePosition), Mathf.Infinity, CombatUtils.gameTerrainMask);
        System.Array.Sort(hitInfos, (x, y) => x.distance.CompareTo(y.distance));

        foreach(RaycastHit currentHit in hitInfos)
        {
            
            if (currentHit.collider.CompareTag("Floor") || currentHit.collider.CompareTag("Map Base"))
            {
                foundNode = GridGen.instance.NodeFromWorldPoint(currentHit.point);
                break;
            }
        }

        if (foundNode != null && foundNode != currentNode)
        {
            currentNode = foundNode;
            nodePos = foundNode.worldPosition;

            if (CursorNewNodeEVENT != null)
            {
                CursorNewNodeEVENT(currentNode); //EVENT for new Node selection. 
            }
        }
    }

    private void NewSelection(GameObject selection)
    {
        isActive = true;
        visual.SetActive(true);
        currentNode = GridGen.instance.NodeFromWorldPoint(selection.transform.position);
        nodePos = currentNode.worldPosition;
        visual.transform.position = currentNode.worldPosition;
    }

    private void ClearedSelection()
    {
        isActive = false;
        visual.SetActive(false);
    }
}
