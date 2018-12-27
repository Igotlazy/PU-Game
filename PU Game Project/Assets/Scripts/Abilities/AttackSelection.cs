using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using MHA.BattleBehaviours;

public abstract class AttackSelection : MonoBehaviour {

    protected int numOfSelections = 1;
    public bool hasLoadedTargets;
    public CharAbility givenAbility;
    protected HashSet<Node> collectedNodes = new HashSet<Node>();
    public TargetPacket attachedTargetPacket;

    public GameObject selectionIndicator;


    public void Initialize(int selectorIndex)
    {
        InitializeImpl(selectorIndex);
    }
    protected abstract void InitializeImpl(int selectorIndex);

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
        if (Input.GetKeyDown(KeyCode.B))
        {
            GatherClick();
        }
    }

    protected void MadeSelection()
    {
        NodeDisplayCleanup();
        foreach(TargetSpecs currentSpec in attachedTargetPacket.targetObjectSpecs)
        {
            currentSpec.targetLivRef.healthBar.HideHitChance();
            Destroy(currentSpec.indicator);
        }

        MadeSelectionImpl();

        attachedTargetPacket.TargetNodes = collectedNodes;

        hasLoadedTargets = true;

        Destroy(this.gameObject);
    }
    protected abstract void MadeSelectionImpl();

    protected void CancelSelection()
    {
        NodeDisplayCleanup();

        foreach (TargetSpecs currentSpec in attachedTargetPacket.targetObjectSpecs)
        {
            currentSpec.targetLivRef.healthBar.HideHitChance();
            Destroy(currentSpec.indicator);
        }
        CancelSelectionImpl();

        givenAbility.CancelTargets();

        Destroy(this.gameObject);
    }
    protected abstract void CancelSelectionImpl();

    public virtual void NodeDisplayCleanup()
    {
        DrawIndicators.instance.ClearTileMatStates(true, true, true);
        foreach(Node currNode in collectedNodes)
        {
            if (currNode.IsAttackable)
            {
                currNode.IsAttackable = false;
            }
        }
    }

    public virtual void AddToNodeSet(Node givenNode)
    {

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
                bool alreadyHas = false;
                foreach (TargetSpecs currentSpec in attachedTargetPacket.targetObjectSpecs)
                {
                    if (currentSpec.targetObj.Equals(hitObject))
                    {
                        if (currentSpec.indicator != null)
                        {
                            Destroy(currentSpec.indicator);
                        }
                        attachedTargetPacket.targetObjectSpecs.Remove(currentSpec);
                        currentSpec.targetLivRef.healthBar.HideHitChance();
                        alreadyHas = true;
                        break;
                    }
                }

                if (!alreadyHas)
                {
                    Debug.Log(numOfSelections);
                    Debug.Log(attachedTargetPacket.targetObjectSpecs.Count);
                    if(attachedTargetPacket.targetObjectSpecs.Count >= numOfSelections)
                    {
                        Debug.Log("We're Here");
                        Destroy(attachedTargetPacket.targetObjectSpecs[0].indicator);
                        attachedTargetPacket.targetObjectSpecs.RemoveAt(0);
                    }

                    float hitChance = CombatUtils.AttackHitCalculation(givenAbility.associatedCreature.gameObject, hitObject);
                    TargetSpecs newSpecs = new TargetSpecs(hitObject, new Vector2(10f, 5f), hitChance, "", CombatUtils.GiveShotConnector(givenAbility.associatedCreature.gameObject));
                    attachedTargetPacket.targetObjectSpecs.Add(newSpecs);

                    Unit unitScript = hitObject.GetComponent<Unit>();
                    newSpecs.indicator = Instantiate(selectionIndicator, unitScript.centerPoint.transform.position, Quaternion.identity);
                }
            }
        }
    }
}
