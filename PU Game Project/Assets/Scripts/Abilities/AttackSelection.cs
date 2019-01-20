using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using MHA.BattleBehaviours;

public abstract class AttackSelection : MonoBehaviour {

    protected int maxNumOfSelections;
    public bool hasLoadedTargets;
    public CharAbility givenAbility;
    protected HashSet<Node> collectedNodes = new HashSet<Node>();
    protected List<TargetSpecs> selectedSpecs = new List<TargetSpecs>();
    protected List<TargetSpecs> allSpecs = new List<TargetSpecs>();
    public SelectorPacket attachedTargetPacket;

    public GameObject selectionIndicator;

    protected SelectorPacket.SelectionType selectType = SelectorPacket.SelectionType.Null;


    public void Initialize()
    {
        selectType = attachedTargetPacket.selectionType;
        if(selectType == SelectorPacket.SelectionType.Target)
        {
            maxNumOfSelections = attachedTargetPacket.maxNumOfSelect;
        }

        InitializeImpl();
    }
    protected abstract void InitializeImpl();

    protected virtual void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            MadeSelection();
        }
        if (Input.GetMouseButtonDown(0))
        {
            CancelSelection();
        }


        if (Input.GetKeyDown(KeyCode.B) && (selectType == SelectorPacket.SelectionType.Target))
        {
            GatherClick();
        }
    }

    protected void MadeSelection()
    {
        DisplayCleanup();

        attachedTargetPacket.targetObjectSpecs = selectedSpecs;
        attachedTargetPacket.TargetNodes = collectedNodes;

        MadeSelectionImpl();

        hasLoadedTargets = true;

        Destroy(this.gameObject);
    }
    protected abstract void MadeSelectionImpl();

    protected void CancelSelection()
    {
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
            currentSpec.targetLivRef.healthBar.HideHitChance();

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
                            currentSpec.targetLivRef.healthBar.DisplayHitChance();
                            selectedSpecs.Add(currentSpec);

                            if (selectedSpecs.Count > maxNumOfSelections)
                            {
                                Destroy(selectedSpecs[0].indicator);
                                selectedSpecs[0].targetLivRef.healthBar.FadeHitChance();
                                selectedSpecs.RemoveAt(0);
                            }
                        }
                        else
                        {
                            Destroy(currentSpec.indicator);
                            currentSpec.targetLivRef.healthBar.FadeHitChance();
                            selectedSpecs.Remove(currentSpec);
                        }
                        break;
                    }
                }
            }
        }
    }
}
