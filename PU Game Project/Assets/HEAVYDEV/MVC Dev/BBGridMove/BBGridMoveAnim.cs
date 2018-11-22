using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MHA.BattleBehaviours
{
    public class BBGridMoveAnim : BattleAnimation
    {

        Vector3 finalPos;
        GameObject moveTarget;
        float speed;

        public BBGridMoveAnim(Vector3 _finalPos, GameObject _moveTarget, float _speed)
        {
            finalPos = _finalPos;
            moveTarget = _moveTarget;
            speed = _speed;

            LoadBattleAnimation();
        }

        protected override void PlayBattleAnimationImpl()
        {
            mono.StartCoroutine(GridMoveAnim());
        }

        private IEnumerator GridMoveAnim()
        {
            while (true)
            {
                if (moveTarget.transform.position == finalPos)
                {
                    break;
                }

                moveTarget.transform.position = Vector3.MoveTowards(moveTarget.transform.position, finalPos, speed * Time.deltaTime);
                yield return null;
            }

            AnimFinished = true;
        }
    }
}
