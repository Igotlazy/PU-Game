using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneNodeCollectorTrigger : AttackSelection
{
    public List<Node> nodeList = new List<Node>();
    public AttackSelection attackInd;

    private void Awake()
    {
        attackInd = GetComponent<AttackSelection>();
    }

    protected override void InitializeImpl(int selectorIndex)
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
