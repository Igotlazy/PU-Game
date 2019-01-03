using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using MHA.BattleBehaviours;
using MHA.Events;

public class TPorterProjectile : BattleEffect
{
    TargetPacket givenPacket;
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

    bool preListIncrease = false;

    public TPorterProjectile(EffectDataPacket _effectData, TargetPacket _givenPacket, GameObject _fireObjectRef) : base(_effectData)
    {
        this.givenPacket = _givenPacket;
        this.givenTSpecs = givenPacket.targetObjectSpecs;
        this.fireObjectRef = _fireObjectRef;

        if(givenPacket.selectionType == TargetPacket.SelectionType.AoE)
        {
            Debug.LogWarning("WARNING: AoE Selection Type for Projectile is AreaTarget without Peeking. It does not stop the warning.");
        }

        /*
        foreach(TargetSpecs currentSpec in givenTSpecs)
        {
            targetPaths.Add(CombatUtils.ProjectilePathSplicer(currentSpec.fireOriginPoint, CombatUtils.GiveShotConnector(currentSpec.targetObj)));
            originalTargetPositions.Add(currentSpec.targetObj.transform.position);
        }
        */
    }

    protected override void WarnEffect()
    {
        if(indexTracker == 0 && !preListIncrease)
        {
            if (!givenPacket.isPure && (givenTSpecs[listTracker].selectionType == TargetPacket.SelectionType.Target || givenTSpecs[listTracker].selectionType == TargetPacket.SelectionType.AreaTarget))
            {
                TargetSpecs currentSpec = givenTSpecs[listTracker];
                Vector3 targetShot = CombatUtils.GiveShotConnector(currentSpec.targetObj);
                Vector3 targetPartial = CombatUtils.GivePartialCheck(currentSpec.targetObj);
                CombatUtils.MainFireCalculation(currentSpec.fireOriginPoint, targetShot, targetPartial, out currentSpec.didPeek, out currentSpec.fireOriginPoint);

                if (currentSpec.didPeek)
                {
                    Unit sourceObj = ((LivingCreature)effectData.GetValue("Caster", false)[0]).gameObject.GetComponent<Unit>();
                    EventFlags.ANIMStartPeek(this, new EventFlags.EPeekStart(sourceObj, currentSpec.fireOriginPoint, sourceObj.gameObject.transform.position)); //EVENT
                }
            }

            //Anim Call to Prep Animation. 

            targetPaths.Add(CombatUtils.ProjectilePathSplicer(givenTSpecs[listTracker].fireOriginPoint, CombatUtils.GiveShotConnector(givenTSpecs[listTracker].targetObj)));
            originalTargetPositions.Add(givenTSpecs[listTracker].targetObj.transform.position);
        }
    }

    protected override void RunEffectImpl()
    {
        TPorterFinishOverride = false;
        TPorterRemoveOverride = false;

        if (!preListIncrease)
        {
            if (indexTracker == 0 && fireObjectRef != null) //If it's the first, instantiate the projectile.
            {
                Quaternion lookRotation = Quaternion.LookRotation(targetPaths[listTracker][1] - targetPaths[listTracker][0]);
                currentMovingObj = GameObject.Instantiate(fireObjectRef, targetPaths[listTracker][indexTracker], lookRotation);
            }

            Vector3 rayDir = targetPaths[listTracker][indexTracker + 1] - currentMovingObj.transform.position;
            RaycastHit hitInfo;
            if (!givenPacket.isPure && Physics.Raycast(currentMovingObj.transform.position, rayDir, out hitInfo, rayDir.magnitude, CombatUtils.shotMask))
            {
                new AnimMoveToPos(hitInfo.point, currentMovingObj, (baseMoveSpeed + (moveSpeedAccel * indexTracker)), true);

                //indexTracker = 0;
                preListIncrease = true;

                Debug.LogWarning("PROJECTILE BLOCKED");
                return;
            }
            else
            {
                if (indexTracker >= targetPaths[listTracker].Count - 2)
                {
                    new AnimMoveToPos(targetPaths[listTracker][indexTracker + 1], currentMovingObj, (baseMoveSpeed + (moveSpeedAccel * indexTracker)), true);
                }
                else
                {
                    new AnimMoveToPos(targetPaths[listTracker][indexTracker + 1], currentMovingObj, (baseMoveSpeed + (moveSpeedAccel * indexTracker)), false);
                }
            }

            indexTracker++;

            if (indexTracker >= targetPaths[listTracker].Count - 1)
            {
                if (givenTSpecs[listTracker].targetObj.transform.position == originalTargetPositions[listTracker]) //May check this condition (Checks if target is still in same position. 
                {

                    Unit unitScript = givenTSpecs[listTracker].targetObj.GetComponent<Unit>();
                    if (unitScript == null)
                    {
                        effectData.AppendValue(REPORTKEY, givenTSpecs[listTracker].targetObj);
                        TPorterFinishOverride = true;
                    }
                    else
                    {
                        if (!givenPacket.isPure)
                        {
                            Vector3 fireDir = (targetPaths[listTracker][targetPaths[listTracker].Count - 1]) - (targetPaths[listTracker][targetPaths[listTracker].Count - 2]);

                            if (CombatUtils.AttackHitPercentages(CombatUtils.coverCheckCalculation(fireDir, unitScript.partialCoverCheck.transform.position, unitScript.shotConnecter.transform.position)))
                            {
                                effectData.AppendValue(REPORTKEY, givenTSpecs[listTracker].targetObj);
                                TPorterFinishOverride = true;
                            }
                            else
                            {
                                Debug.LogWarning("PROJECTILE MISSED");
                            }
                        }
                        else
                        {
                            effectData.AppendValue(REPORTKEY, givenTSpecs[listTracker].targetObj);
                            TPorterFinishOverride = true;
                        }
                    }

                }
                else
                {
                    Debug.LogWarning("PROJECTILE MISSED");
                }

                //indexTracker = 0;
                preListIncrease = true;
            }
        }
        else
        {
            if (givenTSpecs[listTracker].didPeek && (givenTSpecs[listTracker].selectionType == TargetPacket.SelectionType.Target || givenTSpecs[listTracker].selectionType == TargetPacket.SelectionType.AreaTarget))
            {
                EventFlags.ANIMEndPeek(this, new EventFlags.EPeekEnd(((LivingCreature)effectData.GetValue("Caster", false)[0]).gameObject.GetComponent<Unit>()));
            }
            indexTracker = 0;
            listTracker++;
            preListIncrease = false;
        }

        if (listTracker > givenTSpecs.Count - 1)
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
