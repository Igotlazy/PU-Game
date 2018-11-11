using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MHA.GenericBehaviours;

namespace MHA.Kits.TakahiroAbilities
{

}
public class BETakahiroBasic : BattleEvent
{

    private GameObject attackProjectile;
    public List<Node> targetNodes = new List<Node>();
    public GameObject sourceObject;
    public Attack attack;

    public BETakahiroBasic(Attack _attack, GameObject _attackProjectile, GameObject _sourceObject, List<Node> _targetNodes) : base(null)
    {
        this.attack = _attack;
        this.attackProjectile = _attackProjectile;
        this.sourceObject = _sourceObject;

        foreach (Node currentNode in _targetNodes)
        {
            this.targetNodes.Add(currentNode);
        }

    }





    protected override void BattleEventRunImpl()
    {
        bEventMonoBehaviour.StartCoroutine(TakahiroBasic());
    }
    protected override void BattleEventPauseImpl()
    {

    }
    protected override void BattleEventResumeImpl()
    {

    }
    protected override void BattleEventCancelImpl()
    {
        bEventMonoBehaviour.StopCoroutine(TakahiroBasic());
    }
    protected override void BattleEventFinishImpl()
    {

    }


    IEnumerator TakahiroBasic()
    {
        List<GameObject> objectsToDamage = CombatUtils.GetAllAttackablesInNodes(targetNodes, sourceObject);
        foreach (GameObject currentObject in objectsToDamage)
        {
            if(currentObject != null)
            {
                Vector3 sourcePos = CombatUtils.GiveShotConnector(sourceObject);
                Vector3 targetPos = CombatUtils.GiveShotConnector(currentObject);

                GameObject projectile = GameObject.Instantiate(attackProjectile, sourcePos, Quaternion.LookRotation(targetPos - sourcePos));

                GBMoveObject moveObjectBehav = new GBMoveObject(this);
                yield return bEventMonoBehaviour.StartCoroutine(moveObjectBehav.MoveObject(projectile, targetPos));
                GameObject.Destroy(projectile);

                GBDealDamage dealDamageBehav = new GBDealDamage(this);
                yield return bEventMonoBehaviour.StartCoroutine(dealDamageBehav.DealDamage(attack, currentObject));
            }
        }

        BattleEventFinish();
    }
}