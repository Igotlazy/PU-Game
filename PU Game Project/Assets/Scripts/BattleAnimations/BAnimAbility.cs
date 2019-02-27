using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MHA.BattleAnimations
{
    public class BAnimAbility : BattleAnimation
    {
        public GameObject animObject;
        GameObject spawnedAnimObject;
        Vector3 spawnLocation;
        AnimAbilityControl animController;
        public bool forceGo;

        public BAnimAbility(object _source, GameObject _animObject, Vector3 _spawnLocation) : base (_source)
        {
            this.animObject = _animObject;
            this.spawnLocation = _spawnLocation;
        }

        protected override void PlayBattleAnimationImpl()
        {
            spawnedAnimObject = GameObject.Instantiate(animObject, spawnLocation, Quaternion.identity);
            animController = spawnedAnimObject.GetComponent<AnimAbilityControl>();

            if (animController.shouldWait && !forceGo)
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

        public override string ToString()
        {
            return "AnimAbility: " + animObject.name;
        }
    }
}

