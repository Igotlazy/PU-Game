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

            if (selectorData.fireFromSelector)
            {
                UpdateTargetPositions();
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
        if(foundEntity == null || (!selectorData.includeCaster && foundEntity == givenAbility.associatedEntity))
        {         
            return;
        }

        GameEntity.EntityType foundType = TargetTypeConversion(selectorData.mainTargetType);

        if (foundEntity.entityType == foundType)
        {
            if(foundEntity.entityType == GameEntity.EntityType.Tile) //Just to make it more obvious;
            {
                Tile tileScript = (Tile)foundEntity;
                if (!visualNodes.Contains(tileScript.carryingNode))
                {
                    tileScript.carryingNode.IsAttackable = true;
                    visualNodes.Add(tileScript.carryingNode);
                }
            }
            //Node enteringNode = GridGen.instance.NodeFromWorldPoint(enteringCollider.gameObject.transform.position);
            GameObject hitObject = foundEntity.gameObject;
            foreach (TargetSpecs currentSpec in allSpecs)
            {
                if (currentSpec.targetObj.Equals(hitObject))
                {
                    return;
                }
            }
            if (selectType == SelectorData.SelectionType.Pick)
            {
                float hitChance = 0;
                TargetSpecs newSpec = TargetSpecCreator(foundEntity, false, out hitChance);
                UnitHitChanceDisplay(newSpec, hitChance, true);

                allSpecs.Add(newSpec);
            }
            if (selectType == SelectorData.SelectionType.AreaPick)
            {
                float hitChance = 0;
                TargetSpecs newSpec = TargetSpecCreator(foundEntity, false, out hitChance);
                UnitHitChanceDisplay(newSpec, hitChance, false);

                newSpec.indicator = Instantiate(selectionIndicator, CombatUtils.GiveCenterPoint(foundEntity.gameObject), Quaternion.identity);

                allSpecs.Add(newSpec);
                selectedSpecs.Add(newSpec);
            }
            if (selectType == SelectorData.SelectionType.AoE)
            {
                float hitChance = 0;
                TargetSpecs newSpec = TargetSpecCreator(foundEntity, true, out hitChance);
                UnitHitChanceDisplay(newSpec, hitChance, false);

                allSpecs.Add(newSpec);
                selectedSpecs.Add(newSpec);
            }
        }
    }  

    private TargetSpecs TargetSpecCreator(GameEntity foundEntity, bool isAOE, out float hitChance)
    {

        bool peekResult = false;
        hitChance = 0;
        Vector3 sourceShot = CombatUtils.GiveShotConnector(givenAbility.associatedEntity.gameObject);
        Vector3 targetShot = CombatUtils.GiveShotConnector(foundEntity.gameObject);
        Vector3 selectorPos = new Vector3(transform.position.x, transform.position.y + (GridGen.instance.nodeHeightDif * 0.25f), transform.position.z);
        Vector3 targetPartial = CombatUtils.GivePartialCheck(foundEntity.gameObject);

        if (!isAOE)
        {
            if (selectorData.isPure)
            {
                hitChance = 100f;
            }
            else
            {
                if (!selectorData.fireFromSelector)
                {
                    Vector3 newFireSource;
                    hitChance = CombatUtils.MainFireCalculation(sourceShot, targetShot, targetPartial, out peekResult, out newFireSource);
                }
                else
                {
                    hitChance = CombatUtils.MainFireCalculation(selectorPos, targetShot, targetPartial);
                }
            }
        }
        else
        {
            if (selectorData.isPure)
            {
                hitChance = 100f;
            }
            else
            {
                if (!selectorData.fireFromSelector)
                {
                    hitChance = CombatUtils.MainFireCalculation(givenAbility.associatedEntity.gameObject, foundEntity.gameObject);
                }
                else
                {
                    hitChance = CombatUtils.MainFireCalculation(
                        new Vector3(transform.position.x, transform.position.y + (GridGen.instance.nodeHeightDif * 0.25f), transform.position.z), 
                        targetShot, targetPartial);
                }
            }
        }

        if (!selectorData.fireFromSelector)
        {
            TargetSpecs newSpec = new TargetSpecs(foundEntity, sourceShot);
            newSpec.didPeek = peekResult;
            return newSpec;
        }
        else
        {
            TargetSpecs newSpec = new TargetSpecs(foundEntity, selectorPos);
            return newSpec;
        }

    }

    protected void UpdateTargetPositions()
    {
        Vector3 selectorPos = new Vector3(transform.position.x, transform.position.y + (GridGen.instance.nodeHeightDif * 0.25f), transform.position.z);

        foreach (TargetSpecs target in allSpecs)
        {
            target.fireOrigin = selectorPos;
            if(selectorData.selectionType == SelectorData.SelectionType.Pick)
            {
                UnitHitChanceDisplay(target, CombatUtils.MainFireCalculation(target.fireOrigin, target.entityScript.gameObject), true);
            }
            else
            {
                UnitHitChanceDisplay(target, CombatUtils.MainFireCalculation(target.fireOrigin, target.entityScript.gameObject), false);
            }
        }
    }


    private void OnTriggerExit(Collider exitingCollider)
    {
        GameEntity foundEntity = exitingCollider.gameObject.GetComponent<GameEntity>();
        if (foundEntity == null)
        {
            return;
        }

        GameEntity.EntityType foundType = TargetTypeConversion(selectorData.mainTargetType);

        if (foundEntity.entityType == foundType)
        {
            if (foundEntity.entityType == GameEntity.EntityType.Tile)
            {
                Tile tileScript = (Tile)foundEntity;
                if (visualNodes.Contains(tileScript.carryingNode))
                {
                    tileScript.carryingNode.IsAttackable = false;
                    visualNodes.Remove(tileScript.carryingNode);
                }
            }

            GameObject hitObject = foundEntity.gameObject;
            foreach (TargetSpecs currentSpec in allSpecs)
            {
                if (currentSpec.targetObj.Equals(hitObject))
                {
                    if (currentSpec.indicator != null)
                    {
                        Destroy(currentSpec.indicator);
                    }
                    if (selectedSpecs.Contains(currentSpec))
                    {
                        selectedSpecs.Remove(currentSpec);
                    }
                    allSpecs.Remove(currentSpec);

                    if (currentSpec.targetType == TargetSpecs.TargetType.Unit)
                    {
                        Unit unit = (Unit)currentSpec.entityScript;
                        unit.healthBar.HideHitChance();
                    }
                    break;
                }
            }
        }                 
    }

    protected void GameEntitySwitch(GameEntity givenEntity)
    {

    }
}
