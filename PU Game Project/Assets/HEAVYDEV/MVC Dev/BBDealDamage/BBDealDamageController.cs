using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BBDealDamageController : BattleBehaviourController {

    public BBDealDamageModel dealDamageModel;

    public BBDealDamageController(BBDealDamageModel givenModel)
    {
        dealDamageModel = givenModel;
    }

    //float damageToDeal;

    protected override IEnumerator RunBehaviourImpl()
    {
        //damageTodeal
        yield return TurnManager.instance.StartCoroutine(attachedBattleEvent.ALLOWINTERRUPT(0f));

        if (dealDamageModel.targetObject != null)
        {
            dealDamageModel.targetObject.GetComponent<LivingCreature>().CreatureHit(dealDamageModel.attackToDeal);

            EventFlags.EVENTTookDamage();
            yield return attachedBattleEvent.bEventMonoBehaviour.StartCoroutine(attachedBattleEvent.ALLOWINTERRUPT(0.2f));
        }

        FinishBehaviour();
    }

    protected override void CancelBehaviourImpl()
    {
        throw new System.NotImplementedException();
    }

    protected override void FinishBehaviourImpl()
    {
        throw new System.NotImplementedException();
    }

}
