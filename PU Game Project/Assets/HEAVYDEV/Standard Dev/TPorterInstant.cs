using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TPorterInstant : BattleEffect
{
    TargetPacket givenPacket;
    List<TargetSpecs> givenSpecs;
    GameObject fireObjectRef;

    GameObject currentMovingObj;
    int indexTracker;
    bool isTrue;

    public string REPORTKEY;

    public TPorterInstant(EffectDataPacket _effectData, TargetPacket _givenPacket, GameObject _fireObjectRef, bool _isTrue) : base(_effectData)
    {
        this.givenPacket = _givenPacket;
        this.fireObjectRef = _fireObjectRef;
        this.isTrue = _isTrue;

        this.givenSpecs = givenPacket.targetObjectSpecs;
    }

    protected override void WarnEffect()
    {

    }

    protected override void RunEffectImpl()
    {
        TPorterFinishOverride = false;
        TPorterRemoveOverride = false;

        if (indexTracker == 0) //If it's the first, instantiate the projectile.
        {
            //Quaternion lookRotation = Quaternion.LookRotation(targetPaths[listTracker][1] - targetPaths[listTracker][0]);
            currentMovingObj = GameObject.Instantiate(fireObjectRef, ((LivingCreature)effectData.GetValue("Caster", false)[0]).gameObject.transform.position, Quaternion.identity);
        }

        /*
        Node currentSpecNode = givenSpecs[indexTracker].targetObj.GetComponent<Unit>().currentNode;
        if (givenPacket.TargetNodes.Contains(currentSpecNode))
        {
            CombatUtils.MainFireCalculation()
        }


        indexTracker++;

        if (indexTracker >= targetPaths[listTracker].Count - 1)
        {
            if (givenPacket[listTracker].targetObj.transform.position == originalTargetPositions[listTracker]) //May check this condition (Checks if target is still in same position. 
            {

                Unit unitScript = givenPacket[listTracker].targetObj.GetComponent<Unit>();
                if (unitScript == null)
                {
                    effectData.AppendValue(REPORTKEY, givenPacket[listTracker].targetObj);
                    TPorterFinishOverride = true;
                }
                else
                {
                    Vector3 fireDir = (targetPaths[listTracker][targetPaths[listTracker].Count - 1]) - (targetPaths[listTracker][targetPaths[listTracker].Count - 2]);

                    if (CombatUtils.AttackHitPercentages(CombatUtils.coverCheckCalculation(fireDir, unitScript)))
                    {
                        effectData.AppendValue(REPORTKEY, givenPacket[listTracker].targetObj);
                        TPorterFinishOverride = true;
                    }
                    else
                    {
                        Debug.LogWarning("MISSED");
                    }
                }

            }
            else
            {
                Debug.LogWarning("MISSED");
            }

            indexTracker = 0;
            listTracker++;
        }

        if (listTracker >= targetPaths.Count)
        {
            TPorterRemoveOverride = true;
        }
        */
    }


    protected override void CancelEffectImpl()
    {

    }

    public void SoftEffectCancel()
    {
        /*
        indexTracker = 0;
        listTracker++;

        if (listTracker >= targetPaths.Count)
        {
            RemoveSelfFromResolveList();
        }
        */
    }

    protected override bool EffectSpecificCondition()
    {
        return true;
    }
}
