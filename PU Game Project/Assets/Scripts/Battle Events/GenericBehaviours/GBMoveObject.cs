using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MHA.GenericBehaviours;

public class GBMoveObject : GBBase {

    public GameObject objectToMove;
    public Vector3 endPosition;
    public float moveSpeed;

    public GBMoveObject(BattleEvent _attachedBattleEvent) : base(_attachedBattleEvent)
    {

    }

    public IEnumerator MoveObject(GameObject givenObject, Vector3 givenEndPosition)
    {
        objectToMove = givenObject;
        endPosition = givenEndPosition;

        Projectile projScript = givenObject.GetComponent<Projectile>();
        if(projScript != null)
        {
            moveSpeed = projScript.projectileMovementSpeed;
        }
        else
        {
            moveSpeed = 5f;
        }

        while (Vector3.Distance(givenObject.transform.position, givenEndPosition) > 0.05f)
        {
            yield return attachedBattleEvent.bEventMonoBehaviour.StartCoroutine(attachedBattleEvent.ALLOWINTERRUPT(0f));

            if (givenObject != null)
            {
                givenObject.transform.position = Vector3.MoveTowards(givenObject.transform.position, givenEndPosition, moveSpeed * Time.deltaTime);
            }
            else
            {
                break;
            }
        }

        this.behaviourDone = true;
    }

}
