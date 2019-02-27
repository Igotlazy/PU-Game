using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MHA.BattleAnimations
{
    public class BAnimDeath : BattleAnimation
    {
        Unit deathUnit;
        public BAnimDeath(object _source, Unit _deathUnit) : base(_source)
        {
            deathUnit = _deathUnit;
        }

        protected override void PlayBattleAnimationImpl()
        {
            mono.StartCoroutine(DeathAnim());
        }

        private IEnumerator DeathAnim()
        {
            yield return new WaitForSeconds(1f);
            deathUnit.gameObject.SetActive(false);
            AnimFinished = true;
            Debug.LogWarning(deathUnit.gameObject.name + " Died");
        }


    }
}

