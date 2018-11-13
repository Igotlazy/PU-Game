using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MHA.GenericBehaviours
{
    [System.Serializable]
    public class GBMoveObject : GBBase
    {

        public GameObject objectToMove;
        public Vector3 endPosition;
        public float moveSpeed;

        public GBMoveObject(GameObject _objectToMove, Vector3 _endPosition, BattleEvent _attachedBattleEvent) : base(_attachedBattleEvent)
        {
            this.objectToMove = _objectToMove;
            this.endPosition = _endPosition;
        }

        protected override IEnumerator RunBehaviourImpl()
        {
            Projectile projScript = objectToMove.GetComponent<Projectile>();
            if (projScript != null)
            {
                moveSpeed = projScript.projectileMovementSpeed;
            }
            else
            {
                moveSpeed = 5f;
            }

            while (Vector3.Distance(objectToMove.transform.position, endPosition) > 0.05f)
            {
                yield return attachedBattleEvent.bEventMonoBehaviour.StartCoroutine(attachedBattleEvent.ALLOWINTERRUPT(0f));

                if (objectToMove != null)
                {
                    objectToMove.transform.position = Vector3.MoveTowards(objectToMove.transform.position, endPosition, moveSpeed * Time.deltaTime);
                }
                else
                {
                    break;
                }
            }

            FinishBehaviour();
        }

    }
}

