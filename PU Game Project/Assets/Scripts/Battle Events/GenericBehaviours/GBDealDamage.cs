using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MHA.GenericBehaviours
{
    public class GBDealDamage :GBBase
    {
        //Identifiers
        public float damageToDeal;
        public float remainingHealth;

        public GBDealDamage(BattleEvent _attachedBattleEvent) : base(_attachedBattleEvent)
        {

        }
            
        public IEnumerator DealDamage(Attack givenAttack, GameObject targetObject)
        {
            damageToDeal = givenAttack.damageValue;
            yield return attachedBattleEvent.bEventMonoBehaviour.StartCoroutine(attachedBattleEvent.ALLOWINTERRUPT(0f)); 

            if(targetObject != null)
            {
                targetObject.GetComponent<LivingCreature>().CreatureHit(givenAttack);

                EventFlags.EVENTTookDamage();
                yield return attachedBattleEvent.bEventMonoBehaviour.StartCoroutine(attachedBattleEvent.ALLOWINTERRUPT(0.2f));
            }

            this.behaviourDone = true;
        }
      
    }
}
