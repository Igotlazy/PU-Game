using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneNodeCollectorTrigger : MonoBehaviour
{
    public List<Node> nodeList = new List<Node>();
    public AttackSelection attackInd;

    private void Awake()
    {
        attackInd = GetComponent<AttackSelection>();
    }

    private void OnTriggerEnter(Collider enteringCollider)
    {
        if (enteringCollider.gameObject.CompareTag("Tile"))
        {
            Node enteringNode = GridGen.instance.NodeFromWorldPoint(enteringCollider.gameObject.transform.position);
            enteringNode.IsAttackable = true;
            attackInd.targettedNodes.Add(enteringNode);
        }
    }

    private void OnTriggerExit(Collider exitingCollider)
    {
        if (exitingCollider.gameObject.CompareTag("Tile"))
        {
            Node exitingNode = GridGen.instance.NodeFromWorldPoint(exitingCollider.gameObject.transform.position);
            if (attackInd.targettedNodes.Contains(exitingNode))
            {
                exitingNode.IsAttackable = false;
                attackInd.targettedNodes.Remove(exitingNode);
            }
        }
    }
}
