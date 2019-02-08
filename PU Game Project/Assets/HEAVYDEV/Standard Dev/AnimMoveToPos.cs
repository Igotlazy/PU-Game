using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace MHA.BattleBehaviours
{
    public class AnimMoveToPos : BattleAnimation
    {

        public Vector3 finalPos;
        public GameObject moveTarget;
        float speed;
        bool destroyAtEnd;
        int index;

        public bool dontHoldUpQueue;

        public AnimMoveToPos(Vector3 _position, GameObject _moveTarget, float _speed, bool _destroyAtEnd)
        {
            finalPos = _position;
            moveTarget = _moveTarget;
            speed = _speed;
            destroyAtEnd = _destroyAtEnd;
        }

        protected override void PlayBattleAnimationImpl()
        {
            mono.StartCoroutine(GridMoveAnim());
            if (dontHoldUpQueue)
            {
                AnimFinished = true;
            }
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

            if (destroyAtEnd)
            {
                GameObject.Destroy(moveTarget);
            }

            if (!dontHoldUpQueue)
            {
                AnimFinished = true;
            }
        }


        public static bool SmartChecker(Vector3 nextPos, GameObject moveTarget)
        {
            try
            {
                if(ResolutionManager.instance.animationQueue.Count > 0)
                {
                    AnimMoveToPos posAnim = (AnimMoveToPos)ResolutionManager.instance.animationQueue.Peek();
                    Debug.Log(posAnim.finalPos.normalized);
                    Debug.Log(nextPos.normalized);
                    if (moveTarget == posAnim.moveTarget && posAnim.finalPos.normalized == nextPos.normalized)
                    {
                        Debug.Log("True");
                        posAnim.finalPos += nextPos;
                        return true;
                    }
                    else
                    {
                        Debug.Log("Close False");
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch(InvalidCastException)
            {
                return false;
            }

        }
    }
}
