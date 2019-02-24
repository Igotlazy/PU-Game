using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneNodeCollectorTrigger : AbilitySelection
{
    public List<Node> nodeList = new List<Node>();
    public AbilitySelection attackInd;

    private void Awake()
    {
        attackInd = GetComponent<AbilitySelection>();
    }

    protected override void InitializeImpl()
    {

    }

    private void OnTriggerEnter(Collider enteringCollider)
    {
        if (enteringCollider.gameObject.CompareTag("Tile"))
        {
            Node enteringNode = GridGen.instance.NodeFromWorldPoint(enteringCollider.gameObject.transform.position);
            enteringNode.IsAttackable = true;
            collectedNodes.Add(enteringNode);
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
            }
        }
    }

    protected override void MadeSelectionImpl()
    {

    }
    protected override void CancelSelectionImpl()
    {

    }


}
