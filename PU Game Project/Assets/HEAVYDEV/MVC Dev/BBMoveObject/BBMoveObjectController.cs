using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MHA.BattleBehaviours
{
    public class BBMoveObjectController : BattleBehaviourController
    {

        public BBMoveObjectModel moveObjectModel;

        public BBMoveObjectController(BBMoveObjectModel givenModel) : base(givenModel)
        {
            moveObjectModel = givenModel;
            RunBehaviourImplList.Add(ObjectMove);
        }


        protected void ObjectMove()
        {
            if (moveObjectModel.instantiate)
            {
                moveObjectModel.objectToMove = GameObject.Instantiate(moveObjectModel.objectToInstantiate, moveObjectModel.associatedCharAbilityModel.associatedCreature.gameObject.transform.position, Quaternion.identity
                   /*Quaternion.LookRotation(moveObjectModel.target - charAbilityController.associatedModel.associatedObject.transform.position)*/);
            }

            while (true)
            {

                if (moveObjectModel.objectToMove.transform.position == moveObjectModel.finalPos)
                {
                    break;
                }

                moveObjectModel.objectToMove.transform.position = Vector3.MoveTowards(moveObjectModel.objectToMove.transform.position, moveObjectModel.finalPos, moveObjectModel.speed * Time.deltaTime);
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
