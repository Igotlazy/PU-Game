using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using MHA.BattleBehaviours;

public class TPorterProjectile : BattleEffect
{
    List<TargetSpecs> givenTSpecs;
    List<List<Vector3>> targetPaths = new List<List<Vector3>>();
    List<Vector3> originalTargetPositions = new List<Vector3>();
    GameObject fireObjectRef;

    GameObject currentMovingObj;
    int indexTracker;
    int listTracker;

    public float baseMoveSpeed = 5;
    public float moveSpeedAccel = 1f;

    public string REPORTKEY;

    public TPorterProjectile(EffectDataPacket _effectData, List<TargetSpecs> _givenTSpecs, GameObject _fireObjectRef) : base(_effectData)
    {
        this.givenTSpecs = _givenTSpecs;
        this.fireObjectRef = _fireObjectRef;

        foreach(TargetSpecs currentSpec in givenTSpecs)
        {
            targetPaths.Add(CombatUtils.ProjectilePathSplicer(currentSpec.fireOriginPoint, CombatUtils.GiveShotConnector(currentSpec.targetObj)));
            originalTargetPositions.Add(currentSpec.targetObj.transform.position);
        }
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
            Quaternion lookRotation = Quaternion.LookRotation(targetPaths[listTracker][1] - targetPaths[listTracker][0]);
            currentMovingObj = GameObject.Instantiate(fireObjectRef, targetPaths[listTracker][indexTracker], lookRotation);
        }

        Vector3 rayDir = targetPaths[listTracker][indexTracker + 1] - currentMovingObj.transform.position;
        RaycastHit hitInfo;
        if (Physics.Raycast(currentMovingObj.transform.position, rayDir, out hitInfo, rayDir.magnitude, CombatUtils.shotMask)) //Checks for Hitting Obstacles
        {
            new AnimMoveToPos(hitInfo.point, currentMovingObj, (baseMoveSpeed + (moveSpeedAccel * indexTracker)), true);
            indexTracker = 0;
            listTracker++;

            if (listTracker >= targetPaths.Count)
            {
                TPorterRemoveOverride = true;
            }

            Debug.LogWarning("BLOCKED");
            return;

        }
        else
        {
            if(indexTracker >= targetPaths[listTracker].Count - 2)
            {
                new AnimMoveToPos(targetPaths[listTracker][indexTracker + 1], currentMovingObj, (baseMoveSpeed + (moveSpeedAccel * indexTracker)), true);
            }
            else
            {
                new AnimMoveToPos(targetPaths[listTracker][indexTracker + 1], currentMovingObj, (baseMoveSpeed + (moveSpeedAccel * indexTracker)), false);
            }
        }

        indexTracker++;

        if(indexTracker >= targetPaths[listTracker].Count - 1)
        {
            if (givenTSpecs[listTracker].targetObj.transform.position == originalTargetPositions[listTracker]) //May check this condition (Checks if target is still in same position. 
            {

                Unit unitScript = givenTSpecs[listTracker].targetObj.GetComponent<Unit>();
                if(unitScript == null)
                {
                    effectData.AppendValue(REPORTKEY, givenTSpecs[listTracker].targetObj);
                    TPorterFinishOverride = true;
                }
                else
                {
                    Vector3 fireDir = (targetPaths[listTracker][targetPaths[listTracker].Count - 1]) - (targetPaths[listTracker][targetPaths[listTracker].Count - 2]);

                    if(CombatUtils.AttackHitPercentages(CombatUtils.coverCheckCalculation(fireDir, unitScript)))
                    {
                        effectData.AppendValue(REPORTKEY, givenTSpecs[listTracker].targetObj);
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
    }


    protected override void CancelEffectImpl()
    {

    }

    public void SoftEffectCancel()
    {
        indexTracker = 0;
        listTracker++;

        if (listTracker >= targetPaths.Count)
        {
            RemoveSelfFromResolveList();
        }
    }

    protected override bool EffectSpecificCondition()
    {
        return true;
    }
}
