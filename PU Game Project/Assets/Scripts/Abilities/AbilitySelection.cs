using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using MHA.BattleBehaviours;
using MHA.UserInterface;

public abstract class AbilitySelection : MonoBehaviour {

    protected int maxNumOfSelections;
    public bool hasLoadedTargets;
    public CharAbility givenAbility;

    protected HashSet<Node> collectedNodes = new HashSet<Node>();
    protected List<TargetSpecs> selectedSpecs = new List<TargetSpecs>();
    public List<TargetSpecs> allSpecs = new List<TargetSpecs>();

    public SelectorPacket selPacket;
    public SelectorData selectorData;

    public GameObject selectionIndicator;

    protected SelectorData.SelectionType selectType = SelectorData.SelectionType.Null;

    public bool isAIControlled;


    public void Initialize()
    {
        AbilityBar.AbilityButtonClickEVENT += CancelSelection;
        CursorController.instance.CursorNewNodeEVENT += MoveSelector;
        ClickSelection.instance.canSelect = false;

        selectorData = selPacket.selectorData;

        selectType = selectorData.selectionType;
        if(selectType == SelectorData.SelectionType.Pick)
        {
            maxNumOfSelections = selectorData.maxNumOfSelect;
        }

        InitializeImpl();
    }
    protected abstract void InitializeImpl();

    protected virtual void Update()
    {
        if (!isAIControlled)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                CancelSelection();
            }
        }
    }

    public void MadeSelection()
    {

        if((selectedSpecs.Count > 0 && 
            (selectorData.selectionType == SelectorData.SelectionType.Pick || selectorData.selectionType == SelectorData.SelectionType.AreaPick)) 
            || 
            selectorData.selectionType == SelectorData.SelectionType.AoE
            || 
            selectorData.selectionType == SelectorData.SelectionType.Null)
        {         
            CleanUp();

            MadeSelectionImpl();

            selPacket.targetObjectSpecs = selectedSpecs;
            selPacket.TargetNodes = collectedNodes;

            hasLoadedTargets = true;

            DestroyImmediate(this.gameObject);
        }
    }
    protected abstract void MadeSelectionImpl();

    public void CancelSelection()
    {
        CleanUp();

        CancelSelectionImpl();

        givenAbility.CancelTargets();

        Destroy(this.gameObject);
    }
    protected abstract void CancelSelectionImpl();

    public virtual void CleanUp()
    {
        AbilityBar.AbilityButtonClickEVENT -= CancelSelection;
        CursorController.instance.CursorNewNodeEVENT -= MoveSelector;

        ClickSelection.instance.canSelect = true;

        DrawIndicators.instance.ClearTileMatStates(true, true, true);
        foreach(Node currNode in collectedNodes)
        {
            if (currNode.IsAttackable)
            {
                currNode.IsAttackable = false;
            }
        }
        foreach (TargetSpecs currentSpec in allSpecs)
        {
            if(currentSpec.targetType == TargetSpecs.TargetType.Unit)
            {
                Unit unit = (Unit)currentSpec.entityScript;
                unit.healthBar.HideHitChance();
            }

            if (currentSpec.indicator != null)
            {
                Destroy(currentSpec.indicator);
            }
        }
    }

    protected void GatherClick()
    {
        RaycastHit hitInfo;
        bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo, 100f, CombatUtils.gameEntityMask);

        if (hit && !EventSystem.current.IsPointerOverGameObject()) //Makes sure it doesn't interact with UI
        {
            GameObject hitObject = hitInfo.collider.gameObject;
            Gather(hitObject);
        }
    }

    protected virtual void MoveSelector(Node givenNode)
    {
        
    }
    protected virtual void RotateSelector()
    {

    }

    public void Gather(GameObject hitObject)
    {
        if (hitObject.CompareTag("Champion") && hitObject.GetComponent<Unit>().currentNode.IsAttackable)
        {
            foreach (TargetSpecs currentSpec in allSpecs)
            {
                if (currentSpec.targetObj.Equals(hitObject))
                {
                    if (currentSpec.indicator == null)
                    {
                        Unit unitScript = hitObject.GetComponent<Unit>();
                        currentSpec.indicator = Instantiate(selectionIndicator, unitScript.centerPoint.transform.position, Quaternion.identity);

                        if(currentSpec.targetType == TargetSpecs.TargetType.Unit)
                        {
                            Unit unit = (Unit)currentSpec.entityScript;
                            unit.healthBar.DisplayHitChance(); 
                        }

                        selectedSpecs.Add(currentSpec);

                        if (selectedSpecs.Count > maxNumOfSelections)
                        {
                            TargetSpecs zero = selectedSpecs[0];
                            Destroy(zero.indicator);

                            if (zero.targetType == TargetSpecs.TargetType.Unit)
                            {
                                Unit unit = (Unit)zero.entityScript;
                                unit.healthBar.FadeHitChance();
                            }

                            selectedSpecs.RemoveAt(0);
                        }
                    }
                    else
                    {
                        Destroy(currentSpec.indicator);
                        if (currentSpec.targetType == TargetSpecs.TargetType.Unit)
                        {
                            Unit unit = (Unit)currentSpec.entityScript;
                            unit.healthBar.FadeHitChance();
                        }
                        selectedSpecs.Remove(currentSpec);
                    }
                    break;
                }
            }
        }
    }

    protected void UnitHitChanceDisplay(TargetSpecs givenSpec, float hitChance, bool faded)
    {
        if (givenSpec.targetType == TargetSpecs.TargetType.Unit)
        {
            Unit unit = (Unit)givenSpec.entityScript;
            unit.healthBar.SetHitChance(hitChance);
            if (faded)
            {
                unit.healthBar.FadeHitChance();
            }
            else
            {
                unit.healthBar.DisplayHitChance();
            }

        }
    }
}
