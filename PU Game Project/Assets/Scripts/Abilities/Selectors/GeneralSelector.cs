using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GeneralSelector : AttackSelection
{
    List<Collider> colliders = new List<Collider>();



    protected override void Update()
    {
        base.Update();
    }

    protected override void InitializeImpl()
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
            if(givenAbility.associatedCreature.gameObject.GetComponent<Unit>().currentNode != enteringNode)
            {
                enteringNode.IsAttackable = true;
                collectedNodes.Add(enteringNode);

                if (enteringNode.occupant != null)
                {
                    GameObject hitObject = enteringNode.occupant;

                    bool alreadyHas = false;
                    foreach (TargetSpecs currentSpec in allSpecs)
                    {
                        if (currentSpec.targetObj.Equals(hitObject))
                        {
                            alreadyHas = true;
                            break;
                        }
                    }

                    if (!alreadyHas && selectType == TargetPacket.SelectionType.Single)
                    {
                        float hitChance = CombatUtils.MainFireCalculation(givenAbility.associatedCreature.gameObject, hitObject);
                        TargetSpecs newSpec = new TargetSpecs(hitObject, new Vector2(10f, 5f), hitChance, "", CombatUtils.GiveShotConnector(givenAbility.associatedCreature.gameObject));
                        newSpec.targetLivRef.healthBar.FadeHitChance();
                        allSpecs.Add(newSpec);
                    }
                    if(!alreadyHas && selectType == TargetPacket.SelectionType.AoE)
                    {
                        float hitChance = CombatUtils.MainFireCalculation(givenAbility.associatedCreature.gameObject, hitObject);
                        TargetSpecs newSpec = new TargetSpecs(hitObject, new Vector2(10f, 5f), hitChance, "", CombatUtils.GiveShotConnector(givenAbility.associatedCreature.gameObject));
                        newSpec.targetLivRef.healthBar.DisplayHitChance();
                        allSpecs.Add(newSpec);
                        selectedSpecs.Add(newSpec);
                    }
                    if(!alreadyHas && selectType == TargetPacket.SelectionType.PureAOE)
                    {
                        TargetSpecs newSpec = new TargetSpecs(hitObject, new Vector2(10f, 5f), 100f, "", CombatUtils.GiveShotConnector(givenAbility.associatedCreature.gameObject));
                        newSpec.targetLivRef.healthBar.DisplayHitChance();
                        allSpecs.Add(newSpec);
                        selectedSpecs.Add(newSpec);
                    }
                }

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

                if(exitingNode.occupant != null)
                {
                    GameObject hitObject = exitingNode.occupant;
                    foreach (TargetSpecs currentSpec in allSpecs)
                    {
                        if (currentSpec.targetObj.Equals(hitObject))
                        {
                            if(currentSpec.indicator != null)
                            {
                                Destroy(currentSpec.indicator);
                                selectedSpecs.Remove(currentSpec);
                            }

                            allSpecs.Remove(currentSpec);
                            currentSpec.targetLivRef.healthBar.HideHitChance();
                            break;
                        }
                    }
                }
            }
        }
    }


}
