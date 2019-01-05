using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MHA.Events;

public class TPorterInstant : BattleEffect
{
    SelectorPacket givenPacket;
    List<TargetSpecs> givenTSpecs;
    GameObject fireObjectRef;

    int indexTracker;
    bool preIndexIncrease = false;
    bool warnOnce;

    public string REPORTKEY;


    public TPorterInstant(EffectDataPacket _effectData, SelectorPacket _givenPacket, GameObject _fireObjectRef) : base(_effectData)
    {
        this.givenPacket = _givenPacket;
        this.fireObjectRef = _fireObjectRef;
        if(givenPacket.selectionType == SelectorPacket.SelectionType.AoE)
        {
            this.warnOnce = true;
        }

        this.givenTSpecs = givenPacket.targetObjectSpecs;
    }

    protected override void WarnEffect()
    {
        if (!preIndexIncrease)
        {
            if (!givenPacket.isPure && (givenTSpecs[indexTracker].selectionType == SelectorPacket.SelectionType.Target || givenTSpecs[indexTracker].selectionType == SelectorPacket.SelectionType.AreaTarget))
            {
                TargetSpecs currentSpec = givenTSpecs[indexTracker];
                Vector3 targetShot = CombatUtils.GiveShotConnector(currentSpec.targetObj);
                Vector3 targetPartial = CombatUtils.GivePartialCheck(currentSpec.targetObj);
                CombatUtils.MainFireCalculation(currentSpec.fireOriginPoint, targetShot, targetPartial, out currentSpec.didPeek, out currentSpec.fireOriginPoint);

                if (currentSpec.didPeek)
                {
                    Unit sourceObj = ((Unit)effectData.GetValue("Caster", false)[0]);
                    EventFlags.ANIMStartPeek(this, new EventFlags.EPeekStart(sourceObj, currentSpec.fireOriginPoint, sourceObj.gameObject.transform.position)); //EVENT
                }
            }
        }
        if (warnOnce)
        {
            TPorterWarnOverride = false;
        }
    }

    protected override void RunEffectImpl()
    {
        TPorterFinishOverride = false;
        TPorterRemoveOverride = false;

        if (!preIndexIncrease)
        {
            if (indexTracker == 0 && fireObjectRef != null) //If it's the first, instantiate the projectile.
            {
                Quaternion lookRotation = Quaternion.LookRotation(givenTSpecs[indexTracker].targetObj.GetComponent<Unit>().shotConnecter.transform.position - givenTSpecs[indexTracker].fireOriginPoint);
                GameObject.Instantiate(fireObjectRef, ((Unit)effectData.GetValue("Caster", false)[0]).gameObject.transform.position, Quaternion.identity);
            }

            Unit targetScript = givenTSpecs[indexTracker].targetObj.GetComponent<Unit>();
            if (givenPacket.TargetNodes.Contains(targetScript.currentNode))
            {
                if (!givenPacket.isPure)
                {
                    float result = CombatUtils.MainFireCalculation(givenTSpecs[indexTracker].fireOriginPoint, targetScript.shotConnecter.transform.position, targetScript.partialCoverCheck.transform.position);
                    if (CombatUtils.AttackHitPercentages(result))
                    {
                        TPorterFinishOverride = true;
                        effectData.AppendValue(REPORTKEY, givenTSpecs[indexTracker].targetObj);
                    }
                    else
                    {
                        Debug.LogWarning("INSTANT BLOCKED");
                    }
                }
                else
                {
                    TPorterFinishOverride = true;
                    effectData.AppendValue(REPORTKEY, givenTSpecs[indexTracker].targetObj);
                }
            }

            //foreach

            preIndexIncrease = true;
        }
        else
        {
            if (givenTSpecs[indexTracker].didPeek && (givenTSpecs[indexTracker].selectionType == SelectorPacket.SelectionType.Target || givenTSpecs[indexTracker].selectionType == SelectorPacket.SelectionType.AreaTarget)) 
            {
                if(warnOnce)
                {
                    if(indexTracker >= givenTSpecs.Count)
                    {
                        EventFlags.ANIMEndPeek(this, new EventFlags.EPeekEnd(((Unit)effectData.GetValue("Caster", false)[0])));
                    }
                }
                else 
                {
                    EventFlags.ANIMEndPeek(this, new EventFlags.EPeekEnd(((Unit)effectData.GetValue("Caster", false)[0])));
                }

            }

            indexTracker++;
            preIndexIncrease = false;
        }



        if(indexTracker >= givenTSpecs.Count)
        {
            TPorterRemoveOverride = true;
        }
    }


    protected override void CancelEffectImpl()
    {

    }

    public void SoftEffectCancel()
    {
        indexTracker++;

        if (indexTracker >= givenTSpecs.Count)
        {
            RemoveSelfFromResolveList();
        }        
    }

    protected override bool EffectSpecificCondition()
    {
        return true;
    }
}
