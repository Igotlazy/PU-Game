using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MHA.DebugGame
{
    public class TestInterrupt : MonoBehaviour
    {
        public GameObject projectile;
        public GameObject parent;

        private void OnTriggerEnter(Collider enteredCollider)
        {
            if((TurnManager.instance.CurrentBattlePhase == TurnManager.BattlePhase.ActionPhase) && enteredCollider.gameObject.tag == "Champion" && enteredCollider.gameObject != parent)
            {
                Attack attack = new Attack(5f, Attack.DamageType.Physical, this.gameObject);
                BattleAbility battleAttack = new BattleAbility(attack, projectile, this.gameObject, new List<Node> { enteredCollider.gameObject.GetComponent<Unit>().currentNode });
                TurnManager.instance.EventResolutionReceiver(battleAttack);
            }
        }

    }
}
