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
                Debug.Log("Entered Collider: " + enteredCollider.gameObject.name);
                Attack attack = new Attack(5f, Attack.DamageType.Physical);
                //BattleAbility battleAttack = new BattleAbility(attack, projectile, parent, new List<Node> { enteredCollider.gameObject.GetComponent<Unit>().currentNode });
                Debug.Log(enteredCollider.gameObject.GetComponent<Unit>().currentNode.tilePrefab.name);
                //TurnManager.instance.EventResolutionReceiver(battleAttack);
            }
        }

    }
}
