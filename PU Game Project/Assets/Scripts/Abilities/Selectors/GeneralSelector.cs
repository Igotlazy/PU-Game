using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using MHA.UserInterface;

public class GeneralSelector : AbilitySelection
{
    public GameObject selectTargetsButton;

    public GameObject followZone;
    public AnimationCurve followAnimGrow;
    public float followAnimGrowTime;

    public GameObject followZoneMarker;


    private GameObject spawnedButton;
    private Vector3 spawnLocation;
    private Plane rotatePlane;
    private Camera mainCam;

    private GameObject spawnedFollowZone;
    private GameObject spawnedFollowZoneMarker;

    [Space]
    [Header("State Bools")]
    public bool moveMode;
    [SerializeField] private bool rotateMode;
    public bool RotateMode
    {
        get
        {
            return rotateMode;
        }
        set
        {
            rotateMode = value;
            if (rotateMode)
            {
                rotatePlane = new Plane(Vector3.up, transform.position);
            }
        }
    }
    [SerializeField] private bool selectMode;
    public bool SelectMode
    {
        get
        {
            return selectMode;
        }
        set
        {
            selectMode = value;
            if (selectMode)
            {
                Button buttonScript = spawnedButton.GetComponent<Button>();
                buttonScript.interactable = true;
            }
            else
            {
                Button buttonScript = spawnedButton.GetComponent<Button>();
                buttonScript.interactable = false;
            }
        }
    }

    protected override void InitializeImpl()
    {
        mainCam = Camera.main;
        spawnLocation = transform.position;
        if(selectorData.rotate && selectorData.rotateType == SelectorData.RotateType.FourSidedD)
        {
            transform.LookAt(transform.position + Vector3.right + Vector3.forward);
        }

        if (selectorData.follow)
        {
            spawnedFollowZone = Instantiate(followZone, transform.position, Quaternion.identity);
            spawnedFollowZoneMarker = Instantiate(followZoneMarker, transform.position, Quaternion.identity);
            StartCoroutine(FollowZoneGrow());
        }

        spawnedButton = Instantiate(selectTargetsButton, BattleUIReferences.instance.mainButtonSelection.transform);
        Button buttonScript = spawnedButton.GetComponent<Button>();
        if(selectorData.follow || selectorData.rotate)
        {
            SelectMode = false;
            if (selectorData.follow)
            {
                moveMode = true;
            }
            else
            {
                RotateMode = true;
            }
        }
        else
        {
            SelectMode = true;
        }
        buttonScript.onClick.AddListener(MadeSelection);
    }

    IEnumerator FollowZoneGrow()
    {
        float currentTime = 0;
        float range = selectorData.followRange * GridGen.instance.NodeDiameter;
        float scale = range + ((1.5f / range));

        while (currentTime < followAnimGrowTime)
        {
            currentTime += Time.deltaTime;
            float currentSize = followAnimGrow.Evaluate(currentTime / followAnimGrowTime);
            float newScale = scale * currentSize;
            spawnedFollowZone.transform.localScale = new Vector3(newScale, newScale, newScale);

            yield return null;
        }
        spawnedFollowZone.transform.localScale = new Vector3(scale, scale, scale);
    }


    protected override void Update()
    {
        base.Update();

        if (!isAIControlled)
        {
            if (Input.GetMouseButtonDown(0) && (selectType == SelectorData.SelectionType.Pick) && selectMode)
            {
                GatherClick();
            }
            if (Input.GetMouseButtonDown(0))
            {
                ClickProgress();
            }
            if (Input.GetMouseButtonDown(1))
            {
                ClickRegress();
            }

            RotateSelector();
        }
    }
    private void ClickProgress()
    {
        if(selectorData.follow && moveMode)
        {
            moveMode = false;
            //Stop movement.
            if (selectorData.rotate)
            {
                RotateMode = true;
            }
            else
            {
                SelectMode = true;
            }
        }
        else if(selectorData.rotate && RotateMode)
        {
            RotateMode = false;
            //Stop rotation
            SelectMode = true;
        }
    }
    private void ClickRegress()
    {
        if (selectorData.rotate && SelectMode)
        {
            SelectMode = false;
            RotateMode = true;
            //Stop Select.
        }
        else if (selectorData.follow && (RotateMode || SelectMode))
        {
            RotateMode = false;
            SelectMode = false;
            moveMode = true;
            MoveSelector(CursorController.instance.currentNode);
            //Stop Rotation
        }
    }

    protected override void MoveSelector(Node givenNode)
    {
        base.MoveSelector(givenNode);
        if (moveMode)
        {
            if(Vector3.Distance(givenNode.worldPosition, spawnLocation) <= selectorData.followRange * GridGen.instance.NodeDiameter)
            {
                transform.position = givenNode.worldPosition;
                spawnedFollowZoneMarker.transform.position = givenNode.worldPosition;
            }
            else
            {
                Vector3 endPos = (givenNode.worldPosition - spawnLocation).normalized * (selectorData.followRange * GridGen.instance.NodeDiameter) + spawnLocation;
                Node returnedNode = GridGen.instance.NodeFromWorldPoint(endPos);
                if(returnedNode != null)
                {
                    transform.position = returnedNode.worldPosition;
                    spawnedFollowZoneMarker.transform.position = returnedNode.worldPosition;
                }
            }
        }
    }

    protected override void RotateSelector()
    {
        if (RotateMode)
        {
            base.RotateSelector();

            float distance;
            Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
            if (rotatePlane.Raycast(ray, out distance))
            {
                Vector3 point = ray.GetPoint(distance);

                Vector3 compare = point - transform.position;

                Vector3 direction = Vector3.zero;
                float lowest = 360f;

                if(selectorData.rotateType == SelectorData.RotateType.FourSidedA)
                {
                    RotateAux(Vector3.right, compare, direction, lowest, out direction, out lowest);
                    RotateAux(Vector3.forward, compare, direction, lowest, out direction, out lowest);
                    RotateAux(Vector3.left, compare, direction, lowest, out direction, out lowest);
                    RotateAux(Vector3.back, compare, direction, lowest, out direction, out lowest);
                }
                else if(selectorData.rotateType == SelectorData.RotateType.FourSidedD)
                {
                    RotateAux(Vector3.right + Vector3.forward, compare, direction, lowest, out direction, out lowest);
                    RotateAux(Vector3.forward + Vector3.left, compare, direction, lowest, out direction, out lowest);
                    RotateAux(Vector3.left + Vector3.back, compare, direction, lowest, out direction, out lowest);
                    RotateAux(Vector3.back + Vector3.right, compare, direction, lowest, out direction, out lowest);
                }
                else if(selectorData.rotateType == SelectorData.RotateType.EightSided)
                {
                    RotateAux(Vector3.right, compare, direction, lowest, out direction, out lowest);
                    RotateAux(Vector3.forward, compare, direction, lowest, out direction, out lowest);
                    RotateAux(Vector3.left, compare, direction, lowest, out direction, out lowest);
                    RotateAux(Vector3.back, compare, direction, lowest, out direction, out lowest);

                    RotateAux(Vector3.right + Vector3.forward, compare, direction, lowest, out direction, out lowest);
                    RotateAux(Vector3.forward + Vector3.left, compare, direction, lowest, out direction, out lowest);
                    RotateAux(Vector3.left + Vector3.back, compare, direction, lowest, out direction, out lowest);
                    RotateAux(Vector3.back + Vector3.right, compare, direction, lowest, out direction, out lowest);
                }

                transform.LookAt(transform.position + direction);

            }
        }
    }

    private void RotateAux(Vector3 givenVector, Vector3 compareVector, Vector3 currDirection, float currLowest, out Vector3 direction, out float lowest)
    {
        float angle = Vector3.Angle(givenVector, compareVector);
        if (angle < currLowest)
        {
            direction = givenVector;
            lowest = angle;
        }
        else
        {
            direction = currDirection;
            lowest = currLowest;
        }
    }


    protected override void MadeSelectionImpl()
    {
        if(spawnedButton != null)
        {
            Destroy(spawnedButton);
        }
        if(spawnedFollowZone != null)
        {
            Destroy(spawnedFollowZone);
        }
        if (spawnedFollowZoneMarker != null)
        {
            Destroy(spawnedFollowZoneMarker);
        }
    }

    protected override void CancelSelectionImpl()
    {
        if (spawnedButton != null)
        {
            Destroy(spawnedButton);
        }
        if (spawnedFollowZone != null)
        {
            Destroy(spawnedFollowZone);
        }
        if (spawnedFollowZoneMarker != null)
        {
            Destroy(spawnedFollowZoneMarker);
        }
    }



    private void OnTriggerEnter(Collider enteringCollider)
    {
        GameEntity foundEntity = enteringCollider.gameObject.GetComponent<GameEntity>();
        if(foundEntity == null)
        {
            return;
        }

        GameEntity.EntityType foundType = TargetTypeConversion(selectorData.mainTargetType);

        if (foundEntity.entityType == foundType)
        {
            if(foundType != GameEntity.EntityType.Unit)
            {

            }
            Node enteringNode = GridGen.instance.NodeFromWorldPoint(enteringCollider.gameObject.transform.position);
            Unit unit = (Unit)givenAbility.associatedEntity;
            if (!collectedNodes.Contains(enteringNode) && unit.currentNode != enteringNode)
            {
                enteringNode.IsAttackable = true;
                collectedNodes.Add(enteringNode);

                if (enteringNode.occupant != null)
                {
                    GameObject hitObject = enteringNode.occupant;

                    bool alreadyHas = false;
                    foreach (TargetSpecs currentSpec in allSpecs)
                    {
                        if (currentSpec.targetObj.Equals(hitObject))
                        {
                            alreadyHas = true;
                            break;
                        }
                    }

                    if (!alreadyHas && selectType == SelectorData.SelectionType.Pick)
                    {
                        Vector3 sourceShot = CombatUtils.GiveShotConnector(givenAbility.associatedEntity.gameObject);
                        Vector3 targetShot = CombatUtils.GiveShotConnector(hitObject);
                        Vector3 targetPartial = CombatUtils.GivePartialCheck(hitObject);
                        bool peekResult = false;
                        float hitChance = 0;
                        if (selectorData.isPure)
                        {
                            hitChance = 100f;
                        }
                        else
                        {
                            Vector3 newFireSource;
                            hitChance = CombatUtils.MainFireCalculation(sourceShot, targetShot, targetPartial, out peekResult, out newFireSource);
                        }

                        TargetSpecs newSpec = new TargetSpecs(hitObject.GetComponent<GameEntity>(), hitChance, sourceShot);
                        newSpec.didPeek = peekResult;

                        UnitHitChanceDisplay(newSpec, hitChance, true);

                        allSpecs.Add(newSpec);
                    }
                    if (!alreadyHas && selectType == SelectorData.SelectionType.AreaPick)
                    {
                        Vector3 sourceShot = CombatUtils.GiveShotConnector(givenAbility.associatedEntity.gameObject);
                        Vector3 targetShot = CombatUtils.GiveShotConnector(hitObject);
                        Vector3 targetPartial = CombatUtils.GivePartialCheck(hitObject);
                        bool peekResult = false;
                        float hitChance = 0;
                        if (selectorData.isPure)
                        {
                            hitChance = 100f;
                        }
                        else
                        {
                            Vector3 newFireSource;
                            hitChance = CombatUtils.MainFireCalculation(sourceShot, targetShot, targetPartial, out peekResult, out newFireSource);
                        }

                        TargetSpecs newSpec = new TargetSpecs(hitObject.GetComponent<GameEntity>(), hitChance, sourceShot);
                        newSpec.didPeek = peekResult;

                        UnitHitChanceDisplay(newSpec, hitChance, false);
                        Unit unitScript = hitObject.GetComponent<Unit>();
                        newSpec.indicator = Instantiate(selectionIndicator, unitScript.centerPoint.transform.position, Quaternion.identity);
                        allSpecs.Add(newSpec);
                        selectedSpecs.Add(newSpec);
                    }
                    if (!alreadyHas && selectType == SelectorData.SelectionType.AoE)
                    {
                        float hitChance = 0;
                        if (selectorData.isPure)
                        {
                            hitChance = 100f;
                        }
                        else
                        {
                            hitChance = CombatUtils.MainFireCalculation(givenAbility.associatedEntity.gameObject, hitObject);
                        }
                        TargetSpecs newSpec = new TargetSpecs(hitObject.GetComponent<GameEntity>(), hitChance, CombatUtils.GiveShotConnector(givenAbility.associatedEntity.gameObject));
                        UnitHitChanceDisplay(newSpec, hitChance, false);
                        allSpecs.Add(newSpec);
                        selectedSpecs.Add(newSpec);
                    }
                }

            }
        }
    }  
        
   
    private void OnTriggerExit(Collider exitingCollider)
    {
        if (exitingCollider.gameObject.CompareTag("Tile"))
        {
            Node exitingNode = GridGen.instance.NodeFromWorldPoint(exitingCollider.gameObject.transform.position);
            if (collectedNodes.Contains(exitingNode))
            {
                exitingNode.IsAttackable = false;
                collectedNodes.Remove(exitingNode);

                if(exitingNode.occupant != null)
                {
                    GameObject hitObject = exitingNode.occupant;
                    foreach (TargetSpecs currentSpec in allSpecs)
                    {
                        if (currentSpec.targetObj.Equals(hitObject))
                        {
                            if(currentSpec.indicator != null)
                            {
                                Destroy(currentSpec.indicator);                               
                            }
                            if (selectedSpecs.Contains(currentSpec))
                            {
                                selectedSpecs.Remove(currentSpec);
                            }
                            allSpecs.Remove(currentSpec);

                            if(currentSpec.targetType == TargetSpecs.TargetType.Unit)
                            {
                                Unit unit = (Unit)currentSpec.entityScript;
                                unit.healthBar.HideHitChance();
                            }
                            break;
                        }
                    }
                }
            }
        }
    }


    private GameEntity.EntityType TargetTypeConversion(SelectorData.TargetType type)
    {
        switch (type)
        {
            case SelectorData.TargetType.Unit:
                return GameEntity.EntityType.Unit;

            case SelectorData.TargetType.Tile:
                return GameEntity.EntityType.Tile;

            case SelectorData.TargetType.Structure:
                return GameEntity.EntityType.Structure;

            case SelectorData.TargetType.Projectile:
                return GameEntity.EntityType.Projectile;

            default: return GameEntity.EntityType.Unit;
        }
    }


}
