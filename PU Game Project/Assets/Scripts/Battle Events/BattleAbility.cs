//Event that handles Abilities.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MHA.GenericBehaviours;
using MHA.DebugGame;

[System.Serializable]
public class BattleAbility : BattleEvent {

    private GameObject attackProjectile;
    public List<Node> targetNodes = new List<Node>();
    public GameObject sourceObject;
    public Attack attack;

    public BattleAbility(Attack _attack, GameObject _attackProjectile, GameObject _sourceObject, List<Node> _targetNodes) : base(null)
    {
        this.attack = _attack;
        this.attackProjectile = _attackProjectile;
        this.sourceObject = _sourceObject;

        foreach(Node currentNode in _targetNodes)
        {
            this.targetNodes.Add(currentNode);
        }
       
    }





    protected override void BattleEventRunImpl()
    {
        bEventMonoBehaviour.StartCoroutine(FireAttack());
    }
    protected override void BattleEventPauseImpl()
    {

    }
    protected override void BattleEventResumeImpl()
    {

    }
    protected override void BattleEventCancelImpl()
    {
        bEventMonoBehaviour.StopCoroutine(FireAttack());
    }
    protected override void BattleEventFinishImpl()
    {

    }


    IEnumerator FireAttack()
    {
        Debug.Log("Fire Attack: " + sourceObject.name);
        List<GameObject> objectsToDamage = CombatUtils.GetAllAttackablesInNodes(targetNodes, sourceObject);
        Debug.Log("Size of Node Array: " + targetNodes.Count);
        Debug.Log("Size of Target Array: " + objectsToDamage.Count);
        foreach (GameObject currentObject in objectsToDamage)
        {
            if (currentObject != null)
            {
                Vector3 sourcePos = CombatUtils.GiveShotConnector(sourceObject);
                Vector3 targetPos = CombatUtils.GiveShotConnector(currentObject);

                GameObject projectile = GameObject.Instantiate(attackProjectile, sourcePos, Quaternion.LookRotation(targetPos - sourcePos));

                GBMoveObject moveObjectBehav = new GBMoveObject(projectile, targetPos, this);
                yield return bEventMonoBehaviour.StartCoroutine(moveObjectBehav.RunBehaviour());
                GameObject.Destroy(projectile);

                GBDealDamage dealDamageBehav = new GBDealDamage(attack, currentObject, this);
                yield return bEventMonoBehaviour.StartCoroutine(dealDamageBehav.RunBehaviour());
            }
        }

        BattleEventFinish();
    } 
}
