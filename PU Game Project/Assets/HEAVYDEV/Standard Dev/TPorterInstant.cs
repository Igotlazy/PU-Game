using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MHA.Events;

public class TPorterInstant : TPorter
{
    GameObject fireObjectRef;

    public string REPORTKEY;


    public TPorterInstant(EffectDataPacket _effectData, SelectorPacket _givenPacket, GameObject _fireObjectRef) : base(_effectData, _givenPacket)
    {
        this.fireObjectRef = _fireObjectRef;
    }

    protected override void TPorterWarn()
    {
        PeekCheck();

        Debug.Log("Instant START Anim Event");
        Debug.LogWarning("Instant WARN Event");
    }

    protected override void TPorterRun()
    {

        if (runIndex == 0 && fireObjectRef != null) //If it's the first, instantiate the projectile.
        {
            Quaternion lookRotation = Quaternion.LookRotation(givenTSpecs[runIndex].targetObj.GetComponent<Unit>().shotConnecter.transform.position - givenTSpecs[runIndex].fireOriginPoint);
            GameObject.Instantiate(fireObjectRef, ((Unit)effectData.GetValue("Caster", false)[0]).gameObject.transform.position, Quaternion.identity);
        }

        InstantLogic();
    }

    private void InstantLogic()
    {

        Unit targetScript = givenTSpecs[runIndex].targetObj.GetComponent<Unit>();
        if (givenPacket.TargetNodes.Contains(targetScript.currentNode))
        {
            if (!givenPacket.isPure)
            {
                float result = CombatUtils.MainFireCalculation(givenTSpecs[runIndex].fireOriginPoint, targetScript.shotConnecter.transform.position, targetScript.partialCoverCheck.transform.position);
                if (CombatUtils.AttackHitPercentages(result))
                {
                    TPorterFinishOverride = true;
                    effectData.AppendValue(REPORTKEY, givenTSpecs[runIndex].targetObj);

                    Debug.Log("Instant HIT Anim Event");
                    Debug.LogWarning("Instant HIT Event");
                }
                else
                {
                    Debug.Log("Instant MISSED Anim Event");
                    Debug.LogWarning("Instant MISSED Event");
                }
            }
            else
            {
                TPorterFinishOverride = true;
                effectData.AppendValue(REPORTKEY, givenTSpecs[runIndex].targetObj);

                Debug.Log("Instant HIT Anim Event");
                Debug.LogWarning("Instant HIT Event");
            }
        }

        //if AoE foreach

        doEnding = true;
    }


    protected override void CancelEffectImpl()
    {

    }

    public void SoftEffectCancel()
    {
        PeekRecovery();
        runIndex++;

        if (runIndex >= givenTSpecs.Count)
        {
            RemoveSelfFromResolveList();
        }        
    }

    protected override bool EffectSpecificCondition()
    {
        return true;
    }
}
