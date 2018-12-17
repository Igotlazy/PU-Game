//Handles player input and clicking.
//UNHOLY MESS OF A SCRIPT. WORKS BUT IS A SPAGHETTI MESS. FIX.

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClickSelection : MonoBehaviour
{
    public ParticleSystem selectionParticles;
    [Space]

    [Header("Selection References")]
    public GameObject selectedUnitObj;
    public LivingCreature selectedCreatureScript;
    public Unit selectedUnitScript;
    public Node lastHitNode;
    [Space]

    [Header("State Bools")]
    public bool hasSelection;
    [Space]

    public bool prepMoving;
    public bool prepAttack;
    public bool prepInv;

    private bool particlesPlaying;

    public static ClickSelection instance;

    public delegate void NewUnitSelection(GameObject newSelection);
    public event NewUnitSelection NewSelectionEvent;

    public delegate void ClearUnitSelection();
    public event ClearUnitSelection ClearSelectionEvent;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        
    }

    void Update()
    {
        if (TurnManager.instance.CurrentBattlePhase == TurnManager.BattlePhase.PlayerInput)
        {
            if (Input.GetMouseButtonDown(0) && !prepAttack)
            {
                SelectionClick();
            }
        }

        //For Selection Particles
        if (selectedUnitObj != null && !particlesPlaying)
        {
            selectionParticles.gameObject.SetActive(true);
            particlesPlaying = true;
        }
        else if (selectedUnitObj == null && particlesPlaying)
        {
            selectionParticles.gameObject.SetActive(false);
            particlesPlaying = false;
        }
    }

    private void LateUpdate()
    {
        if (selectedUnitObj != null) //For Selection Indicator
        {
            selectionParticles.gameObject.transform.position = new Vector3(
                selectedUnitObj.transform.position.x,
                selectedUnitObj.transform.position.y + 0.6f,
                selectedUnitObj.transform.position.z);
        }
    }



    private void SelectionClick()
    {
        RaycastHit hitInfo = new RaycastHit();
        bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo, 100f, CombatUtils.clickLayerMask);

        if (!EventSystem.current.IsPointerOverGameObject()) //Makes sure it doesn't interact with UI
        {
            if (hit && (hitInfo.transform.gameObject.CompareTag("Champion") && hitInfo.collider.gameObject.activeInHierarchy))
            {
                ClickedSelection(hitInfo.collider.gameObject.transform);
            }
            else
            {
                ClearSelection();
                ResetToDefault();
            }
        }
    }

    public void ClickedSelection(Transform potentialNewSelection)
    {
        hasSelection = true;

        if(potentialNewSelection.gameObject != selectedUnitObj)
        {
            selectedUnitObj = potentialNewSelection.gameObject;
            selectedUnitScript = selectedUnitObj.GetComponent<Unit>();
            selectedCreatureScript = selectedUnitObj.GetComponent<LivingCreature>();

            if (NewSelectionEvent != null)
            {
                NewSelectionEvent(selectedUnitObj); //EVENT TO RESPOND TO NEW SELECTION;
            }

            ResetToDefault();
            CameraManager.instance.SetCameraTargetBasic(selectedUnitObj.transform); //Makes camera follow selected Unit.
        }
    }

    public void ClearSelection()
    {
        hasSelection = false;
        selectedUnitObj = null;
        selectedUnitScript = null;
        selectedCreatureScript = null;

        if(ClearSelectionEvent != null)
        {
            ClearSelectionEvent(); //EVENT TO RESPOND TO CLEARED SELECTION.
        }
    }

    public void ResetToDefault()
    {

        DrawIndicators.instance.ClearTileMatStates(true, true, true);

        prepAttack = false;
        prepMoving = false;
    }

    public void DrawMoveZone() //Draws where the places can move.
    {
        //DrawIndicators.instance.ClearTileMatStates(true, true, true); //Clears tiles if you selected a new target and already had one selected.

        List<Node> availableNodes = Pathfinding.instance.DisplayAvailableMoves(selectedUnitScript.currentNode, selectedCreatureScript.CurrentEnergy); //BFS Call to get nodes.
        DrawIndicators.instance.BFSSelectableSet(availableNodes); //Sets nodes as selectable.
    }
}
