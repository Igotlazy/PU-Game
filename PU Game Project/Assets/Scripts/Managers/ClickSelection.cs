//Handles player input and clicking.
//UNHOLY MESS OF A SCRIPT. WORKS BUT IS A SPAGHETTI MESS. FIX.

using System.Collections.Generic;
using UnityEngine;

public class ClickSelection : MonoBehaviour
{
    private Camera currentCamera;
    public ParticleSystem selectionParticles;
    public LayerMask clickLayerMask;


    [Header("Selection References")]
    public GameObject selectedUnitObj;
    private LivingCreature selectedCreatureScript;
    private Unit selectedUnitScript;
    public GameObject hitTileObj;


    [Header("State Bools")]
    public bool hasSelection;
    public bool isAttacking;
    public bool isPlayerInput;

    public GameObject basicAttackProjectile; //Testing

    private bool particlesPlaying;

    public static ClickSelection instance;

    private void Awake()
    {
        instance = this;
        currentCamera = Camera.main;
    }

    void Start()
    {

    }

    void Update()
    {
        if (TurnManager.instance.CurrentBattlePhase == TurnManager.BattlePhase.PlayerInput)
        {
            if (Input.GetMouseButtonDown(0))
            {
                SelectionClick();
            }
            if (Input.GetMouseButtonDown(1))
            {
                if (hasSelection)
                {
                    if (!isAttacking)
                    {
                        MoveClick();
                    }
                    else
                    {
                        AttackClick();
                    }

                }

            }

            if(!isAttacking && hasSelection)
            {
                SetMovePath();
            }

            if (Input.GetKeyDown(KeyCode.A))
            {
                if (hasSelection)
                {
                    isAttacking = !isAttacking;
                    if (isAttacking)
                    {
                        AttackSelect();
                    }
                    else
                    {
                        DrawIndicators.instance.ClearTileMatStates(true, true, true);
                        DrawIndicators.instance.BFSSelectableReturn();
                    }
                }
            }


            if (selectedUnitObj != null && !particlesPlaying) //For Selection Particles
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
        if (Input.GetMouseButtonDown(0))
        {

            RaycastHit hitInfo = new RaycastHit();
            bool hit = Physics.Raycast(currentCamera.ScreenPointToRay(Input.mousePosition), out hitInfo, 100f, clickLayerMask);

            if (hit)
            {
                if ((hitInfo.transform.gameObject.tag == "Champion") && hitInfo.collider.gameObject.activeInHierarchy)
                {
                    hasSelection = true;
                    isAttacking = false;

                    selectedUnitObj = hitInfo.collider.gameObject;
                    selectedUnitScript = selectedUnitObj.GetComponent<Unit>();
                    selectedCreatureScript = selectedUnitObj.GetComponent<LivingCreature>();

                    DrawMoveZone();

                    TurnManager.instance.SetCameraTargetBasic(selectedUnitScript.unitCamera); //Makes camera follow selected Unit.
                }
                else
                {
                    ClearSelection();
                    DrawIndicators.instance.ClearTileMatStates(true, true, true); //Clears path idicators on null selection.
                }
            }
            else
            {
                ClearSelection();
                DrawIndicators.instance.ClearTileMatStates(true, true, true); //Clears path idicators on null selection.
            }
                   
        }
    }

    private void ClearSelection()
    {
        selectedUnitObj = null;
        selectedUnitScript = null;
        selectedCreatureScript = null;
        hasSelection = false;
    }

    private void MoveClick()
    {
        if(selectedCreatureScript.CurrentEnergy >= selectedUnitScript.path.Length && selectedUnitScript.hasSuccessfulPath)
        {        
            selectedUnitScript.RunFollowPath(); //Starts the path move.           
            selectedCreatureScript.CurrentEnergy -= selectedUnitScript.path.Length; //Reduces energy by the size of length of the path. (1 Node Movement = 1 Energy).
        }
    }

    public void DrawMoveZone() //Draws where the places can move.
    {
        DrawIndicators.instance.ClearTileMatStates(true, true, true); //Clears tiles if you selected a new target and already had one selected.

        List<Node> availableNodes = Pathfinding.instance.DisplayAvailableMoves(selectedUnitScript.currentNode, selectedCreatureScript.CurrentEnergy); //BFS Call to get nodes.
        DrawIndicators.instance.BFSSelectableSet(availableNodes); //Sets nodes as selectable.
    }

    private void SetMovePath() //Drawing of path happens in the pathfinding script. 
    {
        RaycastHit hitInfo = new RaycastHit();
        bool hit = Physics.Raycast(currentCamera.ScreenPointToRay(Input.mousePosition), out hitInfo, 100f, clickLayerMask);

        if (hit && hitInfo.collider.gameObject != hitTileObj) //Makes it so the script doesnt run every frame, only when a new tile is hovered over. 
        {
            if ((hitInfo.transform.gameObject.tag == "Map") && hitInfo.collider.gameObject.activeInHierarchy)
            {
                hitTileObj = hitInfo.collider.gameObject;
                Node hitNode = GridGen.instance.NodeFromWorldPoint(hitInfo.point);

                if (hitNode.IsSelectable)
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
    }

    private void AttackSelect()
    {
        List<Node> attackNodes = GetAttackableTiles();

        DrawIndicators.instance.ClearTileMatStates(true, true, true);
        DrawIndicators.instance.AttackableSet(attackNodes);

        hitTileObj = null; //Just so when you return to movement select you don't need to hover over a new tile to get the path to be drawn. 
    }

    private List<Node> GetAttackableTiles()
    {
        List<Node> nodesToReturn = new List<Node>();

        Collider[] hitObjects = Physics.OverlapSphere(new Vector3(selectedUnitObj.transform.position.x, 0.55f, selectedUnitObj.transform.position.z), selectedCreatureScript.range.Value, clickLayerMask);

        foreach(Collider currentCol in hitObjects)
        {
            Tile currentTile = currentCol.gameObject.GetComponent<Tile>();
            if(currentTile != null && (currentTile.carryingNode.IsWalkable || currentTile.carryingNode.IsOccupied))
            {
                nodesToReturn.Add(currentTile.carryingNode);
            }
        }

        return nodesToReturn;
    }

    private void AttackClick()
    {
        RaycastHit hitInfo = new RaycastHit();
        bool hit = Physics.Raycast(currentCamera.ScreenPointToRay(Input.mousePosition), out hitInfo, 100f, clickLayerMask);

        if (hit)
        {
            if ((hitInfo.transform.gameObject.tag == "Champion") && hitInfo.collider.gameObject.activeInHierarchy)
            {
                Unit targetUnitScript = hitInfo.collider.gameObject.GetComponent<Unit>();

                if (targetUnitScript.currentNode.IsAttackable)
                {
                    Attack attack = new Attack(5, Attack.DamageType.Magical, selectedUnitObj);
                    BattleAttack battleAttack = new BattleAttack(attack, basicAttackProjectile, selectedUnitObj, hitInfo.collider.gameObject);
                    TurnManager.instance.EventResolutionReceiver(battleAttack);

                    CombatUtils.AttackHitCalculation(selectedUnitObj, hitInfo.collider.gameObject); //[TESTING FOR % CHECK.]                    
                }
            }
        }
    }

    public void ReturnToPlayerInput(bool fromBattle)
    {
        TurnManager.instance.CurrentBattlePhase = TurnManager.BattlePhase.PlayerInput;
        DrawMoveZone();

        if (fromBattle)
        {
            isAttacking = false;
        }
    }
}
