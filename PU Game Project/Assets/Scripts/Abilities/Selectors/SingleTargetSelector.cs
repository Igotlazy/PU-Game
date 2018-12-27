using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SingleTargetSelector : AttackSelection
{

    RaycastHit hitInfo;

    List<Collider> colliders = new List<Collider>();



    protected override void Update()
    {
        base.Update();
    }

    protected override void InitializeImpl(int selectorIndex)
    {
        
    }

    protected override void MadeSelectionImpl()
    {

    }
    protected override void CancelSelectionImpl()
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
        if (enteringCollider.gameObject.CompareTag("Champion"))
        {
            GameObject hitObject = enteringCollider.gameObject;

            bool alreadyHas = false;
            foreach (TargetSpecs currentSpec in attachedTargetPacket.targetObjectSpecs)
            {
                if (currentSpec.targetObj.Equals(hitObject))
                {
                    attachedTargetPacket.targetObjectSpecs.Remove(currentSpec);
                    currentSpec.targetLivRef.healthBar.HideHitChance();
                    alreadyHas = true;
                    break;
                }
            }

            if (!alreadyHas)
            {
                float hitChance = CombatUtils.AttackHitCalculation(givenAbility.associatedCreature.gameObject, hitObject);
                TargetSpecs newSpecs = new TargetSpecs(hitObject, new Vector2(10f, 5f), hitChance, "", CombatUtils.GiveShotConnector(givenAbility.associatedCreature.gameObject));
                attachedTargetPacket.targetObjectSpecs.Add(newSpecs);
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
            }
        }
        if (exitingCollider.gameObject.CompareTag("Champion"))
        {
            GameObject hitObject = exitingCollider.gameObject;
            foreach (TargetSpecs currentSpec in attachedTargetPacket.targetObjectSpecs)
            {
                if (currentSpec.targetObj.Equals(hitObject))
                {
                    attachedTargetPacket.targetObjectSpecs.Remove(currentSpec);
                    currentSpec.targetLivRef.healthBar.HideHitChance();
                    break;
                }
            }
        }
    }


}
