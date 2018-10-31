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
                Debug.Log(this.gameObject.name + ": Spawned Attack and " + enteredCollider.gameObject.name + " triggered me.");
                Attack attack = new Attack(5f, Attack.DamageType.Physical, this.gameObject);
                BattleAttack battleAttack = new BattleAttack(attack, projectile, this.gameObject, enteredCollider.gameObject);
                TurnManager.instance.EventResolutionReceiver(battleAttack);
            }
        }

    }
}
