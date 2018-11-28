using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MHA.BattleBehaviours;

public abstract class AttackSelection : MonoBehaviour{

    public bool hasSentTargets;
    public CharAbility givenAbility;
    protected List<Node> collectedNodes = new List<Node>();



    protected virtual void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            Debug.Log("Send Ability");
            MadeSelection();
        }
        if (Input.GetMouseButtonDown(0))
        {
            CancelSelection();
        }
    }

    protected void MadeSelection()
    {
        NodeDisplayCleanup();
        MadeSelectionImpl();
        givenAbility.abilityTargets.Add(collectedNodes);
        hasSentTargets = true;

        Destroy(this.gameObject);
    }
    public abstract void MadeSelectionImpl();

    protected void CancelSelection()
    {
        NodeDisplayCleanup();
        givenAbility.CancelTargets();
        Destroy(this.gameObject);
    }

    public virtual void NodeDisplayCleanup()
    {
        DrawIndicators.instance.ClearTileMatStates(true, true, true);
    }
}
