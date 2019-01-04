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
            if(!collectedNodes.Contains(enteringNode) && givenAbility.associatedCreature.gameObject.GetComponent<Unit>().currentNode != enteringNode)
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

                    if (!alreadyHas && selectType == SelectorPacket.SelectionType.Target)
                    {
                        Vector3 sourceShot = CombatUtils.GiveShotConnector(givenAbility.associatedCreature.gameObject);
                        Vector3 targetShot = CombatUtils.GiveShotConnector(hitObject);
                        Vector3 targetPartial = CombatUtils.GivePartialCheck(hitObject);
                        bool peekResult = false;
                        float hitChance = 0;
                        if (attachedTargetPacket.isPure)
                        {
                            hitChance = 100f;
                        }
                        else
                        {
                            Vector3 newFireSource;
                            hitChance = CombatUtils.MainFireCalculation(sourceShot, targetShot, targetPartial, out peekResult, out newFireSource);
                        }

                        TargetSpecs newSpec = new TargetSpecs(hitObject, new Vector2(10f, 5f), hitChance, "", sourceShot, SelectorPacket.SelectionType.Target);
                        newSpec.didPeek = peekResult;

                        newSpec.targetLivRef.healthBar.FadeHitChance();
                        allSpecs.Add(newSpec);
                    }
                    if (!alreadyHas && selectType == SelectorPacket.SelectionType.AreaTarget)
                    {
                        Vector3 sourceShot = CombatUtils.GiveShotConnector(givenAbility.associatedCreature.gameObject);
                        Vector3 targetShot = CombatUtils.GiveShotConnector(hitObject);
                        Vector3 targetPartial = CombatUtils.GivePartialCheck(hitObject);
                        bool peekResult = false;
                        float hitChance = 0;
                        if (attachedTargetPacket.isPure)
                        {
                            hitChance = 100f;
                        }
                        else
                        {
                            Vector3 newFireSource;
                            hitChance = CombatUtils.MainFireCalculation(sourceShot, targetShot, targetPartial, out peekResult, out newFireSource);
                        }

                        TargetSpecs newSpec = new TargetSpecs(hitObject, new Vector2(10f, 5f), hitChance, "", sourceShot, SelectorPacket.SelectionType.AreaTarget);
                        newSpec.didPeek = peekResult;

                        newSpec.targetLivRef.healthBar.DisplayHitChance();
                        Unit unitScript = hitObject.GetComponent<Unit>();
                        newSpec.indicator = Instantiate(selectionIndicator, unitScript.centerPoint.transform.position, Quaternion.identity);
                        allSpecs.Add(newSpec);
                        selectedSpecs.Add(newSpec);
                    }
                    if (!alreadyHas && selectType == SelectorPacket.SelectionType.AoE)
                    {
                        float hitChance = 0;
                        if (attachedTargetPacket.isPure)
                        {
                            hitChance = 100f;
                        }
                        else
                        {
                            hitChance = CombatUtils.MainFireCalculation(givenAbility.associatedCreature.gameObject, hitObject); 
                        }
                        TargetSpecs newSpec = new TargetSpecs(hitObject, new Vector2(10f, 5f), hitChance, "", CombatUtils.GiveShotConnector(givenAbility.associatedCreature.gameObject), SelectorPacket.SelectionType.AoE);
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
                            }
                            if (selectedSpecs.Contains(currentSpec))
                            {
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
