using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MHA.BattleBehaviours
{
    public class BBDealDamageController : BattleBehaviourController
    {

        public BBDealDamageModel dealDamageModel;

        public BBDealDamageController(BBDealDamageModel givenModel) : base(givenModel)
        {
            dealDamageModel = givenModel;
            RunBehaviourImplList.Add(DealDamage);
        }

        protected void DealDamage()
        {
            Debug.Log(dealDamageModel.targetObject);

            //damageTodeal


            if (dealDamageModel.targetObject != null)
            {
                dealDamageModel.targetObject.GetComponent<LivingCreature>().CreatureHit(dealDamageModel.attackToDeal);
                Debug.Log(dealDamageModel.targetObject.GetComponent<LivingCreature>());

                EventFlags.EVENTTookDamage();
            }
        }

        protected override void CancelBehaviourImpl()
        {
            throw new System.NotImplementedException();
        }

        protected override void FinishBehaviourImpl()
        {

        }

    }
}
