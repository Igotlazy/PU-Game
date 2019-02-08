using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MHA.BattleBehaviours;

namespace MHA.BattleBehaviours
{
    public class AnimAbility : BattleAnimation
    {
        GameObject animObject;
        GameObject spawnedAnimObject;
        Vector3 spawnLocation;
        AnimAbilityControl animController;


        public AnimAbility(GameObject _animObject, Vector3 _spawnLocation)
        {
            this.animObject = _animObject;
            this.spawnLocation = _spawnLocation;
        }

        protected override void PlayBattleAnimationImpl()
        {
            spawnedAnimObject = GameObject.Instantiate(animObject, spawnLocation, Quaternion.identity);
            animController = spawnedAnimObject.GetComponent<AnimAbilityControl>();

            if (animController.shouldWait)
            {
                animController.animFinished = false;
                mono.StartCoroutine(PlayBattleAnim());
            }
            else
            {
                animController.BeginAbilityAnimation();
                AnimFinished = true;
            }

        }

        IEnumerator PlayBattleAnim()
        {
            animController.BeginAbilityAnimation();
            yield return new WaitUntil(() => animController.animFinished);
            AnimFinished = true;
        }
    }
}

