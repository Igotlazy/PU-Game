using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using MHA.BattleBehaviours;
using MHA.UserInterface;

public abstract class AttackSelection : MonoBehaviour {

    protected int maxNumOfSelections;
    public bool hasLoadedTargets;
    public CharAbility givenAbility;

    protected HashSet<Node> collectedNodes = new HashSet<Node>();
    protected List<TargetSpecs> selectedSpecs = new List<TargetSpecs>();
    public List<TargetSpecs> allSpecs = new List<TargetSpecs>();

    public SelectorPacket attachedTargetPacket;

    public GameObject selectionIndicator;

    protected SelectorPacket.SelectionType selectType = SelectorPacket.SelectionType.Null;

    public bool isAIControlled;


    public void Initialize()
    {
        AbilityBar.AbilityButtonClickEVENT += CancelSelection;
        ClickSelection.instance.canSelect = false;

        selectType = attachedTargetPacket.selectionType;
        if(selectType == SelectorPacket.SelectionType.Target)
        {
            maxNumOfSelections = attachedTargetPacket.maxNumOfSelect;
        }

        isAIControlled = givenAbility.associatedUnit.isAIControlled;
        if (isAIControlled)
        {
            AISelect();
        }

        InitializeImpl();
    }
    protected abstract void InitializeImpl();
    protected virtual void AISelect()
    {

    }

    protected virtual void Update()
    {
        if (!isAIControlled)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                MadeSelection();
            }
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                CancelSelection();
            }

            if (Input.GetMouseButtonDown(0) && (selectType == SelectorPacket.SelectionType.Target))
            {
                GatherClick();
            }
        }
    }

    public void MadeSelection()
    {
        if((selectedSpecs.Count > 0 && (attachedTargetPacket.selectionType == SelectorPacket.SelectionType.Target || attachedTargetPacket.selectionType == SelectorPacket.SelectionType.AreaTarget)) 
            || attachedTargetPacket.selectionType == SelectorPacket.SelectionType.AoE || attachedTargetPacket.selectionType == SelectorPacket.SelectionType.Null)
        {
            AbilityBar.AbilityButtonClickEVENT -= CancelSelection;
            ClickSelection.instance.canSelect = true;

            DisplayCleanup();

            attachedTargetPacket.targetObjectSpecs = selectedSpecs;
            attachedTargetPacket.TargetNodes = collectedNodes;

            MadeSelectionImpl();

            hasLoadedTargets = true;

            Destroy(this.gameObject);
        }
    }
    protected abstract void MadeSelectionImpl();

    public void CancelSelection()
    {
        AbilityBar.AbilityButtonClickEVENT -= CancelSelection;
        ClickSelection.instance.canSelect = true;

        DisplayCleanup();

        CancelSelectionImpl();

        givenAbility.CancelTargets();

        Destroy(this.gameObject);
    }
    protected abstract void CancelSelectionImpl();

    public virtual void DisplayCleanup()
    {
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

    private void GatherClick()
    {
        RaycastHit hitInfo;
        bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo, 100f, CombatUtils.gameEntityMask);

        if (hit && !EventSystem.current.IsPointerOverGameObject()) //Makes sure it doesn't interact with UI
        {
            GameObject hitObject = hitInfo.collider.gameObject;
            Gather(hitObject);
        }
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
                                Unit unit = (Unit)currentSpec.entityScript;
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
