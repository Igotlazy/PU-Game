using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CursorController : MonoBehaviour
{
    public GameObject visual;

    public Node currentNode;
    public Vector3 nodePos;

    public float catchupSpeed;

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
            transform.Translate((nodePos - transform.position) * Time.deltaTime * catchupSpeed);
        }
        /*
        if (Input.GetKeyDown(KeyCode.U))
        {
            MoveUpFloor();
        }
        if (Input.GetKeyDown(KeyCode.J))
        {
            MoveDownFloor();
        }
        */
    }

    void LateUpdate()
    {
        if (isActive)
        {
            SetPositionOfCursor();
        }
    }

    private void MoveUpFloor()
    {
        if (attemptFloorDown)
        {
            MoveFloorReset();
        }
        else
        {
            attemptFloorUp = true;
            visual.transform.localScale = new Vector3(1f, 1.5f, 1f);
        }
    }

    private void MoveDownFloor()
    {
        if (attemptFloorUp)
        {
            MoveFloorReset();
        }
        else
        {
            attemptFloorDown = true;
            visual.transform.localScale = new Vector3(1f, 0.1f, 1f);
        }
    }

    private void MoveFloorReset()
    {
        attemptFloorUp = false;
        attemptFloorDown = false;
        visual.transform.localScale = new Vector3(1f, 0.75f, 1f);
    }


    private void SetPositionOfCursor()
    {
        Node foundNode = null;
        RaycastHit hitInfo;
        bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo, Mathf.Infinity, CombatUtils.gameTerrainMask);
        if (hit)
        {
            foundNode = GridGen.instance.NodeFromWorldPoint(hitInfo.point);
            Debug.Log(hitInfo.collider.gameObject.name);
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
        transform.position = currentNode.worldPosition;
    }

    private void ClearedSelection()
    {
        isActive = false;
        visual.SetActive(false);
    }
}
