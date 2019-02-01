using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using MHA.BattleBehaviours;
using MHA.Events;

public class TPorterProjectile : TPorter
{
    List<List<Vector3>> targetPaths = new List<List<Vector3>>();
    List<Vector3> originalTargetPositions = new List<Vector3>();
    GameObject fireObjectRef;

    GameObject currentMovingObj;
    int subRIndex;

    public float baseMoveSpeed = 5;
    public float moveSpeedAccel = 1f;

    public string REPORTKEY;

    public TPorterProjectile(EffectDataPacket _effectData, SelectorPacket _givenPacket, GameObject _fireObjectRef) : base(_effectData, _givenPacket)
    {
        this.fireObjectRef = _fireObjectRef;
    }

    protected override void PeekCheck()
    {
        if(subRIndex == 0)
        {
            base.PeekCheck();
        }
    }

    protected override void TPorterWarn()
    {
        if(subRIndex == 0)
        {
            EventFlags.ANIMStartCastCALL(this, new EventFlags.ECastAnim());

            if (warnOnce)
            {
                foreach(TargetSpecs currSpecs in givenTSpecs)
                {
                    targetPaths.Add(CombatUtils.ProjectilePathSplicer(currSpecs.fireOriginPoint, CombatUtils.GiveShotConnector(currSpecs.targetObj)));
                    originalTargetPositions.Add(currSpecs.targetObj.transform.position);
                }
            }
            else
            {
                if(givenTSpecs.Count > 0)
                {
                    targetPaths.Add(CombatUtils.ProjectilePathSplicer(givenTSpecs[runIndex].fireOriginPoint, CombatUtils.GiveShotConnector(givenTSpecs[runIndex].targetObj)));
                    originalTargetPositions.Add(givenTSpecs[runIndex].targetObj.transform.position);
                }
            }
        }

        Debug.LogWarning("Projectile WARN Event");
    }

    protected override void TPorterRun()
    {
        if (subRIndex == 0 && fireObjectRef != null) //If it's the first, instantiate the projectile.
        {
            Quaternion lookRotation = Quaternion.LookRotation(targetPaths[runIndex][1] - targetPaths[runIndex][0]);
            currentMovingObj = GameObject.Instantiate(fireObjectRef, targetPaths[runIndex][subRIndex], lookRotation);
        }

        ProjectileLogic();
    }

    private void ProjectileLogic()
    {
        if(subRIndex == 0)
        {
            EventFlags.ANIMFinishCastCALL(this, new EventFlags.ECastAnim());
        }

        Vector3 rayDir = targetPaths[runIndex][subRIndex + 1] - currentMovingObj.transform.position;
        RaycastHit hitInfo;
        if (!givenPacket.isPure && Physics.Raycast(currentMovingObj.transform.position, rayDir, out hitInfo, rayDir.magnitude, CombatUtils.shotMask))
        {
            new AnimMoveToPos(hitInfo.point, currentMovingObj, (baseMoveSpeed + (moveSpeedAccel * subRIndex)), true);

            EventFlags.ANIMFinishProjCALL(this, new EventFlags.ECastAnim());
            Debug.LogWarning("Projectile BLOCKED Event");

            subRIndex = 0;
            doEnding = true;

            return;
        }
        else
        {
            if (subRIndex >= targetPaths[runIndex].Count - 2)
            {
                new AnimMoveToPos(targetPaths[runIndex][subRIndex + 1], currentMovingObj, (baseMoveSpeed + (moveSpeedAccel * subRIndex)), true);
                Debug.Log("Projectile MOVE[END] Anim Event");
            }
            else
            {
                new AnimMoveToPos(targetPaths[runIndex][subRIndex + 1], currentMovingObj, (baseMoveSpeed + (moveSpeedAccel * subRIndex)), false);
                Debug.Log("Projectile MOVE Anim Event");
            }
        }

        subRIndex++;

        if (subRIndex >= targetPaths[runIndex].Count - 1)
        {
            if (givenTSpecs[runIndex].targetObj.transform.position == originalTargetPositions[runIndex]) //May check this condition (Checks if target is still in same position. 
            {

                Unit unitScript = givenTSpecs[runIndex].targetObj.GetComponent<Unit>();
                if (unitScript == null)
                {
                    effectData.AppendValue(REPORTKEY, givenTSpecs[runIndex].targetObj);
                    TPorterFinishOverride = true;

                    EventFlags.ANIMFinishProjCALL(this, new EventFlags.ECastAnim());
                }
                else
                {
                    if (!givenPacket.isPure)
                    {
                        Vector3 fireDir = (targetPaths[runIndex][targetPaths[runIndex].Count - 1]) - (targetPaths[runIndex][targetPaths[runIndex].Count - 2]);

                        if (CombatUtils.AttackHitPercentages(CombatUtils.coverCheckCalculation(fireDir, unitScript.partialCoverCheck.transform.position, unitScript.shotConnecter.transform.position)))
                        {
                            effectData.AppendValue(REPORTKEY, givenTSpecs[runIndex].targetObj);
                            TPorterFinishOverride = true;

                            EventFlags.ANIMFinishProjCALL(this, new EventFlags.ECastAnim());
                        }
                        else
                        {
                            Debug.LogWarning("Projectile MISSED Event");
                        }
                    }
                    else
                    {
                        effectData.AppendValue(REPORTKEY, givenTSpecs[runIndex].targetObj);
                        TPorterFinishOverride = true;

                        EventFlags.ANIMFinishProjCALL(this, new EventFlags.ECastAnim());
                    }
                }

            }
            else
            {
                Debug.LogWarning("PROJECTILE MISSED");
            }

            subRIndex = 0;
            doEnding = true;
        }
    }


    protected override void CancelEffectImpl()
    {

    }

    public void SoftEffectCancel()
    {
        PeekRecovery();
        subRIndex = 0;
        runIndex++;

        if (runIndex >= targetPaths.Count)
        {
            RemoveSelfFromResolveList();
        }
    }

    protected override bool EffectSpecificCondition()
    {
        return true;
    }
}
