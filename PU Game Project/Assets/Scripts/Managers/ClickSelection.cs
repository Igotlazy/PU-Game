﻿//Handles player input and clicking.
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

    public GameObject basicAttackProjectile; //Testing

    private bool particlesPlaying;

    public static ClickSelection instance;

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
            /*
            if (selectedUnitObj != null)
            {
                selectedUnitObj.GetComponent<HeroCharacter>().UnitAbilityCleanup();
            }
            */

            if (hit && (hitInfo.transform.gameObject.CompareTag("Champion") && hitInfo.collider.gameObject.activeInHierarchy))
            {
                ClickedSelection(hitInfo.collider.gameObject);
            }
            else
            {
                ClearSelection();
                ResetToDefault();
            }
        }
    }

    public void ClickedSelection(GameObject newSelection)
    {
        hasSelection = true;

        selectedUnitObj = newSelection;
        selectedUnitScript = selectedUnitObj.GetComponent<Unit>();
        selectedCreatureScript = selectedUnitObj.GetComponent<LivingCreature>();

        //DrawMoveZone();
        ResetToDefault();
        CameraManager.instance.SetCameraTargetBasic(selectedUnitScript.unitCamera); //Makes camera follow selected Unit.
    }

    public void ClearSelection()
    {
        hasSelection = false;
        selectedUnitObj = null;
        selectedUnitScript = null;
        selectedCreatureScript = null;
    }

    public void ResetToDefault()
    {

        DrawIndicators.instance.ClearTileMatStates(true, true, true);

        prepAttack = false;
        prepMoving = false;
    }

    /*
    private void MoveClick()
    {
        if(selectedCreatureScript.CurrentEnergy >= selectedUnitScript.path.Length && selectedUnitScript.hasSuccessfulPath)
        {                  
            selectedCreatureScript.CurrentEnergy -= selectedUnitScript.path.Length; //Reduces energy by the size of length of the path. (1 Node Movement = 1 Energy).
            DrawIndicators.instance.ClearTileMatStates(true, true, true);
            selectedUnitScript.RunFollowPath(); //Starts the path move. 
        }
    }
    */

    public void DrawMoveZone() //Draws where the places can move.
    {
        //DrawIndicators.instance.ClearTileMatStates(true, true, true); //Clears tiles if you selected a new target and already had one selected.

        List<Node> availableNodes = Pathfinding.instance.DisplayAvailableMoves(selectedUnitScript.currentNode, selectedCreatureScript.CurrentEnergy); //BFS Call to get nodes.
        DrawIndicators.instance.BFSSelectableSet(availableNodes); //Sets nodes as selectable.
    }

    /*
    private void SetMovePath() //Drawing of path happens in the pathfinding script. 
    {
        RaycastHit hitInfo = new RaycastHit();
        bool hit = Physics.Raycast(currentCamera.ScreenPointToRay(Input.mousePosition), out hitInfo, 100f, clickLayerMask);

        if (hit && (GridGen.instance.NodeFromWorldPoint(hitInfo.point) != lastHitNode) && hitInfo.transform.gameObject.CompareTag("Tile") && hitInfo.collider.gameObject.activeInHierarchy) //Makes it so the script doesnt run every frame, only when a new tile is hovered over. 
        {
            lastHitNode = GridGen.instance.NodeFromWorldPoint(hitInfo.point);

            if (lastHitNode.IsSelectable)
            {
                selectedUnitObj.GetComponent<Unit>().SelectionPathRequest(hitInfo.point);
            }
            else
            {
                selectedUnitScript.path = null;
                selectedUnitScript.hasSuccessfulPath = false;

                DrawIndicators.instance.ClearTileMatStates(true, false, false);
            }
        }
    }
    */


    private void AttackClick()
    {
        RaycastHit hitInfo = new RaycastHit();
        bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo, 100f, CombatUtils.clickLayerMask);

        if (hit && hitInfo.transform.gameObject.CompareTag("Champion") && hitInfo.collider.gameObject.activeInHierarchy)
        {
            Unit targetUnitScript = hitInfo.collider.gameObject.GetComponent<Unit>();

            if (targetUnitScript.currentNode.IsAttackable)
            {
                Attack attack = new Attack(5, Attack.DamageType.Magical);
                //BattleAbility battleAttack = new BattleAbility(attack, basicAttackProjectile, selectedUnitObj, new List<Node> { hitInfo.collider.gameObject.GetComponent<Unit>().currentNode });
                //TurnManager.instance.EventResolutionReceiver(battleAttack);

                CombatUtils.AttackHitCalculation(selectedUnitObj, hitInfo.collider.gameObject); //[TESTING FOR % CHECK.]                    
            }
        }
    }
}
