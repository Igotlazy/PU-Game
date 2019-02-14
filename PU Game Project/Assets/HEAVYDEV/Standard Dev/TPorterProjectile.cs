using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using MHA.BattleBehaviours;
using MHA.Events;

public class TPorterProjectile : TPorter
{
    List<Projectile> projectiles = new List<Projectile>();
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
    protected override void CameraMove()
    {
        if(subRIndex == 0)
        {
            base.CameraMove();
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
                    originalTargetPositions.Add(currSpecs.targetObj.transform.position);
                    Projectile proj = new Projectile() { path = CombatUtils.ProjectilePathSplicer(currSpecs.fireOrigin, CombatUtils.GiveShotConnector(currSpecs.targetObj)) };
                    proj.currentPos = proj.path[0];
                    projectiles.Add(proj);

                }
            }
            else
            {
                if(givenTSpecs.Count > 0)
                {
                    originalTargetPositions.Add(givenTSpecs[runIndex].targetObj.transform.position);
                    Projectile proj = new Projectile() { path = CombatUtils.ProjectilePathSplicer(givenTSpecs[runIndex].fireOrigin, CombatUtils.GiveShotConnector(givenTSpecs[runIndex].targetObj)) };
                    proj.currentPos = proj.path[0];
                    projectiles.Add(proj);

                }
            }
        }

        Debug.LogWarning("Projectile WARN Event");
    }

    protected override void TPorterRun()
    {
        if (subRIndex == 0 && fireObjectRef != null) //If it's the first, instantiate the projectile.
        {
            Quaternion lookRotation = Quaternion.LookRotation(projectiles[runIndex].path[1] - projectiles[runIndex].path[0]);
            currentMovingObj = GameObject.Instantiate(fireObjectRef, projectiles[runIndex].path[subRIndex], lookRotation);
        }

        ProjectileLogic();
    }

    private void ProjectileLogic()
    {
        if(subRIndex == 0)
        {
            EventFlags.ANIMFinishCastCALL(this, new EventFlags.ECastAnim());
        }

        Projectile currentProj = projectiles[runIndex];

        Vector3 rayDir = currentProj.path[subRIndex + 1] - currentMovingObj.transform.position;
        RaycastHit hitInfo;
        if (!givenPacket.isPure && Physics.Raycast(currentMovingObj.transform.position, rayDir, out hitInfo, rayDir.magnitude, CombatUtils.shotMask))
        {
            new AnimMoveToPos(this, hitInfo.point, currentMovingObj, (baseMoveSpeed + (moveSpeedAccel * subRIndex)), true);

            EventFlags.ANIMFinishProjCALL(this, new EventFlags.ECastAnim());
            Debug.LogWarning("Projectile BLOCKED Event");

            subRIndex = 0;
            doEnding = true;

            return;
        }
        else
        {
            if (subRIndex >= currentProj.path.Count - 2)
            {
                new AnimMoveToPos(this, currentProj.path[subRIndex + 1], currentMovingObj, (baseMoveSpeed + (moveSpeedAccel * subRIndex)), true);
                Debug.Log("Projectile MOVE[END] Anim Event");
            }
            else
            {
                new AnimMoveToPos(this, currentProj.path[subRIndex + 1], currentMovingObj, (baseMoveSpeed + (moveSpeedAccel * subRIndex)), false);
                Debug.Log("Projectile MOVE Anim Event");
            }
        }

        //EventFlags.EVENTProjectileMove(this, new EventFlags.EProjectileMoveArgs(targetPaths[runIndex][subRIndex], targetPaths[runIndex][subRIndex + 1]));
        subRIndex++;

        if (subRIndex >= currentProj.path.Count - 1)
        {
            if (givenTSpecs[runIndex].targetObj.transform.position == originalTargetPositions[runIndex]) //May check this condition (Checks if target is still in same position. 
            {

                Unit unitScript = givenTSpecs[runIndex].targetObj.GetComponent<Unit>();
                if (unitScript == null)
                {
                    effectData.AppendValue(REPORTKEY, givenTSpecs[runIndex].targetObj);
                    TPorterFinishActive = true;

                    EventFlags.ANIMFinishProjCALL(this, new EventFlags.ECastAnim());
                }
                else
                {
                    if (!givenPacket.isPure)
                    {
                        Vector3 fireDir = (currentProj.path[currentProj.path.Count - 1]) - (currentProj.path[currentProj.path.Count - 2]);

                        if (CombatUtils.PercentageCalculation(CombatUtils.coverCheckCalculation(fireDir, unitScript.partialCoverCheck.transform.position, unitScript.shotConnecter.transform.position)))
                        {
                            effectData.AppendValue(REPORTKEY, givenTSpecs[runIndex].targetObj);
                            TPorterFinishActive = true;

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
                        TPorterFinishActive = true;

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

        if (runIndex >= projectiles.Count)
        {
            RemoveSelfFromResolveList();
        }
    }

    protected override bool EffectSpecificCondition()
    {
        return true;
    }

    public class Projectile
    {
        public List<Vector3> path = new List<Vector3>();
        public Vector3 currentPos;
    }
}
