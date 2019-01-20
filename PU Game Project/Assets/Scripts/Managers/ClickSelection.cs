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
    public Unit selectedUnitScript;
    public Node lastHitNode;
    [Space]

    [Header("State Bools")]
    public bool hasSelection;
    [Space]

    public bool prepAttack;
    public bool canSelect;

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
        TurnManager.instance.BattlePhaseResponseEVENT += BattlePhaseReceiver;
    }
    private void OnDestroy()
    {
        TurnManager.instance.BattlePhaseResponseEVENT -= BattlePhaseReceiver;
    }

    void BattlePhaseReceiver(TurnManager.BattlePhase givenPhase)
    {
        if(givenPhase == TurnManager.BattlePhase.PlayerInput)
        {
            canSelect = true;
        }
        else if (givenPhase == TurnManager.BattlePhase.ActionPhase)
        {
            canSelect = false;
        }
        else
        {
            canSelect = false;
            ClearSelection();
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !prepAttack && canSelect)
        {
            SelectionClick();
        }
    }

    private void LateUpdate()
    {
        if (particlesPlaying) //For Selection Indicator
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

            if (NewSelectionEvent != null)
            {
                NewSelectionEvent(selectedUnitObj); //EVENT TO RESPOND TO NEW SELECTION;
            }

            selectionParticles.gameObject.SetActive(true);
            particlesPlaying = true;

            ResetToDefault();
            CameraManager.instance.SetCameraTargetBasic(selectedUnitObj.transform); //Makes camera follow selected Unit.
        }
    }

    public void ClearSelection()
    {
        hasSelection = false;
        selectedUnitObj = null;
        selectedUnitScript = null;

        selectionParticles.gameObject.SetActive(false);
        particlesPlaying = false;

        if (ClearSelectionEvent != null)
        {
            ClearSelectionEvent(); //EVENT TO RESPOND TO CLEARED SELECTION.
        }
    }

    public void ResetToDefault()
    {

        DrawIndicators.instance.ClearTileMatStates(true, true, true);

        prepAttack = false;
        //prepMoving = false;
    }
}
