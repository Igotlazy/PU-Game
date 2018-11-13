using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MHA.GenericBehaviours
{
    [System.Serializable]
    public class GBDealDamage : GBBase
    {
        //Identifiers
        public float damageToDeal;
        public float remainingHealth;

        public Attack givenAttack;
        public GameObject targetObject;

        public GBDealDamage(Attack _givenAttack, GameObject _targetObject, BattleEvent _attachedBattleEvent) : base(_attachedBattleEvent)
        {
            this.givenAttack = _givenAttack;
            this.targetObject = _targetObject; 
        }
            
        protected override IEnumerator RunBehaviourImpl()
        {
            damageToDeal = givenAttack.damageValue;
            yield return attachedBattleEvent.bEventMonoBehaviour.StartCoroutine(attachedBattleEvent.ALLOWINTERRUPT(0f)); 

            if(targetObject != null)
            {
                targetObject.GetComponent<LivingCreature>().CreatureHit(givenAttack);

                EventFlags.EVENTTookDamage();
                yield return attachedBattleEvent.bEventMonoBehaviour.StartCoroutine(attachedBattleEvent.ALLOWINTERRUPT(0.2f));
            }

            FinishBehaviour();
        }     
    }
}
